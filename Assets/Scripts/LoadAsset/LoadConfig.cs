using UnityEngine;
using System.Collections;

public class LoadConfig : MonoBehaviour
{
    private static LoadConfig _instance;
    public static LoadConfig GetInstance()
    {
        if (_instance == null)
        {
            _instance = Camera.main.gameObject.AddComponent<LoadConfig>();
        }
        return _instance;
    }

    public LoadALLData m_downLoader;

    //public GameObject LoadPanel;
    public void Awake()
    {
        _instance = this;
        m_downLoader = new LoadALLData();
        m_downLoader.Init();
    }
    public void Update()
    {
        if (m_downLoader == null)
        {
            return;
        }
        m_downLoader.UpdateDownload();

        //if (m_downLoader.DoneLoadOver == false)
        //{
        //    LoadPanel.SetActive(true);
        //}
        //else
        //{
        //    TimeEnding -= Time.deltaTime;
        //    if (TimeEnding <= 0)
        //    {
        //        LoadPanel.SetActive(false);
        //        TimeEnding = 1f;
        //    }
        //}
    }
    //float TimeEnding = 1f;
}
