using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// </summary>

[Serializable]
public class TraditionalMarketData : BaseShopInfoData // 전톳시장 데이터 
{
    //public int Num;
    public bool isSetup; // true면 설치
    //public string[] MarketName = new string[(int)Language.EndOfIndex]; // 시장명
    //public string[] Province = new string[(int)Language.EndOfIndex]; // 지역1
    //public string[] District = new string[(int)Language.EndOfIndex]; // 지역2
    //public string[] Address = new string[(int)Language.EndOfIndex]; // 주소
    //public string[] HashTag = new string[(int)Language.EndOfIndex]; // 해시태그
    //public string[] Description = new string[(int)Language.EndOfIndex]; // 설명
    //public string OpeningTime; // 운영시간
    //public string ContactNum; // 연락처
    //public string NaverLink; // 네이버 링크
    //public Sprite[] spriteImage = new Sprite[6]; // 이미지 배열
}

[Serializable]
public class AttractionData : BaseShopInfoData // 관광명소 데이터
{
    public AttractionData() { }
    //public int Num;
    public bool isSetup; // true면 설치
    public string[] Category = new string[(int)Language.EndOfIndex];
    public string[] Fee = new string[(int)Language.EndOfIndex];

    //public string[] MarketName = new string[(int)Language.EndOfIndex]; // 시장명
    //public string[] Province = new string[(int)Language.EndOfIndex]; // 지역1
    //public string[] District = new string[(int)Language.EndOfIndex]; // 지역2
    //public string[] Address = new string[(int)Language.EndOfIndex]; // 주소
    //public string[] HashTag = new string[(int)Language.EndOfIndex]; // 해시태그
    //public string[] Description = new string[(int)Language.EndOfIndex]; // 설명
    //public string OpeningTime; // 운영시간
    //public string ContactNum; // 연락처
    //public string NaverLink; // 네이버 링크
    //public Sprite[] spriteImage = new Sprite[6]; // 이미지 배열


}

[Serializable]
public class ServiceAreaData : BaseShopInfoData
{
    //public int Num;
    public bool isSetup; // true면 설치
    //public string[] MarketName = new string[(int)Language.EndOfIndex]; // 시장명
    //public string[] Province = new string[(int)Language.EndOfIndex]; // 지역1
    //public string[] District = new string[(int)Language.EndOfIndex]; // 지역2
    //public string[] Address = new string[(int)Language.EndOfIndex]; // 주소
    //public string[] HashTag = new string[(int)Language.EndOfIndex]; // 해시태그
    //public string[] Description = new string[(int)Language.EndOfIndex]; // 설명
    //public string OpeningTime; // 운영시간
    //public string ContactNum; // 연락처
    //public string NaverLink; // 네이버 링크
    //public Sprite[] spriteImage = new Sprite[6]; // 이미지 배열
}
