using LuaFramework;
using System.Collections.Generic;
using UnityEngine;

public class PanelGoodList : PanelBase
{
    #region 消息注册
    List<string> MessageList
    {
        get
        {
            return new List<string>()
            {
                MessageDef.GoodsInfomation,
            };
        }
    }



    void Awake()
    {
        RemoveMessage(this, MessageList);
        RegisterMessage(this, MessageList);
        GoodsDictionary = new Dictionary<string, GoodsItem>();
        goodItemList = new List<GoodsItem>();
    }

    protected override void OnDestroyFront()
    {
        base.OnDestroyFront();
        RemoveMessage(this, MessageList);
    }

    #endregion

    public Transform content;
    private Dictionary<string, GoodsItem> GoodsDictionary;
    private List<GoodsItem> goodItemList;
    //public horizontalScrollview m_horizontalScrollview;

    #region 初始化
    protected override void OnInitSkin()
    {
        base.SetMainSkinPath("UI/PanelGoodList");
        base.OnInitSkin();
        _type = PanelType.PanelGoodList;
        _showStyle = UIManager.PanelShowStyle.Nomal;

        //m_horizontalScrollview = GetComponentInChildren<horizontalScrollview>();
    }
    protected override void OnInitSkinDone()
    {
        content = skinTransform.Find("BG/Viewport/Content");
    }

    protected override void OnInitDone()
    {
        base.OnInitDone();
    }




    protected override void OnClick(GameObject click)
    {
        base.OnClick(click);
        ClickButton(click);
    }

    #endregion

    public void ClickButton(GameObject click)
    {
        string name = click.name;
        GoodsItem value;
        if (GoodsDictionary.TryGetValue(name, out value))
        {
            if (value.stock > 0)
            {
                UIManager.ShowPanel(PanelType.PanelChoose, click.GetComponent<GoodsInfomation>().rawimge.mainTexture);
                GameManager.LastSelectItem = value;
                facade.SendMessageCommand(MessageDef.ChooseGoods, value);
            }
            else
            {
                //提示无货
            }
        }
        else
        {
            Debug.Log("没有信息");
        }

        if (click.name.Equals("Button_Close"))
        {
            Close();
        }
        //else if (click.name.Equals("RightButton"))
        //{
        //    m_horizontalScrollview.ClickToNext();
        //}
        //else if (click.name.Equals("LeftButton"))
        //{
        //    m_horizontalScrollview.ClickToPrevious();
        //}
    }



    #region 接受消息
    public override void OnMessage(IMessage message)
    {
        string name = message.Name;
        switch (name)
        {
            case MessageDef.GoodsInfomation:      //更新消息
                List<GoodsItem> body = (List<GoodsItem>)message.Body;
                //GoodsItem[] body = (GoodsItem[])message.Body;
                GetGoodsList(body);
                break;
        }
    }

    void GetGoodsList(List<GoodsItem> data)
    {
        int length = data.Count;
        GoodsDictionary.Clear();
        foreach (Transform item in content.transform)
        {
            Destroy(item.gameObject);
        }
        if (length == 0)
        {
            return;
        }
        GameObject objparent = ResManager.CreateGameObject("PanelGoodBox", true);
        objparent.transform.SetParent(content);
        int index = 0;
        for (int i = 0; i < 24; i++)
        {
            GameObject obj = ResManager.CreateGameObject("GoodElementNew", true);
            obj.transform.SetParent(objparent.transform);
            if (index >= length)
            {
                index = 0;
            }
            GoodsInfomation info = obj.GetComponent<GoodsInfomation>();
            info.SetDate(data[index].image, data[index].price, data[index].productName, data[index].stock, ClickButton);
            obj.name = GoodsDictionary.Count.ToString();
            GoodsDictionary.Add(obj.name, data[index]);
            index++;
        }
        //m_horizontalScrollview.Init(count + 1);
    }

    //void GetGoodsList(List<GoodsItem> data)
    //{
    //    int length = data.Count;
    //    GoodsDictionary.Clear();
    //    foreach (Transform item in content.transform)
    //    {
    //        Destroy(item.gameObject);
    //    }

    //    if (length == 0)
    //    {
    //        return;
    //    }
    //    int count = length / 15;
    //    for (int i = 0; i <= count; i++)
    //    {
    //        GameObject obj1 = ResManager.CreateGameObject("PanelGoodBox", true);
    //        obj1.transform.SetParent(content);
    //        int k = length > (i + 1) * 15 ? 15 * (i + 1) : length;
    //        for (int j = i * 15; j < k; j++)
    //        {
    //            GameObject obj = ResManager.CreateGameObject("GoodElement", true);
    //            obj.transform.SetParent(obj1.transform);

