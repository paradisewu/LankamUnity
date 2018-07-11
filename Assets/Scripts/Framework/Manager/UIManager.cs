using LuaFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGFrame;
using DG.Tweening;

public class UIManager : Manager
{
    #region 初始化
    public Dictionary<SceneType, SceneBase> scenes;
    private List<SwitchRecorder> switchRecoders;

    private UIManager()
    {
        switchRecoders = new List<SwitchRecorder>();
        mLayerDic = new Dictionary<LayerType, GameObject>();
        panels = new Dictionary<PanelType, PanelBase>();
        scenes = new Dictionary<SceneType, SceneBase>();
    }

    public void OnDestroy()
    {
        OnSwitchingSceneHandler = null;

        switchRecoders.Clear();
        switchRecoders = null;

        scenes.Clear();
        scenes = null;
    }
    #endregion

    private Transform parentObj = null;

    private Transform m_UIRoot;
    private Transform UIRoot
    {
        get
        {
            if (m_UIRoot == null)
            {
                m_UIRoot = ResManager.CreateGameObject("UI/UIRoot", false).transform;
                DontDestroyOnLoad(m_UIRoot.gameObject);
            }
            return m_UIRoot;
        }
    }


    public SceneType CurrentSceneType;

    public delegate void OnSwitchingScene(SceneType type);
    public OnSwitchingScene OnSwitchingSceneHandler;
    private const SceneType mainSceneType = SceneType.SceneHome;

    public SceneBase currentSceneBase;


    #region SceneMgr
    public void SwitchingScene(SceneType sceneType, params object[] sceneArgs)
    {
        if (currentSceneBase != null)
        {
            if (sceneType == currentSceneBase.type)
            {
                Debug.Log("试图切换场景至当前场景：" + sceneType.ToString());
                //HideCurrentScene();
                //ShowScene(sceneType, sceneArgs);
                return;
            }
        }

        if (sceneType == mainSceneType)//进入主场景，把切换场景记录清空
        {
            switchRecoders.Clear();
        }
        switchRecoders.Add(new SwitchRecorder(sceneType, sceneArgs));//切换记录

        HideCurrentScene();
        ShowScene(sceneType, sceneArgs);
        if (OnSwitchingSceneHandler != null)
        {
            OnSwitchingSceneHandler(sceneType);
        }
    }
    /// <summary>
    /// 切换到上一个场景
    /// </summary>
    public void SwitchToPrevScene()
    {
        SwitchRecorder sr = switchRecoders[switchRecoders.Count - 2];
        switchRecoders.RemoveRange(switchRecoders.Count - 2, 2);//将当前场景，以及要切换的场景。从记录中清空
        SwitchingScene(sr.sceneType, sr.sceneArgs);
    }
    private void ShowScene(SceneType sceneType, params object[] sceneArgs)
    {
        if (scenes.ContainsKey(sceneType))
        {
            currentSceneBase = scenes[sceneType];
            currentSceneBase.OnShowing();
            currentSceneBase.OnResetArgs(sceneArgs);
            currentSceneBase.gameObject.SetActive(true);
            currentSceneBase.OnShowed();
        }
        else
        {
            if (parentObj == null)
            {
                parentObj = UIRoot;
            }
            string name = sceneType.ToString();

            GameObject scene = new GameObject(name);
            UnityHelper.AddUIChildNodeToParentNode(parentObj, scene.transform);
            SceneBase currentScene = scene.AddComponent(Type.GetType(name)) as SceneBase;

            currentSceneBase = currentScene;
            CurrentSceneType = sceneType;

            currentSceneBase.OnInit(sceneArgs);
            currentSceneBase.OnShowing();
            SetLayer(currentSceneBase.gameObject, LayerType.Scene);
            currentSceneBase.OnShowed();
            scenes.Add(sceneType, currentSceneBase);
            switchRecoders.Add(new SwitchRecorder(sceneType, sceneArgs));
        }
    }

    /// <summary>
    /// 关闭当前场景
    /// </summary>
    private void HideCurrentScene()
    {
        if (currentSceneBase != null)
        {
            currentSceneBase.OnHiding();
            currentSceneBase.gameObject.SetActive(false);
            currentSceneBase.OnHided();

            if (!currentSceneBase.cache)
            {
                scenes.Remove(currentSceneBase.type);
                GameObject.Destroy(currentSceneBase.gameObject);
            }
        }
    }


