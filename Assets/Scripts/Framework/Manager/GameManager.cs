using System.IO;
using XHConfig;
using System.Text;
using LitJson;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Xml;
using UnityEngine.Events;
using System;

namespace LuaFramework
{
    public class GameManager : Manager
    {
        public UserData m_UserData;
        private List<VideoNode> ResourceList;
        private string m_token;
        public string Token
        {
            get
            {
                return m_token;
            }
            set
            {
                if (m_token != value)
                {
                    m_token = value;
                    facade.SendMessageCommand(MessageDef.OpenWebSocket, m_token);   //配置当前websocket
                    facade.SendMessageCommand(MessageDef.ConnectWebSocket);
                }
            }
        }

        private string filePath;
        private string ConfigPath = string.Empty;
        private string VideoDicPath;


        public void CheckNetWork(UnityAction action)
        {
            StartCoroutine(CheckNetWorkIE(action));
        }
        private IEnumerator CheckNetWorkIE(UnityAction action)
        {
            while (true)
            {
                if (!Util.NetAvailable)
                {
                    yield return null;
                }
                else
                {
                    break;
                }
            }
            Debug.Log("net available");
            yield return null;
            WWW www;
            WaitForSeconds wait = new WaitForSeconds(1f);
            while (true)
            {
                www = new WWW("https://lankam.shop");
                yield return www;
                if (string.IsNullOrEmpty(www.error))
                {
                    Debug.Log("成功");
                    break;
                }
                else
                {
                    Debug.Log(www.error);
                    yield return wait;
                }
            }
            if (action != null)
            {
                action();
            }

        }

        public void ReLogin()
        {
            StartCoroutine(ReLoginIE());
        }
        private IEnumerator ReLoginIE()
        {
            string JsonToken = LoginAccount(AppConst.UserName, AppConst.Password);
            yield return JsonToken;
            string JsonVersion = ClientVersion(JsonToken);
        }

        private void Start()
        {
            StartCoroutine(CheckNetWorkIE(NetWorkConnectAction));
        }

        private void NetWorkConnectAction()
        {
            Debug.Log("start");
            Init();
            UIManager.ShowPanel(PanelType.PanelVideo);
            UIManager.ShowPanel(PanelType.PanelVideoNew);
            UIManager.ShowPanel(PanelType.PanelGoodList);

            StartCoroutine(CheckMoivesResource());
            StartCoroutine(CheckGoodsResource());
        }

        void Init()
        {
            ResourceList = new List<VideoNode>();
            ConfigPath = Util.ConfigDicPath;
            ConfigManager.ResPath = Util.ConfigDicPath;
            if (string.IsNullOrEmpty(ConfigPath))
            {
                return;
            }
            VideoDicPath = Util.VideoDicPath;
            filePath = ConfigPath + "/json/videoConfig.json";
            MeachineItem meachineItem = MeachineConfig.GetMeachine();
            AppConst.AppID = meachineItem.AppId;
            AppConst.MeachineID = meachineItem.MachineId;
            AppConst.UserName = meachineItem.UserName;
            AppConst.Password = meachineItem.Password;
            AppConst.Local = meachineItem.Local;

            if (!AppConst.UserName.Contains(AppConst.MeachineID))
            {
                AppConst.UserName = "lankam" + AppConst.MeachineID;
            }

            DontDestroyOnLoad(gameObject);  //防止销毁自己
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Application.targetFrameRate = AppConst.GameFrameRate;
        }

        #region 玩游戏
        //public List<GameNode> GameList;
        //void GetGameList()
        //{
        //    GameList = GameJson.GetResource();
        //    GameCount = GameList.Count;
        //}

        //private int GameIndex = 0;
        //private int GameCount;
        //public void PlayGame(out double price)
        //{
        //    price = 0f;
        //    string Name = string.Empty;
        //    if (GameCount > 0)
        //    {
        //        GameIndex %= GameCount;
        //        Name = GameList[GameIndex].name;
        //        price = GameList[GameIndex].price;
        //        GameIndex++;
        //    }
        //    if (Name != string.Empty)
        //    {
        //        facade.SendMessageCommand(MessageDef.PlayGame, Name);
        //    }
        //}
        #endregion


