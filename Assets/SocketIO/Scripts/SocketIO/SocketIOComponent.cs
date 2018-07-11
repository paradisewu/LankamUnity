#region License
/*
 * SocketIO.cs
 *
 * The MIT License
 *
 * Copyright (c) 2014 Fabio Panettieri
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

#endregion

//#define SOCKET_IO_DEBUG			// Uncomment this for debug
using LuaFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Net;

namespace SocketIO
{
    public class SocketIOComponent : View
    {
        #region Public Properties

        //private string url = "wss://lankam.shop/websocket/connect?token=d9c6789815dcc320afbd7282fcabdc9f";
        public bool autoConnect = true;
        public int reconnectDelay = 5;
        public float ackExpirationTime = 1800f;
        public float pingInterval = 25f;
        public float pingTimeout = 60f;

        public WebSocket socket { get { return ws; } }
        public string sid { get; set; }
        public bool IsConnected { get { return connected; } }

        #endregion

        #region Private Properties

        private volatile bool connected;
        private volatile bool thPinging;
        private volatile bool thPong;
        private volatile bool wsConnected;

        private Thread socketThread;
        private Thread pingThread;
        private WebSocket ws;

        private Encoder encoder;
        private Decoder decoder;
        private Parser parser;

        private Dictionary<string, List<Action<SocketIOEvent>>> handlers;
        private List<Ack> ackList;

        private int packetId;

        private object eventQueueLock;
        private Queue<SocketIOEvent> eventQueue;

        private object ackQueueLock;
        private Queue<Packet> ackQueue;

        private object MsgQueueLock;
        private Queue<string> MsgQueue;

        #endregion

#if SOCKET_IO_DEBUG
		public Action<string> debugMethod;
#endif

        #region Unity interface

        #region 消息注册
        List<string> MessageList
        {
            get
            {
                return new List<string>()
                {
                    MessageDef.OpenWebSocket,
                    MessageDef.ConnectWebSocket
                };
            }
        }
        void Awake()
        {
            RemoveMessage(this, MessageList);
            RegisterMessage(this, MessageList);

            encoder = new Encoder();
            decoder = new Decoder();
            parser = new Parser();
            handlers = new Dictionary<string, List<Action<SocketIOEvent>>>();
            ackList = new List<Ack>();
            sid = null;
            packetId = 0;
        }
        public override void OnMessage(IMessage message)
        {
            string name = message.Name;
            switch (name)
            {
                case MessageDef.OpenWebSocket:      //更新消息
                    string m_token = message.Body.ToString();
                    InitWebSocket(m_token);
                    break;
                case MessageDef.ConnectWebSocket:      //更新消息
                    StartToConnect();
                    break;
            }
        }
        #endregion


        public void InitWebSocket(string url)
        {
            if (ws != null)
            {
                try
                {
                    ws.Close();
                }
                catch (Exception)
                {

                }
                ws = null;
            }
            string webUrl = AppConst.LankamWebSocket + url;
            ws = new WebSocket(webUrl);
            ws.OnOpen += OnOpen;
            ws.OnMessage += OnMessage;
            ws.OnError += OnError;
            ws.OnClose += OnClose;
            wsConnected = false;

            eventQueueLock = new object();
            eventQueue = new Queue<SocketIOEvent>();

            ackQueueLock = new object();
            ackQueue = new Queue<Packet>();

            MsgQueueLock = new object();
            MsgQueue = new Queue<string>();

            connected = false;

#if SOCKET_IO_DEBUG
			if(debugMethod == null) { debugMethod = Debug.Log; };
#endif
        }

        public void StartToConnect()
        {
            if (socket != null && socket.IsConnected)
            {
                return;
            }
            GameManager.CheckNetWork(() =>
            {
                Connect();
            });
        }

        public void Update()
        {
            if (!connected)
            {
                return;
            }
            lock (eventQueueLock)
            {
                while (eventQueue.Count > 0)
                {
                    EmitEvent(eventQueue.Dequeue());
                }
            }

            lock (ackQueueLock)
            {
                while (ackQueue.Count > 0)
                {
                    InvokeAck(ackQueue.Dequeue());
                }
            }

            lock (MsgQueueLock)
            {
                while (MsgQueue.Count > 0)
                {
                    InvokeMsg(MsgQueue.Dequeue());
                }
            }


            if (wsConnected != ws.IsConnected)
            {
                wsConnected = ws.IsConnected;
                if (wsConnected)
                {
                    EmitEvent("connect");
                }
                else
                {
                    EmitEvent("disconnect");
                }
            }

            // GC expired acks
            if (ackList.Count == 0) { return; }
            if (DateTime.Now.Subtract(ackList[0].time).TotalSeconds < ackExpirationTime) { return; }
            ackList.RemoveAt(0);
        }

        private void InvokeMsg(string packet)
        {
            Debug.Log(packet);
            if (packet.Contains("bad user"))
            {
                //重新登录
                GameManager.ReLogin();
            }
            else if (packet.Contains("GameTimes"))
            {
                //{ "username": "Young &Young", "openid": "WXOPENID_oWt5V06102xXUHAwTQBQUezKq-iU", "msgtype": "GameTimes", "gametimes": "3"}
                facade.SendMessageCommand(MessageDef.ReceiveLoginMsg, packet);
            }
            else if (packet.Contains("LeShanPublish"))
            {
                //{"username": "on0vys2tGh3mTWOqFpWagYMgC2-s", "openid": "on0vys2tGh3mTWOqFpWagYMgC2-s", "msgtype": "LeShanPublish", "discounttimes": "1"}
                facade.SendMessageCommand(MessageDef.AttentionSuccess, packet);
            }
            else if (packet.Contains("RMNails"))
            {
                //{"username": "on0vys2tGh3mTWOqFpWagYMgC2-s", "openid": "on0vys2tGh3mTWOqFpWagYMgC2-s", "msgtype": "LeShanPublish", "discounttimes": "1"}
                facade.SendMessageCommand(MessageDef.AttentionNails, packet);
            }
            else if (packet.Contains("operate"))
            {
                facade.SendMessageCommand(MessageDef.CloseMeachine);
            }
        }

        public void OnDestroy()
        {
            RemoveMessage(this, MessageList);
            if (socketThread != null) { socketThread.Abort(); }
            if (pingThread != null) { pingThread.Abort(); }
        }

        public void OnApplicationQuit()
        {
            Close();
        }

        #endregion

        #region Public Interface

        public void Connect()
        {
            connected = true;
            if (socketThread != null)
            {
                socketThread.Abort();
                socketThread = null;
            }
            socketThread = new Thread(RunSocketThread);
            socketThread.Start(ws);
            if (pingThread != null)
            {
                pingThread.Abort();
                pingThread = null;
            }
            pingThread = new Thread(RunPingThread);
            pingThread.Start(ws);
        }

        public void Close()
        {
            EmitClose();
            connected = false;
        }

        public void On(string ev, Action<SocketIOEvent> callback)
        {
            if (!handlers.ContainsKey(ev))
            {
                handlers[ev] = new List<Action<SocketIOEvent>>();
            }
            handlers[ev].Add(callback);
        }

        public void Off(string ev, Action<SocketIOEvent> callback)
        {
            if (!handlers.ContainsKey(ev))
            {
#if SOCKET_IO_DEBUG
				debugMethod.Invoke("[SocketIO] No callbacks registered for event: " + ev);
#endif
                return;
            }

            List<Action<SocketIOEvent>> l = handlers[ev];
            if (!l.Contains(callback))
            {
#if SOCKET_IO_DEBUG
				debugMethod.Invoke("[SocketIO] Couldn't remove callback action for event: " + ev);
#endif
                return;
            }

            l.Remove(callback);
            if (l.Count == 0)
            {
                handlers.Remove(ev);
            }
        }

        public void Emit(string ev)
        {
            EmitMessage(-1, string.Format("[\"{0}\"]", ev));
        }

        public void Emit(string ev, Action<JSONObject> action)
        {
            EmitMessage(++packetId, string.Format("[\"{0}\"]", ev));
            ackList.Add(new Ack(packetId, action));
        }

        public void Emit(string ev, JSONObject data)
        {
            EmitMessage(-1, string.Format("[\"{0}\",{1}]", ev, data));
        }

        public void Emit(string ev, JSONObject data, Action<JSONObject> action)
        {
            EmitMessage(++packetId, string.Format("[\"{0}\",{1}]", ev, data));
            ackList.Add(new Ack(packetId, action));
        }

        #endregion

        #region Private Methods

        private void RunSocketThread(object obj)
        {
            WebSocket webSocket = (WebSocket)obj;
            while (connected)
            {
                if (webSocket.IsConnected)
                {
                    Thread.Sleep(reconnectDelay);
                }
                else
                {
                    webSocket.Connect();
                }
            }
            webSocket.Close();
        }

        private void RunPingThread(object obj)
        {
            WebSocket webSocket = (WebSocket)obj;

            int timeoutMilis = Mathf.FloorToInt(pingTimeout * 1000);
            int intervalMilis = Mathf.FloorToInt(pingInterval * 1000);

            DateTime pingStart;

            while (connected)
            {
                if (!wsConnected)
                {
                    Thread.Sleep(reconnectDelay);
                }
                else
                {
                    thPinging = true;
                    thPong = false;

                    EmitPacket("@heart");
                    //EmitPacket(new Packet(EnginePacketType.PING));
                    pingStart = DateTime.Now;

                    while (webSocket.IsConnected && thPinging && (DateTime.Now.Subtract(pingStart).TotalSeconds < timeoutMilis))
                    {
                        Thread.Sleep(200);
                    }

                    if (!thPong)
                    {
                        webSocket.Close();
                    }

                    Thread.Sleep(intervalMilis);
                }
            }
        }

        private void EmitMessage(int id, string raw)
        {
            EmitPacket(new Packet(EnginePacketType.MESSAGE, SocketPacketType.EVENT, 0, "/", id, new JSONObject(raw)));
        }

        private void EmitClose()
        {
            EmitPacket(new Packet(EnginePacketType.MESSAGE, SocketPacketType.DISCONNECT, 0, "/", -1, new JSONObject("")));
            EmitPacket(new Packet(EnginePacketType.CLOSE));
        }


        private void EmitPacket(string packet)
        {
#if SOCKET_IO_DEBUG
			debugMethod.Invoke("[SocketIO] " + packet);
#endif
            try
            {
                if (ws != null)
                {
                    ws.Send(packet);
                }
            }
            catch (SocketIOException ex)
            {
#if SOCKET_IO_DEBUG
				debugMethod.Invoke(ex.ToString());
#endif
            }
        }
        private void EmitPacket(Packet packet)
        {
#if SOCKET_IO_DEBUG
			debugMethod.Invoke("[SocketIO] " + packet);
#endif
            try
            {
                if (ws != null)
                {
                    ws.Send(encoder.Encode(packet));
                }
            }
            catch (SocketIOException ex)
            {
#if SOCKET_IO_DEBUG
				debugMethod.Invoke(ex.ToString());
#endif
            }
        }

        private void OnOpen(object sender, EventArgs e)
        {
            EmitEvent("open");
        }

        string resut;
        private void OnMessage(object sender, MessageEventArgs e)
        {
#if SOCKET_IO_DEBUG
			debugMethod.Invoke("[SocketIO] Raw message: " + e.Data);
#endif
            //resut = e.Data;
            //Debug.Log("ws:" + resut);

            //if (resut.Length > 3)
            //{
            //    MsgQueue.Enqueue(resut);
            //}
            //Packet packet = decoder.Decode(e);
            //switch (packet.enginePacketType)
            //{
            //    case EnginePacketType.OPEN: HandleOpen(packet); break;
            //    case EnginePacketType.CLOSE: EmitEvent("close"); break;
            //    case EnginePacketType.PING: HandlePing(); break;
            //    case EnginePacketType.PONG: HandlePong(); break;
            //    case EnginePacketType.MESSAGE: HandleMessage(packet); break;
            //}

            resut = e.Data;
            Debug.Log("ws " + resut);
            switch (resut)
            {
                case "@heart": HandlePong(); break;
            }
            if (resut.Length > 7)
            {
                MsgQueue.Enqueue(resut);
            }
        }

        private void HandleOpen(Packet packet)
        {
#if SOCKET_IO_DEBUG
			debugMethod.Invoke("[SocketIO] Socket.IO sid: " + packet.json["sid"].str);
#endif
            sid = packet.json["sid"].str;
            EmitEvent("open");
        }

        private void HandlePing()
        {
            EmitPacket("@heart");
            //EmitPacket(new Packet(EnginePacketType.PONG));
        }

        float timeEnd = 69f;
        public float timeOver = 69f;
        bool StartTime = false;
        public void FixedUpdate()
        {
            if (StartTime)
            {
                timeOver -= Time.fixedDeltaTime;
                if (timeOver <= 0)
                {
                    StartToConnect();
                    StartTime = false;
                }
            }
        }

        private void HandlePong()
        {
            StartTime = true;
            timeOver = timeEnd;
            thPong = true;
            thPinging = false;
        }

        private void HandleMessage(Packet packet)
        {
            if (packet.json == null) { return; }

            if (packet.socketPacketType == SocketPacketType.ACK)
            {
                for (int i = 0; i < ackList.Count; i++)
                {
                    if (ackList[i].packetId != packet.id) { continue; }
                    lock (ackQueueLock) { ackQueue.Enqueue(packet); }
                    return;
                }

#if SOCKET_IO_DEBUG
				debugMethod.Invoke("[SocketIO] Ack received for invalid Action: " + packet.id);
#endif
            }

            if (packet.socketPacketType == SocketPacketType.EVENT)
            {
                SocketIOEvent e = parser.Parse(packet.json);
                lock (eventQueueLock) { eventQueue.Enqueue(e); }
            }
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            EmitEvent("error");
        }

        private void OnClose(object sender, CloseEventArgs e)
        {
            EmitEvent("close");
        }

        private void EmitEvent(string type)
        {
            EmitEvent(new SocketIOEvent(type));
        }

        private void EmitEvent(SocketIOEvent ev)
        {
            if (!handlers.ContainsKey(ev.name)) { return; }
            foreach (Action<SocketIOEvent> handler in this.handlers[ev.name])
            {
                try
                {
                    handler(ev);
                }
                catch (Exception ex)
                {
#if SOCKET_IO_DEBUG
					debugMethod.Invoke(ex.ToString());
#endif
                }
            }
        }

        private void InvokeAck(Packet packet)
        {
            Ack ack;
            for (int i = 0; i < ackList.Count; i++)
            {
                if (ackList[i].packetId != packet.id) { continue; }
                ack = ackList[i];
                ackList.RemoveAt(i);
                ack.Invoke(packet.json);
                return;
            }
        }

        #endregion
    }
}