    //            GoodsInfomation info = obj.GetComponent<GoodsInfomation>();
    //            info.SetDate(data[j].image, data[j].price, data[j].productName, data[j].stock, ClickButton);

    //            obj.name = GoodsDictionary.Count.ToString();
    //            GoodsDictionary.Add(obj.name, data[j]);
    //        }
    //        if (k < 15 * (i + 1))
    //        {
    //            int copyTimes = (15 * (i + 1) - k) / k;
    //            int leftTimes = (15 * (i + 1) - k) % k;
    //            Debug.Log("copyTimes" + copyTimes + "    leftTimes:" + leftTimes);
    //            for (int m = 0; m < copyTimes; m++)
    //            {
    //                for (int l = 0; l < k; l++)
    //                {
    //                    GameObject obj = ResManager.CreateGameObject("GoodElement", true);
    //                    obj.transform.SetParent(obj1.transform);

    //                    GoodsInfomation info = obj.GetComponent<GoodsInfomation>();
    //                    info.SetDate(data[l].image, data[l].price, data[l].productName, data[l].stock, ClickButton);
    //                    obj.name = GoodsDictionary.Count.ToString();
    //                    GoodsDictionary.Add(obj.name, data[l]);
    //                }
    //            }
    //            for (int n = 0; n < leftTimes; n++)
    //            {
    //                GameObject obj = ResManager.CreateGameObject("GoodElement", true);
    //                obj.transform.SetParent(obj1.transform);

    //                GoodsInfomation info = obj.GetComponent<GoodsInfomation>();
    //                info.SetDate(data[n].image, data[n].price, data[n].productName, data[n].stock, ClickButton);
    //                obj.name = (GoodsDictionary.Count).ToString();
    //                GoodsDictionary.Add(obj.name, data[n]);
    //            }
    //        }
    //    }
    //    //m_horizontalScrollview.Init(count + 1);
    //}

    //void GetGoodsList(GoodsItem[] data)
    //{
    //    GoodsDictionary.Clear();
    //    foreach (Transform item in content.transform)
    //    {
    //        Destroy(item.gameObject);
    //    }
    //    int length = data.Length;
    //    if (length == 0)
    //    {
    //        return;
    //    }
    //    int count = length / 15;
    //    for (int i = 0; i <= count; i++)
    //    {
    //        GameObject obj1 = ResManager.CreateGameObject("PanelGoodBox", true);
    //        obj1.transform.SetParent(content);
    //        int k = length > (i + 1) * 15 ? 15 * (i + 1) : length;
    //        for (int j = i * 15; j < k; j++)
    //        {
    //            GameObject obj = ResManager.CreateGameObject("GoodEleme", true);
    //            obj.transform.SetParent(obj1.transform);

    //            GoodsInfomation info = obj.GetComponent<GoodsInfomation>();

    //            info.SetDate(data[j].image, data[j].price, data[j].productName, data[j].stock, ClickButton);

    //            obj.name = GoodsDictionary.Count.ToString();
    //            GoodsDictionary.Add(obj.name, data[j]);
    //        }
    //        if (k < 15)
    //        {
    //            int copyTimes = (15 - k) / k;
    //            int leftTimes = 15 % k;
    //            for (int m = 0; m < copyTimes; m++)
    //            {
    //                for (int l = 0; l < k; l++)
    //                {
    //                    GameObject obj = ResManager.CreateGameObject("GoodEleme", true);
    //                    obj.transform.SetParent(obj1.transform);

    //                    GoodsInfomation info = obj.GetComponent<GoodsInfomation>();
    //                    info.SetDate(data[l].image, data[l].price, data[l].productName, data[l].stock, ClickButton);
    //                    obj.name = GoodsDictionary.Count.ToString();
    //                    GoodsDictionary.Add(obj.name, data[l]);
    //                }
    //            }
    //            for (int n = 0; n < leftTimes; n++)
    //            {
    //                GameObject obj = ResManager.CreateGameObject("GoodEleme", true);
    //                obj.transform.SetParent(obj1.transform);

    //                GoodsInfomation info = obj.GetComponent<GoodsInfomation>();
    //                info.SetDate(data[n].image, data[n].price, data[n].productName, data[n].stock, ClickButton);
    //                obj.name = (GoodsDictionary.Count).ToString();
    //                GoodsDictionary.Add(obj.name, data[n]);
    //            }
    //        }
    //    }
    //}

    //private Dictionary<string, string> ImageDic = new Dictionary<string, string>();

    #endregion

}