        #region 播放视频
        /// <summary>
        /// 售货机注册
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public string RegisterAccount(string userName, string password, string location)
        {
            Encoding encoding = Encoding.UTF8;
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("username", userName);
            parameters.Add("password", password);
            parameters.Add("location", location);
            string retString = HttpWebResponseUtility.CreatePostHttpResponse(AppConst.RegisterUrl, parameters, null, null, encoding, null);
            return retString;
        }

        /// <summary>
        /// 售货机登陆
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public string LoginAccount(string userName, string password)
        {
            Encoding encoding = Encoding.UTF8;
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("username", userName);
            parameters.Add("password", password);
            string retString = HttpWebResponseUtility.CreatePostHttpResponse(AppConst.LoginUrl, parameters, null, null, encoding, null);
            return retString;
        }

        /// <summary>
        /// 当前售货机版本
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public string ClientVersion(string token)
        {
            Token version;
            try
            {
                version = SerializeHelper.DeSerializeJson<Token>(token);  //获取到Token码
            }
            catch (System.Exception e)
            {
                return string.Empty;
            }
            Token = version.token;
            if (string.IsNullOrEmpty(m_token))
            {
                return string.Empty;
            }
            string retString = HttpWebResponseUtility.CreateGetHttpResponse(AppConst.VersionUrl, "", null, null, null, Encoding.UTF8, version.token);
            return retString;
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="fileid"></param>
        /// <returns></returns>
        public string DownLoadFile(string fileid)
        {
            if (string.IsNullOrEmpty(m_token))
            {
                return string.Empty;
            }
            Encoding encoding = Encoding.UTF8;
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("fileid", fileid);
            string retString = HttpWebResponseUtility.CreatePostHttpResponse(AppConst.DownloadUrl, parameters, null, null, encoding, null, m_token);
            return retString;
        }

        /// <summary>
        /// 保存配置文件
        /// </summary>
        /// <param name="account"></param>
        public void SaveDataToLocal(string account)
        {
            string xmlFile = ConfigPath + "/Xml/MeachineConfig.xml";
            if (File.Exists(xmlFile))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlFile);

                XmlNode xn = xmlDoc.SelectSingleNode("MeachineConfig/Content");
                xn.Attributes["UserName"].Value = account;
                xmlDoc.Save(xmlFile);
            }
        }

