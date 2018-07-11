using LuaFramework;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GoodsInfomation : MonoBehaviour
{
    [HideInInspector]
    public RawImage rawimge;
    [HideInInspector]
    public Text text_price;
    [HideInInspector]
    public Text text_name;
    [HideInInspector]
    public Button button;
    [HideInInspector]
    public RawImage Sealed;

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
    private string price;
    public string Price
    {
        get
        {
            return price;
        }
        set
        {
            if (price == value)
            {
                return;
            }
            DispatchValueUpdateEvent("price", price, value);
            price = value;
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
        rawimge = transform.Find("RawImage").GetComponent<RawImage>();
        text_price = transform.Find("Text_Price").GetComponent<Text>();
        //text_name = transform.Find("Text_Name").GetComponent<Text>();
        Sealed = transform.Find("Sealed").GetComponent<RawImage>();

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

    public void SetDate(string imageFile, double price, string goodName, double stock, UnityAction<GameObject> callback)
    {
        this.ImageFile = imageFile;
        this.Price = price.ToString();
        this.GoodName = goodName;
        if (stock > 0)
        {
            button.interactable = true;
            Sealed.enabled = false;
        }
        else
        {
            Sealed.enabled = true;
            button.interactable = false;
        }
        ClickFunction += callback;
    }

    public void DispatchValueUpdateEvent(string key, object oldValue, object newValue)
    {
        switch (key)
        {
            case "imageFile":
                string url = newValue.ToString();
                string GoodsName = Path.GetFileName(url);
                string localurl = Application.streamingAssetsPath + "/GoodsData/" + GoodsName;
                if (File.Exists(localurl))
                {
                    Texture2D t = Util.LoadByIO(localurl);
                    rawimge.texture = t;
                }
                else
                {
                    StartCoroutine(LoadPicture(url));
                }
                break;
            case "price":
                text_price.text = newValue.ToString();
                break;
            case "goodName":
                //text_name.text = newValue.ToString();
                break;
        }
    }



    IEnumerator LoadPicture(string filePath)
    {
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
