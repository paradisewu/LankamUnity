using System;
using System.Collections.Generic;


public partial class GoodsJson
{
    /// <summary>
    /// 
    /// </summary>
    public int errCode { get; set; }

    public string msg { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public List<GoodsItem> data { get; set; }
}

public class GoodsItem
{
    /// <summary>
    /// 产品ID
    /// </summary>
    public double productId { get; set; }
    /// <summary>
    /// 康师傅经典奶茶
    /// </summary>
    public string productName { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string image { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public double stock { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public double price { get; set; }
}

