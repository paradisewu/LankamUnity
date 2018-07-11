#region Version Info
/**
	文件名：Downloader
	Author: Kenny
	Time: 2015/5/20 星期三 下午 3:44:55
	Desctription: 
*/
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public delegate void OnDownloadBack(object data, string item);
public class Downloader
{
    private List<DownLoadData> m_listTempLoad = null;
    private List<DownLoadData> m_listLoad = null;
    //private DownLoadData m_strCurErrData = null;
    DownLoadData data = null;
    float _m_fCurProcess = 0f;
    bool bStartLoading = false;

    public void Clear()
    {
        bStartLoading = false;
        m_listTempLoad.Clear();
        m_listLoad.Clear();
    }

    public float m_fCurProcess
    {
        set { _m_fCurProcess = value; }
        get { return _m_fCurProcess; }
    }

    long _m_CurFileSize;
    public long m_CurFileSize
    {
        set { _m_CurFileSize = value; }
        get { return _m_CurFileSize; }
    }

    int _m_nTotalCount = 0;
    public int m_nTotalCount
    {
        set { _m_nTotalCount = value; }
        get { return _m_nTotalCount; }
    }
    int _m_nCurCount = 0;
    public int m_nCurCount
    {
        set { _m_nCurCount = value; }
        get { return _m_nCurCount; }
    }

    public void Init()
    {
        m_listTempLoad = new List<DownLoadData>();
        m_listLoad = new List<DownLoadData>();
    }

    public void UpdateDownload()
    {
        if (m_listTempLoad != null && m_listTempLoad.Count > 0)
        {
            int nCount = m_listTempLoad.Count;
            for (int i = 0; i < nCount; i++)
            {
                //Debug.Log("Temp   "+m_listTempLoad[i].strWgetUrl);
                DownLoadData dataTemp = m_listTempLoad[i];
                m_listLoad.Add(dataTemp);
            }
            m_listTempLoad.Clear();
        }

        if (!bStartLoading && m_listLoad.Count == 0) return;
        if (!bStartLoading)
        {
            data = m_listLoad[0];
            m_listLoad.RemoveAt(0);
        }

        if (data == null) return;
        if (data.www == null)
        {
            if (data.data != null)
            {
                //RevisionData revisionData = data.data as RevisionData;
                //m_CurFileSize = revisionData.revisionFile.size;
            }

            bStartLoading = true;
            string strWgetUrl = data.strServerUrl;
            data.www = new WWW(strWgetUrl);
            //Debug.Log(strWgetUrl);
            data.www.threadPriority = ThreadPriority.High;
            m_fCurProcess = 0;
        }
        if (data.www.error != null)
        {
            //Debug.Log(data.www.error);
            RestartCurUrl();
        }
        else if (data.www.isDone && data.www.progress == 1)
        {
            m_fCurProcess = 0;
            switch (data.type)
            {
                case eDownloadType.Type_Texture:
                    OnTextureComplete(data);
                    break;
                case eDownloadType.Type_AssetBundle:
                    OnAssetBundleComplete(data);
                    break;
            }

            data = null;
            bStartLoading = false;
        }
        else
        {
            m_fCurProcess = data.www.progress;
        }
    }

    public void RestartCurUrl()
    {
        data.www = null;
        string strUrl = data.strWgetUrl;
        if (!data.bWget || strUrl == "")
        {
            m_nCurCount += 1;
            if (data.failBack != null) 
                data.failBack.Method.Invoke(data.failBack.Target, new object[] { data.strFile,data.item});
            data = null;
            return;
        }

        data.www = new WWW(strUrl);
        data.www.threadPriority = ThreadPriority.Low;
        m_fCurProcess = 0;
        //send message:a bundle download start.
    }

    public bool StartDownload(List<string> servers, string strPath, string strFile, string item, object data, eDownloadType type,
        OnDownloadBack back = null, OnDownloadBack faileBack = null, bool bLoop = false)
    {
        DownLoadData downLoadData = new DownLoadData();
        downLoadData.bWget = true;
        downLoadData.bLoop = bLoop;
        downLoadData.serverList = servers;
        downLoadData.item = item;
        if (type == eDownloadType.Type_Texture)
        {
            if (data != null)
                downLoadData.strPersistentPath = strPath + data.ToString() + strFile;
            else
                downLoadData.strPersistentPath = strPath + strFile;
        }
        downLoadData.strFile = strFile;
        downLoadData.type = type;
        downLoadData.data = data;
        downLoadData.reBack = back;
        downLoadData.failBack = faileBack;
        m_listTempLoad.Add(downLoadData);
        m_nTotalCount += 1;
        //bStartLoading = false;
        return true;
    }

