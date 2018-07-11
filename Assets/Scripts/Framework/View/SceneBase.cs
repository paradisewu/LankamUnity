using UnityEngine;
using System.Collections;

public class SceneBase : LayerBase
{

    protected bool _cache = false;
    /// <summary>
    /// 缓存标识
    /// 如为false,则在关闭时destroy。
    /// </summary>
    public bool cache
    {
        get
        {
            return _cache;
        }
    }

    protected SceneType _type;
    /// <summary>
    /// 场景ID
    /// </summary>
    public SceneType type
    {
        get
        {
            return _type;
        }
    }
    protected object[] _sceneArgs;
    /// <summary>
    /// 记录场景init时参数
    /// </summary>
    public object[] sceneArgs
    {
        get
        {
            return _sceneArgs;
        }
    }

    /// <summary>
    /// 初始化场景
    /// </summary>
    /// <param name="sceneArgs">场景参数</param>
    public virtual void OnInit(params object[] sceneArgs)
    {
        _sceneArgs = sceneArgs;
        Init();
    }

    /// <summary>
    /// 开始显示
    /// </summary>
    public virtual void OnShowing()
    {

    }
    /// <summary>
    /// 重值数据
    /// </summary>
    /// <param name="sceneArgs"></param>
    public virtual void OnResetArgs(params object[] sceneArgs)
    {
        _sceneArgs = sceneArgs;
    }
    /// <summary>
    /// 显示场景后
    /// </summary>
    public virtual void OnShowed()
    {

    }
    /// <summary>
    /// 开始隐藏
    /// </summary>
    public virtual void OnHiding()
    {

    }
    /// <summary>
    /// 隐藏后
    /// </summary>
    public virtual void OnHided()
    {

    }
}

public enum SceneType
{
    /// <summary> 登陆界面 </summary>
    SceneLogin,
    /// <summary> 主界面 </summary>
    SceneHome,
    /// <summary> 加载 </summary>
    SceneLoding,
    /// <summary> 邮件 </summary>
    SceneMail,
    /// <summary> 聊天 </summary>
    SceneChat,
    /// <summary> 商城 </summary>
    SceneShop,

    SceneScaning,  //扫描

    SceneIntroduce, //产品

    SceneLogo,

    SceneVRHome,

    SceneVRSelect,

    SceneImage
}