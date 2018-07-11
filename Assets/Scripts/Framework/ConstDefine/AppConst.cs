using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class AppConst
{
    public const bool DebugMode = false;                       //调试模式-用于内部测试
                                                               /// <summary>
                                                               /// 如果想删掉框架自带的例子，那这个例子模式必须要
                                                               /// 关闭，否则会出现一些错误。
                                                               /// </summary>
    public const bool ExampleMode = true;                       //例子模式 

    /// <summary>
    /// 如果开启更新模式，前提必须启动框架自带服务器端。
    /// 否则就需要自己将StreamingAssets里面的所有内容
    /// 复制到自己的Webserver上面，并修改下面的WebUrl。
    /// </summary>
    public const bool UpdateMode = false;                       //更新模式-默认关闭 
    public const bool LuaByteMode = false;                       //Lua字节码模式-默认关闭 
    public const bool LuaBundleMode = true;                    //Lua代码AssetBundle模式

    public const int TimerInterval = 1;
    public const int GameFrameRate = 60;                        //游戏帧频

    public const string AppName = "LuaFramework";               //应用程序名称
    public const string LuaTempDir = "Lua/";                    //临时目录
    public const string AppPrefix = AppName + "_";              //应用程序前缀
    public const string ExtName = ".unity3d";                   //素材扩展名
    public const string AssetDir = "StreamingAssets";           //素材目录 
    public const string WebUrl = "https://119.23.45.38/";      //测试更新地址


    public const string GoldUrl = "http://api.homson.cn/gate.aspx";
    public const string ResultUrl = "http://www.freesell.cn/apis/sales/result";
    public const string GoodsUrl = "http://www.freesell.cn/apis/machine/products";
    public const string LoginUrl = "https://lankam.shop/login";
    public const string RegisterUrl = "https://lankam.shop/register";
    public const string VersionUrl = "https://lankam.shop/resourcelist/version";
    public const string DownloadUrl = "https://lankam.shop/resource/download";
    public const string ResourcelistUrl = "https://lankam.shop/resourcelist";
    public const string CouponAmtUrl = "http://www.freesell.cn/apis/machine/coupon";

    public const string GamePriceUrl = "https://lankam.shop/game/getpriceinfo";
    public const string GamePayUrl = "https://lankam.shop/game/getgameinfo";
    public const string LankamQRUrl = "https://lankam.shop/weixinServer/getqrcode";  //获取微信公众号的二维码
    public const string LankamGameResultUrl = "https://lankam.shop/weixinServer/user/playedgame"; //向服务器提交用户已经游戏事件
    public const string LankamWebSocket = "wss://lankam.shop/websocket/connect?token=";



    public const string key = "VcAXByUfxX6WtzIm85Pu7xLXFTW2CArM";


    public static string AppID = string.Empty;
    public static string MeachineID = string.Empty;
    public static string UserName = string.Empty;
    public static string Password = string.Empty;
    public static string Local = string.Empty;

    public static string UserId = string.Empty;                 //用户ID
    public static int SocketPort = 0;                           //Socket服务器端口
    public static string SocketAddress = string.Empty;          //Socket服务器地址

    public static string FrameworkRoot
    {
        get
        {
            return Application.dataPath + "/" + AppName;
        }
    }
}
