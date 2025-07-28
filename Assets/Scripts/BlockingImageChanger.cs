using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class BlockingImageChanger : MonoBehaviour, ILocalizableImage
{
    [SerializeField] Image InsaImage;
    [SerializeField] Image GuideImage;

    public void InitLocalizableImage()
    {

    }

    public void UpdateLocalizableImage()
    {
        var nowLang = UIManager.Instance.NowLanguage;

        InsaImage.sprite = ResourceManager.Instance.PhotoBlockingImage[(int)nowLang];
        GuideImage.sprite = ResourceManager.Instance.PhotoGuideImage[(int)nowLang];
    }
}