    public string LoadFile(string strPath)
    {
        string strData;
        if (File.Exists(strPath))
        {
            StreamReader sr = File.OpenText(strPath);
            strData = sr.ReadToEnd();
            sr.Close();
        }
        else
        {
            WWW rs = new WWW(strPath);
            strData = rs.text;
        }

        return strData;
    }

    public void DeleteFile(string path)
    {
        if (!File.Exists(path)) return;
        File.Delete(path);
    }

    public void CreateFile(string path, byte[] bytes, Delegate reback = null, DownLoadData _data = null)
    {
        //         int index = path.LastIndexOf("/");
        //         Directory.CreateDirectory(path.Substring(0, index));
        //         FileStream fstream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.None, bytes.Length, true);
        //         AsyncState asyncState = new AsyncState { fs = fstream, buffer = bytes, callback = reback, data = _data };
        //         //IAsyncResult asyncResult = fstream.BeginWrite(bytes, 0, bytes.Length, EndWriteCallback, asyncState);
        //         fstream.BeginWrite(bytes, 0, bytes.Length, EndWriteCallback, asyncState);
    }

    public void OnTextureComplete(DownLoadData _data)
    {
        Texture texture = _data.www.texture;
        string item = _data.item;
        _data.reBack.Method.Invoke(_data.reBack.Target, new object[] { texture, item });
    }

    public void OnAssetBundleComplete(DownLoadData _data)
    {
     
        AssetBundle assetbundle = _data.www.assetBundle;
        string item = _data.item;
        _data.reBack.Method.Invoke(_data.reBack.Target, new object[] { assetbundle, item });
    }

    void EndWriteCallback(IAsyncResult asyncResult)
    {
        AsyncState asyncState = (AsyncState)asyncResult.AsyncState;
        asyncState.fs.EndWrite(asyncResult);
        asyncState.fs.Flush();
        asyncState.fs.Close();
        //Debug.Log("EndWrite:" + asyncState.fs.Name);
        //ILog.Debug("put assetZip to tempFold");
        if (asyncState.callback != null)
        {
            asyncState.callback.Method.Invoke(asyncState.callback.Target, new object[] { asyncState.data });
        }
    }

    public void ContinueDownload()
    {
        m_nCurCount += 1;
        bStartLoading = false;
        //m_strCurErrData = null;
    }
}

public class DownLoadData
{
    public object AssetItem;
    public string item;
    public WWW www = null;
    public string strPersistentPath = "";
    public string strFile = "";

    public eDownloadType type = eDownloadType.Type_Unkown;
    public bool bLoop = false;
    public object data = null;
    public Delegate reBack = null;
    public Delegate failBack = null;

    public List<string> serverList = null;
    public bool bWget = true;

    private int iWget = 0;
    private string _strServerUrl = "";
    public string strServerUrl
    {
        get { return _strServerUrl != "" ? _strServerUrl : strWgetUrl; }
        set { _strServerUrl = value; }
    }
    public string strWgetUrl
    {
        get
        {
            if (serverList == null && iWget == 0 && _strServerUrl != "")
            {
                iWget++;
                return _strServerUrl;
            }

            if (serverList == null) return "";
            if (!bLoop)
            {
                if (iWget >= serverList.Count) return "";
            }
            else
            {
                if (iWget >= serverList.Count) iWget = 0;
            }
            _strServerUrl = serverList[iWget] + strFile;
           
            iWget++;
            return _strServerUrl;
        }
    }

    //     public static DownLoadData ParseZipData(string strServerUrl, string strPersistentUrl, string strFile, OnDownloadBack back)
    //     {
    //         DownLoadData downLoadData = new DownLoadData();
    //         downLoadData.strServerUrl = strServerUrl;
    //         downLoadData.strPersistentPath = strPersistentUrl;
    //         downLoadData.strFile = strFile;
    //         downLoadData.reBack = back;
    //         return downLoadData;
    //     }
}

class AsyncState
{
    public FileStream fs { get; set; }
    public object data { get; set; }
    public byte[] buffer { get; set; }

    public Delegate callback;
}

public enum eDownloadType
{
    Type_Unkown = -1,
    Type_Texture,
    Type_AssetBundle
}