using UnityEngine;
using System.Collections;
using LuaFramework;
using RenderHeads.Media.AVProVideo;

public class StartUpCommand : ControllerCommand
{
    public override void Execute(IMessage message)
    {
        //if (!Util.CheckEnvironment()) return;


        //-----------------关联命令-----------------------
        //AppFacade.Instance.RegisterCommand(NotiConst.DISPATCH_MESSAGE, typeof(SocketCommand));

        //-----------------初始化管理器-----------------------

      
      
        AppFacade.Instance.AddManager<ApplicationServer>(ManagerName.ApplicationServer);

        AppFacade.Instance.AddManager<UIManager>(ManagerName.UIManager);

        AppFacade.Instance.AddManager<MediaPlayer>(ManagerName.MediaPlayer);

        AppFacade.Instance.AddManager<ResourceManager>(ManagerName.Resource);

        //AppFacade.Instance.AddManager<ThreadManager>(ManagerName.Thread);

        AppFacade.Instance.AddManager<GameManager>(ManagerName.GameManager);
       
    }
}