        IEnumerator CheckMoivesResource()
        {
            string JsonToken = string.Empty;
            try
            {
                JsonToken = LoginAccount(AppConst.UserName, AppConst.Password);
            }
            catch (Exception e)
            {
                ResourceList = LoadVideoFilePath();
                yield break;
            }

            yield return JsonToken;
            Debug.Log("json:" + JsonToken);
            string JsonVersion = ClientVersion(JsonToken);
            Debug.Log("jsonversion:" + JsonVersion);
            if (string.IsNullOrEmpty(JsonVersion))
            {
                //string account = Util.GetSystemInfo();
                string account = "lankam" + AppConst.MeachineID;
                string result = RegisterAccount(account, "123456", "shanghai");
                yield return result;
                if (string.IsNullOrEmpty(result))
                {
                    //没有网络重新登陆
                    Debug.Log("没有网路");
                }
                else
                {
                    if (result.Contains("error"))
                    {
                        SaveDataToLocal(account);
                        AppConst.UserName = account;
                        JsonToken = LoginAccount(AppConst.UserName, AppConst.Password);
                        yield return JsonToken;
                        JsonVersion = ClientVersion(JsonToken);
                        if (!string.IsNullOrEmpty(JsonVersion))
                        {
                            Version ver = SerializeHelper.DeSerializeJson<Version>(JsonVersion);
                            string version = ver.version;
                            if (ver != null)
                            {
                                ResourceList = CheckUpdateVersion(version);  //获取播放列表
                            }
                        }
                    }
                    else
                    {
                        RegisterCode registerCode = SerializeHelper.DeSerializeJson<RegisterCode>(result);
                        if (registerCode.msg != null)
                        {
                            if (registerCode.msg.Equals("OK", System.StringComparison.InvariantCultureIgnoreCase))
                            {
                                //注册成功，将数据保存到本地
                                SaveDataToLocal(account);
                                AppConst.UserName = account;
                                JsonToken = LoginAccount(AppConst.UserName, AppConst.Password);
                                yield return JsonToken;
                                JsonVersion = ClientVersion(JsonToken);
                                if (!string.IsNullOrEmpty(JsonVersion))
                                {
                                    Version ver = SerializeHelper.DeSerializeJson<Version>(JsonVersion);
                                    string version = ver.version;
                                    if (ver != null)
                                    {
                                        ResourceList = CheckUpdateVersion(version);  //获取播放列表
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Version ver = SerializeHelper.DeSerializeJson<Version>(JsonVersion);
                string version = ver.version;
                if (ver != null)
                {
                    ResourceList = CheckUpdateVersion(version);  //获取播放列表
                }
            }
            ResourecStartPlay();
        }

        private void ResourecStartPlay()
        {
            if (ResourceList == null)
            {
                return;
            }
            if (ResourceList.Count > 0)
            {
                #region 检查本地文件
                for (int i = ResourceList.Count - 1; i >= 0; i--)
                {
                    if (ResourceList[i].type == "video" || ResourceList[i].type == "picture")
                    {
                        string FileName = ResourceList[i].name;
                        string path = Path.Combine(VideoDicPath, FileName);
                        if (File.Exists(path))
                        {
                            if (FileName.Contains("_Idler"))
                            {
                                m_IdlertimeVideoList.Add(ResourceList[i]);
                            }
                            else
                            {
                                m_BusyTimeVideoList.Add(ResourceList[i]);
                            }
                        }
                    }
                    else if (ResourceList[i].type == "game")
                    {
                        string FileName = ResourceList[i].name;
                        string AppPath = Application.dataPath + "/../../Games/" + FileName + "\\" + FileName + ".exe";
                        AppPath = AppPath.Replace('\\', '/');
                        if (File.Exists(AppPath))
                        {
                            if (!m_GameDictionary.ContainsKey(ResourceList[i].id))
                            {
                                m_GameDictionary.Add(ResourceList[i].id, ResourceList[i]);
                                m_GameList.Add(ResourceList[i]);
                            }
                        }
                    }
                }
                #endregion

                #region 广告排序（待定）
                //开始针对广告排序
                m_BusyTimeVideoList.Sort((left, right) =>
                {
                    int leftid = int.Parse(left.id);
                    int rightid = int.Parse(right.id);
                    if (leftid > rightid)
                        return 1;
                    else
                        return -1;
                });
                #endregion

                facade.SendMessageCommand(MessageDef.VideoPlay/*, m_VideoList*/);
            }
        }


        private List<VideoNode> m_GameList = new List<VideoNode>();
        private List<VideoNode> m_BusyTimeVideoList = new List<VideoNode>();
        private List<VideoNode> m_IdlertimeVideoList = new List<VideoNode>();
        private Dictionary<string, VideoNode> m_GameDictionary = new Dictionary<string, VideoNode>();

        public List<VideoNode> GameList
        {
            get
            {
                return m_GameList;
            }
        }
        public List<VideoNode> BusyTimeVideoList
        {
            get
            {
                return m_BusyTimeVideoList;
            }
        }

        public List<VideoNode> IdlerTimeVideoList
        {
            get
            {
                return m_IdlertimeVideoList;
            }
        }

        private List<VideoNode> CheckUpdateVersion(string ServerVersion)
        {
            List<VideoNode> VideoitemList = new List<VideoNode>();
            string currentVersion = string.Empty;
            if (File.Exists(filePath))
            {
                currentVersion = VideoJson.GetVersion();
                if (currentVersion != ServerVersion)
                {
                    //进行版本跟新
                    VideoitemList = UpdateVersion();
                }
                else
                {
                    //不需要跟新，检测本地文件，开始播放
                    VideoitemList = LoadVideoFilePath();
                }
            }
            else
            {
                VideoitemList = UpdateVersion();
            }
            return VideoitemList;
        }

        private List<VideoNode> LoadVideoFilePath()
        {
            List<VideoNode> VideoList = new List<VideoNode>();
            VideoList = VideoJson.GetResource();
            return VideoList;
        }

        //获取视频播放列表
        private List<VideoNode> UpdateFiles()
        {
            #region XML
            //if (!File.Exists(FilePath))
            //{
            //    return null;
            //}
            //if (HttpHelper.HttpDownload(LoadPath, DownLoadPath))
            //{
            //    VideoItem[] items = VideoConfig.GetVideos();
            //    VideoitemList = items.ToList<VideoItem>();
            //    if (VideoitemList.Count > 0)
            //    {
            //        return VideoitemList;
            //    }
            //}
            //return null;
            #endregion
            string filePath = ConfigPath + "/json/videoConfig.json";
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            string result = HttpWebResponseUtility.CreateGetHttpResponse(AppConst.VersionUrl, "", null, null, null, Encoding.UTF8, m_token);
            using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(result);
                }
            }
            return null;
        }

        private List<VideoNode> UpdateVersion()
        {
            List<VideoNode> VideoList = new List<VideoNode>();
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            string result = HttpWebResponseUtility.CreateGetHttpResponse(AppConst.ResourcelistUrl, "", null, null, null, Encoding.UTF8, m_token);

            //UnityEngine.Debug.Log(result);
            using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    sw.Write(result);
                }
            }
            if (File.Exists(filePath))
            {
                VideoList = VideoJson.GetResource();
                return VideoList;
            }
            return null;
        }
        #endregion


        #region Update
        bool hasTouch = false;
        float timeOver = 1200f;
        public float timeTouch = 600;
        private void Update()
        {
            timeOver -= Time.deltaTime;
            if (timeOver <= 0)
            {
                GoodsResource();

            }

            if (Input.GetMouseButtonDown(0))
            {
                if (IdlerTimeVideoList.Count > 0)
                {
                    timeTouch = 600;
                    hasTouch = true;
                }
            }
            if (hasTouch)
            {
                timeTouch -= Time.deltaTime;
                if (timeTouch <= 0)
                {
                    facade.SendMessageCommand(MessageDef.VideoPlay);
                    hasTouch = false;
                }
            }
        }
        #endregion




        #region 获取商品列表
        public GoodsItem LastSelectItem;
        public void GoodsResource()
        {
            timeOver = 1200f;
            StartCoroutine(CheckGoodsResource());
        }

        private void MoviesResource()
        {
            string JsonVersion = ClientVersion(Token);
            if (!string.IsNullOrEmpty(JsonVersion))
            {
                Version ver = SerializeHelper.DeSerializeJson<Version>(JsonVersion);
                string version = ver.version;
                if (ver != null)
                {
                    ResourceList = CheckUpdateVersion(version);  //获取播放列表
                }
            }
        }

        IEnumerator CheckGoodsResource()
        {
            string appId = AppConst.AppID;
            string meachineId = AppConst.MeachineID;
            string key = AppConst.key;
            string sign = Util.GetSign(new List<string> { appId, meachineId, key });
            string josn = string.Empty;
            try
            {
                josn = ClientGoods(appId, meachineId, sign);
            }
            catch (Exception)
            {
                yield break;
            }
            yield return josn;
            GoodsJson goodsJson = JsonMapper.ToObject<GoodsJson>(josn);  //解析Json文件
            if (goodsJson != null)
            {
                facade.SendMessageCommand(MessageDef.GoodsInfomation, goodsJson.data);
            }
        }

        public string ClientGoods(string appId, string MachineId, string sign)
        {
            Encoding encoding = Encoding.UTF8;
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("appId", appId);
            parameters.Add("MachineId", MachineId);
            parameters.Add("Sign", sign);
            string retString = HttpWebResponseUtility.CreatePostHttpResponse(AppConst.GoodsUrl, parameters, null, null, encoding, null);
            return retString;
        }
        #endregion


        #region 获取游戏列表

        #endregion


        #region 获取微信公众号

        public void LoadQR(UnityAction<Texture2D> action, string QrcodeType)
        {
            StartCoroutine(LoadQRFormserver(action, QrcodeType));
        }

        IEnumerator LoadQRFormserver(UnityAction<Texture2D> action, string QrcodeType)
        {
            string result = "";
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    result = QRUrl(GameManager.m_token, QrcodeType);
                    Debug.Log("获取result地址:" + result);
                }
                catch (Exception e)
                {
                    continue;
                }
                yield return result;
                if (result != "")
                {
                    if (result.Contains("error"))
                    {
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    if (i == 6)
                    {
                        string JsonToken = LoginAccount(AppConst.UserName, AppConst.Password);
                        yield return JsonToken;
                        string JsonVersion = ClientVersion(JsonToken);
                        if (string.IsNullOrEmpty(JsonVersion))
                        {
                            yield break;
                        }
                    }
                }
            }
            using (WWW www = new WWW(result))
            {
                yield return www;
                if (www.error == null)
                {
                    if (action != null)
                    {
                        action(www.texture);
                    }
                }
            }
        }
        public string QRUrl(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return string.Empty;
            }
            string retString = HttpWebResponseUtility.CreateGetHttpResponse(AppConst.LankamQRUrl, "", null, null, null, Encoding.UTF8, token);
            return retString;
        }
        public string QRUrl(string token, string QrcodeType/* string param = "", string res = "" */)
        {
            if (string.IsNullOrEmpty(token))
            {
                return string.Empty;
            }
            string data = "QrcodeType=" + QrcodeType;
            string retString = HttpWebResponseUtility.CreateGetHttpResponse(AppConst.LankamQRUrl, data, null, null, null, Encoding.UTF8, token);
            return retString;
        }


