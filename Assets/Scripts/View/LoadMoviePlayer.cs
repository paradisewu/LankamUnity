using LuaFramework;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using XHConfig;
using System;
using RenderHeads.Media.AVProVideo;

public class LoadMoviePlayer : View
{
    private DisplayUGUI vPlayer;
    List<string> MessageList
    {
        get
        {
            return new List<string>()
            {
                NotiConst.UPDATE_MESSAGE,
                NotiConst.PLAYPICTURE,
            };
        }
    }

    private RawImage PicturePlayer;
    void Awake()
    {
        RemoveMessage(this, MessageList);
        RegisterMessage(this, MessageList);
    }

    void Start()
    {
        vPlayer = gameObject.GetComponentInChildren<DisplayUGUI>();
        vPlayer._mediaPlayer = MediaPlayerMgr;

        PicturePlayer = transform.Find("PicturePlayer").gameObject.GetComponent<RawImage>();
    }


    public override void OnMessage(IMessage message)
    {
        string name = message.Name;
        object body = message.Body;
        switch (name)
        {
            case NotiConst.UPDATE_MESSAGE:      //更新消息
                UpdateMessage(body as VideoNode);
                break;
            case NotiConst.PLAYPICTURE:      //更新消息
                UpdatePicture(body as VideoNode);
                break;
        }
    }

    private void UpdatePicture(VideoNode resourceItem)
    {
        string message = resourceItem.name;
        string url = Application.streamingAssetsPath + "/VideoData/" + resourceItem.name;
        if (!File.Exists(url))
        {
            return;
        }
        PicturePlayer.enabled = true;
        PicturePlayer.texture = Util.LoadByIO(url);
    }

    public void UpdateMessage(VideoNode data)
    {
        string message = data.name;
        string url = Application.streamingAssetsPath + "/VideoData/" + data.name;
        if (!File.Exists(url))
        {
            return;
        }

        vPlayer._mediaPlayer = MediaPlayerMgr;
        PicturePlayer.enabled = false;

        MediaPlayerMgr.OpenVideoFromFile(MediaPlayer.FileLocation.AbsolutePathOrURL, url, true);

        MediaPlayerMgr.Events.AddListener(FinishVideo);

        //vPlayer.url = url;
        //vPlayer.prepareCompleted += Prepared;
        //vPlayer.Prepare();
        //vPlayer.loopPointReached += EndPlayer;
    }

    private void FinishVideo(MediaPlayer media, MediaPlayerEvent.EventType type, ErrorCode error)
    {
        if (type == MediaPlayerEvent.EventType.FinishedPlaying && error == ErrorCode.None)
        {
            media.Stop();
           // SetIndexNum();
        }
    }
}
