using LuaFramework;
using RenderHeads.Media.AVProVideo;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using XHConfig;
public class PanelVideoNew : PanelBase
{

    private int CurrentIndex = 0;
    private RawImage PicturePlayer;
    private MediaPlayer vPlayer1;
    private MediaPlayer vPlayer2;
    private MediaPlayer vPlayer3;

    #region 消息注册
    List<string> MessageList
    {
        get
        {
            return new List<string>()
            {
                MessageDef.VideoPlay,
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
        base.SetMainSkinPath("UI/PanelVideoNew");
        base.OnInitSkin();

        _type = PanelType.PanelVideoNew;
        _showStyle = UIManager.PanelShowStyle.Nomal;
    }

    protected override void OnInitSkinDone()
    {
        base.OnInitSkinDone();
        //MediaPlayer[] array = gameObject.GetComponentsInChildren<MediaPlayer>();
        //vPlayer1 = array[0];
        //vPlayer2 = array[1];
        //vPlayer3 = array[2];


        vPlayer1 = gameObject.GetComponentInChildren<MediaPlayer>();

        //vPlayer1._mediaPlayer = MediaPlayerMgr;
        PicturePlayer = skinTransform.Find("1/PicturePlayer").gameObject.GetComponent<RawImage>();
    }


    protected override void OnInitDone()
    {
        base.OnInitDone();
        vPlayer1.Events.AddListener(FinishVideo);
        VideoList = GameManager.IdlerTimeVideoList;
        //vPlayer2.Events.AddListener(FinishVideo);
        //vPlayer3.Events.AddListener(FinishVideo);
        //开始播放视频
    }


    protected override void OnClick(GameObject click)
    {
        base.OnClick(click);
        ClickButton(click);
    }
    #endregion

    #region 播放视频
    private List<VideoNode> VideoList;
    public int player1Index = 0;
    public int player2Index = 1;
    public int player3Index = 2;
    public void SetIndexNum(int index)
    {
        if (VideoList != null)
        {
            //if (VideoList.Count > 0)
            //{
            //    CurrentIndex %= VideoList.Count;
            //    PlayVideo(CurrentIndex);
            //    CurrentIndex++;
            //}
            if (VideoList.Count < 2)
            {

            }
            else if (VideoList.Count < 3)
            {

            }
            else
            {
                index += 3;
                index %= VideoList.Count;
                PlayVideo(index);
            }
        }
    }

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
            //if (current % 3 == 0)
            //{
            //    player1Index = current;
            //    VideoPlay(item, vPlayer1);
            //}
            //else if ((current - 1) % 3 == 0)
            //{
            //    player2Index = current;
            //    VideoPlay(item, vPlayer2);
            //}
            //else if ((current + 1) % 3 == 0)
            //{
            //    player3Index = current;
            //    VideoPlay(item, vPlayer3);
            //}
            VideoPlay(item, vPlayer1);
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
            case MessageDef.VideoPlay:      //更新消息
                //VideoList = (List<VideoNode>)message.Body;
                if (VideoList.Count > 0)
                {
                    this.skinTransform.gameObject.SetActive(true);
                }
                else
                {
                    facade.SendMessageCommand(MessageDef.VideoPlayBusy);
                    this.Close();
                }
                SetIndexNum();
                //SetIndexNum(player1Index);
                //SetIndexNum(player2Index);
                //SetIndexNum(player3Index);
                break;
            case MessageDef.VideoPlayBusy:


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

    public void VideoPlay(VideoNode data, MediaPlayer player)
    {
        string message = data.name;
        string url = Util.VideoDicPath + data.name;
        if (!File.Exists(url))
        {
            return;
        }
        PicturePlayer.enabled = false;
        player.OpenVideoFromFile(MediaPlayer.FileLocation.AbsolutePathOrURL, url, true);
        vPlayer1.Play();
    }

    private void FinishVideo(MediaPlayer media, MediaPlayerEvent.EventType type, ErrorCode error)
    {
        if (type == MediaPlayerEvent.EventType.FinishedPlaying && error == ErrorCode.None)
        {
            media.Stop();
            SetIndexNum();

            #region GC
            //int temp = int.Parse(media.gameObject.name);
            //Debug.Log(temp);
            //if (temp == 1)
            //{
            //    SetIndexNum(player1Index);
            //}
            //else if (temp == 2)
            //{
            //    SetIndexNum(player2Index);
            //}
            //else if (temp == 3)
            //{
            //    SetIndexNum(player3Index);
            //}
            #endregion
        }
    }

    #endregion


    public void ClickButton(GameObject click)
    {
        Debug.Log(click.name);
        if (click.name.Equals("Button_Close"))
        {
            //Close();
            vPlayer1.Stop();
            facade.SendMessageCommand(MessageDef.VideoPlayBusy);
            this.skinTransform.gameObject.SetActive(false);
        }
    }

}
