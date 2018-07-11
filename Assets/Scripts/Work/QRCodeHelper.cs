using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using ZXing;//引入库  
using ZXing.QrCode;

class QRCodeHelper : MonoBehaviour
{
    ////定义Texture2D对象和用于对应网站的字符串  
    //public Texture2D encoded;
    //public string Lastresult;
    ////定义一个UI，来接收图片  
    //public RawImage ima;
    //void Start()
    //{
    //    encoded = new Texture2D(256, 256);
    //    Lastresult = "http://www.hao123.com";
    //    var textForEncoding = Lastresult;
    //    if (textForEncoding != null)
    //    {
    //        //二维码写入图片  
    //        var color32 = Encode(textForEncoding, encoded.width, encoded.height);
    //        encoded.SetPixels32(color32);
    //        encoded.Apply();
    //        //生成的二维码图片附给RawImage  
    //        ima.texture = encoded;
    //    }
    //}
    //private static Color32[] Encode(string textForEncoding, int width, int height)
    //{
    //    var writer = new BarcodeWriter
    //    {
    //        Format = BarcodeFormat.QR_CODE,
    //        Options = new QrCodeEncodingOptions
    //        {
    //            Height = height,
    //            Width = width
    //        }
    //    };
    //    return writer.Write(textForEncoding);
    //}


}
