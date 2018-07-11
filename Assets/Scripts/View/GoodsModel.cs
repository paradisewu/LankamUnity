using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoodsModel : BaseModel
{
    private List<GoodsItem> _data;
    public List<GoodsItem> data
    {
        get
        {
            return _data;
        }
        set
        {
            if (_data == value)
            {
                return;
            }

            DispatchValueUpdateEvent("Goods", _data, value);
            _data = value;
        }
    }

}
