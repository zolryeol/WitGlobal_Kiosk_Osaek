using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class BlockingImageChanger : MonoBehaviour, ILocalizableImage
{
    [SerializeField] Image InsaImage;

    public void InitLocalizableImage()
    {
        
    }

    public void UpdateLocalizableImage()
    {
        var nowLang = UIManager.Instance.NowLanguage;

        InsaImage.sprite = ResourceManager.Instance.PhotoBlockingImage[(int)nowLang];
    }
}
