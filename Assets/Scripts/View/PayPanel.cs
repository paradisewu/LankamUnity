using LuaFramework;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using XHConfig;
using System;
using System.Text;
using LitJson;

public class PayPanel : View
{

    private RawImage GoodsPic;
    private Text GoodsName;
    private Button BuyButton;
    private Button GameButton;

    private string appId;
    private string meachineId;
    private string key = "VcAXByUfxX6WtzIm85Pu7xLXFTW2CArM";
    List<string> MessageList
    {
        get
        {
            return new List<string>()
            {
                NotiConst.UPDATE_GOODSPAY,
                MessageDef.GameResult,
            };
        }
    }

    void Awake()
    {
        RemoveMessage(this, MessageList);
        RegisterMessage(this, MessageList);

        GoodsPic = Util.Child(this.gameObject, "GoodsPic").GetComponent<RawImage>();
        GoodsName = Util.Child(this.gameObject, "GoodsName").GetComponent<Text>();
        BuyButton = Util.Child(this.gameObject, "BuyButton").GetComponent<Button>();
        GameButton = Util.Child(this.gameObject, "GameButton").GetComponent<Button>();
    }
  
    void Start()
    {
        
        appId = AppConst.AppID;
        meachineId = AppConst.MeachineID;

        BuyButton.onClick.AddListener(ClickBuyButton);
        GameButton.onClick.AddListener(ClickGameButton);
    }

    Texture2D CreateQR(string orderId, bool PlayGame = false)
    {
        StringBuilder sb = new StringBuilder();
        string id = LastSelected.productId.ToString();
        if (!PlayGame)
        {
            sb.Append(AppConst.GoldUrl);
            sb.Append("?id=");
            sb.Append(id);
            sb.Append("x");
            sb.Append(meachineId);
            sb.Append("&orderId=");
            sb.Append(orderId);
            sb.Append("&v=2");
        }
        else
        {
            sb.Append(AppConst.GoldUrl);
            sb.Append("?id=");
            sb.Append(id);
            sb.Append("x");
            sb.Append(meachineId);
            sb.Append("&v=2");
            sb.Append("&orderId=");
            sb.Append(orderId);
        }
        string LineUrl = sb.ToString();
        Debug.Log(LineUrl);
        return Util.GetQRTexture(LineUrl);
    }

    private void ClickBuyButton()
    {
        //生成二维码
        if (LastSelected != null)
        {
            //GameManager.GoodsItemList
            ClickPayButton = true;

            Texture2D t = CreateQR(Util.GetGUID());
            GoodsPic.texture = t;
        }
    }
    string GUID;
    bool ClickPayButton = false;
    float timeChack = 5;
    int chackNum = 0;
    private void Update()
    {
        if (ClickPayButton)
        {
            //查询是否出货成功 60秒内 每5秒查询一次
            timeChack -= Time.fixedDeltaTime;
            if (timeChack <= 0)
            {
                timeChack = 5;
                chackNum++;
                if (chackNum <= 12)
                {
                    string result = ChackResult(appId, meachineId, GUID);
                    Shipment shipment = JsonMapper.ToObject<Shipment>(result);
                    if (shipment.errCode == 0)
                    {
                        if (shipment.rec == 1)
                        {
                            //出货成功
                            ClickPayButton = false;
                            timeChack = 5;
                            chackNum = 0;

                            //关闭当前界面
                        }
                        else if (shipment.rec == -1)
                        {
                            //出货失败
                            ClickPayButton = false;
                            timeChack = 5;
                            chackNum = 0;

                            //关闭当前界面
                        }
                    }
                }
                else
                {

                }
            }
        }
    }
    //进入体感游戏
    private void ClickGameButton()
    {
        facade.SendMessageCommand(NotiConst.PLAYGAME, "wuyang");
    }
    public string ChackResult(string appId, string meachineId, string orderId)
    {
        Encoding encoding = Encoding.UTF8;
        IDictionary<string, string> parameters = new Dictionary<string, string>();
        parameters.Add("appId", appId);
        parameters.Add("machineId", meachineId);
        parameters.Add("orderId", orderId);
        string sign = Util.GetSign(new List<string> { appId, meachineId, orderId, key });
        parameters.Add("Sign", sign);
        string retString = HttpWebResponseUtility.CreatePostHttpResponse(AppConst.ResultUrl, parameters, null, null, encoding, null);
        UnityEngine.Debug.Log(retString);
        return retString;
    }

    public override void OnMessage(IMessage message)
    {
        string name = message.Name;
        switch (name)
        {
            case NotiConst.UPDATE_GOODSPAY:
                GoodsItem body = (GoodsItem)message.Body;
                UpdateMessage(body);
                break;
            case MessageDef.GameResult:
                UpdateResult(message);
                break;
        }
    }

    private GoodsItem LastSelected;
    public void UpdateMessage(GoodsItem data)
    {
        string name = Path.GetFileName(data.image);
        string url = Application.streamingAssetsPath + "/GoodsData/" + name;
        if (File.Exists(url))
        {
            Texture2D t = Util.LoadByIO(url);
            if (LastSelected != data)
            {
                LastSelected = data;
                GoodsPic.texture = t;
                GoodsName.text = data.productName;
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
        UnityEngine.Debug.Log(retString);
        return retString;
    }


    public void UpdateResult(IMessage message)
    {
        string data = (string)message.Body;
        GUID = Util.GetGUID(); //生成新的GUID
        string result = "";
        if (data == "成功")
        {
            //上传guid
            result = PostGUID(appId, meachineId, GUID, 0.4f);
        }
        else if (data == "失败")
        {
            result = PostGUID(appId, meachineId, GUID, 0f);
        }
        ServerManager.SendMessageClient("退出"); //关闭游戏
        if (result != string.Empty)
        {
            PostGUID post = JsonMapper.ToObject<PostGUID>(result);
            //生成二维码
            if (post.errCode == 0)
            {
                Texture t = CreateQR(GUID, true);
                GoodsPic.texture = t;
            }
        }
    }
}

//{
//  "errCode": 0, // 错误代码，只有0时表示正确
//  "rec": 1, // 出货结果，0出货中或订单不存在，1出货成功，-1出货失败
//  "msg": "出货成功" // 描述
//}
//{
//        "errCode": 0, // 错误代码，0为正常
//        "msg": "ok" // 错误描述
//    }

