using LuaFramework;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using LitJson;
using XHConfig;
using UnityEngine.UI;
using System.Collections;
using System;

public class PanelGameList : PanelBase
{

    #region 消息注册
    List<string> MessageList
    {
        get
        {
            return new List<string>()
            {
                MessageDef.GamesInfomation,
                MessageDef.GameResult,
                MessageDef.ReceiveLoginMsg
            };
        }
    }
    void Awake()
    {
        RemoveMessage(this, MessageList);
        RegisterMessage(this, MessageList);
        GoodsDictionary = new Dictionary<string, VideoNode>();
    }
    protected override void OnDestroyFront()
    {
        base.OnDestroyFront();
        RemoveMessage(this, MessageList);
    }
    #endregion

    #region 初始化
    public Transform content;
    public Transform PanelLogin;
    public Transform PanelWaiting;
    private Text TimeLine;
    private RawImage LoginImage;
    private Text Warning;

    //private Dictionary<string, GameNode> GoodsDictionary;
    private Dictionary<string, VideoNode> GoodsDictionary;

    private string appId;
    private string meachineId;
    private string GUID;
    private string key;

    private void Start()
    {
        appId = AppConst.AppID;
        meachineId = AppConst.MeachineID;
        key = AppConst.key;
    }
    protected override void OnInitSkin()
    {
        base.SetMainSkinPath("UI/PanelGameList");
        base.OnInitSkin();

        _type = PanelType.PanelGameList;
        _showStyle = UIManager.PanelShowStyle.Nomal;
    }
    protected override void OnInitSkinDone()
    {
        content = skinTransform.Find("BG/Viewport/Content");
        PanelLogin = skinTransform.Find("BG/PanelLogin");
        LoginImage = skinTransform.Find("BG/PanelLogin/Content/LoginPicture").GetComponent<RawImage>();

        PanelWaiting = skinTransform.Find("BG/PanelWaiting");
        TimeLine = skinTransform.Find("BG/Time").GetComponent<Text>();
        Warning = skinTransform.Find("BG/PanelLogin/Warning").GetComponent<Text>();
    }

    protected override void OnInitDone()
    {
        base.OnInitDone();

        TimeLine.text = Times.ToString();

        PanelWaiting.gameObject.SetActive(false);
        PanelLogin.gameObject.SetActive(false);

        //GameManager.LoadQR((t) => { LoginImage.texture = t; });

        LastSelected = (GoodsItem)panelArgs[0];

        CheckGamesResource();
        facade.SendMessageCommand(MessageDef.ConnectWebSocket);
        ClickPayButton = true;
        Warning.enabled = false;
    }
    protected override void OnClick(GameObject click)
    {
        base.OnClick(click);
        ClickButton(click);
    }

    #endregion

    /// <summary>
    /// 上传当前用户游戏次数
    /// </summary>
    /// <returns></returns>
    //IEnumerator UpLoadGameResult()
    //{
    //    if (userData != null)
    //    {
    //        string data = "weixinaccount=" + userData.openid;
    //        string result = GameTimes(GameManager.m_token, data);
    //        yield return result;
    //    }
    //}
    ////上传接口
    //public string GameTimes(string token, string data)
    //{
    //    if (string.IsNullOrEmpty(token))
    //    {
    //        return string.Empty;
    //    }
    //    string retString = HttpWebResponseUtility.CreateGetHttpResponse(AppConst.LankamGameResultUrl, data, null, null, null, token);
    //    return retString;
    //}



    /// <summary>
    /// 下载微信公众号二维码
    /// </summary>
    /// <returns></returns>
    //IEnumerator LoadQR()
    //{
    //    string result = "";
    //    for (int i = 0; i < 3; i++)
    //    {
    //        try
    //        {
    //            result = QRUrl(GameManager.m_token);
    //        }
    //        catch (Exception)
    //        {
    //            continue;
    //        }
    //        yield return result;
    //        if (result != "")
    //        {
    //            break;
    //        }
    //    }
    //    WWW www = new WWW(result);
    //    yield return www;
    //    if (www.error == null)
    //    {
    //        LoginImage.texture = www.texture;
    //    }
    //}
    ////下载二维码接口
    //public string QRUrl(string token)
    //{
    //    if (string.IsNullOrEmpty(token))
    //    {
    //        return string.Empty;
    //    }
    //    string retString = HttpWebResponseUtility.CreateGetHttpResponse(AppConst.LankamQRUrl, "", null, null, null, token);
    //    return retString;
    //}


