using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum MenuName
{
    Museum, Food, Shopping, Help, Hotel, AI
}

public class CategoryController : MonoBehaviour
{
    // ko, en, ja, zh
    public static string GetCategory(MenuName menuName, int categoryIndex, string language)
    {
        switch (menuName)
        {
            // 인사 미술관
            case MenuName.Museum:
                switch (language)
                {
                    case "ko":
                        switch (categoryIndex)
                        {
                            case 0:
                                return "고미술";
                            case 1:
                                return "화랑";
                            case 2:
                                return "표구";
                            case 3:
                                return "전시관";
                            case 4:
                                return "역사유적지";
                            case 5:
                                return "기타";
                            default:
                                Debug.Log("존재하지 않는 카테고리");
                                return "고미술";
                        }
                    case "en":
                        switch (categoryIndex)
                        {
                            case 0:
                                return "Antique Art";
                            case 1:
                                return "Gallery";
                            case 2:
                                return "Calligraphy";
                            case 3:
                                return "Exhibition Hall";
                            case 4:
                                return "Historical Site";
                            case 5:
                                return "Other";
                            default:
                                Debug.Log("존재하지 않는 카테고리");
                                return "Antique Art";
                        }
                    case "ja":
                        switch (categoryIndex)
                        {
                            case 0:
                                return "骨董品";
                            case 1:
                                return "ギャラリー";
                            case 2:
                                return "書道";
                            case 3:
                                return "展示館";
                            case 4:
                                return "歴史的遺跡";
                            case 5:
                                return "その他";
                            default:
                                Debug.Log("존재하지 않는 카테고리");
                                return "骨董品";
                        }
                    case "zh":
                        switch (categoryIndex)
                        {
                            case 0:
                                return "古董艺术";
                            case 1:
                                return "画廊";
                            case 2:
                                return "装裱";
                            case 3:
                                return "展览馆";
                            case 4:
                                return "历史遗址";
                            case 5:
                                return "其他";
                            default:
                                Debug.Log("존재하지 않는 카테고리");
                                return "古董艺术";
                        }
                    default:
                        return "화랑";
                }

            // 인사 뭐먹지       
            case MenuName.Food:
                switch (language)
                {
                    case "ko":
                        switch (categoryIndex)
                        {
                            case 0:
                                return "한식";
                            case 1:
                                return "코리안바베큐";
                            case 2:
                                return "분식";
                            case 3:
                                return "한정식";
                            case 4:
                                return "사찰";
                            case 5:
                                return "채식/비건";
                            case 6:
                                return "아시안";
                            case 7:
                                return "중식";
                            case 8:
                                return "전통차";
                            case 9:
                                return "카페";
                            default:
                                Debug.Log("존재하지 않는 카테고리");
                                return "코리안바베큐";
                        }
                    case "en":
                        switch (categoryIndex)
                        {
                            case 0:
                                return "Korean food";
                            case 1:
                                return "Korean BBQ";
                            case 2:
                                return "Street Food";
                            case 3:
                                return "course meal";
                            case 4:
                                return "Temple Food";
                            case 5:
                                return "Vegetarian";
                            case 6:
                                return "Asian";
                            case 7:
                                return "Chinese food";
                            case 8:
                                return "Traditional Tea";
                            case 9:
                                return "Cafe";
                            default:
                                Debug.Log("존재하지 않는 카테고리");
                                return "Korean BBQ";
                        }
                    case "ja":
                        switch (categoryIndex)
                        {
                            case 0:
                                return "韓国料理";
                            case 1:
                                return "韓国のバーベキュー";
                            case 2:
                                return "韓国の軽食";
                            case 3:
                                return "韓定食";
                            case 4:
                                return "査察料理";
                            case 5:
                                return "ベジタリアン/ビーガン";
                            case 6:
                                return "アジア";
                            case 7:
                                return "昼食";
                            case 8:
                                return "伝統的な車";
                            case 9:
                                return "カフェ";
                            default:
                                Debug.Log("존재하지 않는 카테고리");
                                return "骨董品";
                        }
                    case "zh":
                        switch (categoryIndex)
                        {
                            case 0:
                                return "韩国菜";
                            case 1:
                                return "韩国烧烤";
                            case 2:
                                return "韩国小吃";
                            case 3:
                                return "韩定食";
                            case 4:
                                return "寺庙食物";
                            case 5:
                                return "素食/纯素食";
                            case 6:
                                return "亚洲人";
                            case 7:
                                return "午餐";
                            case 8:
                                return "传统茶";
                            case 9:
                                return "咖啡馆";
                            default:
                                Debug.Log("존재하지 않는 카테고리");
                                return "古董艺术";
                        }
                    default:
                        return "화랑";
                }

            // 인사 뭐사지
            case MenuName.Shopping:
                switch (language)
                {
                    case "ko":
                        switch (categoryIndex)
                        {
                            case 0:
                                return "의류";
                            case 1:
                                return "공예품";
                            case 2:
                                return "수제도장";
                            case 3:
                                return "엔틱";
                            case 4:
                                return "필방";
                            case 5:
                                return "한복";
                            case 6:
                                return "잡화";
                            case 7:
                                return "표구액자";
                            case 8:
                                return "기타";
                            default:
                                Debug.Log("존재하지 않는 카테고리");
                                return "의류";
                        }
                    case "en":
                        switch (categoryIndex)
                        {
                            case 0:
                                return "Clothing";
                            case 1:
                                return "Crafts";
                            case 2:
                                return "Handmade Stamps";
                            case 3:
                                return "Antiques";
                            case 4:
                                return "Calligraphy";
                            case 5:
                                return "Hanbok";
                            case 6:
                                return "Essentials";
                            case 7:
                                return "Mounting and Framing";
                            case 8:
                                return "Others";
                            default:
                                Debug.Log("존재하지 않는 카테고리");
                                return "Clothing";
                        }
                    case "ja":
                        switch (categoryIndex)
                        {
                            case 0:
                                return "衣類";
                            case 1:
                                return "工芸品";
                            case 2:
                                return "手作り印鑑";
                            case 3:
                                return "アンティーク";
                            case 4:
                                return "書道";
                            case 5:
                                return "韓服";
                            case 6:
                                return "雑貨";
                            case 7:
                                return "作品の装丁と額装";
                            case 8:
                                return "その他";
                            default:
                                Debug.Log("존재하지 않는 카테고리");
                                return "衣類";
                        }
                    case "zh":
                        switch (categoryIndex)
                        {
                            case 0:
                                return "服装";
                            case 1:
                                return "工艺品";
                            case 2:
                                return "手工印章";
                            case 3:
                                return "古董";
                            case 4:
                                return "书法";
                            case 5:
                                return "韩服";
                            case 6:
                                return "杂货";
                            case 7:
                                return "装裱与框架";
                            case 8:
                                return "其他";
                            default:
                                Debug.Log("존재하지 않는 카테고리");
                                return "服装";
                        }
                    default:
                        return "의류";
                }

            // 도와줘
            case MenuName.Help:
                switch (language)
                {
                    case "ko":
                        switch (categoryIndex)
                        {
                            case 0:
                                return "안내소";
                            case 1:
                                return "편의점";
                            case 2:
                                return "병원";
                            case 3:
                                return "약국";
                            case 4:
                                return "은행";
                            case 5:
                                return "환전소";
                            case 6:
                                return "교회";
                            case 7:
                                return "불교";
                            case 8:
                                return "천도교";
                            case 9:
                                return "인테리어";
                            case 10:
                                return "사무실";
                            case 11:
                                return "기타";
                            default:
                                Debug.Log("존재하지 않는 카테고리");
                                return "안내소";
                        }
                    case "en":
                        switch (categoryIndex)
                        {
                            case 0:
                                return "Information Center";
                            case 1:
                                return "Convenience Store";
                            case 2:
                                return "Hospital";
                            case 3:
                                return "Pharmacy";
                            case 4:
                                return "Bank";
                            case 5:
                                return "Currency Exchange";
                            case 6:
                                return "Church";
                            case 7:
                                return "Buddhist Temple";
                            case 8:
                                return "Cheondoism Temple";
                            case 9:
                                return "Construction and Electrical Services";
                            case 10:
                                return "Company Office";
                            case 11:
                                return "Other";
                            default:
                                Debug.Log("존재하지 않는 카테고리");
                                return "Information Center";
                        }
                    case "ja":
                        switch (categoryIndex)
                        {
                            case 0:
                                return "案内所";
                            case 1:
                                return "コンビニエンスストア";
                            case 2:
                                return "病院";
                            case 3:
                                return "薬局";
                            case 4:
                                return "銀行";
                            case 5:
                                return "両替所";
                            case 6:
                                return "教会";
                            case 7:
                                return "仏教寺院";
                            case 8:
                                return "天道教寺院";
                            case 9:
                                return "建設・電気サービス";
                            case 10:
                                return "会社事務所";
                            case 11:
                                return "その他";
                            default:
                                Debug.Log("존재하지 않는 카테고리");
                                return "案内所";
                        }
                    case "zh":
                        switch (categoryIndex)
                        {
                            case 0:
                                return "信息中心";
                            case 1:
                                return "便利店";
                            case 2:
                                return "医院";
                            case 3:
                                return "药房";
                            case 4:
                                return "银行";
                            case 5:
                                return "兑换处";
                            case 6:
                                return "教堂";
                            case 7:
                                return "佛教寺院";
                            case 8:
                                return "天道教寺院";
                            case 9:
                                return "建筑与电气服务";
                            case 10:
                                return "公司办公室";
                            case 11:
                                return "其他";
                            default:
                                Debug.Log("존재하지 않는 카테고리");
                                return "信息中心";
                        }
                    default:
                        return "안내소";
                }

            // 숙박안내
            case MenuName.Hotel:
                switch (language)
                {
                    case "ko":
                        switch (categoryIndex)
                        {
                            case 0:
                                return "호텔";
                            case 1:
                                return "호스텔";
                            case 2:
                                return "게스트하우스";
                            default:
                                Debug.Log("존재하지 않는 카테고리");
                                return "호텔";
                        }
                    case "en":
                        switch (categoryIndex)
                        {
                            case 0:
                                return "Hotel";
                            case 1:
                                return "Hostel";
                            case 2:
                                return "Guesthouse";
                            default:
                                Debug.Log("존재하지 않는 카테고리");
                                return "Hotel";
                        }
                    case "ja":
                        switch (categoryIndex)
                        {
                            case 0:
                                return "ホテル";
                            case 1:
                                return "ホステル";
                            case 2:
                                return "ゲストハウス";
                            default:
                                Debug.Log("존재하지 않는 카테고리");
                                return "ホテル";
                        }
                    case "zh":
                        switch (categoryIndex)
                        {
                            case 0:
                                return "酒店";
                            case 1:
                                return "旅舍";
                            case 2:
                                return "宾馆";
                            default:
                                Debug.Log("존재하지 않는 카테고리");
                                return "酒店";
                        }
                    default:
                        return "호텔";
                }

            // 인사 뭐하지
            case MenuName.AI:
                switch (language)
                {
                    case "ko":
                        switch (categoryIndex)
                        {
                            case 0:
                                return "코리안바베큐";
                            case 1:
                                return "한식";
                            case 2:
                                return "한정식";
                            case 3:
                                return "사찰음식";
                            case 4:
                                return "채식/비건";
                            case 5:
                                return "아시안";
                            case 6:
                                return "다과";
                            case 7:
                                return "중식";
                            case 8:
                                return "분식";
                            case 9:
                                return "막걸리";
                            case 10:
                                return "전통주";
                            case 11:
                                return "카페";
                            case 12:
                                return "전통차";
                            case 13:
                                return "기념품";
                            case 14:
                                return "체험";
                            case 15:
                                return "사진촬영";
                            case 16:
                                return "K-POP";
                            case 17:
                                return "의류";
                            case 18:
                                return "공예품";
                            case 19:
                                return "수제도장";
                            case 20:
                                return "엔틱";
                            case 21:
                                return "한복";
                            case 22:
                                return "필방";
                            case 23:
                                return "잡화";
                            case 24:
                                return "표구";
                            case 25:
                                return "표구액자";
                            case 26:
                                return "고미술";
                            case 27:
                                return "화랑";
                            case 28:
                                return "전시관";
                            case 29:
                                return "역사유적지";
                            default:
                                Debug.Log("존재하지 않는 카테고리");
                                return "코리안바베큐";
                        }
                    case "en":
                        switch (categoryIndex)
                        {
                            case 0:
                                return "Korean BBQ";
                            case 1:
                                return "Korean food";
                            case 2:
                                return "course meal";
                            case 3:
                                return "Temple Food";
                            case 4:
                                return "Vegetarian";
                            case 5:
                                return "Asian";
                            case 6:
                                return "Snacks";
                            case 7:
                                return "Chinese food";
                            case 8:
                                return "Street Food";
                            case 9:
                                return "makgeolli";
                            case 10:
                                return "traditional liquor";
                            case 11:
                                return "Cafe";
                            case 12:
                                return "Traditional Tea";
                            case 13:
                                return "souvenir";
                            case 14:
                                return "experience";
                            case 15:
                                return "photo shoot";
                            case 16:
                                return "K-POP";
                            case 17:
                                return "Clothing";
                            case 18:
                                return "Crafts";
                            case 19:
                                return "Handmade Stamps";
                            case 20:
                                return "Antiques";
                            case 21:
                                return "Hanbok";
                            case 22:
                                return "Calligraphy";
                            case 23:
                                return "Essentials";
                            case 24:
                                return "Calligraphy";
                            case 25:
                                return "Mounting and Framing";
                            case 26:
                                return "Antique Art";
                            case 27:
                                return "Gallery";
                            case 28:
                                return "Exhibition Hall";
                            case 29:
                                return "Historical Site";
                            default:
                                Debug.Log("존재하지 않는 카테고리");
                                return "Korean BBQ";
                        }
                    case "ja":
                        switch (categoryIndex)
                        {
                            case 0:
                                return "韓国のバーベキュー";
                            case 1:
                                return "韓国料理";
                            case 2:
                                return "韓定食";
                            case 3:
                                return "査察料理";
                            case 4:
                                return "ベジタリアン/ビーガン";
                            case 5:
                                return "アジア";
                            case 6:
                                return "お菓子";
                            case 7:
                                return "昼食";
                            case 8:
                                return "韓国の軽食";
                            case 9:
                                return "マッコリ";
                            case 10:
                                return "伝統主";
                            case 11:
                                return "カフェ";
                            case 12:
                                return "伝統的な車";
                            case 13:
                                return "お土産";
                            case 14:
                                return "体験";
                            case 15:
                                return "写真撮影";
                            case 16:
                                return "K-POP";
                            case 17:
                                return "衣類";
                            case 18:
                                return "工芸品";
                            case 19:
                                return "手作り印鑑";
                            case 20:
                                return "アンティーク";
                            case 21:
                                return "韓服";
                            case 22:
                                return "書道";
                            case 23:
                                return "雑貨";
                            case 24:
                                return "書道";
                            case 25:
                                return "作品の装丁と額装";
                            case 26:
                                return "骨董品";
                            case 27:
                                return "ギャラリー";
                            case 28:
                                return "展示館";
                            case 29:
                                return "歴史的遺跡";
                            default:
                                Debug.Log("존재하지 않는 카테고리");
                                return "韓国のバーベキュー";
                        }
                    case "zh":
                        switch (categoryIndex)
                        {
                            case 0:
                                return "韩国烧烤";
                            case 1:
                                return "韩国菜";
                            case 2:
                                return "韩定食";
                            case 3:
                                return "寺庙食物";
                            case 4:
                                return "素食/纯素食";
                            case 5:
                                return "亚洲人";
                            case 6:
                                return "小吃";
                            case 7:
                                return "午餐";
                            case 8:
                                return "韩国小吃";
                            case 9:
                                return "马格利酒";
                            case 10:
                                return "传统酒";
                            case 11:
                                return "咖啡馆";
                            case 12:
                                return "传统茶";
                            case 13:
                                return "纪念品";
                            case 14:
                                return "经验";
                            case 15:
                                return "照片拍摄";
                            case 16:
                                return "K-POP";
                            case 17:
                                return "服装";
                            case 18:
                                return "工艺品";
                            case 19:
                                return "手工印章";
                            case 20:
                                return "古董";
                            case 21:
                                return "韩服";
                            case 22:
                                return "书法";
                            case 23:
                                return "杂货";
                            case 24:
                                return "装裱";
                            case 25:
                                return "装裱与框架";
                            case 26:
                                return "古董艺术";
                            case 27:
                                return "画廊";
                            case 28:
                                return "展览馆";
                            case 29:
                                return "历史遗址";
                            default:
                                Debug.Log("존재하지 않는 카테고리");
                                return "코리안바베큐";
                        }
                    default:
                        return "코리안바베큐";
                }
                    
            // 기본값
            default:
                return null;
        }
    }
}
