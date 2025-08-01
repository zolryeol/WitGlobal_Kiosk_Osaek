using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  Enum 모음
/// </summary>
public enum Language
{
    Korean, // 한국어
    English, // 영어
    Japanese, // 일본어
    Chinese, // 중국어

    EndOfIndex // 언어 끝
}
public enum Category_Base // ShopManager에서 다루는 Category들
{
    Default, // 기본값
    ToEat,  // 인사뭐먹지
    ToBuy,  // 인사뭐사지
    ToGallery,  // 인사동미술관
    ToHelp, // 인사 도와줘
    ToStay, // 숙박정보
}

public enum Category_ETC // ShopManager에서 다루지 않는 Category들
{
    Default,
    Palace, // 고궁
    HanbokExplain, // 한복
    Map, // 지도
    Here, // 지역소개
    Greeting, // 하이 인사 (AI 정보)
    Photo,
    Event, // 이벤트
    Mission, // 미션
    Exchange,
    Transport,
    AISelect, // AI 선택
    Search,
    Toilet,
}

public enum VideoType
{
    Default,
    Photo,
    Photo_SelectHanbok,
    Photo_Creating,
    Photo_Complete,
    CompletePhoto,
    ToEat,
    ToEat_Category,
    ToEat_Detail,
    ToBuy,
    ToBuy_Category,
    ToBuy_Detail,
    ToGallery,
    ToGallery_Category,
    ToGallery_Detail,
    ToHelp,
    ToHelp_Category,
    ToHelp_Detail,
    ToStay,
    ToStay_Category,
    ToStay_Detail,
    Palace,
    Palace_Detail,
    HanbokExplain,
    Map,
    Here,
    Greeting,
    Event,
    Event_Category,
    Mission, // 미션
    AISearch,
    AISearch_Category,
    AISearch_Detail,
    Exchange,
    Transport,
    ChangeLanguage, // 언어변경
    ChangeLanguage_KR, // 
    ChangeLanguage_EN, // 
    ChangeLanguage_JP, // 
    ChangeLanguage_CH, // 
    Search,
    Search_Enter,
    Search_Detail,
}

#region 하위카테고리 사용하지않음
public enum Category_ToEat
{
    Default, // 기본값
    KoreanBBQ, // 코리안   
    Korean, // 한식
    koreanTraditional, // 한정식
    Snack, // 분식
    TempleFood, // 사찰음식
    Vegetarian, // 채식/비건
    Asian,  // 아시안
    Chinese, // 중식
    TraditionalTea, // 전통차
    Cafe, // 카페 
    ETC, // 기타
}

public enum Category_ToBuy
{
    Default, // 기본값
    Clothing, // 의류
    Crafts, // 공예품
    HandmadeSeal, // 수제도장
    Antique, // 앤틱
    Calligraphy, // 서예
    Hanbok, // 한복
    Accessories, // 잡화
    Framing, // 표구액자
    ETC, // 기타
}

public enum Category_ToGallery
{
    Default, // 기본값
    AntiqueArt, // 고미술
    Gallery, // 화랑
    Framing, // 표구
    Exhibition, // 전시관
    HistoricalSite, // 역사유적지
    ETC, // 기타
}

public enum Category_ToHelp
{
    Default, // 기본값
    InformationCenter, // 안내소
    ConvenienceStore, // 편의점
    Hospital, // 병원
    Pharmacy, // 약국
    Bank, // 은행
    ExchangeOffice, // 환전소
    Church, // 교회
    Buddhism, // 불교
    Cheondogyo, // 천도교
    Interior, // 인테리어
    Office, // 사무실
    ETC, // 기타
}

public enum Category_ToStay
{
    Default, // 기본값
    Hotel, // 호텔
    GuestHouse, // 게스트하우스
    YouthHostel, // 유스호스텔
}

enum ShopDataRowKey
{
    ShopID = 0, Photo = 1, ShopName_Kr = 2, BaseCategory_Kr = 3, SecondCategory_Kr = 4, AICategory_Kr = 5, Address_Kr = 6, HashTag_Kr = 7, Description_Kr = 8,
    ShopName_En = 9, BaseCategory_En = 10, SecondCategory_En = 11, AICategory_En = 12, Address_En = 13, HashTag_En = 14, Description_En = 15,
    ShopName_Jp = 16, BaseCategory_Jp = 17, SecondCategory_Jp = 18, AICategory_Jp = 19, Address_Jp = 20, HashTag_Jp = 21, Description_Jp = 22,
    ShopName_Ch = 23, BaseCategory_Ch = 24, SecondCategory_Ch = 25, AICategory_Ch = 26, Address_Ch = 27, HashTag_Ch = 28, Description_Ch = 29,
    OpeningTime = 30, ContactNumber = 31, NaverLink = 32,NaverRating = 33,
}

enum AICategoryRowKey
{
    Num, Kr, En, Jp, Ch
}
#endregion

public enum EventState
{
    Today,
    Week,
    Upcomming,
    Ended,
}

public enum KeyboardETC
{
    Default,
    Shift,
    LanguageChange,
    Enter,
    BackSpace,
    Symbol,
    Space,
}
