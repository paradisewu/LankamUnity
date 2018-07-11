using UnityEngine;
using LuaFramework;
using System.Collections.Generic;
using RenderHeads.Media.AVProVideo;

public class Base : MonoBehaviour
{
    private AppFacade m_Facade;

    private ResourceManager m_ResMgr;
    private ApplicationServer m_ServerMgr;
    private SoundManager m_SoundMgr;
    private TimerManager m_TimerMgr;
    private ThreadManager m_ThreadMgr;
    private ObjectPoolManager m_ObjectPoolMgr;
    private GameManager m_GameManager;
    private MediaPlayer m_MediaPlayerMgr;
    private UIManager m_UIManager;


    protected UIManager UIManager
    {
        get
        {
            if (m_UIManager == null)
            {
                m_UIManager = facade.GetManager<UIManager>(ManagerName.UIManager);
            }
            return m_UIManager;
        }
    }
    protected GameManager GameManager
    {
        get
        {
            if (m_GameManager == null)
            {
                m_GameManager = facade.GetManager<GameManager>(ManagerName.GameManager);
            }
            return m_GameManager;
        }
    }
   
    protected MediaPlayer MediaPlayerMgr
    {
        get
        {
            if (m_MediaPlayerMgr == null)
            {
                m_MediaPlayerMgr = facade.GetManager<MediaPlayer>(ManagerName.MediaPlayer);
            }
            return m_MediaPlayerMgr;
        }
    }

    /// <summary>
    /// 注册消息
    /// </summary>
    /// <param name="view"></param>
    /// <param name="messages"></param>
    protected void RegisterMessage(IView view, List<string> messages)
    {
        if (messages == null || messages.Count == 0) return;
        Controller.Instance.RegisterViewCommand(view, messages.ToArray());
    }
    /// <summary>
    /// 移除消息
    /// </summary>
    /// <param name="view"></param>
    /// <param name="messages"></param>
    protected void RemoveMessage(IView view, List<string> messages)
    {
        if (messages == null || messages.Count == 0) return;
        Controller.Instance.RemoveViewCommand(view, messages.ToArray());
    }

    protected AppFacade facade
    {
        get
        {
            if (m_Facade == null)
            {
                m_Facade = AppFacade.Instance;
            }
            return m_Facade;
        }
    }

    protected ApplicationServer ServerManager
    {
        get
        {
            if (m_ServerMgr == null)
            {
                m_ServerMgr = facade.GetManager<ApplicationServer>(ManagerName.ApplicationServer);
            }
            return m_ServerMgr;
        }
    }

    protected ResourceManager ResManager
    {
        get
        {
            if (m_ResMgr == null)
            {
                m_ResMgr = facade.GetManager<ResourceManager>(ManagerName.Resource);
            }
            return m_ResMgr;
        }
    }

  

    protected SoundManager SoundManager
    {
        get
        {
            if (m_SoundMgr == null)
            {
                m_SoundMgr = facade.GetManager<SoundManager>(ManagerName.Sound);
            }
            return m_SoundMgr;
        }
    }

    
    protected TimerManager TimerManager
    {
        get
        {
            if (m_TimerMgr == null)
            {
                m_TimerMgr = facade.GetManager<TimerManager>(ManagerName.Timer);
            }
            return m_TimerMgr;
        }
    }

    protected ThreadManager ThreadManager
    {
        get
        {
            if (m_ThreadMgr == null)
            {
                m_ThreadMgr = facade.GetManager<ThreadManager>(ManagerName.Thread);
            }
            return m_ThreadMgr;
        }
    }

    protected ObjectPoolManager ObjPoolManager
    {
        get
        {
            if (m_ObjectPoolMgr == null)
            {
                m_ObjectPoolMgr = facade.GetManager<ObjectPoolManager>(ManagerName.ObjectPool);
            }
            return m_ObjectPoolMgr;
        }
    }
}
