using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Page_Photo : MonoBehaviour
{
    ElgatoController elgatoController;

    [SerializeField] Button selectButton;
    [SerializeField] Button confirmButton;
    [SerializeField] GameObject blocking;

    [SerializeField] GameObject resultParent;
    [SerializeField] Image resultImage;
    [SerializeField] Image Photoresult_Display2;

    [SerializeField] Button savePhotoButton; // 
    [SerializeField] Button closeQRFocusButton; // 팝업시 닫기버튼

    [SerializeField] Button GotoHomeButton;
    [SerializeField] GameObject _QRFocus;

    [SerializeField] GameObject photoRenderTexture;

    [SerializeField] GameObject PrivacyPolicy;
    private void Awake()
    {
        elgatoController = GetComponent<ElgatoController>();

        selectButton.onClick.AddListener(OnSelectButton);
        confirmButton.onClick.AddListener(OnConfirmButton);
        savePhotoButton.onClick.AddListener(ActiveQRFocus);
        closeQRFocusButton.onClick.AddListener(InActiveQRFocus);
        GotoHomeButton.onClick.AddListener(InitPage);
        GotoHomeButton.onClick.AddListener(() => UIManager.Instance.CloseAllPages());
    }

    public void InitPage()
    {
        photoRenderTexture.SetActive(false);

        elgatoController.adCountParent.SetActive(false);

        InActiveQRFocus();
        Photoresult_Display2.sprite = null;
        InActiveButton(confirmButton);
        blocking.SetActive(false);
        Photoresult_Display2.gameObject.SetActive(false);

        resultImage.gameObject.SetActive(false);
        resultParent.SetActive(false);
    }

    void OnSelectButton()
    {
        ActiveButton(confirmButton);
    }

    void OnConfirmButton()
    {
        blocking.SetActive(true);
        //elgatoController.StartElgatoDirect();
        //elgatoController.StartElgato();

        photoRenderTexture.SetActive(true);
        elgatoController.StartElgato();
        //StartCoroutine(CaptureAndShowResult());
    }

    void ActiveQRFocus()
    {
        _QRFocus.SetActive(true);
    }
    void InActiveQRFocus()
    {
        _QRFocus.SetActive(false);
    }

    void ActiveButton(Button targetButton)
    {
        targetButton.gameObject.SetActive(true);
    }

    void InActiveButton(Button targetButton)
    {
        targetButton.gameObject.SetActive(false);
    }

    public void Final()
    {
        string path = elgatoController.LatestResultImagePath;

        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("LatestResultImagePath가 null 또는 빈 문자열입니다.");

            InitPage();
            return;
        }

        if (!File.Exists(path))
        {
            Debug.LogError("파일 존재하지 않음: " + path);

            InitPage();
            return;
        }

        // 파일 로딩
        byte[] imageData = File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageData);

        resultParent.SetActive(true);
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
        resultImage.sprite = sprite;
        resultImage.preserveAspect = true;

        if (resultImage.gameObject.activeSelf == false)      // 혹시 꺼져있었다면
        {
            resultImage.gameObject.SetActive(true);
        }

        Photoresult_Display2.gameObject.SetActive(true);
        Photoresult_Display2.sprite = sprite;

        VideoPlayManager.Instance.PlayVideo(VideoType.Photo_Complete);
    }

    public void ActivePrivacyPolicy()
    {
        PrivacyPolicy.SetActive(true);
    }

    public void InActivePrivacyPolicy()
    {
        PrivacyPolicy.SetActive(false);
    }
}
