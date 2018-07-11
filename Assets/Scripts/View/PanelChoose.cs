using LitJson;
using LuaFramework;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using XHConfig;

public class PanelChoose : PanelBase
{
    #region 消息注册
    List<string> MessageList
    {
        get
        {
            return new List<string>()
            {
                MessageDef.ChooseGoods
                //MessageDef.AttentionSuccess
            };
        }
    }

    void Awake()
    {
        RemoveMessage(this, MessageList);
        RegisterMessage(this, MessageList);
    }
    protected override void OnDestroyFront()
    {
        base.OnDestroyFront();
        RemoveMessage(this, MessageList);
    }

    #endregion

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
    #region 初始化相关

    protected override void OnInitSkin()
    {
        base.SetMainSkinPath("UI/PanelChoose");
        base.OnInitSkin();

        _type = PanelType.PanelChoose;
        _showStyle = UIManager.PanelShowStyle.Nomal;
    }


    public RawImage GoodsPic;
    public Text GoodsName;
    public Text GoodsPrice;
    private Text TimeLine;
    protected override void OnInitDone()
    {
        base.OnInitDone();

        GoodsPic = skinTransform.Find("BG/GoodsBG/GoodsPic").GetComponent<RawImage>();
        GoodsName = skinTransform.Find("BG/GoodsName").GetComponent<Text>();
        GoodsPrice = skinTransform.Find("BG/GoodsPrice").GetComponent<Text>();
        TimeLine = skinTransform.Find("BG/Time").GetComponent<Text>();

        TimeLine.text = Times.ToString();
        HasClick = false;
        ClickPayButton = true;
        key = AppConst.key;


         t = (Texture)panelArgs[0];
    }


    protected override void OnClick(GameObject click)
    {
        base.OnClick(click);
        ClickButton(click);
    }

    #endregion


    #region 点击事件
    public void ClickButton(GameObject click)
    {
        if (click.name.Equals("Button_Close"))
        {
            Close();
        }
        else if (click.name.Equals("Button_Buy"))
        {
            ClickBuyButton();
        }
        else if (click.name.Equals("Button_Play"))
        {
            ClickGameButton();
        }
    }

    private void ClickBuyButton()
    {
        //生成二维码
        if (LastSelected != null)
        {
            UIManager.ShowPanel(PanelType.PanelPay, Util.GetGUID(), (float)0, false);
            Close();
        }
    }

    bool HasClick = false;
    private void ClickGameButton()
    {
        UIManager.ShowPanel(PanelType.PanelGameList, LastSelected);
        Close();
    }
    #endregion


    #region 接受消息
    public override void OnMessage(IMessage message)
    {
        string name = message.Name;
        switch (name)
        {
            case MessageDef.ChooseGoods:
                GoodsItem body = (GoodsItem)message.Body;
                ShowGoods(body);
                break;
                //case MessageDef.AttentionSuccess:
                //    AttentionSuccess(message);
                //    break;
        }
    }


    //void AttentionSuccess(IMessage message)
    //{
    //    string ReceiveMsg = message.Body.ToString();
    //    AttentionData userData = SerializeHelper.DeSerializeJson<AttentionData>(ReceiveMsg);
    //    if (userData != null)
    //    {
    //        if (userData.discounttimes == "0")
    //        {
    //            GUID = Util.GetGUID(); //生成新的GUID
    //            string result = "";
    //            double goldPrice = 0.5f;
    //            result = PostGUID(appId, meachineId, GUID, goldPrice);
    //            if (result != string.Empty)
    //            {
    //                PostGUID post = JsonMapper.ToObject<PostGUID>(result);
    //                if (post.errCode == 0)
    //                {
    //                    UIManager.ShowPanel(PanelType.PanelPay, GUID, goldPrice, true);
    //                    Close();
    //                }
    //            }
    //        }
    //    }
    //}

    public string PostGUID(string appId, string meachineId, string orderId, double Decimal)
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

    private GoodsItem LastSelected; //上次选择的商品
    private Texture t;
    public void ShowGoods(GoodsItem data)
    {
        string name = Path.GetFileName(data.image);
        string url = Application.streamingAssetsPath + "/GoodsData/" + name;
        GoodsPic.texture = t;
        GoodsName.text = "名称:" + data.productName;
        GoodsPrice.text = "￥:" + data.price.ToString();
        if (LastSelected!=data)
        {
            LastSelected = data;
        }
        //if (File.Exists(url))
        //{
        //    t = Util.LoadByIO(url);
        //    if (LastSelected != data)
        //    {
        //        LastSelected = data;
        //        GoodsPic.texture = t;
        //        GoodsName.text = "名称:" + data.productName;
        //        GoodsPrice.text = "￥:" + data.price.ToString();
        //    }
        //}

        //获取当前商品的游戏列表
        //GameManager.GetGoods_GameList(GameManager.Token, data.productId);
    }
    #endregion

    #region 记时间
    private bool ClickPayButton = false;
    private float timeChack = 1;
    private int chackNum = 0;
    private int Times = 60;
    private void FixedUpdate()
    {
        if (ClickPayButton)
        {
            //查询是否出货成功 60秒内 每5秒查询一次
            timeChack -= Time.fixedDeltaTime;
            if (timeChack <= 0)
            {
                timeChack = 1;
                Times -= 1;
                TimeLine.text = Times.ToString();
                if (Times <= 0)
                {
                    Close();
                }
            }
        }
    }
    #endregion

    protected override void OnDestroyDone()
    {
        //Resources.UnloadAsset(t);
    }

}

public class AttentionData
{
    public string username { get; set; }
    public string openid { get; set; }
    public string msgtype { get; set; }
    public string discounttimes { get; set; }
}
