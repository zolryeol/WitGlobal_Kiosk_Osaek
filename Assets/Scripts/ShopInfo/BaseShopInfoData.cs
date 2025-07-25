using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BaseShopInfoData
{
    public int ShopID;
    public string[] ShopName = new string[(int)Language.EndOfIndex];
    public string[] Address = new string[(int)Language.EndOfIndex];
    public string[] HashTag = new string[(int)Language.EndOfIndex];
    public string[] ShopDescription = new string[(int)Language.EndOfIndex];
    public string[] BaseCategoryString = new string[(int)Language.EndOfIndex];
    public string[] SecondCategoryString = new string[(int)Language.EndOfIndex];
    public string[] AICategoryString = new string[(int)Language.EndOfIndex];
    public string OpeningTime;
    public string ContactNumber;
    public string NaverLink;
    public Sprite[] spriteImage = new Sprite[6];
    public float NaverRating;
    // Base 클래스의 생성자
    public BaseShopInfoData(int shopId, string[] shopName, string[] address,
    string[] hashTag, string[] shopDescription, string[] baseCategoryString,
    string[] secondCategoryString, string[] aiCategoryString,
    string openingTime, string contactNumber, string naverLink, float naverRating)
    {
        ShopID = shopId;
        ShopName = shopName;
        Address = address;
        HashTag = hashTag;
        ShopDescription = shopDescription;
        BaseCategoryString = baseCategoryString;
        SecondCategoryString = secondCategoryString;
        AICategoryString = aiCategoryString;
        OpeningTime = openingTime;
        ContactNumber = contactNumber;
        NaverLink = naverLink;
        NaverRating = naverRating;
    }

    public void SetPhoto(Sprite[] sprites)
    {
        spriteImage = sprites;
    }

    //// 파라미터 객체를 받는 생성자
    //public BaseShopInfoData(ShopInfoArgs args)
    //{
    //    ShopID = args.ShopID;
    //    ShopName = args.ShopName;
    //    Address = args.Address;
    //    HashTag = args.HashTag;
    //    ShopDescription = args.ShopDescription;
    //    BaseCategoryString = args.BaseCategoryString;
    //    SecondCategoryString = args.SecondCategoryString;
    //    AICategoryString = args.AICategoryString;
    //    OpeningTime = args.OpeningTime;
    //    ContactNumber = args.ContactNumber;
    //}
}