    public List<VideoNode> GameList;
    public void CheckGamesResource()
    {
        GameList = GameManager.GameList;
        facade.SendMessageCommand(MessageDef.GamesInfomation, GameList);
    }



    IEnumerator UpLoadGamePrice(string token, string gameid)
    {
        string result = GamePrice(token, gameid, GameManager.LastSelectItem.productId);
        yield return result;
        GamePriceData gamePrice = SerializeHelper.DeSerializeJson<GamePriceData>(result);
        if (gamePrice.price != null)
        {
            float price = float.Parse(gamePrice.price);
            float range = UnityEngine.Random.Range(-1, 2) * 0.1f;
            goldPrice = price + range;
            goldPrice = goldPrice >= 0 ? goldPrice : 0;
        }
    }
    //上传接口
    public string GamePrice(string token, string gameid)
    {
        if (string.IsNullOrEmpty(token))
        {
            return string.Empty;
        }
        Encoding encoding = Encoding.UTF8;
        IDictionary<string, string> parameters = new Dictionary<string, string>();
        parameters.Add("gameid", gameid);
        string retString = HttpWebResponseUtility.CreatePostHttpResponse(AppConst.GamePayUrl, parameters, null, null, encoding, null, token);
        return retString;
    }
    public string GamePrice(string token, string gameid, double goodsid)
    {
        if (string.IsNullOrEmpty(token))
        {
            return string.Empty;
        }
        Encoding encoding = Encoding.UTF8;
        IDictionary<string, string> parameters = new Dictionary<string, string>();
        parameters.Add("goodsid", goodsid.ToString());
        parameters.Add("gameid", gameid);
        string retString = HttpWebResponseUtility.CreatePostHttpResponse(AppConst.GamePriceUrl, parameters, null, null, encoding, null, token);
        return retString;
    }



    //old method
    //public void ClickButton(GameObject click)
    //{
    //    string name = click.name;
    //    VideoNode value;
    //    if (GoodsDictionary.TryGetValue(name, out value))
    //    {
    //        //玩游戏
    //        if (ClickPayButton)
    //        {
    //            ClickPayButton = false;
    //            ReSetTime();
    //            StartCoroutine(UpLoadGamePrice(GameManager.m_token, value.id)); //获取当前游戏ID 上传到服务器并返回
    //            PanelWaiting.gameObject.SetActive(true);  //等待面板
    //            facade.SendMessageCommand(MessageDef.PlayGame, value.name);
    //        }
    //    }

    //    if (click.name.Equals("Button_Close"))
    //    {
    //        Close();
    //    }
    //}
    public void ClickButton(GameObject click)
    {
        string name = click.name;
        VideoNode value;
        if (GoodsDictionary.TryGetValue(name, out value))
        {
            //玩游戏
            if (ClickPayButton)
            {
                ClickPayButton = false;
                ReSetTime();
                GameManager.LoadQR((t) => { LoginImage.texture = t; }, value.name);
                PanelLogin.gameObject.SetActive(true);
                StartCoroutine(UpLoadGamePrice(GameManager.Token, value.id)); //获取当前游戏ID 上传到服务器并返回
                PlayGameValue = value.name;
                //PanelWaiting.gameObject.SetActive(true);  //等待面板
                //facade.SendMessageCommand(MessageDef.PlayGame, value.name);
            }
        }
        if (click.name.Equals("Button_Close"))
        {
            Close();
        }
    }
    private string PlayGameValue;


    #region 接受消息
    public override void OnMessage(IMessage message)
    {
        string name = message.Name;
        switch (name)
        {
            case MessageDef.GamesInfomation:      //更新消息
                List<VideoNode> body = (List<VideoNode>)message.Body;
                GetGamesList(body);
                break;
            case MessageDef.GameResult:
                UpdateResult(message);
                break;
            case MessageDef.ReceiveLoginMsg:
                ReceiveMsg(message);
                break;
        }
    }

    private UserData userData;
    /// <summary>
    /// 获取用户信息，判断游戏次数
    /// </summary>
    /// <param name="message"></param>
    private void ReceiveMsg(IMessage message)
    {
        string ReceiveMsg = message.Body.ToString();
        userData = SerializeHelper.DeSerializeJson<UserData>(ReceiveMsg);
        GameManager.m_UserData = userData;
        if (userData != null)
        {
            int times = int.Parse(userData.gametimes);
            if (times > 0)
            {
                LoginGame = true;
                ReSetTime();
                PanelLogin.gameObject.SetActive(false);
                PanelWaiting.gameObject.SetActive(true);
                MediaPlayerMgr.SetVolume(0);
                facade.SendMessageCommand(MessageDef.PlayGame, PlayGameValue);
            }
            else
            {
                Warning.enabled = true;
            }
        }
    }
    private bool LoginGame = false;



