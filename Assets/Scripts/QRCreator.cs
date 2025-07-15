using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZXing.QrCode;
using ZXing;
using Unity.VisualScripting;

public class QRCreator : MonoBehaviour
{
    public static void CreateQR(string link, RawImage rawImage)
    {
        BarcodeWriter barcode = new BarcodeWriter
        {
            //���ڵ弼��
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Width = 256,
                Height = 256
            }
        };

        Color32[] encoded = barcode.Write(link);

        Texture2D qrcode = new Texture2D(256, 256);
        qrcode.SetPixels32(encoded);
        qrcode.Apply();

        rawImage.texture = qrcode;
    }
}