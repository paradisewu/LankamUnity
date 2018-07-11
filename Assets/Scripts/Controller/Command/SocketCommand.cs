using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LuaFramework;

public class SocketCommand : ControllerCommand
{
    public override void Execute(IMessage message)
    {
        object data = message.Body;
        if (data == null) return;
        KeyValuePair<string, string> buffer = (KeyValuePair<string, string>)data;

        Debug.Log(buffer.Key + "   " + buffer.Value);

        if (buffer.Value == "成功")
        {
            //生成二维码   上传GUID
            AppFacade.Instance.SendMessageCommand(MessageDef.GameResult, "成功");
        }
        else if (buffer.Value == "失败")
        {
            //生成二维码
            AppFacade.Instance.SendMessageCommand(MessageDef.GameResult, "失败");
        }
    }
}
