using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventContent : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI ID;
    [SerializeField] protected TextMeshProUGUI eventName;
    [SerializeField] protected TextMeshProUGUI address;
    [SerializeField] protected TextMeshProUGUI openingTime;
    [SerializeField] protected TextMeshProUGUI contactNum;
    [SerializeField] protected TextMeshProUGUI hashTag;

    [SerializeField] Image thumbnail;
    [SerializeField] protected RawImage qrCodeImage;

    Button button;

    EventData myData;

    public void Init()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => UIManager.Instance.OpenPage(UIManager.Instance.EventDetailPage));
        button.onClick.AddListener(() => UIManager.Instance.EventContentDetail.FetchContent(myData));
    }

    public virtual void FetchContent(EventData data)
    {
        myData = data;

        var nowLang = (int)UIManager.Instance.NowLanguage;

        if (ID != null) ID.text = data.Num.ToString();

        eventName.text = data.EventNameString[nowLang].ToString();
        address.text = data.EventAddressString[nowLang].ToString();
        openingTime.text = data.OpeningTime.ToString();
        hashTag.text = data.HashTagString[nowLang].ToString();

        contactNum.text = data.ContactNum;

        thumbnail.sprite = data.ThumbNailImage;

    }
}


//public int Num; // 번호
//    public EventState EventState; // 0 종료, 1 오늘, 2 예정된 , 3 종료

//    public string[] EventNameString = new string[(int)Language.EndOfIndex]; // 업체명
//    public string[] EventAddressString = new string[(int)Language.EndOfIndex]; // 주소
//    public string[] HashTagString = new string[(int)Language.EndOfIndex]; // 해시태그
//    public string[] DescriptionString = new string[(int)Language.EndOfIndex]; // 정보
//    public string[] OpeningTime = new string[(int)Language.EndOfIndex]; // 운영시간
//    public string[] Age = new string[(int)Language.EndOfIndex]; // 볼거리
//    public string[] Fee = new string[(int)Language.EndOfIndex]; // 입장료

//    public string ContactNum; // 연락처
//    public string Period; // 기간

//    public string ImageUrl;

//    public Sprite ThumbNailImage;