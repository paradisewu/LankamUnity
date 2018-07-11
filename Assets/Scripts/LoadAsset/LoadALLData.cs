using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public delegate void OnDownloadALLBack(string item);
public class LoadALLData
{
    private List<DownLoadALLData> m_listTempLoad = null;
    private List<DownLoadALLData> m_listLoad = null;
    DownLoadALLData data = null;
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

    public bool DoneLoadOver;
    public void Init()
    {
        m_listTempLoad = new List<DownLoadALLData>();
        m_listLoad = new List<DownLoadALLData>();
    }

    public void UpdateDownload()
    {
        //Debug.LogError("开始下载把");
        //return;
        if (m_listTempLoad != null && m_listTempLoad.Count > 0)
        {
            int nCount = m_listTempLoad.Count;
            for (int i = 0; i < nCount; i++)
            {
                //Debug.Log("Temp   "+m_listTempLoad[i].strWgetUrl);
                DownLoadALLData dataTemp = m_listTempLoad[i];
                m_listLoad.Add(dataTemp);
            }
            m_listTempLoad.Clear();
        }
        //Debug.LogError("还剩下多少？" + m_listLoad.Count);
        if (!bStartLoading && m_listLoad.Count == 0)
        {
            DoneLoadOver = true;
            return;
        }
        if (!bStartLoading)
        {
            data = m_listLoad[0];
            m_listLoad.RemoveAt(0);
        }

        if (data == null) return;
        if (data.www == null)
        {
            DoneLoadOver = false;
            if (data.data != null)
            {
                //RevisionData revisionData = data.data as RevisionData;
                //m_CurFileSize = revisionData.revisionFile.size;
            }
            bStartLoading = true;
            string strWgetUrl = data.strServerUrl;
            Debug.LogError(strWgetUrl);
            data.www = new WWW(strWgetUrl);
            //Debug.Log(strWgetUrl);
            data.www.threadPriority = ThreadPriority.High;
            m_fCurProcess = 0;
        }
        if (data.www.error != null)
        {
            Debug.Log(data.www.error + data.strServerUrl);
            //RestartCurUrl();
            data = null;
            bStartLoading = false;
        }
        else if (data.www.isDone && data.www.progress == 1)
        {
            m_fCurProcess = 0;

            OnDoneComplete(data);

            data = null;
            bStartLoading = false;
            time = 10f;
        }
        else
        {
            m_fCurProcess = data.www.progress;
            time -= Time.deltaTime;
            if (time <= 0)
            {
                if (m_fCurProcess >= 0.5f)
                {
                    Debug.Log("继续下载");
                }
                else
                {
                    Debug.LogWarning("资源错误");
                    time = 10;
                    data = null;
                    bStartLoading = false;
                }
            }
        }
    }


    private float time = 10;
    private void OnDoneComplete(DownLoadALLData data)
    {
        LoadFile(data.item, data.www.bytes);
        Debug.LogWarning("下载了成功了。。。。。" + data.item);
    }

    public void RestartCurUrl()
    {
        data.www = null;
        string strUrl = data.strWgetUrl;
        if (!data.bWget || strUrl == "")
        {
            m_nCurCount += 1;
            if (data.failBack != null)
                data.failBack.Method.Invoke(data.failBack.Target, new object[] { data.strFile, data.item });
            data = null;
            return;
        }

        data.www = new WWW(strUrl);
        data.www.threadPriority = ThreadPriority.Low;
        m_fCurProcess = 0;
    }

    public bool StartDownload(string path, bool bLoop = false)
    {
        string item = Path.GetFileName(path);
        if (!File.Exists(DirectoryPath + item))
        {
            DownLoadALLData downLoadData = new DownLoadALLData();
            downLoadData.bWget = true;
            downLoadData.bLoop = bLoop;
            //downLoadData.serverList = servers;

            downLoadData.item = item;
            downLoadData.strFile = path;
            //downLoadData.data = data;
            m_listTempLoad.Add(downLoadData);
            m_nTotalCount += 1;
            //bStartLoading = false;
            return true;
        }
        else
        {
            Debug.LogWarning("不需要下载");
            return false;
        }


    }


    string DirectoryPath = Application.streamingAssetsPath + "/GoodsData/";
    public void LoadFile(string strPath, byte[] data)
    {
        //string FileName = Path.GetFileName(strPath);
        if (!Directory.Exists(DirectoryPath))
        {
            Directory.CreateDirectory(DirectoryPath);
        }
        if (!File.Exists(DirectoryPath + strPath))
        {
            CreateModelFile(DirectoryPath, strPath, data, data.Length);
        }
        else
        {
            Debug.Log("该文件已下载过了");
        }

    }
    void CreateModelFile(string path, string name, byte[] info, int length)
    {
        //文件流信息
        //StreamWriter sw;
        Stream sw;
        FileInfo t = new FileInfo(path + "//" + name);
        if (!t.Exists)
        {
            //如果此文件不存在则创建
            sw = t.Create();
        }
        else
        {
            //如果此文件存在则打开
            //sw = t.Append();
            return;
        }
        //以行的形式写入信息
        //sw.WriteLine(info);
        sw.Write(info, 0, length);
        //关闭流
        sw.Close();
        //销毁流
        sw.Dispose();
    }
    public void DeleteFile(string path)
    {
        if (!File.Exists(path)) return;
        File.Delete(path);
    }

    public void CreateFile(string path, byte[] bytes, Delegate reback = null, DownLoadALLData _data = null)
    {
        //         int index = path.LastIndexOf("/");
        //         Directory.CreateDirectory(path.Substring(0, index));
        //         FileStream fstream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.None, bytes.Length, true);
        //         AsyncState asyncState = new AsyncState { fs = fstream, buffer = bytes, callback = reback, data = _data };
        //         //IAsyncResult asyncResult = fstream.BeginWrite(bytes, 0, bytes.Length, EndWriteCallback, asyncState);
        //         fstream.BeginWrite(bytes, 0, bytes.Length, EndWriteCallback, asyncState);
    }

    //public void OnTextureComplete(DownLoadALLData _data)
    //{
    //    Texture texture = _data.www.texture;
    //    string item = _data.item;
    //    _data.reBack.Method.Invoke(_data.reBack.Target, new object[] { texture, item });
    //}

    //public void OnAssetBundleComplete(DownLoadALLData _data)
    //{

    //    AssetBundle assetbundle = _data.www.assetBundle;
    //    string item = _data.item;
    //    _data.reBack.Method.Invoke(_data.reBack.Target, new object[] { assetbundle, item });
    //}

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

public class DownLoadALLData
{

    public object AssetItem;
    public string item;
    public WWW www = null;
    public string strPersistentPath = "";
    public string strFile = "";

    public bool bLoop = false;
    public object data = null;
    public Delegate reBack = null;
    public Delegate failBack = null;

    public List<string> serverList = new List<string>() { "" };
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
}
