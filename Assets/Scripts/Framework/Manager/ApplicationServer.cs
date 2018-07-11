using UnityNetwork;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace LuaFramework
{
    public class ApplicationServer : Manager, IView
    {
        private ChatServer server;
        ChatServer SocketClient
        {
            get
            {
                if (server == null)
                    server = new ChatServer();
                return server;
            }
        }

        List<string> MessageList
        {
            get
            {
                return new List<string>()
                {
                     MessageDef.AttentionNails
                };
            }
        }


        void Awake()
        {
            Init();
            RemoveMessage(this, MessageList);
            RegisterMessage(this, MessageList);
        }

        void Init()
        {
            SocketClient.StartServer("127.0.0.1", 10001);
            SocketClient.AddHandler("chat", OnChat);
        }


        public void Update()
        {
            SocketClient.Update();
        }

        public void OnChat(NetPacket packet)
        {
            // 在服务器上显示聊天内容
            Chat.ChatProto proto = packet.ReadObject<Chat.ChatProto>();
            if (proto != null)
            {
                Debug.Log(proto.userName + ":" + proto.chatMsg);
                if (proto.chatMsg == "成功")
                {
                    //生成二维码   上传GUID
                    facade.SendMessageCommand(MessageDef.GameResult, "成功");
                }
                else if (proto.chatMsg == "失败")
                {
                    //生成二维码
                    facade.SendMessageCommand(MessageDef.GameResult, "失败");
                }
            }
        }

        public void SendMessageClient(string msg)
        {
            SocketClient.SendMessage(msg);
        }

        public void OnDestroy()
        {
            RemoveMessage(this, MessageList);
            SocketClient.OnCloseServer();
        }

        public void OnMessage(IMessage message)
        {
            string name = message.Name;
            switch (name)
            {
                case MessageDef.AttentionNails:
                    AttentionNailSuccess();
                    break;
            }
        }

        private void AttentionNailSuccess()
        {
            SendMessageClient("关注nail成功");
        }
    }
}