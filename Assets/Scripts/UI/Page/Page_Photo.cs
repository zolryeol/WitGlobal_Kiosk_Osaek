using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Page_Photo : MonoBehaviour
{
    ElgatoController elgatoController;

    [SerializeField] Button selectButton;
    [SerializeField] Button selectAloneButton; //1A4D7E // 616161 회색
    [SerializeField] Button selectTogetherButton;

    [SerializeField] Button confirmButton;
    [SerializeField] GameObject blocking;

    [SerializeField] GameObject resultParent;
    [SerializeField] Image resultImage;
    [SerializeField] Image Photoresult_Display2;
    [SerializeField] GameObject ParkLogo_Display2; // 박술녀로고 추가기능 예외처리

    [SerializeField] Button savePhotoButton; // 
    [SerializeField] Button closeQRFocusButton; // 팝업시 닫기버튼

    [SerializeField] Button tryAgainButton;
    [SerializeField] GameObject _QRFocus;
    [SerializeField] GameObject photoRenderTexture;

    [SerializeField] GameObject PrivacyPolicy;
    private void Awake()
    {
        elgatoController = GetComponent<ElgatoController>();

        selectButton.onClick.AddListener(OnSelectButton);
        //selectAloneButton.onClick.AddListener(SelectAlone);
        //selectTogetherButton.onClick.AddListener(SelectTogether);

        confirmButton.onClick.AddListener(OnConfirmButton);
        savePhotoButton.onClick.AddListener(ActiveQRFocus);

        closeQRFocusButton.onClick.AddListener(InActiveQRFocus);
        tryAgainButton.onClick.AddListener(InitPage);
        //GotoHomeButton.onClick.AddListener(() => UIManager.Instance.CloseAllPages());
    }

    public void InitPage()
    {
        photoRenderTexture.SetActive(false);

        elgatoController.StopElgato();
        elgatoController.adCountParent.SetActive(false);

        InActiveQRFocus();
        //InActiveQRFocusPurchase(); // 기간제 이벤트용

        Photoresult_Display2.sprite = null;
        InActiveButton(confirmButton);
        blocking.SetActive(false);
        Photoresult_Display2.gameObject.SetActive(false);

        resultImage.gameObject.SetActive(false);
        resultParent.SetActive(false);

        elgatoController.IsSuccessed = false;
        //VideoPlayManager.Instance.PackLogo.SetActive(true);

        selectAloneButton.image.color = new Color32(0x61, 0x61, 0x61, 0xFF);

        selectTogetherButton.image.color = new Color32(0x61, 0x61, 0x61, 0xFF);
    }


    void SelectAlone()
    {
        selectAloneButton.image.color = new Color32(0x1A, 0x4D, 0x7E, 0xFF);

        if (elgatoController.isTogether == true)
        {
            elgatoController.isTogether = false;
        }

        selectTogetherButton.image.color = new Color32(0x61, 0x61, 0x61, 0xFF);
        ActiveButton(confirmButton);

    }
    void SelectTogether()
    {
        selectTogetherButton.image.color = new Color32(0x1A, 0x4D, 0x7E, 0xFF);

        if (elgatoController.isTogether == false)
        {
            elgatoController.isTogether = true;
        }

        selectAloneButton.image.color = new Color32(0x61, 0x61, 0x61, 0xFF);

        ActiveButton(confirmButton);
    }

    void OnSelectButton()
    {
        elgatoController.isTogether = false;
        ActiveButton(confirmButton);
    }

    void OnConfirmButton()
    {
        //VideoPlayManager.Instance.PackLogo.SetActive(false);

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

        if (elgatoController.hanbokIndex <= (13 + 20)) // 박술녀로고 추가기능 예외처리
        {
            ParkLogo_Display2.gameObject.SetActive(true);
        }
        else ParkLogo_Display2.gameObject.SetActive(false);

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