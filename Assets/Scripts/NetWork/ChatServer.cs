using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityNetwork;
using UnityEngine;
using LuaFramework;

public class ChatServer : NetworkManager
{
    List<Socket> peerList;  // 保存所有的客户端连接
    TCPPeer server;  // 服务器


    public ChatServer()
    {
        peerList = new List<Socket>();   // 创建一个列表保存每个客户端的Socket
    }

    public void StartServer(string ip, int port)
    {
        // 注册事件，本例中只有一个聊天消息
        server = new TCPPeer(this);
        server.Listen(ip, port);
    }

    // 处理服务器接受客户端的连接
    public override void OnAccepted(NetPacket packet)
    {
        Debug.Log("接受新的连接");
        peerList.Add(packet.socket);
        AppFacade.Instance.SendMessageCommand(MessageDef.GameResult, "连接到客户端");
    }

    // 处理丢失连接
    public override void OnLost(NetPacket packet)
    {
        Debug.Log("丢失连接");
        peerList.Remove(packet.socket);
    }

    public override void OnConnected(NetPacket packet)
    {
        Debug.Log("连接上服务器");

       
    }


    public void SendMessage(string msg)
    {
        Chat.ChatProto proto = new Chat.ChatProto();
        proto.userName = "kehudan";
        proto.chatMsg = msg;

        NetPacket p = new NetPacket();
        p.BeginWrite("chat");
        p.WriteObject<Chat.ChatProto>(proto);
        p.EncodeHeader();

        foreach (Socket sk in peerList)
        {
            server.Send(sk, p);
        }
    }

    public override void OnCloseServer()
    {
        //server.ApplicationClose();
        if (server.socket.Connected)
        {
            base.OnCloseServer();
        }
       
    }
}