    private Dictionary<string, string> ImageDic = new Dictionary<string, string>();
    /// <summary>
    /// 动态生成游戏列表Icon
    /// </summary>
    /// <param name="data"></param>
    void GetGamesList(GameNode[] data)
    {
        GoodsDictionary.Clear();
        int length = data.Length;
        Transform child = content.GetChild(0);
        int childconut = child.childCount;
        for (int i = 0; i < length; i++)
        {
            Transform obj = child.GetChild(i);
            GameInfomation info = obj.GetComponent<GameInfomation>();
            info.SetDate(data[i].describe, data[i].name);
            // GoodsDictionary.Add(obj.name, data[i]);
        }
    }

    void GetGamesList(List<VideoNode> data)
    {
        GoodsDictionary.Clear();
        int length = data.Count;
        Transform child = content.GetChild(0);
        int childconut = child.childCount;
        for (int i = 0; i < length; i++)
        {
            Transform obj = child.GetChild(i);
            GameInfomation info = obj.GetComponent<GameInfomation>();
            info.SetDate(data[i].describe, data[i].name);
            GoodsDictionary.Add(obj.name, data[i]);
        }
    }


    private float goldPrice = 0;
    private GoodsItem LastSelected;
    /// <summary>
    /// 接受游戏反馈消息，并上传id到服务器获取游戏金额
    /// </summary>
    /// <param name="message"></param>
    public void UpdateResult(IMessage message)
    {
        string data = (string)message.Body;
        if (data == "连接到客户端")
        {
            PanelWaiting.gameObject.SetActive(false);
        }
        else
        {
            GUID = Util.GetGUID(); //生成新的GUID
            string result = "";
            float sendPrice = 0;
            if (data == "成功")
            {
                sendPrice = goldPrice;
                result = PostGUID(appId, meachineId, GUID, goldPrice);
            }
            else if (data == "失败")
            {
                sendPrice = 0;
                result = PostGUID(appId, meachineId, GUID, 0f);
            }
            ServerManager.SendMessageClient("退出"); //关闭游戏
            MediaPlayerMgr.SetVolume(1);
            if (result != string.Empty)
            {
                PostGUID post = JsonMapper.ToObject<PostGUID>(result);
                //生成二维码
                if (post.errCode == 0)
                {
                    UIManager.ShowPanel(PanelType.PanelPay, GUID, sendPrice, true);
                    Close();
                }
            }
        }
    }
    public string PostGUID(string appId, string meachineId, string orderId, float Decimal)
    {
        Encoding encoding = Encoding.UTF8;
        IDictionary<string, string> parameters = new Dictionary<string, string>();
        parameters.Add("appId", appId);
        parameters.Add("machineId", meachineId);
        parameters.Add("orderId", orderId);
        parameters.Add("couponAmt", Decimal.ToString());
        string sign = Util.GetSign(new List<string> { appId, meachineId, orderId, Decimal.ToString(), key });
        parameters.Add("sign", sign);
        string retString = HttpWebResponseUtility.CreatePostHttpResponse(AppConst.CouponAmtUrl, parameters, null, null, encoding, null);
        return retString;
    }


    #endregion



    #region 记时间
    private bool TimeOver = false;
    private bool ClickPayButton = false;
    private float timeChack = 1;
    private int chackNum = 0;
    private int Times = 60;
    private void FixedUpdate()
    {
        if (ClickPayButton)
        {
            timeChack -= Time.fixedDeltaTime;
            if (timeChack <= 0)
            {
                timeChack = 1;
                Times -= 1;
                TimeLine.text = Times.ToString();
                if (Times <= 0)
                {
                    TimeOver = true;
                    Close();
                    ClickPayButton = false;
                }
            }
        }
    }

    private void ReSetTime()
    {
        if (ClickPayButton)
        {
            Times = 60;
        }
    }
    #endregion

    protected override void Close()
    {
        if (TimeOver && LoginGame)
        {
            GameManager.UpLoadResult();
        }
        base.Close();
    }
}

public class UserData
{
    public string username { get; set; }
    public string openid { get; set; }
    public string msgtype { get; set; }
    public string gametimes { get; set; }
}

//{"gamename": "Fruit", "price": "0.1", "gameid": "1"}
public class GamePriceData
{
    public string gamename { get; set; }
    public string price { get; set; }
    public string gameid { get; set; }
}