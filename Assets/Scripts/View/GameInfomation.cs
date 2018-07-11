using LuaFramework;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameInfomation : MonoBehaviour
{

    [HideInInspector]
    public RawImage rawimge;
    [HideInInspector]
    public Text text_name;
    [HideInInspector]
    public Button button;


    private string imageFile;
    public string ImageFile
    {
        get
        {
            return imageFile;
        }
        set
        {
            if (imageFile == value)
            {
                return;
            }
            DispatchValueUpdateEvent("imageFile", imageFile, value);
            imageFile = value;
        }
    }

    private string goodName;
    public string GoodName
    {
        get
        {
            return goodName;
        }
        set
        {
            if (goodName == value)
            {
                return;
            }
            DispatchValueUpdateEvent("goodName", goodName, value);
            goodName = value;
        }
    }

    public UnityAction<GameObject> ClickFunction;
    void Awake()
    {
        rawimge = GetComponentInChildren<RawImage>();
        text_name = transform.Find("Text_Name").GetComponent<Text>();
        button = GetComponent<Button>();
        button.onClick.AddListener(ClickButton);
    }

    void ClickButton()
    {
        if (ClickFunction != null)
        {
            ClickFunction(this.gameObject);
        }
    }

    public void SetDate(string imageFile, string goodName, UnityAction<GameObject> callback = null)
    {
        this.ImageFile = imageFile;
        this.GoodName = goodName;
        if (callback!=null)
        {
            ClickFunction += callback;
        }
    }

    public void DispatchValueUpdateEvent(string key, object oldValue, object newValue)
    {
        switch (key)
        {
            case "imageFile":
                string url = newValue.ToString();
                string GoodsName = Path.GetFileName(url);
                //string localurl = Application.streamingAssetsPath + "/GamesData/" + GoodsName;
                string localurl = Util.GameDicPath + GoodsName;
                //Debug.Log(localurl);
                if (File.Exists(localurl))
                {
                    Texture2D t = Util.LoadByIO(localurl);
                    rawimge.enabled = true;
                    rawimge.texture = t;
                }
                else
                {
                    StartCoroutine(LoadPicture(url));
                }
                break;
            case "goodName":
                text_name.text = newValue.ToString();
                break;
        }
    }



    IEnumerator LoadPicture(string filePath)
    {
        filePath = "/root/Resource/" + filePath;
        using (WWW www = new WWW(filePath))
        {
            yield return www;
            if (string.IsNullOrEmpty(www.error))
            {
                Texture2D t = www.texture;
                rawimge.texture = t;
                LoadConfig.GetInstance().m_downLoader.StartDownload(filePath);
            }
            else
            {
                Debug.Log(www.error);
            }
        }
    }

    private void OnDestroy()
    {
        if (ClickFunction != null)
        {
            ClickFunction = null;
        }
    }
}
