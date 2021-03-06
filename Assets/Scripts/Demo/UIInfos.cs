﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Games.UICore;

public class UIInfos
{
    /// <summary>
    /// 核心数据差异化构造不同类型的UI
    /// </summary>
    // 游戏基础UI（角色信息，物品掉落什么的）
    public static UICoreData UI_TypeGameBase = new UICoreData(UIRootType.Fixed, UIShowModel.DoNoting, UIColliderType.Penetrate, UINavigationMode.IngoreNavigation, false, false);
    // 场景基础UI（小地图etc）
    public static UICoreData UI_TypeSceneBase = new UICoreData(UIRootType.Fixed, UIShowModel.DoNoting, UIColliderType.Penetrate, UINavigationMode.IngoreNavigation, false, true);
    // 常规UI，游戏中常用UI
    public static UICoreData UI_TypeNormal = new UICoreData(UIRootType.Base, UIShowModel.HideOther, UIColliderType.Penetrate, UINavigationMode.NormalNavigation, false, true);
    // 常规UI，游戏中常用UI(全屏幕显示，隐藏固定UI)
    public static UICoreData UI_TypeFULLNormal = new UICoreData(UIRootType.Base, UIShowModel.HideEverything, UIColliderType.Penetrate, UINavigationMode.NormalNavigation, false, true);
    // 剧情UI，隐藏其他所有UI界面，界面结束后恢复基础UI (强制清空导航栈信息)
    public static UICoreData UI_TypeStory = new UICoreData(UIRootType.Base, UIShowModel.HideEverything, UIColliderType.Penetrate, UINavigationMode.NormalNavigation, true, true);
    // 普通弹出式UI (直接弹出，任务，游戏说明)
    public static UICoreData UI_TypePop = new UICoreData(UIRootType.PopUp, UIShowModel.DoNoting, UIColliderType.Lucency, UINavigationMode.IngoreNavigation, false, true);
    // 中央弹出式UI (隐藏基础UI，一些小互动，低透明度模态窗口)
    public static UICoreData UI_TypePopCenter = new UICoreData(UIRootType.PopUp, UIShowModel.HideEverything, UIColliderType.ImPenetrable, UINavigationMode.NormalNavigation, false, true, true);
    // Tips弹出式UI (不互斥，物品tips，不会隐藏当前任何UI，全透明模态窗口)
    public static UICoreData UI_TypeTips = new UICoreData(UIRootType.PopUp, UIShowModel.DoNoting, UIColliderType.Lucency, UINavigationMode.IngoreNavigation, false, true, false);

    // 新手引导UI (同类型互斥，新手引导什么的, 不会隐藏当前任何UI，半透明模态窗口)
    public static UICoreData UI_TypeNewGuide = new UICoreData(UIRootType.SecondPopUp, UIShowModel.TypeMutex, UIColliderType.Translucent, UINavigationMode.IngoreNavigation, false, true, true);
    // MessageBox专用 (同类型互斥，不隐藏当前任何UI，全透明模态窗口)，单独的逻辑脚本控制
    public static UICoreData UI_TypeMessageBox = new UICoreData(UIRootType.SecondPopUp, UIShowModel.TypeMutex, UIColliderType.Lucency, UINavigationMode.IngoreNavigation, false, true, true);

    // 游戏中的所有UI定义
    // eg:
    public static UIInfoData DefaultUI = new UIInfoData(UI_TypeGameBase, "", "");
    public static UIInfoData MainUI = new UIInfoData(UI_TypeNormal, "UIPrefabs", "MainUI");
    public static UIInfoData CharacterUI = new UIInfoData(UI_TypeNormal, "UIPrefabs", "CharacterUI");

    public static UIInfoData MessageBoxUI = new UIInfoData(UI_TypeMessageBox, "UIPrefabs", "MessageBoxUI");
}