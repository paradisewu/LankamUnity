/*
 * 脚本名(ScriptName)：    PanelPlayInfo.cs
 * 作者(Author):           小宝
 * 官网(Url):              http://www.youke.pro
 */
using UnityEngine;
using System.Collections;

public class PanelPlayInfo : PanelBase 
{
    #region 初始化相关
    protected override void OnInitSkin()
    {
        base.SetMainSkinPath("Game/UI/Home/PanelPlayInfo");
        base.OnInitSkin();
       // _type = PanelType.PanelPlayInfo;
        _showStyle = UIManager.PanelShowStyle.DownToSlide;
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
        if(click.name.Equals("BtnClose"))
        {
            Close();
        }
        else if(click.name.Equals("BtnTest"))
        {
            //UIManager.ShowPanel(PanelType.PanelPlayInfoAll);
        }
    }
	
}