        public void UpLoadResult()
        {
            StartCoroutine(UpLoadGameResult());
        }

        IEnumerator UpLoadGameResult()
        {
            if (m_UserData != null)
            {
                string data = "weixinaccount=" + m_UserData.openid;
                string result = GameTimes(m_token, data);
                yield return result;
                m_UserData = null;
            }
        }
        //上传接口
        public string GameTimes(string token, string data)
        {
            if (string.IsNullOrEmpty(token))
            {
                return string.Empty;
            }
            string retString = HttpWebResponseUtility.CreateGetHttpResponse(AppConst.LankamGameResultUrl, data, null, null, null, Encoding.UTF8, token);
            return retString;
        }

        #endregion


        #region 获取当前商品游戏列表
        public void GetGoods_GameList(string token, double goodsid)
        {
            StartCoroutine(UpLoadGamePrice(token, goodsid));
        }

        public string Goods_GameList(string token, double goodsid)
        {
            if (string.IsNullOrEmpty(token))
            {
                return string.Empty;
            }
            Encoding encoding = Encoding.UTF8;
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("goodsid", goodsid.ToString());
            string retString = HttpWebResponseUtility.CreatePostHttpResponse(AppConst.GamePayUrl, parameters, null, null, encoding, null, token);
            return retString;
        }
        IEnumerator UpLoadGamePrice(string token, double goodsid)
        {
            m_GameList.Clear();
            string result = Goods_GameList(token, goodsid);
            yield return result;
            //{ "gamenum": "4", "gameinfo": { "2002": "0.4", "2003": "0.4", "2001": "0.4", "2004": "0.4"} }
            //Debug.Log(result);
            if (result.Contains("error"))
            {
                yield break;
            }
            GoodsGame goodsJson = JsonMapper.ToObject<GoodsGame>(result);  //解析Json文件
            if (goodsJson != null)
            {
                foreach (string item in goodsJson.gameinfo.Keys)
                {
                    if (m_GameDictionary.ContainsKey(item))
                    {
                        m_GameList.Add(m_GameDictionary[item]);
                    }
                }
            }
            yield return null;
        }
        #endregion

    }




    #region Class
    public class GoodsGame
    {
        public String gamenum { get; set; }

        public Dictionary<string, string> gameinfo { get; set; }

    }

    public class SerializeHelper
    {
        public static T DeSerializeJson<T>(string json)
        {
            try
            {
                return JsonMapper.ToObject<T>(json);
            }
            catch (System.Exception)
            {
                return default(T);
            }

        }
    }

    public class Token
    {
        public string token { get; set; }
    }

    public class Version
    {
        public string version { get; set; }
    }

    public class RegisterCode
    {
        public string code { get; set; }
        public string msg { get; set; }
    }
    #endregion
}