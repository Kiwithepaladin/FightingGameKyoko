using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using System.Net.Sockets;
using ZXing.Common;
using System.Net;

public class GenerateBarCode : MonoBehaviour
{
    [SerializeField] private BarcodeFormat format = BarcodeFormat.QR_CODE;
    [SerializeField] public static string data;
    [SerializeField] private int width = 512;
    [SerializeField] private int height = 512;
    private RawImage cRawImage;
    private void Awake()
    {
        IPAddress[] localIP = Dns.GetHostAddresses(Dns.GetHostName());
        foreach (IPAddress address in localIP)
        {
            if(address.AddressFamily == AddressFamily.InterNetwork)
            {
                data = address.ToString();
            }
        }
        Debug.Log(data);
        cRawImage = GetComponent<RawImage>();
        // Generate the texture
        Texture2D tex = GenerateBarcode(data, format, width, height);
        // Setup the RawImage
        cRawImage.texture = tex;
        cRawImage.rectTransform.sizeDelta = new Vector2(tex.width, tex.height);
    }

    private Texture2D GenerateBarcode(string data, BarcodeFormat format, int width, int height)
    {
        // Generate the BitMatrix
        BitMatrix bitMatrix = new MultiFormatWriter()
            .encode(data, format, width, height);
        // Generate the pixel array
        Color[] pixels = new Color[bitMatrix.Width * bitMatrix.Height];
        int pos = 0;
        for (int y = 0; y < bitMatrix.Height; y++)
        {
            for (int x = 0; x < bitMatrix.Width; x++)
            {
                pixels[pos++] = bitMatrix[x, y] ? Color.black : Color.white;
            }
        }
        // Setup the texture
        Texture2D tex = new Texture2D(bitMatrix.Width, bitMatrix.Height);
        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }
}
