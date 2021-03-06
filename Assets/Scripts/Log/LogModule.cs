﻿/*****************************************************************************
 * filename :  LogModule.cs
 * author   :  Zhang Yunxing
 * date     :  2018/08/29 15:38
 * desc     :  日志管理，二次封装log，开关控制，以及写入日志到文件
 * changelog:  
*****************************************************************************/
using UnityEngine;
using System;
using System.Diagnostics;
using Games.CoreDefine;

using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

public class LogModule : MonoBehaviour
{
    #region global var
    // Debug 输出功能是否开启
    private static bool _isDebugEnable = true;
    public static bool IsDebugEnable
    {
        set
        {
            _isDebugEnable = value;
        }
    }

    private static bool _isLogFileEnable = true;
    public static bool IsLogFileEnable
    {
        set
        {
            _isLogFileEnable = value;
        }
    }

    // 是否是从自定义Log中输出信息，用于在LogCallBack中区分信息来源，防止重复写入
    private static bool _isCustomLogOut = false;

    /* 核心字段 */
    //private static List<string> _logCacheList;           //Log日志缓存数据
    private static string _logPath = null;               //Log日志文件路径
    private static CoreGlobeVar.LogModel _logModel;          //Log日志状态（部署模式）
    //private static int _logMaxCapacity;                  //Log日志最大容量
    //private static int _logBufferMaxNumber;              //Log日志缓存最大容量

    #endregion

    private void Awake()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
        _logModel = CoreGlobeVar.LogModel.Develop;
        _logPath = Application.dataPath + "/Log";
#elif UNITY_ANDROID && !UNITY_EDITOR
        _logModel = CoreGlobeVar.LogModel.Deploy;
        _logPath = Application.persistentDataPath + "/Log";
#elif UNITY_IPHONE && !UNITY_EDITOR
        _logModel = CoreGlobeVar.LogModel.Deploy;
        _logPath = Application.persistentDataPath + "/Log";
#else
        // --!!!
        _logModel = CoreGlobeVar.LogModel.Default;
        _logPath = Application.dataPath + "/Log";
#endif
        Application.logMessageReceived += LogCallback;
    }

    #region 二次封装的Log函数
    public static void Log(object message, Object context = null)
    {
        if (_isDebugEnable)
        {
            _isCustomLogOut = true;
            Debug.Log(message, context);
        }
        // condition
        if (_isLogFileEnable && (CoreGlobeVar.LogModel.Develop == _logModel))
        {
            WriteLog2File(GetCallStackLogMsg(message), LogType.Log);
        }
    }

    public static void LogWarning(object message, Object context = null)
    {
        if (_isDebugEnable)
        {
            _isCustomLogOut = true;
            Debug.LogWarning(message, context);
        }
        // condition
        if (_isLogFileEnable && (CoreGlobeVar.LogModel.Deploy == _logModel || CoreGlobeVar.LogModel.Develop == _logModel))
        {
            WriteLog2File(GetCallStackLogMsg(message), LogType.Warning);
        }
    }

    public static void LogError(object message, Object context = null)
    {
        if (_isDebugEnable)
        {
            _isCustomLogOut = true;
            Debug.LogError(message, context);
        }
        // condition
        if (_isLogFileEnable && (CoreGlobeVar.LogModel.Deploy == _logModel || CoreGlobeVar.LogModel.Develop == _logModel))
        {
            WriteLog2File(GetCallStackLogMsg(message), LogType.Error);
        }
    }
    #endregion


    #region custom private func

    /// <summary>
    /// 打印log时调用，写入对应log文件
    /// </summary>
    /// <param name="log"></param>
    /// <param name="type"></param>
    private static void WriteLog2File(string log, LogType type)
    {
        if (string.IsNullOrEmpty(log))
            return;
        switch (type)
        {
            case LogType.Log:
                Utils.CheckTargetPath(_logPath + "/" + CoreGlobeVar.LOG_NORMAL_FILE);
                Utils.AppendStringToFile(_logPath + "/" + CoreGlobeVar.LOG_NORMAL_FILE, log);
                break;
            case LogType.Warning:
                Utils.CheckTargetPath(_logPath + "/" + CoreGlobeVar.LOG_WARNING_FILE);
                Utils.AppendStringToFile(_logPath + "/" + CoreGlobeVar.LOG_WARNING_FILE, log);
                break;
            case LogType.Error:
                Utils.CheckTargetPath(_logPath + "/" + CoreGlobeVar.LOG_ERROR_FILE);
                Utils.AppendStringToFile(_logPath + "/" + CoreGlobeVar.LOG_ERROR_FILE, log);
                break;
            default:
                break;
        }
    }


    /// <summary>
    /// 组装日志写入信息，通过函数调用栈得到调用所在类及函数
    /// </summary>
    /// <param name="message"></param>
    /// <returns>拼接完成的字符串</returns>
    private static string GetCallStackLogMsg(object message)
    {
        StackTrace trace = new StackTrace();
        // 查询堆栈的前两次调用信息
        if (trace.FrameCount < 2)
        {
            return string.Empty;
        }
        StackFrame frame = trace.GetFrame(2);
        return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " [" + frame.GetMethod().DeclaringType.Name + "." +
            frame.GetMethod().Name + "] : " + message;
    }


    /// <summary>
    /// 添加当前函数为Application.logMessageReceived事件响应函数，获取系统输出的Log信息
    /// 一般都是警告和错误，开发模式下全部写入，部署模式只写入错误
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="stackTrace"></param>
    /// <param name="type"></param>
    private void LogCallback(string condition, string stackTrace, LogType type)
    {
        if (false == _isLogFileEnable || CoreGlobeVar.LogModel.Default == _logModel)
            return;
        // 自定义log调用触发不做处理，避免重复输出
        if(true == _isCustomLogOut)
        {
            _isCustomLogOut = false;
            return;
        }

        string logContent = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " [" + type + "][" + stackTrace + "] : " + condition;

        switch (type)
        {
            case LogType.Log:
                if(CoreGlobeVar.LogModel.Develop == _logModel)
                {
                    WriteLog2File(logContent, LogType.Log);
                }
                break;
            case LogType.Warning:
                if (CoreGlobeVar.LogModel.Develop == _logModel)
                {
                    WriteLog2File(logContent, LogType.Warning);
                }
                break;
            case LogType.Assert:
            case LogType.Exception:
            case LogType.Error:
                WriteLog2File(logContent, LogType.Error);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 善后工作
    /// </summary>
    private void OnDestroy()
    {
        Application.logMessageReceived -= LogCallback;
    }
    #endregion
}