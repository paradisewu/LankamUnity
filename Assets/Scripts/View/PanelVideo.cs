using LuaFramework;
using RenderHeads.Media.AVProVideo;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using XHConfig;

public class PanelVideo : PanelBase
{
    protected static int CurrentIndex = 0;
    private RawImage PicturePlayer;
    private DisplayUGUI vPlayer;


    #region 消息注册
    List<string> MessageList
    {
        get
        {
            return new List<string>()
            {
                MessageDef.VideoPlay,
                MessageDef.VideoPlayBusy
            };
        }
    }
    void Awake()
    {
        RemoveMessage(this, MessageList);
        RegisterMessage(this, MessageList);
    }
    private void OnDestroy()
    {
        RemoveMessage(this, MessageList);
    }
    #endregion


    #region 初始化相关
    protected override void OnInitSkin()
    {
        base.SetMainSkinPath("UI/PanelVideo");
        base.OnInitSkin();

        _type = PanelType.PanelVideo;
        _showStyle = UIManager.PanelShowStyle.Nomal;
    }

    protected override void OnInitSkinDone()
    {
        base.OnInitSkinDone();
        vPlayer = gameObject.GetComponentInChildren<DisplayUGUI>();
        vPlayer._mediaPlayer = MediaPlayerMgr;

        PicturePlayer = skinTransform.Find("VideoPlayer/PicturePlayer").gameObject.GetComponent<RawImage>();
    }


    protected override void OnInitDone()
    {
        base.OnInitDone();
        //开始播放视频
        vPlayer._mediaPlayer = MediaPlayerMgr;
        MediaPlayerMgr.Events.AddListener(FinishVideo);

        VideoList = GameManager.BusyTimeVideoList;
    }


    protected override void OnClick(GameObject click)
    {
        base.OnClick(click);
        ClickButton(click);
    }
    #endregion

    #region 播放视频
    private List<VideoNode> VideoList;
    public void SetIndexNum()
    {
        if (VideoList != null)
        {
            if (VideoList.Count > 0)
            {
                CurrentIndex %= VideoList.Count;
                PlayVideo(CurrentIndex);
                CurrentIndex++;
            }
        }
    }

    private void PlayVideo(int current)
    {
        VideoNode item = VideoList[current];
        if (string.Equals(item.type, "video"))
        {
            VideoPlay(item);
        }
        else if (string.Equals(item.type, "picture"))
        {
            PicturePlay(item);
            timeOver = 20f;
        }
        else if (string.Equals(item.type, "game"))
        {
            SetIndexNum();
        }
    }
    private float timeOver = 0;
    private void Update()
    {
        if (timeOver > 0)
        {
            timeOver -= Time.deltaTime;
            if (timeOver <= 0)
            {
                SetIndexNum();
            }
        }
    }
    #endregion


    #region 接受消息
    public override void OnMessage(IMessage message)
    {
        string name = message.Name;
        object body = message.Body;
        switch (name)
        {
            case MessageDef.VideoPlayBusy:      //更新消息
                //VideoList = (List<VideoNode>)message.Body;
                SetIndexNum();
                break;
            case MessageDef.VideoPlay:      //更新消息
                MediaPlayerMgr.Stop();
                break;
        }
    }





    private void PicturePlay(VideoNode resourceItem)
    {
        string message = resourceItem.name;
        string url = Util.VideoDicPath + resourceItem.name;
        if (!File.Exists(url))
        {
            return;
        }
        PicturePlayer.enabled = true;
        PicturePlayer.texture = Util.LoadByIO(url);
    }

    public void VideoPlay(VideoNode data)
    {
        string message = data.name;
        string url = Util.VideoDicPath + data.name;
        if (!File.Exists(url))
        {
            return;
        }
        PicturePlayer.enabled = false;
        MediaPlayerMgr.OpenVideoFromFile(MediaPlayer.FileLocation.AbsolutePathOrURL, url, true);
        MediaPlayerMgr.Play();
    }

    private void FinishVideo(MediaPlayer media, MediaPlayerEvent.EventType type, ErrorCode error)
    {
        if (type == MediaPlayerEvent.EventType.FinishedPlaying && error == ErrorCode.None)
        {
            media.Stop();
            SetIndexNum();
        }
    }

    #endregion


    public void ClickButton(GameObject click)
    {
        if (click.name.Equals("Button_Close"))
        {
            Close();
        }
    }

}
