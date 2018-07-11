using LitJson;
using LuaFramework;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using XHConfig;

public class PanelPay : PanelBase
{
    private string appId;
    private string meachineId;
    private string GUID;
    private string key;
    private void Awake()
    {
        appId = AppConst.AppID;
        meachineId = AppConst.MeachineID;
        key = AppConst.key;
    }


    #region 初始化相关

    protected override void OnInitSkin()
    {
        base.SetMainSkinPath("UI/PanelPay");
        base.OnInitSkin();

        _type = PanelType.PanelPay;
        _showStyle = UIManager.PanelShowStyle.Nomal;
    }

    private RawImage GoodsPicture;
    private RawImage QRPicture;

    private RawImage Success;
    private RawImage Fail;
    private GameObject PanelResult;

    private Text GoodsGoldPrice;
    private Text GoodsPrice;
    private Text GoodsName;
    private Text TimeLine;
    protected override void OnInitDone()
    {
        base.OnInitDone();

        PanelResult = skinTransform.Find("BG/PanelResult").gameObject;
        Success = skinTransform.Find("BG/PanelResult/Success").GetComponent<RawImage>();
        Fail = skinTransform.Find("BG/PanelResult/Fail").GetComponent<RawImage>();
        GoodsGoldPrice = skinTransform.Find("BG/GoodsGoldPrice").GetComponent<Text>();
        GoodsPicture = skinTransform.Find("BG/GoodsBG/GoodsPic").GetComponent<RawImage>();
        QRPicture = skinTransform.Find("BG/Content/GoodsPicture").GetComponent<RawImage>();
        GoodsPrice = skinTransform.Find("BG/GoodsPrice").GetComponent<Text>();
        GoodsName = skinTransform.Find("BG/GoodsName").GetComponent<Text>();
        TimeLine = skinTransform.Find("BG/Time").GetComponent<Text>();

        PanelResult.SetActive(false);

        #region 生成二维码
        //double id = (double)panelArgs[0];
        //string goodName = panelArgs[1].ToString();
        //double price = (double)panelArgs[2];

        GUID = panelArgs[0].ToString();
        float goldPrice = (float)panelArgs[1];
        canPlay = (bool)panelArgs[2];

        GoodsItem item = GameManager.LastSelectItem;
        double id = item.productId;
        ShowGoods(item, goldPrice);

        TimeLine.text = Times.ToString();
        Texture2D t = CreateQR(id.ToString(), meachineId, GUID, canPlay);
        QRPicture.texture = t;
        #endregion

        Times = 60;
        ClickPayButton = true;
    }
    private bool canPlay = false;       //判断是否玩游戏了
    private bool PaySuccessful = false; //判断是否支付成功

    public void ShowGoods(GoodsItem data, float price)
    {
        string name = Path.GetFileName(data.image);
        string url = Application.streamingAssetsPath + "/GoodsData/" + name;
        if (File.Exists(url))
        {
            Texture2D t = Util.LoadByIO(url);
            GoodsPicture.texture = t;
            GoodsName.text = "名称:" + data.productName;
            if (price == 0)
            {
                GoodsGoldPrice.text = "未获得优惠";
            }
            else
            {
                GoodsGoldPrice.text = "已优惠:" + price.ToString();
            }
            GoodsPrice.text = "￥:" + ((float)(data.price) - price).ToString();
        }
    }

    protected override void OnClick(GameObject click)
    {
        base.OnClick(click);
        ClickButton(click);
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
                    ClickPayButton = false;
                    PaySuccessful = false;
                    Close();
                }
                chackNum++;
                if (chackNum >= 8)
                {
                    chackNum = 0;
                    string result = ChackResult(appId, meachineId, GUID);
                    Shipment shipment = JsonMapper.ToObject<Shipment>(result);
                    if (shipment.errCode == 0)
                    {
                        if (shipment.rec == 1) //出货成功
                        {
                            ClickPayButton = false;
                            PaySuccessful = true;
                            StartCoroutine(ShowPayResult(true));
                        }
                        else if (shipment.rec == -1) //出货失败
                        {
                            ClickPayButton = false;
                            PaySuccessful = true;
                            StartCoroutine(ShowPayResult(false));
                        }
                    }
                }
            }
        }
    }

    IEnumerator ShowPayResult(bool successful)
    {
        PanelResult.SetActive(true);
        if (successful)
        {
            Success.enabled = true;
            Fail.enabled = false;
        }
        else
        {
            Success.enabled = false;
            Fail.enabled = true;
        }
        yield return new WaitForSeconds(4);
        chackNum = 0;
        Close();
    }
    #endregion



    #region 查询出货记录

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
        return retString;
    }
    Texture2D CreateQR(string id, string meachineId, string orderId, bool PlayGame = false)
    {
        StringBuilder sb = new StringBuilder();
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
        return Util.GetQRTexture(LineUrl);
    }
    #endregion


    #region 点击事件
    public void ClickButton(GameObject click)
    {
        if (click.name.Equals("Button_Close"))
        {
            Close();
        }
    }
    #endregion


    protected override void Close()
    {
        if (canPlay && !PaySuccessful)
        {
            //上传当前用户游戏次数
            GameManager.UpLoadResult();
        }
        if (PaySuccessful)
        {
            Debug.Log("重新获取数据");
            GameManager.GoodsResource();
        }
        base.Close();
    }
}
class Shipment
{
    public int errCode { get; set; }
    public int rec { get; set; }
    public string msg { get; set; }
}
class PostGUID
{
    public int errCode { get; set; }
    public string msg { get; set; }
}