    /// <summary>
    /// 记录结构体
    /// </summary>
    internal struct SwitchRecorder
    {
        internal SceneType sceneType;
        internal object[] sceneArgs;

        internal SwitchRecorder(SceneType sceneType, params object[] sceneArgs)
        {
            this.sceneType = sceneType;
            this.sceneArgs = sceneArgs;
        }
    }

    #endregion


    #region LayerMgr
    private Dictionary<LayerType, GameObject> mLayerDic;
    private Transform mParent;

    public void SetLayer(GameObject current, LayerType type)
    {
        if (mLayerDic.Count < Enum.GetNames(typeof(LayerType)).Length)
        {
            //初始化
            LayerInit();
        }
        current.transform.parent = mLayerDic[type].transform;
    }

    public void LayerInit()
    {
        mParent = UIRoot;
        int nums = Enum.GetNames(typeof(LayerType)).Length;
        for (int i = 0; i < nums; i++)
        {
            object obj = Enum.GetValues(typeof(LayerType)).GetValue(i);
            mLayerDic.Add((LayerType)obj, CreateLayerGameObject(obj.ToString(), (LayerType)obj));
        }
    }
    GameObject CreateLayerGameObject(string name, LayerType type)
    {

        Transform obj = UnityHelper.FindTheChildNode(mParent.gameObject, name);
        if (obj != null)
        {
            return obj.gameObject;
        }
        else
        {
            GameObject layer = new GameObject(name);

            UnityHelper.AddUIChildNodeToParentNode(mParent, layer.transform);

            layer.transform.SetAsLastSibling();

            return layer;
        }
    }

    #endregion


    #region PanelMgr

    #region 数据定义
    public Dictionary<PanelType, PanelBase> panels;
    public enum PanelShowStyle
    {
        /// <summary> 正常出现 </summary>
        Nomal,
        /// <summary> 中间由小变大 </summary>
        CenterScaleBigNomal,
        /// <summary> 由上往下 </summary>
        TopToSlide,
        /// <summary> 由下往上 </summary>
        DownToSlide,
        /// <summary> 左往中 </summary>
        LeftToSlide,
        /// <summary> 右往中 </summary>
        RightToSlide,
    }

    public enum PanelMaskStyle
    {
        /// <summary> 无背景 </summary>
        None,
        /// <summary> 半透明背景 </summary>
        BlackAlpha,
        /// <summary> 无背景，但有Box关闭组件 </summary>
        Alpha,
    }
    #endregion

    /// <summary> 当前打开的面板 </summary>
    private PanelBase CurrentPanel = null;
    public void ShowPanel(PanelType panelType, params object[] panelArgs)
    {
        if (panels.ContainsKey(panelType))
        {
            Debug.Log("该面板已打开");
            CurrentPanel = panels[panelType];
            //CurrentPanel.gameObject.SetActive(false);
            CurrentPanel.OnResetArgs(panelArgs);
        }
        else
        {
            GameObject scene = new GameObject(panelType.ToString());
            CurrentPanel = scene.AddComponent(Type.GetType(panelType.ToString())) as PanelBase;
            //CurrentPanel.gameObject.SetActive(false);
            CurrentPanel.OnInit(panelArgs);
            panels.Add(panelType, CurrentPanel);
            if (parentObj == null)
            {
                parentObj = UIRoot;
            }
            UnityHelper.AddUIChildNodeToParentNode(parentObj, scene.transform);
            SetLayer(CurrentPanel.gameObject, LayerType.Panel);
        }

        StartShowPanel(CurrentPanel, CurrentPanel.PanelShowStyle, true);
    }

    /// <summary>
    /// 发起关闭
    /// </summary>
    /// <param name="panelType"></param>
    public void HidePanel(PanelType panelType)
    {
        if (panels.ContainsKey(panelType))
        {
            PanelBase pb = panels[panelType];
            StartShowPanel(pb, pb.PanelShowStyle, false);
        }
        else Debug.LogError("少年，你正常执行一个很危险的操作，你要关闭的面板并不存在！！！");
    }
    /// <summary>
    /// 强制KO面板
    /// </summary>
    /// <param name="panelType"></param>
    public void DestroyPanel(PanelType panelType)
    {
        if (panels.ContainsKey(panelType))
        {
            PanelBase pb = panels[panelType];
            GameObject.Destroy(pb.gameObject);
            panels.Remove(panelType);
        }
    }

