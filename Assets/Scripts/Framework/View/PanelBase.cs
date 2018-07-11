using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PanelBase : LayerBase
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

    protected PanelType _type;
    /// <summary>
    /// 面板ID
    /// </summary>
    public PanelType type
    {
        get
        {
            return _type;
        }
    }
    /// <summary> 点击背景关闭panel </summary>
    protected bool _isClickMaskColse = true;
    /// <summary> 点击背景关闭panel </summary>
    public bool isClickMaskColse
    {
        get
        {
            return _isClickMaskColse;
        }
        set
        {
            _isClickMaskColse = value;
        }
    }

    /// <summary> 面板显示方式 </summary>
    protected UIManager.PanelShowStyle _showStyle = UIManager.PanelShowStyle.CenterScaleBigNomal;
    /// <summary>
    /// 面板显示方式
    /// </summary>
    public UIManager.PanelShowStyle PanelShowStyle
    {
        get
        {
            return _showStyle;
        }
    }
    /// <summary> 面板遮罩方式 </summary>
    protected UIManager.PanelMaskStyle _maskStyle = UIManager.PanelMaskStyle.BlackAlpha;
    /// <summary> 
    /// 面板遮罩方式
    /// </summary>
    public UIManager.PanelMaskStyle PanelMaskStyle
    {
        get
        {
            return _maskStyle;
        }
    }
    /// <summary> 面板打开时间 </summary>
    protected float _openDuration = 0.2f;
    /// <summary> 面板打开时间 </summary>
    public float OpenDuration
    {
        get
        {
            return _openDuration;
        }
    }


    protected object[] _panelArgs;
    /// <summary>
    /// 记录面板init时参数
    /// </summary>
    public object[] panelArgs
    {
        get
        {
            return _panelArgs;
        }
    }

    /// <summary>
    /// 初始化面板
    /// </summary>
    /// <param name="panelArgs">面板参数</param>
    public virtual void OnInit(params object[] panelArgs)
    {
        _panelArgs = panelArgs;
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
    /// <param name="panelArgs"></param>
    public virtual void OnResetArgs(params object[] panelArgs)
    {
        _panelArgs = panelArgs;
    }
    /// <summary>
    /// 显示面板后
    /// </summary>
    public virtual void OnShowed()
    {

    }

    /// <summary>
    /// 发起关闭
    /// </summary>
    protected virtual void Close()
    {
        UIManager.HidePanel(type);
    }
    /// <summary>
    /// 立刻关闭
    /// </summary>
    protected virtual void CloseImmediate()
    {
        UIManager.DestroyPanel(type);
    }

    public virtual void OnHideFront()
    {
        _cache = false;
    }

    public virtual void OnHideDone()
    {

    }

}
/// <summary>
/// 面板名字列表（用类名来表示）
/// </summary>
public enum PanelType
{
    PanelShop,

    PanelInfomation,

    PanelVideo,

    PanelChoose,

    PanelPay,

    PanelGoodList,

    PanelNoWifi,

    PanelGameList,

    PanelVideoNew
}