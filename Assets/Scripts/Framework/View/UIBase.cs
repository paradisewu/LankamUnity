using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZGFrame;

public class UIBase : View
{
    /// <summary>
    /// 是否起用秒刷新
    /// </summary>
    protected bool secondUpdateEnabled = false;
    /// <summary>
    /// 秒刷新时间点
    /// </summary>
    private float secondUpdateTime = 0.0f;

    private GameObject _skin;
    /// <summary>
    /// 皮肤
    /// </summary>
    public GameObject skin
    {
        get
        {
            return _skin;
        }
    }
    /// <summary>
    /// 主皮肤
    /// </summary>
    private string mainSkinPath;
    /// <summary>
    /// skin 's transfrom
    /// </summary>
    public Transform skinTransform
    {
        get
        {
            if (skin != null)
            {
                return skin.transform;
            }
            return null;
        }
    }


    /// <summary>
    /// 所有boxCollider
    /// </summary>
    private List<Button> colliderList = new List<Button>();
    //private List<Toggle> TogglecolliderList = new List<Toggle>();


    private bool _initDoneFlag = false;
    /// <summary>
    /// 初始化完成标识
    /// </summary>
    public bool initDoneFlag
    {
        get
        {
            return _initDoneFlag;
        }
    }

    public void OnDestroy()
    {
        OnDestroyFront();

        OnOnDestroy();
        colliderList.Clear();
        colliderList = null;
        StopAllCoroutines();

        OnDestroyDone();
    }

    void Update()
    {
        if (!initDoneFlag)
        {
            return;
        }

        OnUpdate();

        if (secondUpdateEnabled)
        {
            float delta = Time.deltaTime;
            secondUpdateTime += delta;
            if (secondUpdateTime > 1.0f)
            {
                secondUpdateTime -= 1.0f;
                OnSecondUpdate();
            }
        }
    }

    void FixedUpdate()
    {
        if (!initDoneFlag)
        {
            return;
        }
        OnFixedUpdate();
    }
    void LateUpdate()
    {
        if (!initDoneFlag)
        {
            return;
        }
        OnLateUpdate();
    }


    /// <summary>
    /// 初始化
    /// </summary>
    protected void Init()
    {
        if (initDoneFlag)
        {
            return;
        }
        OnInitFront();
        OnInitSkinFront();
        OnInitSkin();
        OnInitSkinDone();

        Button[] colliders = this.GetComponentsInChildren<Button>(true);
        for (int i = 0, len = colliders.Length; i < len; i++)
        {
            Button collider = colliders[i];
            EventTriggerListener listener = EventTriggerListener.GetListener(collider.gameObject);
            listener.onPointerClick = OnClick;
            colliderList.Add(collider);
        }

        //Toggle[] Togglecolliders = this.GetComponentsInChildren<Toggle>(true);
        //for (int i = 0, len = Togglecolliders.Length; i < len; i++)
        //{
        //    Toggle Toggle = Togglecolliders[i];
        //    EventTriggerListener listener = EventTriggerListener.GetListener(Toggle.gameObject);
        //    listener.onPointerClick = OnClick;
        //    TogglecolliderList.Add(Toggle);
        //}

        OnInitDone();



        _initDoneFlag = true;
    }

    /// <summary>
    /// 初始化前
    /// </summary>
    protected virtual void OnInitFront() { }
    /// <summary>
    /// 初始化皮肤前
    /// </summary>
    protected virtual void OnInitSkinFront() { }
    /// <summary>
    /// 初始化皮肤
    /// </summary>
    protected virtual void OnInitSkin()
    {
        if (mainSkinPath != null)
        {
            _skin = LoadSrc(mainSkinPath);
        }
        else
        {
            _skin = new GameObject("Skin");
        }
        UnityHelper.AddUIChildNodeToParentNode(this.transform, _skin.transform);
    }
    /// <summary>
    /// 初始化皮肤完成
    /// </summary>
    protected virtual void OnInitSkinDone() { }
    /// <summary>
    /// 初始化完成
    /// </summary>
    protected virtual void OnInitDone() { }

    /// <summary>
    /// 基于秒的update
    /// </summary>
    protected virtual void OnSecondUpdate() { }
    protected virtual void OnUpdate() { }
    protected virtual void OnFixedUpdate() { }
    protected virtual void OnLateUpdate() { }
    protected virtual void OnDestroyFront() { }
    protected virtual void OnOnDestroy() { }
    protected virtual void OnDestroyDone()
    {
       
    }
    /// <summary>
    /// 点击按钮
    /// </summary>
    /// <param name="target">点击的目标对象</param>
    protected virtual void OnClick(GameObject target) { }

    /// <summary>
    /// 设置该对象下所有boxcollider是否可以交互
    /// </summary>
    /// <param name="enabled"></param>
    public void SetColliderEnabled(bool enabled)
    {
        foreach (Button bc in colliderList)
        {
            bc.interactable = enabled;
        }
    }

    protected void Click(GameObject target)
    {
        if (initDoneFlag)
        {
            OnClick(target);
        }
    }
    /// <summary>
    /// 设置主skin
    /// </summary>
    /// <param name="path"></param>
    protected void SetMainSkinPath(string path)
    {
        if (initDoneFlag)
        {
            Debug.LogWarning("初始化已完成，请在初始化皮肤前设置主皮肤！path=" + path);
        }
        mainSkinPath = path;
    }
    /// <summary>
    /// 加载资源
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    protected GameObject LoadSrc(string path)
    {
        return ResManager.CreateGameObject(path, true);
    }


}