    private void StartShowPanel(PanelBase go, PanelShowStyle showStle, bool isOpen)
    {
        switch (showStle)
        {
            case PanelShowStyle.Nomal:
                ShowNomal(go, isOpen);
                break;
            case PanelShowStyle.CenterScaleBigNomal:
                ShowCenterScaleBigNomal(go, isOpen);
                break;
            case PanelShowStyle.LeftToSlide:
                ShowLeftToSlide(go, isOpen, true);
                break;
            case PanelShowStyle.RightToSlide:
                ShowLeftToSlide(go, isOpen, false);
                break;
            case PanelShowStyle.TopToSlide:
                ShowTopToSlide(go, isOpen, true);
                break;
            case PanelShowStyle.DownToSlide:
                ShowTopToSlide(go, isOpen, false);
                break;
        }
    }

    #region 各种打开效果
    void ShowNomal(PanelBase go, bool isOpen)
    {
        if (!isOpen)
        {
            DestroyPanel(go.type);
        }
        else
        {
            go.gameObject.SetActive(true);
        }
    }
    /// <summary> 中间变大 </summary>
    void ShowCenterScaleBigNomal(PanelBase go, bool isOpen)
    {
        if (isOpen)
        {
            go.transform.localScale = Vector3.zero;
            go.gameObject.SetActive(true);
            go.transform.DOScale(Vector3.one, go.OpenDuration);
        }
        else
        {
            go.transform.localScale = Vector3.one;
            go.transform.DOScale(Vector3.zero, go.OpenDuration).OnComplete(() =>
            {
                DestroyPanel(go.type);
            });
        }

        //TweenScale ts = go.gameObject.GetComponent<TweenScale>();
        //if (ts == null) ts = go.gameObject.AddComponent<TweenScale>();
        //ts.from = Vector3.zero;
        //ts.to = Vector3.one;
        //ts.duration = go.openDuration;
        //ts.SetOnFinished(() =>
        //{
        //    if (!isOpen)
        //        DestroyPanel(go.type);
        //});

        //go.gameObject.SetActive(true);
        //if (!isOpen) ts.Play(isOpen);
    }
    /// <summary> 左右往中 </summary>
    void ShowLeftToSlide(PanelBase go, bool isOpen, bool isLeft)
    {
        //TweenPosition tp = go.gameObject.GetComponent<TweenPosition>();
        //if (tp == null) tp = go.gameObject.AddComponent<TweenPosition>();
        //tp.from = isLeft ? new Vector3(-700,0, 0) : new Vector3(700,0, 0);
        //tp.to = Vector3.zero;
        //tp.duration = go.openDuration;
        //tp.SetOnFinished(() =>
        //{
        //    if (!isOpen)
        //        DestroyPanel(go.type);
        //});
        //go.gameObject.SetActive(true);
        //if (!isOpen) tp.Play(isOpen);
    }

    /// <summary> 上下往中 </summary>
    void ShowTopToSlide(PanelBase go, bool isOpen, bool isTop)
    {
        //TweenPosition tp = go.gameObject.GetComponent<TweenPosition>();
        //if (tp == null) tp = go.gameObject.AddComponent<TweenPosition>();
        //tp.from = isTop ? new Vector3(0, 600, 0) : new Vector3(0, -600, 0);
        //tp.to = Vector3.zero;
        //tp.duration = go.openDuration;
        //tp.SetOnFinished(() =>
        //{
        //    if (!isOpen)
        //        DestroyPanel(go.type);
        //});
        //go.gameObject.SetActive(true);
        //if (!isOpen) tp.Play(isOpen);
    }
    #endregion
    #endregion

}

public enum LayerType
{
    /// <summary> 场景 </summary>
    Scene = 50,
    /// <summary> 面板 </summary>
    Panel = 200,
    /// <summary> 提示 </summary>
    Tips = 400,
    /// <summary> 公告跑马灯 </summary>
    Notice = 1000,
}
