using UnityEngine;
using System.Collections;

public class NotiConst
{
    /// <summary>
    /// Controller层消息通知
    /// </summary>
    public const string START_UP = "StartUp";                       //启动框架
    public const string DISPATCH_MESSAGE = "DispatchMessage";       //派发信息

    /// <summary>
    /// View层消息通知
    /// </summary>
    public const string UPDATE_MESSAGE = "UpdateMessage";           //更新消息
    public const string UPDATE_EXTRACT = "UpdateExtract";           //更新解包
    public const string UPDATE_DOWNLOAD = "UpdateDownload";         //更新下载
    public const string UPDATE_PROGRESS = "UpdateProgress";         //更新进度

    public const string UPDATE_GOODSCONFIG = "Updategoodsconfig";

    public const string UPDATE_GOODSPAY = "UpdateGoodsPay";
    //public const string GameResult = "GameResult";
    public const string PLAYGAME = "PlayGame";
    public const string PLAYPICTURE = "PicturePlay";
}


public class MessageDef
{
    public const string GoodsInfomation = "GoodsInfomation";

    public const string GamesInfomation = "GamesInfomation";

    public const string GameResult = "GameResult";

    public const string PlayGame = "PlayGame";

    public const string CloseMeachine = "CloseMeachine";

    public const string VideoPlay = "VideoPlay";
    public const string VideoPlayBusy = "VideoPlayBusy";

    public const string PicturePlay = "PicturePlay";

    public const string ShowGameResult = "ShowGameResult";

    public const string ChooseGoods = "ChooseGoods";

    public const string SelectGoods = "SelectGoods";

    public const string PlayGames = "PlayGames";

    public const string OpenWebSocket = "OpenWebSocket";

    public const string ConnectWebSocket = "ConnectWebSocket";

    public const string ReceiveLoginMsg = "ReceiveLoginMsg";

    public const string AttentionSuccess = "AttentionSuccess";

    public const string AttentionNails = "AttentionNails";
    //public const string ReLoginMsg = "ReceiveLoginMsg";
}