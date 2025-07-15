using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;

public class LanguageService : MonoBehaviour
{
    // 씬 별 객체 모음 => 어떤 이름을 가진 객체가 있는지
    private Dictionary<string, string[]> scenes;

    // 언어 페이지 번역 정보 넣을 객체
    private Dictionary<string, Dictionary<string, string>> languageScenes;

    // 현재 언어의 확인 후 필요한 텍스트를 담을 객체
    private Dictionary<string, Dictionary<string, string>> resultDictionary;

    // 씬 안의 객체 별 번역된 내용 담은 객체
    Dictionary<string, string>[] scenesTxtArr;    

    /*
        설계 내용 : 씬 별로 사전 타입의 객체를 만들어서 진행 

        키 : 객체 이름 (TextMeshProGUI, 
        값 : 들어갈 문장 (한국어, 영어, 중국어, 일본어)
     */

    void Start(){}

    public static string apiLanguageParse() { return LanguageService.getCurrentLanguage() == "JP" ? "ja" : LanguageService.getCurrentLanguage() == "CN" ? "zh" : LanguageService.getCurrentLanguage() == "KR" ? "ko" : LanguageService.getCurrentLanguage().ToLower(); }

    // 현재 언어 가져오기
    public static string getCurrentLanguage() { return PlayerPrefs.GetString("language") == null || PlayerPrefs.GetString("language") == "" ? "KR" : PlayerPrefs.GetString("language"); }

    // 번역페이지
    public void languageSceneInit() {        

        scenes = new Dictionary<string, string[]>
        {
            {"language" , new string[]
                {
                    "ScenesInfo",
                    "CurrentLanguage",
                    "TransLanguage"
                 }
            }
        };
       
        scenesTxtArr = new Dictionary<string, string>[]
        {
            new Dictionary<string, string>
                            {
                                { "KR", "사용하실 언어를 선택해주세요" },
                                { "EN", "Please select your preferred language" },
                                { "JP", "使用する言語を選択してください" },
                                { "CN", "請選擇語言" }
                            },
            new Dictionary<string, string>
                            {
                                { "KR", "현재 언어" },
                                { "EN", "Current language" },
                                { "JP", "現在の言語" },
                                { "CN", "目前的語言" }
                            },
             new Dictionary<string, string>
                            {
                                { "KR", "변경 언어" },
                                { "EN", "Change language" },
                                { "JP", "変更言語" },
                                { "CN", "變更語言" }
                            }             
        };               

    }


    // 메인화면
    public void mainSceneinit() {

        scenes = new Dictionary<string, string[]>
        {
            {"main" , new string[]
                {
                    "Shopping",
                    "Food",
                    "Art",
                    "Event",
                    "Info",
                    "Hi",
                    "Mission",
                    "Help",
                    "Ex",
                    "Tran",
                    "Lodgment",
                    "Gogoong",
                    "AI",
                    "Text01",
                    "Text02",
                    "Placeholder"
                 }
            }
        };

        scenesTxtArr = new Dictionary<string, string>[]
        {
            new Dictionary<string, string>
                            {
                                { "KR", "인사뭐사지?" },
                                { "EN", "To Buy" },
                                { "JP", "お買い物 おすすめ" },
                                { "CN", "要買什麼？" }
                            },
            new Dictionary<string, string>
                            {
                                { "KR", "인사뭐먹지?" },
                                { "EN", "To Eat" },
                                { "JP", "飲食店のおすすめ" },
                                { "CN", "晚餐吃什麼？" }
                            },
             new Dictionary<string, string>
                            {
                                { "KR", "인사동미술관" },
                                { "EN", "Art Venues" },
                                { "JP", "仁寺洞美術館" },
                                { "CN", "仁寺洞美術館" }
                            },
              new Dictionary<string, string>
                            {
                                { "KR", "인사동이벤트" },
                                { "EN", "Events" },
                                { "JP", "おすすめイベント" },
                                { "CN", "仁寺洞晚會" }
                            },
               new Dictionary<string, string>
                            {
                                { "KR", "여기는인사동" },
                                { "EN", "About Insadong" },
                                { "JP", "インサドンの紹介" },
                                { "CN", "這裡是仁寺洞" }
                            },
            new Dictionary<string, string>
                            {
                                { "KR", "안녕인사" },
                                { "EN", "Hello ‘Insa'" },
                                { "JP", "こんにちは 'INSA'" },
                                { "CN", "你好，Insa" }
                            },
            new Dictionary<string, string>
                            {
                                { "KR", "인사동미션" },
                                { "EN", "Mission" },
                                { "JP", "ミッション挑戦" },
                                { "CN", "仁寺洞 Mission" }
                            },
            new Dictionary<string, string>
                            {
                                { "KR", "도와줘인사" },
                                { "EN", "Help" },
                                { "JP", "ヘルプとサービスです" },
                                { "CN", "救救我，Insa" }
                            },
            new Dictionary<string, string>
                            {
                                { "KR", "환율" },
                                { "EN", "Currency Exchange" },
                                { "JP", "為替レート情報" },
                                { "CN", "匯率" }
                            },
            new Dictionary<string, string>
                            {
                                { "KR", "교통안내" },
                                { "EN", "Transportation" },
                                { "JP", "交通案内" },
                                { "CN", "運輸" }
                            },
            new Dictionary<string, string>
                            {
                                { "KR", "숙박안내" },
                                { "EN", "Accommodation" },
                                { "JP", "宿泊情報" },
                                { "CN", "住宿設施" }
                            },
            new Dictionary<string, string>
                            {
                                { "KR", "고궁안내" },
                                { "EN", "Palace Info." },
                                { "JP", "故宮情報" },
                                { "CN", "宮殿資訊" }
                            },
            new Dictionary<string, string>
                            {
                                { "KR", "인사모하지?(AI검색)" },
                                { "EN", "What to do (Ai)" },
                                { "JP", "何するの旅行先 おすすめ(Ai)" },
                                { "CN", "我該怎麼做？(AI)" }
                            },
            new Dictionary<string, string>
                            {
                                { "KR", "인사동지도" },
                                { "EN", "Insadong Map" },
                                { "JP", "仁寺洞地図" },
                                { "CN", "地图" }
                            },
             new Dictionary<string, string>
                            {
                                { "KR", "한복설명" },
                                { "EN", "About Hanbok" },
                                { "JP", "韓服の説明" },
                                { "CN", "传统服饰说明" }
                            },
             new Dictionary<string, string>
                            {
                                { "KR", "인사동에 대해 검색해보세요!" },
                                { "EN", "Explore Insadong!" },
                                { "JP", "インサドンを探索する！" },
                                { "CN", "探索仁寺洞！" }
                            },
        };
    }


    // 디테일 씬(설명 1개)
    public void detailSceneInit()
    {
        scenes = new Dictionary<string, string[]>
         {
             {"Detail" , new string[]
                 {
                   "ApplicationInfo"
                  }
             }
         };

        scenesTxtArr = new Dictionary<string, string>[]
        {
            new Dictionary<string, string>
            {
                { "KR", "<color=#616161>• QR</color>를 찍으시면  <color=#616161>WIT 어플이</color> 길안내를 해 드려요\n다양한 할인 혜택과 이벤트도 경험하실 수 있어요" },
                { "EN", "<color=#616161>• Scan the QR code</color> to navigate easily with the <color=#616161>WIT app</color>!" },
                { "JP", "<color=#616161>• QRコード</color>をスキャンして、コース推薦、<color=#616161>ミッションに参加し、</color>ギフトや体験を手に入れよう！" },
                { "CN", "<color=#616161>• 扫描</color>，二维码加入课程推荐任务<color=#616161>解锁礼物</color>，和体验！" }
            }
        };
    }


    // 날씨
    public void weatherSceneInit()
    {
        scenes = new Dictionary<string, string[]>
         {
             {"weather" , new string[]
                 {
                   "DayOfTheWeek",
                   "ApparentTemperature",
                   "HumidityText",
                   "WindSpeedText",
                   "ParticulateText",
                   "FineParticulateText",
                   "TextPopular"
                  }
             }
         };

        scenesTxtArr = new Dictionary<string, string>[]
        {
            new Dictionary<string, string>
            {
                { "KR", @"  월        화        수        목         금" },
                { "EN", @"Mon   Tue    Wed   Thu     Fri" },
                { "JP", @"  月       火        水       木        金" },
                { "CN", @"周一  周二   周三   周四   周五" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"체감온도" },
                { "EN", @"Temperature" },
                { "JP", @"体感温度" },
                { "CN", @"體感溫度" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"습도" },
                { "EN", @"Humidity" },
                { "JP", @"湿度" },
                { "CN", @"湿度" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"풍속" },
                { "EN", @"Wind Speed" },
                { "JP", @"風速" },
                { "CN", @"风速" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"미세먼지" },
                { "EN", @"Fine Dust" },
                { "JP", @"微細粉塵" },
                { "CN", @"细颗粒物" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"초미세먼지" },
                { "EN", @"Ultra Fine Dust" },
                { "JP", @"超微細粉塵" },
                { "CN", @"超细颗粒物" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"유명지역 날씨" },
                { "EN", @"Famous Area Weather" },
                { "JP", @"有名地域の天気" },
                { "CN", @"名区天气" }
            }
        };
    }


    // 검색 인사동(아직)
    public void searchSceneInit()
    {
        string todo = "searchDetail";
    }


    // 인사 뭐먹지
    public void foodSceneInit() {
        scenes = new Dictionary<string, string[]>
        {
            {"Food" , new string[]
                {            
                   "Text01",
                  "Button01",
                  "Button02",
                  "Button03",
                  "Button04",
                  "Button05",
                  "Button06",
                  "Button07",
                  "Button08",
                  "Button09",
                  "Button10",
                  "Text02"
                 }
            }
        };

        scenesTxtArr = new Dictionary<string, string>[]
        {
          new Dictionary<string, string>
            {
                { "KR", "인사뭐먹지?" },
                { "EN", "What to eat?" },
                { "JP", "何を食べようか？" },
                { "CN", "晚餐吃什麼？" }
            },
            new Dictionary<string, string>
            {
                { "KR", "한식" },
                { "EN", "Korean food" },
                { "JP", "韓国料理" },
                { "CN", "韩国菜" }
            },
            new Dictionary<string, string>
            {
                { "KR", "코리안바베큐" },
                { "EN", "Korean BBQ" },
                { "JP", "韓国焼肉" },
                { "CN", "韩国烧烤" }
            },
            new Dictionary<string, string>
            {
                { "KR", "분식" },
                { "EN", "Street Food" },
                { "JP", "韓国の軽食" },
                { "CN", "韩国小吃" }
            },
            new Dictionary<string, string>
            {
                { "KR", "한정식" },
                { "EN", "Course meal" },
                { "JP", "韓定食" },
                { "CN", "韩定食" }
            },
            new Dictionary<string, string>
            {
                { "KR", "사찰음식" },
                { "EN", "Temple Food​" },
                { "JP", "査察料理" },
                { "CN", "寺庙食物" }
            },
            new Dictionary<string, string>
            {
                { "KR", "채식/비건" },
                { "EN", "Vegetarian" },
                { "JP", "ベジタリアン/ビーガン" },
                { "CN", "素食/纯素食" }
            },
            new Dictionary<string, string>
            {
                { "KR", "아시안" },
                { "EN", "Asian" },
                { "JP", "アジア" },
                { "CN", "亚洲人" }
            },
            new Dictionary<string, string>
            {
                { "KR", "중식" },
                { "EN", "Chinese food" },
                { "JP", "昼食" },
                { "CN", "午餐" }
            },
            new Dictionary<string, string>
            {
                { "KR", "전통차" },
                { "EN", "Tea House" },
                { "JP", "伝統的な車" },
                { "CN", "传统茶" }
            },
            new Dictionary<string, string>
            {
                { "KR", "카페" },
                { "EN", "Cafe" },
                { "JP", "カフェ" },
                { "CN", "咖啡馆" }
            },
             new Dictionary<string, string>
            {
                { "KR", "<color=#616161>• QR</color>를 찍으시면  <color=#616161>WIT 어플이</color> 길안내를 해드려요" },
                { "EN", "<color=#616161>• Scan the QR code</color> to navigate easily with the <color=#616161>WIT app</color>" },
                { "JP", "<color=#616161>• QR</color>をスキャンすると、<color=#616161>WITアプリ</color>が道案内をしてくれます" },
                { "CN", "<color=#616161>• 扫描 QR</color>，<color=#616161>WIT 应用程序</color>会为您指路" }
            }
        };
    }


    // 인사 뭐사지
    public void shoppingSceneInit()
    {
        scenes = new Dictionary<string, string[]>
         {
             {"Shopping" , new string[]
                 {
                  "Text01",
                  "Text02",
                  "Button01",
                  "Button02",
                  "Button03",
                  "Button04",
                  "Button05",
                  "Button06",
                  "Button07",
                  "Button08",
                  "Button09",
                  }
             }
         };

        scenesTxtArr = new Dictionary<string, string>[]
       {
            new Dictionary<string, string>
            {
                { "KR", "인사뭐사지" },
                { "EN", "What to Buy in Insa-dong" },
                { "JP", "仁寺洞で何を買う？" },
                { "CN", "在仁寺洞买什么" }
            },
            new Dictionary<string, string>
            {
                { "KR", "<color=#616161>• QR</color>를 찍으시면  <color=#616161>WIT APP이</color> 길안내를 해드려요" },
                { "EN", "<color=#616161>• Scan the QR code</color> to navigate easily with the <color=#616161>WIT app</color>" },
                { "JP", "<color=#616161>• QRコード</color>をスキャンすると、<color=#616161>WITアプリ</color>が案内します。" },
                { "CN", "<color=#616161>• 扫描QR码</color>，<color=#616161>WIT APP</color>会为您提供导航。" }
            },
            new Dictionary<string, string>
            {
                { "KR", "의류" },
                { "EN", "Clothing" },
                { "JP", "衣類" },
                { "CN", "服装" }
            },
            new Dictionary<string, string>
            {
                { "KR", "공예품" },
                { "EN", "Crafts" },
                { "JP", "工芸品" },
                { "CN", "工艺品" }
            },
            new Dictionary<string, string>
            {
                { "KR", "수제도장" },
                { "EN", "Stamps" },
                { "JP", "手作り印鑑" },
                { "CN", "手工印章" }
            },
            new Dictionary<string, string>
            {
                { "KR", "엔틱" },
                { "EN", "Antiques" },
                { "JP", "アンティーク" },
                { "CN", "古董" }
            },
            new Dictionary<string, string>
            {
                { "KR", "필방" },
                { "EN", "Calligraphy" },
                { "JP", "書道" },
                { "CN", "书法" }
            },
            new Dictionary<string, string>
            {
                { "KR", "한복" },
                { "EN", "Hanbok" },
                { "JP", "韓服" },
                { "CN", "韩服" }
            },
            new Dictionary<string, string>
            {
                { "KR", "잡화" },
                { "EN", "Essentials" },
                { "JP", "雑貨" },
                { "CN", "杂货" }
            },
            new Dictionary<string, string>
            {
                { "KR", "표구액자" },
                { "EN", "Mounting and Framing" },
                { "JP", "作品の装丁と額装" },
                { "CN", "装裱与框架" }
            },
            new Dictionary<string, string>
            {
                { "KR", "기타" },
                { "EN", "Others" },
                { "JP", "その他" },
                { "CN", "其他" }
            }
       };
    }


    // 인사 미술관
    public void museumSceneInit()
    {
        scenes = new Dictionary<string, string[]>
         {
             {"MuseumList" , new string[]
                 {
                   "Text01",
                   "Text02",
                   "Text03",
                   "Text04",
                   "Text05",
                   "Text06",
                   "Text07",
                   "Text08",
                  }
             }
         };

        scenesTxtArr = new Dictionary<string, string>[]
        {
            new Dictionary<string, string>
            {
                { "KR", @"인사동 미술관" },
                { "EN", @"Insa-dong Art Gallery" },
                { "JP", @"仁寺洞美術館" },
                { "CN", @"仁寺洞美术馆" }
            },
             new Dictionary<string, string>
            {
                { "KR", @"고미술" },
                { "EN", @"Antique Art" },
                { "JP", @"骨董品" },
                { "CN", @"古董艺术" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"화랑" },
                { "EN", @"Gallery" },
                { "JP", @"ギャラリー" },
                { "CN", @"画廊" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"표구" },
                { "EN", @"Calligraphy" },
                { "JP", @"書道" },
                { "CN", @"装裱" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"전시관" },
                { "EN", @"Exhibition" },
                { "JP", @"展示館" },
                { "CN", @"展览馆​" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"역사유적지" },
                { "EN", @"Historical Site" },
                { "JP", @"歴史的遺跡​" },
                { "CN", @"历史遗址" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"기타" },
                { "EN", @"Other" },
                { "JP", @"その他" },
                { "CN", @"其他​" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"<color=#616161>• QR</color>를 찍으시면  <color=#616161>WIT 어플이</color> 길안내를 해드려요" },
                { "EN", @"<color=#616161>• Scan the QR code</color> to navigate easily with the <color=#616161>WIT app</color>" },
                { "JP", @"<color=#616161>• QR</color>をスキャンすると、<color=#616161>WITアプリ</color>が道案内をしてくれます" },
                { "CN", @"<color=#616161>• 扫描 QR</color>，<color=#616161>WIT 应用程序</color>会为您指路" }
            }
        };
    }


    // 여기는 인사동
    public void InfoSceneInit() {
        scenes = new Dictionary<string, string[]>
        {
            {"Info" , new string[]
                {
                  "SceneInfo",
                  "historyTitle",
                  "historyContent",
                  "cultureTitle",
                  "cultureContent",
                  "placeTitle",
                  "placeContent"
                 }
            }
        };

        scenesTxtArr = new Dictionary<string, string>[]
        {
            new Dictionary<string, string>
            {
                { "KR", @"여기는 인사동" },
                { "EN", @"Welcome to Insadong" },
                { "JP", @"インサドンの紹介" },
                { "CN", @"这里是仁寺洞" }
            },
            new Dictionary<string, string>
            {
                { "KR", "역사" },
                { "EN", "History" },
                { "JP", "歴史" },
                { "CN", "历史" }
            },
            new Dictionary<string, string>
            {
                { "KR", "인사동은 조선시대부터 한양의 중심지로서, 선비와 학자들이 모여 글을 쓰고 교류하던 곳이었습니다. 특히 서화와 고서적이 많이 거래되었고, 일제강점기에는 일본인들이 문화재를 거래하기도 했습니다. 이후 한국전쟁을 거치면서 전통 문화와 예술이 남아있는 곳으로 발전했습니다." },
                { "EN", "Insadong has been the center of Hanyang since the Joseon Dynasty, where scholars and literati gathered to write and exchange ideas. It was especially known for the trading of calligraphy, paintings, and old books. During the Japanese occupation, Japanese merchants traded cultural artifacts here. After the Korean War, it evolved into a place where traditional culture and art are preserved." },
                { "JP", "インサドンは、朝鮮時代から漢陽の中心地として、学者や文人たちが集まり、文章を作成し、交流していた場所です。特に書画や古書が盛んに取引されており、日帝時代には日本人が文化財を取引していました。その後、韓国戦争を経て、伝統文化や芸術が残る場所へと発展しました。" },
                { "CN", "仁寺洞自朝鲜时代以来一直是汉阳的中心，文人和学者们在这里聚集，写作和交流。特别是书画和古籍交易非常活跃，在日本占领时期，日本人也在这里交易文化遗产。经过韩国战争后，仁寺洞发展成为一个保存传统文化和艺术的地方。" }
            },
            new Dictionary<string, string>
            {
                { "KR", "문화" },
                { "EN", "Culture" },
                { "JP", "文化" },
                { "CN", "文化" }
            },
            new Dictionary<string, string>
            {
                { "KR", "인사동은 한국의 전통 문화를 체험할 수 있는 장소로 유명합니다. 전통 공예품, 도자기, 한지 공예 등 다양한 전통 예술품을 구매할 수 있으며, 전통 찻집에서 다도(茶道)를 체험할 수도 있습니다. 거리 곳곳에는 갤러리와 전시관이 있어 현대와 전통이 조화롭게 공존하는 문화적 매력을 느낄 수 있습니다." },
                { "EN", "Insadong is famous for being a place where you can experience traditional Korean culture. You can purchase various traditional artworks, such as handicrafts, ceramics, and Hanji crafts, and experience tea ceremonies in traditional tea houses. The streets are lined with galleries and exhibition halls, offering a cultural charm where modern and traditional elements harmoniously coexist." },
                { "JP", "インサドンは、韓国の伝統文化を体験できる場所として有名です。伝統工芸品や陶器、韓紙工芸などの様々な伝統芸術品を購入でき、伝統的な茶屋で茶道を体験することもできます。街の至る所にはギャラリーや展示館があり、現代と伝統が調和して共存する文化的な魅力を感じることができます。" },
                { "CN", "仁寺洞以体验韩国传统文化而闻名。您可以购买各种传统艺术品，如手工艺品、陶瓷和韩纸工艺，还可以在传统茶馆体验茶道。街道两旁有许多画廊和展览馆，展现出现代与传统和谐共存的文化魅力。" }
            },
            new Dictionary<string, string>
            {
                { "KR", "관광명소" },
                { "EN", "Tourist Attraction" },
                { "JP", "観光スポット" },
                { "CN", "景點" }
            },new Dictionary<string, string>
            {
                { "KR", "인사동 거리 자체가 주요 관광 명소로, 골목마다 숨겨진 전통 가옥, 한옥 카페, 맛집들이 자리하고 있습니다. 특히 주말에는 차량이 통제되어 보행자 전용 거리로 변하며, 다양한 거리 공연과 전시회가 열립니다. 근처에 위치한 조계사와 탑골공원도 함께 방문하기 좋은 명소입니다." },
                { "EN", "Insadong itself is a major tourist attraction, with traditional houses, hanok cafes, and eateries tucked away in every alley. On weekends, the street is closed to cars and becomes a pedestrian-only street, with a variety of street performances and exhibitions. Nearby Jogyesa Temple and Tapgol Park are also great places to visit." },
                { "JP", "インサドン通り自体が主要な観光名所で、路地ごとに隠れた伝統的な家屋やハノクカフェ、美味しい食事処があります。特に週末には車両が通行止めになり、歩行者専用の通りとなり、さまざまなストリートパフォーマンスや展示会が行われます。近くに位置する曹渓寺やタッコル公園も訪れるのに適した名所です。" },
                { "CN", "仁寺洞街本身就是一个主要的旅游景点，每条小巷里都有隐藏的传统房屋、韩屋咖啡馆和受欢迎的餐馆。尤其是在周末，街道会禁止车辆通行，变成步行街，举办各种街头表演和展览。附近的曹溪寺和塔谷公园也是值得一游的景点。" }
            }
        };
    }


    // 안녕 인사
    public void HiSceneInit() {
        scenes = new Dictionary<string, string[]>
        {
            { "Hi", new string[]
                {
                    "TextTitle",
                    "NameTitle",
                    "NameContent",
                    "BirthDayTitle",
                    "BirthDayContent",
                    "HomeTownTitle",
                    "HomeTownContent",
                    "NationalityTitle",
                    "NationalityContent",
                    "BloodTypeTitle",
                    "BloodTypeContent",
                    "MBTITitle",
                    "MBTIContent",
                    "BodyInfoTitle",
                    "BodyInfoContent",
                    "HobbyTitle",
                    "HobbyContent",
                    "SpecialtyTitle",
                    "SpecialtyContent",
                    "FutureHopesTitle",
                    "FutureHopesContent",
                    "IntroductionTitle",
                    "IntroductionContent"
                }
            }
        };

        scenesTxtArr = new Dictionary<string, string>[]
     {
            new Dictionary<string, string>
            {
                { "KR", @"안녕하세요! 인사를 소개할게요!" },
                { "EN", @"Hello 'INSA'" },
                { "JP", @"こんにちは 'INSA'" },
                { "CN", @"你好 'INSA'" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"이름" },
                { "EN", @"Name" },
                { "JP", @"名前" },
                { "CN", @"姓名" }
            },
           new Dictionary<string, string>
            {
                { "KR", @"인사" },
                { "EN", @"Insa" },
                { "JP", @"インサ" },
                { "CN", @"英莎" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"출생" },
                { "EN", @"DOB" },
                { "JP", @"生年月日" },
                { "CN", @"出生日期" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"2005년 9월 30일" },
                { "EN", @"September 30, 2005" },
                { "JP", @"2005年9月30日" },
                { "CN", @"2005年9月30日" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"출신" },
                { "EN", @"Home" },
                { "JP", @"出身地" },
                { "CN", @"家乡" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"서울 종로구 인사동" },
                { "EN", @"Insadong, Jongno-gu, Seoul" },
                { "JP", @"ソウル特別市鍾路区仁寺洞" },
                { "CN", @"韩国首尔市钟路区仁寺洞" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"국적" },
                { "EN", @"Origin" },
                { "JP", @"国籍" },
                { "CN", @"国籍" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"대한민국" },
                { "EN", @"South Korea" },
                { "JP", @"韓国" },
                { "CN", @"韩国" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"혈액형" },
                { "EN", @"Blood type" },
                { "JP", @"血液型" },
                { "CN", @"血型" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"A형" },
                { "EN", @"Type A" },
                { "JP", @"A型" },
                { "CN", @"A型" }
            },
             new Dictionary<string, string>
            {
                { "KR", @"MBTI" },
                { "EN", @"MBTI" },
                { "JP", @"MBTI" },
                { "CN", @"MBTI" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"ENTJ" },
                { "EN", @"ENTJ" },
                { "JP", @"ENTJ" },
                { "CN", @"ENTJ" }
            },
             new Dictionary<string, string>
            {
                { "KR", @"신체" },
                { "EN", @"Physique" },
                { "JP", @"体型" },
                { "CN", @"身材" }
            },
            new Dictionary<string, string>
            {
                { "KR", "168cm, 235mm (몸무게는 비밀..) " },
                { "EN", "Height 168cm, Shoe size 36.5, (Body weight is TMI)" },
                { "JP", "身長168cm、靴のサイズ 235、（体重はTMI）" },
                { "CN", "身高168cm，鞋码36.5，（体重是TMI）" }
            },
               new Dictionary<string, string>
            {
                { "KR", @"취미" },
                { "EN", @"Hobbies" },
                { "JP", @"趣味" },
                { "CN", @"爱好" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"카페가기, K-POP춤추기, 테니스" },
                { "EN", @"café hopping, K-Pop dance, Tennis" },
                { "JP", @"カフェ巡り、K-Popダンス、テニス" },
                { "CN", @"咖啡馆巡游、K-Pop舞蹈、网球" }
            },
             new Dictionary<string, string>
            {
                { "KR", @"특기" },
                { "EN", @"Talent" },
                { "JP", @"特技" },
                { "CN", @"特长" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"노래" },
                { "EN", @"Singing" },
                { "JP", @"歌" },
                { "CN", @"唱歌" }
            },
              new Dictionary<string, string>
            {
                { "KR", @"장래희망" },
                { "EN", @"Dream" },
                { "JP", @"夢" },
                { "CN", @"我的梦想" }
            },
            new Dictionary<string, string>
            {
                { "KR", "인사동을 전 세계에 알리는 홍보모델이 되고 싶어요, 한복모델, K-POP아이돌도요!" },
                { "EN", @"I aspire to become a globally recognized PR model, represent traditional Korean clothing like hanbok, and pursue a career as a K-pop idol." },
                { "JP", @"世界的に認知されるPRモデルになり、伝統的な韓国の衣装であるハンボクを代表し、K-popアイドルとしてのキャリアを追求したいです。" },
                { "CN", @"我希望成为全球知名的公关模特，代表传统的韩国服装汉服，并追求成为K-pop偶像的职业。" }
            },
              new Dictionary<string, string>
            {
                { "KR", @"자기소개" },
                { "EN", @"Intro" },
                { "JP", @"自己紹介" },
                { "CN", @"自我介紹" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"MZ세대 입니다.! 강아지와 고양이 러버.
한식을 즐겨먹고 사진찍기, 인스타그램, 틱톡하는 것을 좋아해요. 맛집탐방, 옷도 좋아해요!
또 교복입고 놀이동산가기, 한복입고 고궁 놀러가기, K-POP아이돌도 좋아해요" },
                { "EN", @"I'm 20 years old and part of the MZ generation. I love K-pop, dancing, dogs, cats, and Korean food. I'm passionate about taking photos, using Instagram and TikTok, finding trendy restaurants, and fashion. I enjoy visiting Lotte World in school uniforms and exploring palaces while wearing a hanbok. If you're ever curious about Insadong or anything else, feel free to ask me!" },
                { "JP", @"「こんにちは、インサドン（仁寺洞）のマスコット『インサ』です。 私は今年20歳で、K-POPとダンスが好きなMZ世代の大学生です！ 犬と猫が好きで、韓国料理をよく食べ、写真を撮るのが好きで、インスタグラムやTikTokを楽しんでいます。 グルメを探し、ファッションを楽しみ、制服を着てロッテワールドに行くことや、韓服を着て宮殿に遊びに行くことが好きです。K-POPアイドルが好きです。 これからインサドン（仁寺洞）について知りたいことがあれば私に聞いてください！" },
                { "CN", @"你好，我是仁寺洞的吉祥物“仁寺”。 我今年20岁，是一名喜欢K-POP和跳舞的MZ世代大学生。 我喜欢狗狗和猫咪，喜欢吃韩餐，喜欢拍照，喜欢玩Instagram和TikTok， 还喜欢寻找美食、搭配衣服、穿着校服去乐天世界、穿着韩服游玩宫殿。我非常喜欢K-POP偶像。 今后如果对仁寺洞有什么好奇的，随时可以问我哦！" }
            },
     };
    }


    // 도와줘 인사
    public void helpSceneInit() {
        scenes = new Dictionary<string, string[]>
         {
             {"Help" , new string[]
                 {
                  "Text01",
                  "Text08",
                  "Button01",
                  "Button02",
                  "Button03",
                  "Button04",
                  "Button05",
                  "Button06",
                  "Button07",
                  "Button08",
                  "Button09",
                  "Button10",
                  "Button11",
                  "Button12",
                  }
             }
         };

        scenesTxtArr = new Dictionary<string, string>[]
        {
            new Dictionary<string, string>
            {
                { "KR", "인사야 도와줘" },
                { "EN", "Insa, please help me" },
                { "JP", "インサ、助けてください" },
                { "CN", "仁寺，请帮助我" }
            },
             new Dictionary<string, string>
            {
                { "KR", "<color=#616161>• QR</color>를 찍으시면  <color=#616161>WIT 어플이</color> 길안내를 해드려요" },
                { "EN", "<color=#616161>• Scan the QR code</color> to navigate easily with the <color=#616161>WIT app</color>" },
                { "JP", "<color=#616161>• QR</color>をスキャンすると、<color=#616161>WITアプリ</color>が道案内をしてくれます" },
                { "CN", "<color=#616161>• 扫描 QR</color>，<color=#616161>WIT 应用程序</color>会为您指路" }
            },
            new Dictionary<string, string>
            {
                { "KR", "안내소" },
                { "EN", "Information Center" },
                { "JP", "案内所" },
                { "CN", "信息中心" }
            },
            new Dictionary<string, string>
            {
                { "KR", "편의점" },
                { "EN", "Convenience Store" },
                { "JP", "コンビニエンスストア" },
                { "CN", "便利店" }
            },
            new Dictionary<string, string>
            {
                { "KR", "병원" },
                { "EN", "Hospital" },
                { "JP", "病院" },
                { "CN", "医院" }
            },
            new Dictionary<string, string>
            {
                { "KR", "약국" },
                { "EN", "Pharmacy" },
                { "JP", "薬局" },
                { "CN", "药房" }
            },
            new Dictionary<string, string>
            {
                { "KR", "은행" },
                { "EN", "Bank" },
                { "JP", "銀行" },
                { "CN", "银行" }
            },
            new Dictionary<string, string>
            {
                { "KR", "환전소" },
                { "EN", "Currency Exchange" },
                { "JP", "両替所" },
                { "CN", "兑换处" }
            },
            new Dictionary<string, string>
            {
                { "KR", "교회" },
                { "EN", "Church" },
                { "JP", "教会" },
                { "CN", "教堂" }
            },
            new Dictionary<string, string>
            {
                { "KR", "불교" },
                { "EN", "Buddhist Temple" },
                { "JP", "仏教寺院" },
                { "CN", "佛教寺院" }
            },
            new Dictionary<string, string>
            {
                { "KR", "천도교" },
                { "EN", "Cheondoism Temple" },
                { "JP", "天道教寺院" },
                { "CN", "天道教寺院" }
            },
            new Dictionary<string, string>
            {
                { "KR", "인테리어" },
                { "EN", "Construction" },
                { "JP", "建設・電気サービス" },
                { "CN", "建筑与电气服务" }
            },
            new Dictionary<string, string>
            {
                { "KR", "사무실" },
                { "EN", "Company Office" },
                { "JP", "会社事務所" },
                { "CN", "公司办公室" }
            },
            new Dictionary<string, string>
            {
                { "KR", "기타" },
                { "EN", "Other" },
                { "JP", "その他" },
                { "CN", "其他" }
            }
        };
    }
    

    // 미션
    public void missionSceneInit() {
        scenes = new Dictionary<string, string[]>
         {
             {"Mission" , new string[]
                 {
                   "ScenesInfo",
                   "MissionInfo",
                   "MissionContent1",
                   "MissionContent2",
                   "MissionContent3"
                  }
             }
         };

        scenesTxtArr = new Dictionary<string, string>[]
        {
             new Dictionary<string, string>
             {
                 { "KR", @"인사동 미션 리스트" },
                 { "EN", @"Insa-dong mission list" },
                 { "JP", @"仁寺洞ミッションリストです" },
                 { "CN", @"這是仁寺洞任務清單" }
             },
             new Dictionary<string, string>
             {
                 { "KR", "인사동 미션 수행하고 선물 받기!" },
                 { "EN", "Take on the challenge by joining missions and enjoy gifts and experiences!" },
                 { "JP", "挑戦を受けて、ミッションに参加し、ギフトや体験を楽しもう！" },
                 { "CN", "接受挑战，参加任务，享受礼品和体验！" }
             },
             new Dictionary<string, string>
             {
                 { "KR", "WIT 설치하기" },
                 { "EN", "Join WIT: Install the app and sign up." },
                 { "JP", "WITに参加: アプリをインストールしてサインアップ。" },
                 { "CN", "加入WIT: 安装应用程序并注册。" }
             },
             new Dictionary<string, string>
             {
                 { "KR", "QR찍고 WIT APP을 열고 미션 수행하기 " },
                 { "EN", " Scan the Mission QR Code." },
                 { "JP", "ミッションのQRコードをスキャン。" },
                 { "CN", "扫描任务二维码。" }
             },
             new Dictionary<string, string>
             {
                 { "KR", "성공에 따른 다양한 기프트와 이벤트 상품 받기 " },
                 { "EN", "Complete the mission and collect your prize!" },
                 { "JP", "ミッション成功によるギフトを受け取る" },
                 { "CN", "完成任务并领取奖励！" }
             }
        };

    }


    // 교통안내
    public void transSceneInit()
    {
        scenes = new Dictionary<string, string[]>
         {
             {"trans" , new string[]
                 {
                   "SceneInfo",
                   "ButtonText01",
                   "ButtonText02",
                   "ButtonText03",
                   "Box01Text01",
                   "Box01Text02",
                   "Box01Text03",
                   "Box01Text04",
                   "Box02Text01",                 
                   "MapText01",
                   "MapText02",
                   "MapText03",
                   "MapText04",
                   "ParkText01",
                   "ParkText02",
                   "ParkText03",
                   "ParkText04",
                   "ParkText05",
                   "ParkText06",
                   "ParkText07",
                  }
             }
         };

        scenesTxtArr = new Dictionary<string, string>[]
       {
            new Dictionary<string, string>
            {
                { "KR", @"교통 안내" },
                { "EN", @"Transportation Guide" },
                { "JP", @"交通案内" },
                { "CN", @"交通指南" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"대중교통" },
                { "EN", @"Public Transport" },
                { "JP", @"公共交通機関" },
                { "CN", @"公共交通" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"인사동지도" },
                { "EN", @"Insadong Map" },
                { "JP", @"仁寺洞マップ" },
                { "CN", @"仁寺洞地图" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"주차장 안내" },
                { "EN", @"Parking Information" },
                { "JP", @"駐車場案内" },
                { "CN", @"停车场指南" }
            },
            new Dictionary<string, string>
            {
                 { "KR", @"지하철 <color=#FE6C50>•</color><color=#999999><size=24>   대중교통</size></color>" },
                { "EN", @"Subway <color=#FE6C50>•</color><color=#999999><size=24>   Public Transport</size></color>" },
                { "JP", @"地下鉄 <color=#FE6C50>•</color><color=#999999><size=24>   公共交通機関</size></color>" },
                { "CN", @"地铁 <color=#FE6C50>•</color><color=#999999><size=24>   公共交通</size></color>" },
            },
            new Dictionary<string, string>
            {
                 { "KR", @"1호선 종각역 11번출구" },
                { "EN", @"Line 1: Jonggak Station, Exit 11" },
                { "JP", @"1号線：鐘閣駅 11番出口" },
                { "CN", @"1号线：钟阁站 11号出口" },
            },
            new Dictionary<string, string>
            {
                { "KR", @"3호선 안국역 6번출구" },
                { "EN", @"Line 3: Anguk Station, Exit 6" },
                { "JP", @"3号線：安国駅 6番出口" },
                { "CN", @"3号线：安国站 6号出口" },
            },
            new Dictionary<string, string>
            {
                { "KR", @"종로3가역 4번출구" },
                { "EN", @"Jongno 3-ga Station, Exit 4" },
                { "JP", @"公共交通機関" },
                { "CN", @"公共交通" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"버스 <color=#FE6C50>•</color><color=#999999><size=24>   대중교통</size></color>" },
                { "EN", @"Bus <color=#FE6C50>•</color><color=#999999><size=24>   Public Transport</size></color>" },
                { "JP", @"バス <color=#FE6C50>•</color><color=#999999><size=24>   公共交通機関</size></color>" },
                { "CN", @"公交 <color=#FE6C50>•</color><color=#999999><size=24>   公共交通</size></color>" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"한국의 전통문화를 접할 수 있는 서울의 대표 거리 인사동입니다." },
                { "EN", @"Insadong is a key street in Seoul where you can experience traditional Korean culture." },
                { "JP", @"韓国の伝統文化を体験できるソウルの代表的な通り、インサドンです。" },
                { "CN", @"韩国传统文化体验的首尔代表性街道——仁寺洞。。" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"*인사동의 상점검색, 한복AR착장, Ai 추천 코스, 날씨정보제공, 환율정보제공등 다양한 정보와 체험을 제공하는  인사동의 Ai 사이니즈를  경험해보세요~
(인사동의 홍보 도우미 ‘인사’도 함께 만나보세요) " },
                { "EN", @"Discover a range of information and experiences through Insadong's AI signage, including shop searches, Hanbok AI try-ons, AI-recommended activities, weather updates, and currency exchange info. Don’t miss the chance to meet Insa, Insadong's mascot!" },
                { "JP", @"* インサドンのショップ検索、ハンボクAR着用、AIおすすめコース、天気情報、為替情報など、さまざまな情報や体験を提供するインサドンのAIサイネージを体験してください！ 
（インサドンのプロモーションアシスタント「インサ」にもぜひ会ってみてください）" },
                { "CN", @"* 在仁寺洞，您可以体验到多种信息和活动，包括商店搜索、韩服AR试穿、AI推荐行程、天气信息和汇率信息等，尽情体验仁寺洞的AI标牌！ 
（不要错过与仁寺洞的推广助手“仁寺”见面哦）" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"AI Assistant Digital Signage 설치장소 " },
                { "EN", @"Locations for AI Assistant Digital Signage" },
                { "JP", @"AIアシスタントデジタルサイネージ設置場所" },
                { "CN", @"AI助手数字标牌安装地点" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"1.북인사 마당 앞  / 2. 안녕 인사동 앞 / 3. 인사프라자 앞" },
                { "EN", @"1. North Insa Madang
2. Hi, Insadong
3. Insa Plaza" },
                { "JP", @"1. 北側インサマダン
2. アンニョンインサドン
3. インサプラザ" },
                { "CN", @"1. 北仁寺庭
2. 你好仁寺洞
3. 仁寺广场" }
            },
                new Dictionary<string, string>
            {
                { "KR", @"종로Pick" },
                { "EN", @"Jongno Pick" },
                { "JP", @"ジョンノピック" },
                { "CN", @"钟路Pick" }
            },
                new Dictionary<string, string>
            {
                { "KR", @"무료" },
                { "EN", @"Free" },
                { "JP", @"無料" },
                { "CN", @"免费" }
            },
                new Dictionary<string, string>
            {
                { "KR", @"전체이용가" },
                { "EN", @"All Ages" },
                { "JP", @"全年齢対象" },
                { "CN", @"适合所有年龄" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"앱 상세정보" },
                { "EN", @"App Details" },
                { "JP", @"アプリの詳細情報" },
                { "CN", @"应用详细信息" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"종로에는 종로Pick! 종로구 스마트 라이프앱 종로 Pick을 통해 종로구의 실시간 정보와 행정서비스 이용이 가능해집니다. " },
                { "EN", @"In Jongno, you can access real-time information and administrative services through the Jongno Pick! Smart Life App." },
                { "JP", @"鐘路では、鐘路Pick！鐘路区スマートライフアプリ「鐘路Pick」を通じて、鐘路区のリアルタイム情報や行政サービスを利用できます。" },
                { "CN", @"在钟路，您可以通过钟路Pick！钟路区智能生活应用程序“钟路Pick”访问钟路区的实时信息和行政服务。" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"종로 Pick 앱의 5대 주요 서비스" },
                { "EN", @"Five Key Services of the Jongno Pick App" },
                { "JP", @"鐘路Pickアプリの5つの主要サービス" },
                { "CN", @"钟路Pick应用程序的五大主要服务" }
            },
            new Dictionary<string, string>
            {
                { "KR", "<color=#FE6C50>•</color> 공용주차장 실시간 주차정보\n<color=#FE6C50>•</color> SOS 긴급신고\n<color=#FE6C50>•</color> 정화조 청소예약, 건강이상 서비스 신청 등 민원서비스\n<color=#FE6C50>•</color> 프로그램 / 강좌\n<color=#FE6C50>•</color> 종로산책(골목길 탐방코스)" },
                { "EN", "<color=#FE6C50>•</color> Real-time parking information for public parking lots\n<color=#FE6C50>•</color> SOS emergency reporting\n<color=#FE6C50>•</color> Civil service applications such as septic tank cleaning reservations and health issue services\n<color=#FE6C50>•</color> Programs / Courses\n<color=#FE6C50>•</color> Jongno Walk (Alley Exploration Course)" },
                { "JP", "<color=#FE6C50>•</color> 公共駐車場のリアルタイム駐車情報\n<color=#FE6C50>•</color> SOS緊急通報\n<color=#FE6C50>•</color> 浄化槽清掃予約、健康異常サービス申請などの市民サービス\n<color=#FE6C50>•</color> プログラム / 講座\n<color=#FE6C50>•</color> 鍾路散歩（路地探訪コース）" },
                { "CN", "<color=#FE6C50>•</color> 公共停车场实时停车信息\n<color=#FE6C50>•</color> SOS紧急报告\n<color=#FE6C50>•</color> 市民服务，如化粪池清洗预约和健康异常服务申请\n<color=#FE6C50>•</color> 课程 / 教学\n<color=#FE6C50>•</color> 钟路散步（小巷探访课程）" }
            }
       };
    }


    // 고궁안내
    public void gogoongSceneInit()
    {
        scenes = new Dictionary<string, string[]>
         {
             {"gogoongList" , new string[]
                 {
                   "Text01",
                   "GogoongName1",
                   "GogoongAddress1",
                   "GogoongTime1",
                   "GogoongHashtag1",
                   "GogoongName2",
                   "GogoongAddress2",
                   "GogoongTime2",
                   "GogoongHashtag2",
                   "GogoongName3",
                   "GogoongAddress3",
                   "GogoongTime3",
                   "GogoongHashtag3",
                   "GogoongName4",
                   "GogoongAddress4",
                   "GogoongTime4",
                   "GogoongHashtag4",
                  }
             }
         };

        scenesTxtArr = new Dictionary<string, string>[]
        {
            new Dictionary<string, string>
            {
                { "KR", @"고궁정보" },
                { "EN", @"Palaces" },
                { "JP", @"故宮情報" },
                { "CN", @"故宫信息" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"경복궁  <color=#FE6C50>•</color>  <size=24><color=#999999>고궁</color></size>" },
                { "EN", @"Gyeongbokgung  <color=#FE6C50>•</color>  <size=24><color=#999999>Palace</color></size>" },
                { "JP", @"景福宮 (Gyeongbokgung)  <color=#FE6C50>•</color>  <size=24><color=#999999>故宮</color></size>" },
                { "CN", @"景福宫 (Gyeongbokgung)  <color=#FE6C50>•</color>  <size=24><color=#999999>故宫</color></size>" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"서울 종로구 사직로 161" },
                { "EN", @"161 Sajik-ro, Jongno-gu" },
                { "JP", @"ソウル市鍾路区サジク路161" },
                { "CN", @"首尔市钟路区社稷路161号" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"매일 09:00 - 18:00/  화 정기휴무" },
                { "EN", @"Daily 09:00 - 18:00 / Tuesdays off" },
                { "JP", @"毎日 09:00 - 18:00 / 定休日は火曜日" },
                { "CN", @"每日 09:00 - 18:00 / 定期闭馆为星期二" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"#경복궁" },
                { "EN", @"#Gyeongbokgung: Gyeongbok Palace" },
                { "JP", @"#景福宮 (けいふくきゅう)" },
                { "CN", @"#景福宫" },
            },
            new Dictionary<string, string>
            {
                { "KR", @"창덕궁  <color=#FE6C50>•</color>  <size=24><color=#999999>고궁</color></size>" },
                { "EN", @"Changdeokgung  <color=#FE6C50>•</color>  <size=24><color=#999999>Palace</color></size>" },
                { "JP", @"昌徳宮 (Changdeokgung)  <color=#FE6C50>•</color>  <size=24><color=#999999>故宮</color></size>" },
                { "CN", @"昌德宫 (Changdeokgung)  <color=#FE6C50>•</color>  <size=24><color=#999999>故宫</color></size>" },
            },
            new Dictionary<string, string>
            {
                { "KR", @"서울 종로구 율곡로 99" },
                { "EN", @"99 Yulgok-ro, Jongno-gu" },
                { "JP", @"ソウル市鍾路区ユルゴク路99" },
                { "CN", @"首尔市钟路区律曲路99号" },
            },
            new Dictionary<string, string>
            {
                { "KR", @"매일 09:00 - 18:00/ 월 정기휴무" },
                { "EN", @"Daily 09:00 - 18:00 / Mondays off" },
                { "JP", @"毎日 09:00 - 18:00 / 定休日は月曜日" },
                { "CN", @"每日 09:00 - 18:00 / 定期闭馆为星期一" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"#창덕궁" },
                { "EN", @"#Changdeokgung: Changdeok Palace" },
                { "JP", @"#昌徳宮 (しょうとくきゅう)" },
                { "CN", @"#昌德宫" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"창경궁  <color=#FE6C50>•</color>  <size=24><color=#999999>고궁</color></size>" },
                { "EN", @"Changgyeonggung  <color=#FE6C50>•</color>  <size=24><color=#999999>Palace</color></size>" },
                { "JP", @"昌慶宮 (Changgyeonggung)  <color=#FE6C50>•</color>  <size=24><color=#999999>故宮</color></size>" },
                { "CN", @"昌庆宫 (Changgyeonggung)  <color=#FE6C50>•</color>  <size=24><color=#999999>故宫</color></size>" },
            },
            new Dictionary<string, string>
            {
                { "KR", @"서울 종로구 창경궁로 185" },
                { "EN", @"185 Changgyeonggung-ro, Jongno-gu" },
                { "JP", @"ソウル市鍾路区昌慶宮路185" },
                { "CN", @"首尔市钟路区昌庆宫路185号" },
            },
            new Dictionary<string, string>
            {
                { "KR", @"매일 09:00 - 21:00/ 월 정기휴무" },
                { "EN", @"Daily 09:00 - 21:00 / Mondays off" },
                { "JP", @"毎日 09:00 - 21:00 / 定休日は月曜日" },
                { "CN", @"每日 09:00 - 21:00 / 定期闭馆为星期一" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"#창경궁" },
                { "EN", @"#Changgyeonggung: Changgyeong Palace" },
                { "JP", @"#昌慶宮 (しょうけいきゅう)" },
                { "CN", @"#昌庆宫" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"덕수궁  <color=#FE6C50>•</color>  <size=24><color=#999999>고궁</color></size>" },
                { "EN", @"Deoksugung  <color=#FE6C50>•</color>  <size=24><color=#999999>Palace</color></size>" },
                { "JP", @"徳寿宮 (Deoksugung)  <color=#FE6C50>•</color>  <size=24><color=#999999>故宮</color></size>" },
                { "CN", @"德寿宫 (Deoksugung)  <color=#FE6C50>•</color>  <size=24><color=#999999>故宫</color></size>" },
            },
            new Dictionary<string, string>
            {
                { "KR", @"서울 중구 세종대로 99" },
                { "EN", @"99 Sejong-daero, Jung-gu" },
                { "JP", @"ソウル市中区世宗大路99" },
                { "CN", @"首尔市中区世宗大路99号" },
            },
            new Dictionary<string, string>
            {
                { "KR", @"매일 09:00 - 21:00/ 월 정기휴무" },
                { "EN", @"Daily 09:00 - 21:00 / Mondays off" },
                { "JP", @"毎日 09:00 - 21:00 / 定休日は月曜日" },
                { "CN", @"每日 09:00 - 21:00 / 定期闭馆为星期一" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"#덕수궁" },
                { "EN", @"#Deoksugung: Deoksu Palace" },
                { "JP", @"#徳寿宮 (とくじゅきゅう)" },
                { "CN", @"#德寿宫" }
            }
        };
    }

    public void gogoongDetailSceneInit()
    {
        scenes = new Dictionary<string, string[]>
         {
             {"gogoongDetail" , new string[]
                 {
                   "GogoongTitle1",
                   "HistoryTitle1",
                   "HistoryText1",
                   "ShowTitle1",
                   "ShowText1",
                   "TimeTitle1",
                   "TimeText1",
                   "FeeTitle1",
                   "FeeText1",
                   "GogoongTitle2",
                   "HistoryTitle2",
                   "HistoryText2",
                   "ShowTitle2",
                   "ShowText2",
                   "TimeTitle2",
                   "TimeText2",
                   "FeeTitle2",
                   "FeeText2",
                   "GogoongTitle3",
                   "HistoryTitle3",
                   "HistoryText3",
                   "ShowTitle3",
                   "ShowText3",
                   "TimeTitle3",
                   "TimeText3",
                   "FeeTitle3",
                   "FeeText3",
                   "GogoongTitle4",
                   "HistoryTitle4",
                   "HistoryText4",
                   "ShowTitle4",
                   "ShowText4",
                   "TimeTitle4",
                   "TimeText4",
                   "FeeTitle4",
                   "FeeText4",
                }
             }
         };

        scenesTxtArr = new Dictionary<string, string>[]
        {
            new Dictionary<string, string>
            {
                { "KR", @"경복궁" },
                { "EN", @"Gyeongbokgung" },
                { "JP", @"景福宮 (Gyeongbokgung)" },
                { "CN", @"景福宫 (Gyeongbokgung)" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"역사" },
                { "EN", @"History" },
                { "JP", @"歴史" },
                { "CN", @"历史" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"경복궁(景福宮)은 1392년 조선 건국 후
1395년(태조 4)에 창건한 조선왕조 제일의 법궁(法宮)이다.
‘경복’의 이름은 ‘새 왕조가 큰 복을 누려 번영할 것’이라는 의미가 담겨있다.
경복궁은 1592년(선조 25) 임진왜란으로 소실되었는데 그 후 복구되지 못하였다가 270여년이 지난 1867년(고종 4)에 다시 지어졌다. 
1910년 경술국치 후 경복궁은 계획적으로 훼손되기 시작하여 대부분의 전각들이 철거되었고, 1926년에는 조선총독부 청사를 지어 경복궁의 경관을 훼손하였다. 이후 1990년대부터 본격적으로 경복궁 복원공사가 진행되었고, 1995년부터 1997년까지 조선총독부 청사를 철거하였으며 흥례문 일원, 침전 권역, 건청궁과 태원전, 그리고 광화문 등이 복원되어 현재에 이르고 있다." },
                { "EN", @" Gyeongbokgung Palace (景福宮) is the principal royal palace of the Joseon Dynasty, founded in 1395 (4th year of Taejo's reign) after the establishment of the Joseon Dynasty in 1392. The name ""Gyeongbok"" carries the meaning of “the new dynasty will enjoy great blessings and prosper.”
Gyeongbokgung was destroyed during the Japanese invasions of Korea (Imjin War) in 1592 (25th year of Seonjong's reign) and was not restored for over 270 years. It was rebuilt in 1867 (4th year of Gojong's reign).
After the Korea-Japan Treaty of 1910 (Gyeongsul Treaty), Gyeongbokgung began to be systematically damaged, with most of its buildings being demolished. In 1926, the construction of the Governor-General of Korea’s office further marred the palace's landscape. Restoration work began in earnest in the 1990s. Between 1995 and 1997, the Governor-General’s building was demolished, and areas such as Heungnyeomun, the Inner Court, Geunjeongjeon, Taewonjeon, and Gwanghwamun were restored, leading to its current state." },
                { "JP", @"景福宮（Gyeongbokgung Palace）は、1392年に朝鮮が建国された後、1395年（太祖4年）に建設された朝鮮王朝の主要な宮殿です。**「景福」**の名前には、「新しい王朝が大きな幸福を享受し繁栄する」という意味が込められています。
景福宮は1592年（宣祖25年）の日本による侵略（壬辰倭乱）で焼失し、その後長らく修復されることがありませんでしたが、1867年（高宗4年）に再建されました。
1910年（経述条約）後、景福宮は計画的に破壊され始め、大部分の建物が取り壊されました。1926年には朝鮮総督府が建設され、景福宮の景観がさらに損なわれました。その後、1990年代から本格的に景福宮の復元工事が始まり、1995年から1997年にかけて朝鮮総督府の建物が取り壊され、興礼門、内廷、勤政殿、太元殿、光化門などが復元され、現在に至っています" },
                { "CN", @"景福宫（Gyeongbokgung Palace）是朝鲜王朝的主要皇宫，建于1395年（太祖4年），在1392年朝鲜建立后。**“景福”**的名字寓意着“新王朝将享有巨大的福气和繁荣”。
景福宫在1592年（宣宗25年）由于日本侵略朝鲜（壬辰倭乱）被毁，随后未能恢复，直至270多年后的1867年（高宗4年）才重新建造。
1910年（经述条约）后，景福宫开始受到系统性的破坏，大部分建筑被拆除。1926年，为了修建朝鲜总督府，进一步破坏了景福宫的景观。1990年代开始，景福宫的恢复工程正式启动。1995年至1997年间，朝鲜总督府被拆除，兴礼门、内廷、勤政殿、太元殿以及光化门等区域被恢复，至今保留着恢复后的模样。" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"볼거리" },
                { "EN", @"Sights" },
                { "JP", @"見どころ" },
                { "CN", @"景点" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"광화문, 근정전, 항원정" },
                { "EN", @"Gwanghwamun, Geunjeongjeon, Hangwonjeong" },
                { "JP", @"光化門、勤政殿、寒圷亭" },
                { "CN", @"光化门、勤政殿、寒圷亭" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"영업 시간" },
                { "EN", @"Hours" },
                { "JP", @"営業時間" },
                { "CN", @"开放时间" }
            },
            new Dictionary<string, string>
            {
                { "KR", "매일 09:00 - 18:00/  화 정기휴무\n휴무일이 공휴일 경우 익일 휴무 / 입장마감은 1시간전" },
                { "EN", "Daily 09:00 - 18:00 / Tuesdays off\nIf a holiday falls on a closure day, it’s closed the next day; last entry is one hour prior" },
                { "JP", "毎日 09:00 - 18:00 / 定休日は火曜日\n休業日が祝日の場合、翌日も休業 / 最終入場は閉館の1時間前" },
                { "CN", "每日 09:00 - 18:00 / 定期闭馆为星期二\n如果闭馆日为假日，则次日也闭馆；最后入场时间为关闭前一小时" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"입장료" },
                { "EN", @"Admission" },
                { "JP", @"入場料" },
                { "CN", @"门票" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"내국인 만25~64세(3,000원) / 외국인 만19~64세(3,000원) 그외에는 무료" },
                { "EN", @"Koreans(25 to 64); Foreigners(19 to 64) 3,000 won Free for others" },
                { "JP", @"入場料 3,000ウォン, 韓国国籍 25歳~ 64歳, 外国国籍19歳~ 64歳, その他無料" },
                { "CN", @"25至64岁的韩国人和19至64岁的外国人需支付3,000韩元。其他人免费" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"창덕궁" },
                { "EN", @"Changdeokgung" },
                { "JP", @"昌徳宮 (Changdeokgung)" },
                { "CN", @"昌德宫 (Changdeokgung)" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"역사" },
                { "EN", @"History" },
                { "JP", @"歴史" },
                { "CN", @"历史" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"창덕궁(昌德宮)은 1405년(태종 5) 법궁인 경복궁의 이궁(離宮)으로 창건되었다. 
1592년(선조 25) 임진왜란으로 소실되었다가 1610년(광해군 2) 궁궐 중 처음으로 다시 지어졌으며, 이후 역대 왕들이 1867년 경복궁이 중건될 때까지 약 270여년 동안 창덕궁을 더 많이 사용하여 실질적인 법궁의 역할을 하였다. 특히 대조전 부속 건물인 흥복헌은 1910년 경술국치가 결정되었던 비운의 장소이기도 하며, 낙선재 권역은 광복 이후 대한제국의 마지막 황실 가족(순정황후(순종 두 번째 황후), 의민황태자비(이방자 여사), 덕혜옹주(고종의 딸))이 생활하다가 세상을 떠난 곳이기도 하다.
 창덕궁은 다른 궁궐에 비해 인위적인 구조를 따르지 않고 주변 지형과 조화를 이루도록 자연스럽게 건축하여 가장 한국적인 궁궐이라는 평가를 받아 1997년 유네스코 세계유산에 등재되었다." },
                { "EN", @" Changdeokgung Palace was established in 1405 (5th year of King Taejong) as a secondary palace to the main palace, Gyeongbokgung. It was destroyed during the Japanese invasion in 1592 (25th year of King Seonjo) but was the first to be rebuilt in 1610 (2nd year of King Gwanghaegun). For approximately 270 years, until the reconstruction of Gyeongbokgung in 1867, Changdeokgung served as the primary royal palace, playing the role of the de facto main palace. Notably, the Heungbokheon, an annex of the Daejojeon Hall, is the site where the fateful decision regarding the Japan-Korea Protectorate Treaty was made in 1910. The Naksonjae area was where the last members of the Korean imperial family (Empress Sunjeong, the wife of Crown Prince Uimin, and Princess Deokhye) lived and passed away after the liberation.
Changdeokgung is noted for its natural and harmonious architecture with the surrounding landscape, avoiding artificial structures compared to other palaces, which led to its designation as a UNESCO World Heritage site in 1997." },
                { "JP", @"昌徳宮（Changdeokgung Palace） は、1405年（太宗5年）に法宮である景福宮の離宮として設立されました。1592年（宣祖25年）の日本の侵略で焼失しましたが、1610年（光海君2年）に再建され、1867年に景福宮が再建されるまでの約270年間、歴代の王によって実質的な法宮として使用されました。特に、大昌殿附属の興福軒は、1910年の韓国併合が決定された運命の場所であり、落選斎の区域は、解放後の大韓帝国最後の皇室家族（順正皇后（純宗の二番目の皇后）、義愍皇太子妃（李方子）、徳惠翁主（高宗の娘））が生活し、亡くなった場所でもあります。
昌徳宮は、他の宮殿とは異なり、周囲の地形と調和するように自然に建築されており、最も韓国的な宮殿と評価されています。そのため、1997年にユネスコの世界遺産に登録されました。" },
                { "CN", @"昌德宫（Changdeokgung Palace）于1405年（太宗5年）作为法宫景福宫的离宫建立。1592年（宣祖25年）在日本侵略战乱中被焚毁，1610年（光海君2年）重新建造，是所有宫殿中最早重新建造的一个。之后，直到1867年景福宫重建的约270年间，历代国王主要使用昌德宫，因此实际承担了法宫的角色。特别是，大昌殿附属建筑的兴福轩是1910年韩日合并决定的悲剧性场所，而落选斋区域是解放后大韩帝国最后一代皇室家族（顺正皇后（纯宗的第二任皇后）、义愍皇太子妃（李方子）、德惠翁主（高宗的女儿））生活并去世的地方。
昌德宫与其他宫殿不同，建筑上没有人为的结构，而是与周围地形和谐自然地建造，因此被评为最具韩国特色的宫殿，并于1997年被列为联合国教科文组织世界遗产。" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"볼거리" },
                { "EN", @"Sights" },
                { "JP", @"見どころ" },
                { "CN", @"景点" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"인정전, 궐내각사, 낙선재" },
                { "EN", @"Injeongjeon, Gwelnaegaksa, Nakseonjae" },
                { "JP", @"じんせいでん, けつないかくし, らくぜんさい" },
                { "CN", @"仁政殿, 闕内各司, 樂善齋" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"영업 시간" },
                { "EN", @"Hours" },
                { "JP", @"営業時間" },
                { "CN", @"开放时间" }
            },
            new Dictionary<string, string>
            {
                { "KR", "매일 09:00 - 18:00/ 월 정기휴무\n휴무일이 공휴일 경우 익일 휴무 / 입장마감은 1시간전" },
                { "EN", "Daily 09:00 - 18:00 / Mondays off\nIf a holiday falls on a closure day, it’s closed the next day; last entry is one hour prior." },
                { "JP", "毎日 09:00 - 18:00 / 定休日は月曜日\n休業日が祝日の場合、翌日も休業 / 最終入場は閉館の1時間前" },
                { "CN", "每日 09:00 - 18:00 / 定期闭馆为星期一\n如果闭馆日为假日，则次日也闭馆；最后入场时间为关闭前一小时" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"입장료" },
                { "EN", @"Admission" },
                { "JP", @"入場料" },
                { "CN", @"门票" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"내국인 만25~64세(3,000원) / 외국인 만19~64세(3,000원) 그외에는 무료" },
                { "EN", @"3,000 won for Koreans aged 25 to 64 and foreigners aged 19 to 64. Free for others" },
                { "JP", @"25歳から64歳の韓国人と19歳から64歳の外国人は3,000ウォン。他は無料です" },
                { "CN", @"25至64岁的韩国人和19至64岁的外国人需支付3,000韩元。其他人免费" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"창경궁" },
                { "EN", @"Changgyeonggung" },
                { "JP", @"昌慶宮 (Changgyeonggung)" },
                { "CN", @"昌庆宫 (Changgyeonggung)" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"역사" },
                { "EN", @"History" },
                { "JP", @"歴史" },
                { "CN", @"历史" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"창경궁은 원래 1418년(세종 즉위) 세종이 상왕 태종을 위해 창건한 수강궁(壽康宮)이 있던 자리이다. 이후 1483년(성종 14) 성종이 세 명의 대비(세조의 왕비 정희왕후, 덕종의 왕비 소혜왕후, 예종의 왕비 안순왕후)를 위해 수강궁을 크게 확장하고 궁의 이름을 창경궁이라 하였다. 창경궁은 창덕궁과 경계 없이 동궐(東闕)이라는 하나의 궁궐 영역이었으며 주로 왕실 가족들의 생활 공간으로 사용하였다. 
1907년 순종이 황위에 오른 후 창경궁 내에 동물원과 식물원을 조성하면서 궁궐로서의 모습을 잃기 시작하였다. 1911년에는 일제에 의해 궁의 이름마저 창경원(昌慶苑)으로 격하되어 궁궐이 아닌 공원화가 되어 훼손이 심하였다. 광복 후 1983년에 다시 창경궁으로 명칭을 회복한 후, 궁궐 경내에 있던 동물원을 이전하고 본래 궁궐의 모습으로 복원공사가 진행되어 현재의 모습이 되었다." },
                { "EN", @" Changkyunggung Palace was originally the site of the Sugang Palace (寿康宮), established by King Sejong for retired King Taejong in 1418 (the 1st year of Sejong's reign). Later, in 1483 (the 14th year of King Seongjong), King Seongjong greatly expanded Sugang Palace for the three queen mothers (Queen Jeonghui, Queen Sohye, and Queen Ansun) and renamed it Changkyunggung Palace.
Changkyunggung Palace was part of the eastern palace area, known as Donggyeol (東闕), without a distinct boundary from Changdeokgung Palace, and was mainly used as a living space for the royal family.
After Emperor Sunjong ascended to the throne in 1907, Changkyunggung Palace began to lose its palace features as a zoo and botanical garden were established within the palace grounds. In 1911, the Japanese colonial government even downgraded the palace's name to Changkyungwon (昌慶苑), converting it into a park and causing significant damage. After Korea's liberation, the name was restored to Changkyunggung Palace in 1983, and efforts were made to relocate the zoo and restore the palace to its original appearance, resulting in its current state." },
                { "JP", @"慶宮（Changkyunggung Palace）は、元々1418年（世宗即位）に世宗が上王太宗のために建設した寿康宮（寿康宮）があった場所です。その後、1483年（成宗14年）に成宗が三人の皇太后（世祖の王妃・定熙王后、徳宗の王妃・昭惠王后、睿宗の王妃・安順王后）のために寿康宮を大規模に拡張し、宮の名前を昌慶宮としました。昌慶宮は昌徳宮と境界がなく、東闕（東闕）と呼ばれる一つの宮殿領域であり、主に王室家族の生活空間として使用されていました。
1907年に純宗が皇帝に即位した後、昌慶宮内に動物園と植物園が設けられ、宮殿としての姿を失い始めました。1911年には日本によって宮殿の名前が昌慶苑（昌慶苑）に格下げされ、宮殿ではなく公園化され、損傷がひどくなりました。光復後、1983年に再び昌慶宮という名称に回復され、その後宮殿内にあった動物園を移転し、元の宮殿の姿に復元する工事が行われ、現在の姿となりました" },
                { "CN", @"昌庆宫原本是1418年（世宗即位年）由世宗为上王太宗所建的寿康宫（寿康宫）所在的位置。后来，1483年（成宗14年）成宗为了三位皇太后（世祖的王妃定熙王后、德宗的王妃昭惠王后、睿宗的王妃安顺王后），对寿康宫进行了大规模扩建，并将宫殿的名称更改为昌庆宫。昌庆宫与昌德宫没有明确的界限，是东阙（东阙）的一部分，主要作为皇室家族的生活空间使用。
1907年，纯宗即位后，昌庆宫内设立了动物园和植物园，开始逐渐失去宫殿的功能。1911年，日占时期将宫殿的名称降格为昌庆苑（昌庆苑），转变为公园，造成了严重的损坏。光复后，1983年恢复了昌庆宫的名称，随后迁移了宫殿内的动物园，并进行了恢复工程，恢复到了现在的模样。" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"볼거리" },
                { "EN", @"Sights" },
                { "JP", @"見どころ" },
                { "CN", @"景点" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"옥천교, 명정문, 명정전, 함인정, 양화당" },
                { "EN", @"Okcheongyo, Myeongjeongmun, Myeongjeongjeon" },
                { "JP", @"ぎょくせんきょう, めいせいもん, めいせいでん" },
                { "CN", @"玉川桥,  明正门, 明正殿, 含仁亭, 养和堂" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"영업 시간" },
                { "EN", @"Hours" },
                { "JP", @"営業時間" },
                { "CN", @"开放时间" }
            },
            new Dictionary<string, string>
            {
                { "KR", "매일 09:00 - 21:00/ 월 정기휴무\n휴무일이 공휴일 경우 익일 휴무 / 입장마감은 1시간전" },
                { "EN", "Daily 09:00 - 21:00 / Mondays off\nIf a holiday falls on a closure day, it’s closed the next day; last entry is one hour prior." },
                { "JP", "毎日 09:00 - 21:00 / 定休日は月曜日\n休業日が祝日の場合、翌日も休業 / 最終入場は閉館の1時間前" },
                { "CN", "每日 09:00 - 21:00 / 定期闭馆为星期一\n如果闭馆日为假日，则次日也闭馆；最后入场时间为关闭前一小时" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"입장료" },
                { "EN", @"Admission" },
                { "JP", @"入場料" },
                { "CN", @"门票" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"내국인 만24~64세(1,000원) / 외국인 만18~64세(1,000원) 그외에는 무료 " },
                { "EN", @"1,000 won Koreans aged 24 to 64 and foreigners aged 18 to 64. Free for others" },
                { "JP", @"韓国人24〜64歳（1,000ウォン）、外国人7〜64歳（1,000ウォン）、その他は無料です" },
                { "CN", @"韩国人24至64岁（1,000韩元）；外国人7至64岁（1,000韩元），其他人免费" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"덕수궁" },
                { "EN", @"Deoksugung" },
                { "JP", @"徳寿宮 (Deoksugung)" },
                { "CN", @"德寿宫 (Deoksugung)" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"역사" },
                { "EN", @"History" },
                { "JP", @"歴史" },
                { "CN", @"历史" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"덕수궁은 원래 조선 제9대 성종의 형인 월산대군의 사저였고, 그 후에도 월산대군의 후손이 살던 곳이었다. 1592년(선조 25) 임진왜란으로 도성의 궁들이 모두 소실되자 1593년(선조 26)부터 임시 궁궐로 사용하여 정릉동 행궁(貞陵洞 行宮)이라 불렸다. 이후 1611년(광해군 3) 경운궁(慶運宮)으로 이름이 정해지면서 정식 궁궐이 되었다가, 창덕궁이 중건되면서 다시 별궁으로 남게 되었다.
1897년(광무 1) 고종이 대한제국을 선포하고 황제의 자리에 오르자 대한제국의 황궁으로 사용하였다. 이후 황궁에 맞게 규모를 확장하고 격식을 높였으며, 궁궐 내 서양식 건물을 짓기 시작하여 전통 건축물과 서양식 건축물이 조화를 이루게 되었다. 그러나 1904년(광무 8) 대화재로 많은 건물이 소실되었고, 1907년 일제에 의해 고종이 황위에서 물러나자 궁의 이름이 덕수궁으로 바뀌게 되었다. 1946~47년에는 덕수궁 석조전에서 제1·2차 미소공동위원회를 개최하기도 하였다. " },
                { "EN", @" Deoksugung was originally the residence of Grand Prince Wolsan, the brother of the 9th Joseon king, Seongjong. It continued to be occupied by his descendants. After the palaces in the capital were destroyed during the Imjin War in 1592, it was used as a temporary palace starting in 1593 and was known as Jeongneung-dong Haenggung (행궁).
In 1611, it was officially named Gyeongun Palace (경운궁) and became a formal royal palace. However, after the reconstruction of Changdeokgung, it reverted to being a subsidiary palace.
When Gojong declared the establishment of the Korean Empire in 1897 and ascended to the throne, Deoksugung was used as the imperial palace of the Korean Empire. The palace was expanded to suit its new role, with the construction of Western-style buildings alongside traditional Korean architecture, creating a blend of both styles.
However, in 1904, a major fire destroyed many of the buildings. In 1907, following Gojong's abdication under Japanese pressure, the palace was renamed Deoksugung. From 1946 to 1947, the Seokjojeon Hall in Deoksugung hosted the 1st and 2nd US-Soviet Joint Committees." },
                { "JP", @"德寿宫（Deoksugung）は、元々朝鲜第9代国王成宗の兄、月山大君の私邸であり、その後も月山大君の子孫が住んでいた場所です。1592年（宣祖25年）の壬辰倭乱で都の宮殿がすべて焼失したため、1593年（宣祖26年）から臨時の宮殿として使用され、正陵洞行宮（Jeongneung-dong Haenggung）と呼ばれました。その後、1611年（光海君3年）に慶運宮（Gyeongun Palace）と正式に名付けられ、正式な宮殿となりましたが、昌徳宮の再建に伴い再び別宮となりました。
1897年（光武1年）に高宗が大韓帝国を宣言し、皇帝に即位すると、德寿宫は大韓帝国の皇宮として使用されました。皇宮にふさわしく規模を拡張し、格式を高め、西洋式の建物も建てられ、伝統的な建物と西洋式の建物が調和するようになりました。しかし、1904年（光武8年）の大火で多くの建物が焼失し、1907年には日本の圧力により高宗が退位すると宮殿の名前が德寿宫に変更されました。1946年から1947年には、德寿宫の石造殿で第1次及び第2次米ソ共同委員会が開催されました。" },
                { "CN", @"德寿宫原本是朝鲜第九代国王成宗的兄弟月山大君的私邸，后来也成为了月山大君后代的住所。1592年（宣祖25年）壬辰战争中都城的宫殿全部被毁，1593年（宣祖26年）起被作为临时宫殿使用，称为正陵洞行宫。1611年（光海君3年），正式命名为庆运宫，成为正式的宫殿，但随着昌德宫的重建，德寿宫再次成为别宫。
1897年（光武1年），高宗宣布建立大韩帝国并即位为皇帝，德寿宫被作为大韩帝国的皇宫使用。随后，宫殿规模被扩建，等级被提高，还开始建造西洋风格的建筑，使传统建筑与西洋建筑相互融合。然而，1904年（光武8年）的大火造成了大量建筑物的损毁，1907年，日本人迫使高宗退位后，宫殿的名称被更改为德寿宫。1946年至1947年期间，德寿宫石造殿还举办了第一次和第二次美苏共同委员会会议。" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"볼거리" },
                { "EN", @"Sights" },
                { "JP", @"見どころ" },
                { "CN", @"景点" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"중화전, 정관헌, 돈덕전, 즉조당, 석어당" },
                { "EN", @"Jungwajeon, Jeonggwanheon, Dondeokjeon, Jeukjodang, Seogeodang" },
                { "JP", @"中華殿、正観軒、敦徳殿、即照堂、石御堂" },
                { "CN", @"中华殿、正观轩、敦德殿、即照堂、石御堂" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"영업 시간" },
                { "EN", @"Hours" },
                { "JP", @"営業時間" },
                { "CN", @"开放时间" }
            },
            new Dictionary<string, string>
            {
                { "KR", "매일 09:00 - 21:00/ 월 정기휴무\n휴무일이 공휴일 경우 익일 휴무 / 입장마감은 1시간전" },
                { "EN", "Daily 09:00 - 21:00 / Mondays off\nIf a holiday falls on a closure day, it’s closed the next day; last entry is one hour prior." },
                { "JP", "毎日 09:00 - 21:00 / 定休日は月曜日\n休業日が祝日の場合、翌日も休業 / 最終入場は閉館の1時間前" },
                { "CN", "每日 09:00 - 21:00 / 定期闭馆为星期一\n如果闭馆日为假日，则次日也闭馆；最后入场时间为关闭前一小时" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"입장료" },
                { "EN", @"Admission" },
                { "JP", @"入場料" },
                { "CN", @"门票" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"내국인 만24~64세(1,000원)  / 외국인 만7~64세(1,000원) 그외에는 무료" },
                { "EN", @"Free for youths under 24; 1,000 won for foreign visitors" },
                { "JP", @"韓国人24〜64歳（1,000ウォン）、外国人7〜64歳（1,000ウォン）、その他は無料です" },
                { "CN", @"韩国人24至64岁（1,000韩元）；外国人7至64岁（1,000韩元），其他人免费" }
            }
        };
    }

    public void aiListSceneInit() {
        scenes = new Dictionary<string, string[]>
         {
             {"aiList" , new string[]
                 {
                   "Text01",
                   "BtnText01",
                   "BtnText02",
                   "BtnText03",
                   "CourseText",
                   "ApplicationInfo",
                   "CompanyInfoText",
                   "CourseInfo",
                   "DetailApplicationInfo"
                  }
             }
         };

        scenesTxtArr = new Dictionary<string, string>[]
        {
             new Dictionary<string, string>
            {
                { "KR", @"인사가 추천해요 !" },
                { "EN", @"Insa’s Top Picks for You!" },
                { "JP", @"あなたのためのInsaのお勧めトップピック！" },
                { "CN", @"Insa为您推荐的热门精选！" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"A 코스" },
                { "EN", @"Course A" },
                { "JP", @"Aコース" },
                { "CN", @"A课程" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"B 코스" },
                { "EN", @"Course B" },
                { "JP", @"Bコース" },
                { "CN", @"B课程" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"C 코스" },
                { "EN", @"Course C" },
                { "JP", @"Cコース" },
                { "CN", @"C课程" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"• 코스를 클릭하시면 상세 리스트가 나와요" },
                { "EN", @"• Click the course for details" },
                { "JP", @"• コースをクリックして詳細を確認してください。" },
                { "CN", @"• 点击课程查看详情。" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"<color=#616161>• QR</color>를 찍으시면  <color=#616161>WIT 어플이</color> 길안내를 해 드려요
다양한 할인 혜택과 이벤트도 경험하실 수 있어요" },
                { "EN", @"<color=#616161>• Scan the QR</color> to get directions from the <color=#616161>WIT app</color>. 
You can also experience various discount benefits and events from different businesses." },
                { "JP", @"<color=#616161>• QR</color>をスキャンすると、<color=#616161>WITアプリ</color>が道案内をします。
さまざまな割引特典やイベントも体験できます。" },
                { "CN", @"<color=#616161>• 扫描二维码</color>可以让<color=#616161>WIT应用程序</color>为您导航。
您还可以体验不同商家的各种折扣优惠和活动。" }
            },
          new Dictionary<string, string>
            {
                { "KR", @"• 업체 정보를 클릭하시면 상세 정보가 나와요" },
                { "EN", @"• Click the store for details." },
                { "JP", @"• 店舗をクリックして詳細を確認してください。" },
                { "CN", @"• 点击商店查看详情。" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"<color=#616161>• QR</color>를 찍고 코스추천  <color=#616161>미션을 수행</color> 해보세요
인사동에서 제공한<color=#616161> 다양한 할인쿠폰</color>이 제공됩니다." },
                { "EN", @"<color=#616161>• Scan the QR</color> and try completing the <color=#616161>course recommendation mission</color>. 
You will receive <color=#616161>various discount coupons</color> provided by Insadong." },
                { "JP", @"<color=#616161>• QR</color>をスキャンしてコース推薦の<color=#616161>ミッションを実行</color>してみてください。
インサドンから提供された<color=#616161>さまざまな割引クーポン</color>が手に入ります。" },
                { "CN", @"<color=#616161>• 扫描二维码</color>，尝试完成<color=#616161>课程推荐任务</color>。 
您将获得<color=#616161>由仁寺洞提供的各种优惠券</color>。" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"<color=#616161>• QR</color>를 찍으시면  <color=#616161>WIT 어플이</color> 길안내를 해 드려요
다양한 할인 혜택과 이벤트도 경험하실 수 있어요" },
                { "EN", @"<color=#616161>• Scan the QR</color> to get directions from the <color=#616161>WIT app</color>. 
You can also experience various discount benefits and events from different businesses." },
                { "JP", @"<color=#616161>• QR</color>をスキャンすると、<color=#616161>WITアプリ</color>が道案内をします。
さまざまな割引特典やイベントも体験できます。" },
                { "CN", @"<color=#616161>• 扫描二维码</color>可以让<color=#616161>WIT应用程序</color>为您导航。
您还可以体验不同商家的各种折扣优惠和活动。" }
            },
        };
    }

    // AI 씬 번역
    public void aiSelectSceneInit() {
        scenes = new Dictionary<string, string[]>
         {
             {"aiSelect" , new string[]
                 {
                   "Text01",
                   "Text02",
                   "Text03",
                   "Text04",
                   "BtnText1",
                    "BtnText2",
                    "BtnText3",
                    "BtnText4",
                    "BtnText5",
                    "BtnText6",
                    "BtnText7",
                    "BtnText8",
                    "BtnText9",
                    "BtnText10",
                    "BtnText11",
                    "BtnText12",
                    "BtnText13",
                    "BtnText14",
                    "BtnText15",
                    "BtnText16",
                    "BtnText17",
                    "BtnText18",
                    "BtnText19",
                    "BtnText20",
                    "BtnText21",
                    "BtnText22",
                    "BtnText23",
                    "BtnText24",
                    "BtnText25",
                    "BtnText26",
                    "BtnText27",
                    "BtnText28",
                    "BtnText29",
                    "BtnText30",
                    "BtnText31",
                    "BtnText32",
                    "BtnText33",
                    "BtnText34",
                    "BtnText35",
                    "BtnText36",
                    "BtnText37",
                    "BtnText38",
                    "BtnText39",
                    "BtnText40",
                    "BtnText41"
                  }
             }
         };

        scenesTxtArr = new Dictionary<string, string>[]
        {
            new Dictionary<string, string>
            {
                { "KR", @"나만의 추천 코스 만들기" },
                { "EN", @"Create your personalized itinerary" },
                { "JP", @"あなたのオリジナルの旅程を作成する" },
                { "CN", @"创建您的个性化行程" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"방문인원" },
                { "EN", @"Number of visitors" },
                { "JP", @"訪問者数" },
                { "CN", @"访客人数" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"체류시간" },
                { "EN", @"How long are you planning to be here" },
                { "JP", @"どのくらい滞在する予定ですか？" },
                { "CN", @"您计划在这里停留多久？" }
            },
            new Dictionary<string, string>
            {
                { "KR", @" '인사'에게서 추천받기(3개)" },
                { "EN", @"Get personalised recommendations from Insa(picks 3)" },
                { "JP", @"インサからのパーソナライズされたおすすめを受け取る(3つ選択)" },
                { "CN", @"获取来自Insa的个性化推荐(选择3个)" }
            }, // 텍스트 끝냄 밑에부턴 버튼
             new Dictionary<string, string>
            {
                { "KR", @"1명" },
                { "EN", @"1" },
                { "JP", @"1人" },
                { "CN", @"1人" }
            },
               new Dictionary<string, string>
            {
                { "KR", @"2명" },
                { "EN", @"2" },
                { "JP", @"2人" },
                { "CN", @"2人" }
            },
                new Dictionary<string, string>
            {
                { "KR", @"3명" },
                { "EN", @"3" },
                { "JP", @"3人" },
                { "CN", @"3人" }
            },
             new Dictionary<string, string>
            {
                { "KR", @"4명" },
                { "EN", @"4" },
                { "JP", @"4人" },
                { "CN", @"4人" }
            },
              new Dictionary<string, string>
            {
                { "KR", @"5명" },
                { "EN", @"5" },
                { "JP", @"5人" },
                { "CN", @"5人" }
            },
               new Dictionary<string, string>
            {
                { "KR", @"6명" },
                { "EN", @"6" },
                { "JP", @"6人" },
                { "CN", @"6人" }
            },
                new Dictionary<string, string>
            {
                { "KR", @"0~2시간" },
                { "EN", @"0~2H" },
                { "JP", @"0～2時間" },
                { "CN", @"0~2小时" }
            },
                new Dictionary<string, string>
            {
                { "KR", @"2~4시간" },
                { "EN", @"2~4H" },
                { "JP", @"2~4時間" },
                { "CN", @"2~4小时" }
            },
                new Dictionary<string, string>
            {
                { "KR", @"4~6시간" },
                { "EN", @"4~6H" },
                { "JP", @"4~6時間" },
                { "CN", @"4~6小时" }
            },
                new Dictionary<string, string>
            {
                { "KR", @"6시간~" },
                { "EN", @"6H~" },
                { "JP", @"6時間~" },
                { "CN", @"6小时~" }
            },
                new Dictionary<string, string>
            {
                { "KR", "코리안\n바베큐" },
                { "EN", "Korean BBQ" },
                { "JP", "韓国のバーベキュー" },
                { "CN", "韩国烧烤" }
            },
                new Dictionary<string, string>
            {
                { "KR", "한식" },
                { "EN", "Korean food" },
                { "JP", "韓国料理" },
                { "CN", "韩国菜" }
            },
                new Dictionary<string, string>
            {
                { "KR", "한정식" },
                { "EN", "Course meal" },
                { "JP", "韓定食" },
                { "CN", "韩定食" }
            },
                new Dictionary<string, string>
            {
                { "KR", "사찰 음식" },
                { "EN", "Temple Food​" },
                { "JP", "査察料理" },
                { "CN", "寺庙食物" }
            },
                new Dictionary<string, string>
            {
                { "KR", "채식/비건" },
                { "EN", "Vegetarian" },
                { "JP", "ベジタリアン/ビーガン" },
                { "CN", "素食/纯素食" }
            },
                new Dictionary<string, string>
            {
                { "KR", "아시안푸트" },
                { "EN", "Asian" },
                { "JP", "アジア" },
                { "CN", "亚洲人" }
            },
            new Dictionary<string, string>
            {
                { "KR", "다과" },
                { "EN", "Snacks" },
                { "JP", "お茶菓子" },
                { "CN", "点心" }
            },
                new Dictionary<string, string>
            {
                { "KR", "중식" },
                { "EN", "Chinese food" },
                { "JP", "昼食" },
                { "CN", "午餐" }
            },
            new Dictionary<string, string>
            {
                { "KR", "분식" },
                { "EN", "Street Food" },
                { "JP", "韓国の軽食" },
                { "CN", "韩国小吃" }
            },
            new Dictionary<string, string>
            {
                { "KR", "막걸리" },
                { "EN", "Makgeolli" },
                { "JP", "マッコリ" },
                { "CN", "韩式米酒" }
            },
            new Dictionary<string, string>
            {
                { "KR", "전통주" },
                { "EN", "Traditional Liquor" },
                { "JP", "伝統酒" },
                { "CN", "传统酒" }
            },
            new Dictionary<string, string>
            {
                { "KR", "카페" },
                { "EN", "Cafe" },
                { "JP", "カフェ" },
                { "CN", "咖啡馆" }
            },
               new Dictionary<string, string>
            {
                { "KR", "전통차" },
                { "EN", "Tea House" },
                { "JP", "伝統的な車" },
                { "CN", "传统茶" }
            },
             new Dictionary<string, string>
            {
                { "KR", "기념품" },
                { "EN", "Souvenirs" },
                { "JP", "お土産" },
                { "CN", "纪念品" }
            },
            new Dictionary<string, string>
            {
                { "KR", "체험" },
                { "EN", "Experience" },
                { "JP", "体験" },
                { "CN", "体验" }
            },
            new Dictionary<string, string>
            {
                { "KR", "사진촬영" },
                { "EN", "Photo Shoot" },
                { "JP", "写真撮影" },
                { "CN", "拍照" }
            },
            new Dictionary<string, string>
            {
                { "KR", "K-POP" },
                { "EN", "K-POP" },
                { "JP", "K-POP" },
                { "CN", "韩流音乐" }
            },
            new Dictionary<string, string>
            {
                { "KR", "의류" },
                { "EN", "Clothing" },
                { "JP", "衣類" },
                { "CN", "服饰" }
            },
            new Dictionary<string, string>
            {
                { "KR", "공예품" },
                { "EN", "Crafts" },
                { "JP", "工芸品" },
                { "CN", "工艺品" }
            },
            new Dictionary<string, string>
            {
                { "KR", "수제도장" },
                { "EN", "Handmade Seal" },
                { "JP", "手作り印鑑" },
                { "CN", "手工印章" }
            },
            new Dictionary<string, string>
            {
                { "KR", "엔틱" },
                { "EN", "Antiques" },
                { "JP", "アンティーク" },
                { "CN", "古董" }
            },
            new Dictionary<string, string>
            {
                { "KR", "한복" },
                { "EN", "Hanbok" },
                { "JP", "韓服" },
                { "CN", "韩服" }
            },
            new Dictionary<string, string>
            {
                { "KR", "필방" },
                { "EN", "Stationery Shop" },
                { "JP", "文房具店" },
                { "CN", "文房具店" }
            },
            new Dictionary<string, string>
            {
                { "KR", "잡화" },
                { "EN", "General Goods" },
                { "JP", "雑貨" },
                { "CN", "杂货" }
            },
            new Dictionary<string, string>
            {
                { "KR", "표구" },
                { "EN", "Mounting" },
                { "JP", "表具" },
                { "CN", "裱画" }
            },
            new Dictionary<string, string>
            {
                { "KR", "액자" },
                { "EN", "Frame" },
                { "JP", "額縁" },
                { "CN", "相框" }
            },
            new Dictionary<string, string>
            {
                { "KR", "고미술" },
                { "EN", "Antique Art" },
                { "JP", "古美術" },
                { "CN", "古代美术" }
            },
            new Dictionary<string, string>
            {
                { "KR", "화랑" },
                { "EN", "Gallery" },
                { "JP", "画廊" },
                { "CN", "画廊" }
            },
            new Dictionary<string, string>
            {
                { "KR", "전시관" },
                { "EN", "Exhibition Hall" },
                { "JP", "展示館" },
                { "CN", "展览馆" }
            },
            new Dictionary<string, string>
            {
                { "KR", "역사유적지" },
                { "EN", "Historical Site" },
                { "JP", "歴史遺跡" },
                { "CN", "历史遗址" }
            },
            new Dictionary<string, string>
            {
                { "KR", "인사에게 추천받기" },
                { "EN", "Recommended by Insadong" },
                { "JP", "インサドンのおすすめ" },
                { "CN", "仁寺洞推荐" }
            }
        };
    }

    public void eventSceneInit() {

        
        scenes = new Dictionary<string, string[]>
         {
             {"event" , new string[]
                 {
                   "Text01",
                   "BtnText01",
                   "BtnText02",
                   "BtnText03",
                   "BtnText04",
                   
                  }
             }
         };

        scenesTxtArr = new Dictionary<string, string>[]
        {
            new Dictionary<string, string>
            {
                { "KR", @"인사동 이벤트" },
                { "EN", @"Events" },
                { "JP", @"おすすめイベント" },
                { "CN", @"推荐活动" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"오늘의 행사" },
                { "EN", @"Today's Events" },
                { "JP", @"今日のイベント" },
                { "CN", @"今天的活动" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"이번주 행사" },
                { "EN", @"This Week's Events" },
                { "JP", @"今週のイベント" },
                { "CN", @"本周的活动" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"예정 행사" },
                { "EN", @"Upcoming Events" },
                { "JP", @"予定のイベント" },
                { "CN", @"即将举行的活动" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"지난 행사" },
                { "EN", @"Past Events" },
                { "JP", @"過去のイベント" },
                { "CN", @"过去的活动" }
            }
        };
    }

    // 환율
    public void exchangeSceneInit()
    {

        scenes = new Dictionary<string, string[]>
         {
             {"exchange" , new string[]
                 {
                   "Title",
                  }
             }
         };

        scenesTxtArr = new Dictionary<string, string>[]
        {
            new Dictionary<string, string>
            {
                { "KR", @"오늘의 환율" },
                { "EN", @"Today's exchange rates" },
                { "JP", @"今日の為替レート" },
                { "CN", @"今日汇率" }
            },
        };
    }

    // 한복 설명 번역
    public void aRExplain()
    {
        scenes = new Dictionary<string, string[]>
         {
             {"aRExplain" , new string[]
                 {
                   "Title",
                   "ButtonText1",
                   "ButtonText2",
                   "ButtonText3",
                   "Text06",
                   "Text07",
                 }
             }
         };

        scenesTxtArr = new Dictionary<string, string>[]
        {
            new Dictionary<string, string>
            {
                { "KR", @"한복설명" },
                { "EN", @"Hanbok" },
                { "JP", @"韓服" },
                { "CN", @"韩服" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"여자한복" },
                { "EN", @"Women’s Hanbok" },
                { "JP", @"女性用韓服" },
                { "CN", @"女式韩服" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"남자한복" },
                { "EN", @"Men’s Hanbok" },
                { "JP", @"男性用韓服" },
                { "CN", @"男式韩服" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"궁중한복" },
                { "EN", @"Royal Hanbok" },
                { "JP", @"宮廷韓服" },
                { "CN", @"宫廷韩服" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"한복설명" },
                { "EN", @"Hanbok" },
                { "JP", @"韓服" },
                { "CN", @"韩服" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"한복은 한국 전통 의복으로, 오랜 역사와 전통을 가진 옷입니다. 한복은 고유의 아름다움과 독특한 디자인으로 한국 문화를 대표하는 상징 중 하나로 여겨지며, 특별한 행사나 명절 때 주로 착용됩니다. 직선적인 서양 복식과 달리, 한복은 부드럽게 흐르는 선과 넉넉한 옷자락이 특징입니다. 한복은 단순한 의복을 넘어 한국의 전통과 문화를 상징하는 중요한 문화유산으로, 세계적으로도 그 아름다움을 인정받고 있습니다." },
                { "EN", @"‘Hanbok’is a traditional Korean garment with a long history and deep-rooted traditions. Renowned for its inherent beauty and unique design, ‘Hanbok’ is considered one of the symbols representing Korean culture and is primarily worn during special occasions and holidays. Unlike the linear fashion of Western clothing, Hanbok features soft, flowing lines and generous fabric. It is a significant cultural heritage that symbolizes Korean tradition and culture, and its beauty is recognized worldwide." },
                { "JP", @"「ハンボク(韓服)は韓国の伝統的な衣服で、長い歴史と伝統を持つ服です。
 ハンボク(韓服)は固有の美しさとユニークなデザインで韓国文化を代表する象徴の一つとされ、特別な行事や祝日に主に着用しています。直線を主に表現する西洋服とは異なり、韓服は柔らかく流れるような線とゆったりとした服の裾が特徴です。ハンボク(韓服)は単なる衣服を超えて、韓国の伝統と文化を象徴する重要な文化遺産であり、世界的にもその美しさが認められています。" },
                { "CN", @"韩服是韩国的传统服饰，拥有悠久的历史和传统。韩服因其独特的美感和设计，被视为韩国文化的象征之一，通常在特别的活动或节日时穿着。与直线的西方服饰不同，韩服的特点是柔和流畅的线条和宽松的衣摆。韩服不仅仅是一种服饰，更是象征韩国传统和文化的重要文化遗产，全球也认可它的美丽。" }
            },
        };
    }

    // AR select 번역
    public void aRSelectSceneInit()
    {
        scenes = new Dictionary<string, string[]>
         {
             {"aRSelect" , new string[]
                 {
                   "Text",
                   "Text01",
                   "Text02",
                   "Text03",
                   "Text04",
                   "Text05",
                   "Text06",
                   "Text07",
                   "Text08",
                   "ButtonText1",
                   "ButtonText2",
                   "ButtonText3",
                   "ButtonText4",
                   "ConfirmText",
                   "PhotoButtonText",
                   "PrivacyPolicyText"
                 }
             }
         };

        scenesTxtArr = new Dictionary<string, string>[]
        {
            new Dictionary<string, string>
            {
                { "KR", @"왼쪽 카메라를 봐주세요, 잠시 후 사진촬영이 시작됩니다!" },
                { "EN", @"Please look at the camera on the left, the photo session will begin shortly!" },
                { "JP", @"左側のカメラをご覧ください、まもなく写真撮影が始まります！" },
                { "CN", @"请看左边的摄像头，照片拍摄即将开始！" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"인사동지도" },
                { "EN", @"Hanbok Try-On" },
                { "JP", @"韓服 体験" },
                { "CN", @"韩服 体验" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"한복설명" },
                { "EN", @"(Daily: 10:00 AM - 8:00 PM)" },
                { "JP", @"（毎日: 午前10時〜午後8時）" },
                { "CN", @"（每天: 上午10点 - 下午8点）" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"한복 선택하기" },
                { "EN", @"Choose a Hanbok" },
                { "JP", @"韓服を選ぶ" },
                { "CN", @"选择韩服" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"• 원하시는 의상을 고르고 선택 완료 버튼을 눌러주세요  " },
                { "EN", @"• Select hanbok you would like to try on and press the confirm button" },
                { "JP", @"• 試着したい韓服を選んで、確認ボタンを押してください。" },
                { "CN", @"• 选择您想试穿的韩服，然后按确认按钮。" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"한복협찬 :혜온" },
                { "EN", @"Hanbok Sponsorship :혜온" },
                { "JP", @"韓服協賛 :혜온" },
                { "CN", @"韩服赞助 :혜온" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"사진 찍기" },
                { "EN", @"Take a photo" },
                { "JP", @"写真を撮る" },
                { "CN", @"拍照" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"• 사진찍기 버튼을 누르고 좌측 카메라에 얼굴을 바라봐주세요.  
  5초후 촬영이 시작됩니다.  결과물은 왼쪽 스크린과 앞에 스크린에 모두 나옵니다. " },
                { "EN", @"Press the button and face the camera.
A photo will be taken automatically after 5 seconds." },
                { "JP", @"ボタンを押してカメラを向いてください。
5秒後に自動的に写真が撮影されます。" },
                { "CN", @"按下按钮并面对摄像头。
5秒后将自动拍照。" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"'선택완료'후 '사진찍기'를 클릭시 사진촬영 및 의상합성에 따른 개인정보 보호정책에 동의합니다." },
                { "EN", @"By proceeding to take a photo, you consent to our Privacy Policy" },
                { "JP", @"写真を撮ることに進むことで、当社の プライバシーポリシー に同意したことになります。" },
                { "CN", @"通\过继续拍照，即表示您同意我们的 隐私政策。" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"여자한복" },
                { "EN", @"Women’s Hanbok" },
                { "JP", @"女性用韓服" },
                { "CN", @"女式韩服" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"남자한복" },
                { "EN", @"Men’s Hanbok" },
                { "JP", @"男性用韓服" },
                { "CN", @"男式韩服" }
            },
            new Dictionary<string, string>
            {
                { "KR", @"궁중한복" },
                { "EN", @"Royal Hanbok" },
                { "JP", @"宮廷韓服" },
                { "CN", @"宫廷韩服" }
            },
            new Dictionary<string, string>
            {
                //{ "KR", @"EVENT! 'INSA' 체험" },
                //{ "EN", @"EVENT! 'INSA' Experience" },
                //{ "JP", @"イベント！「INSA」体験" },
                //{ "CN", @"活动！'INSA' 体验" }
                { "KR", @"EVENT! 체험" },
                { "EN", @"EVENT! Experience" },
                { "JP", @"イベント！ 体験" },
                { "CN", @"活动！ 体验" }
            },           
           new Dictionary<string, string>
            {
                { "KR", @"선택 완료" },
                { "EN", @"Confirm" },
                { "JP", @"確認" },
                { "CN", @"确认" }
            },
           new Dictionary<string, string>
            {
                { "KR", @"          사진 찍기" },
                { "EN", @"                  Take a photo" },
                { "JP", @"          写真を撮る" },
                { "CN", @"    拍照" }
            },
           new Dictionary<string, string>
            {
                { "KR", @"개인정보 처리방침
1. 개인정보의 수집 항목 및 방법
당사는 서비스 제공을 위해 아래와 같은 개인정보를 수집합니다:
필수 항목: 사진 파일 및 관련 메타데이터
수집 방법: 사용자가 키오스크에서 의상을 선택후 사진촬영을 통해 생성되는 합성 사진 정보 

2. 개인정보의 수집 및 이용 목적
당사는 수집한 개인정보를 다음과 같은 목적을 위해 이용합니다:
서비스 제공: 사용자에게 사진 합성 사진 데이터 제공 
알림 및 마케팅: 이벤트, 프로모션, 광고 등 사용자 맞춤형 정보를 제공하기 위한 활용

3. 개인정보의 보유 및 이용 기간
당사는 사용자의 개인정보를 수집한 목적을 달성할 때까지 보유하며, 이용 목적이 달성된 후에는 즉시 안전하게 파기합니다. 사용자가 탈퇴를 요청하거나 이용을 중단할 경우에도 개인정보는 관련 법령에 따라 일정 기간 보유한 후 파기됩니다.

4. 개인정보의 제3자 제공
당사는 사용자의 개인정보를 원칙적으로 제3자에게 제공하지 않습니다. 다만, 다음과 같은 경우에 한해 개인정보를 제공할 수 있습니다:
법적 의무: 법령에 의한 요구나 법적 절차에 따르는 경우
서비스 제공을 위한 제휴사: 기능 및 프로모션 제공을 위해 필요한 외부 업체와의 협력 시, 최소한의 개인정보를 공유할 수 있습니다.
사용자의 사전 동의: 사용자가 동의한 경우에 한해 개인정보를 제공할 수 있습니다.

5. 개인정보의 안전성 확보 조치
당사는 사용자의 개인정보를 보호하기 위해 다음과 같은 조치를 취하고 있습니다:
데이터 암호화: 개인정보를 암호화하여 저장하고 전송합니다.
접근 제한: 개인정보에 대한 접근을 필요한 직원 및 제휴사로 제한합니다.
보안 업데이트: 정기적인 보안 점검 및 업데이트를 통해 시스템의 취약점을 방지합니다.

6. 개인정보 처리방침의 변경
본 개인정보 처리방침은 법적 요구사항이나 서비스 변경에 따라 수정될 수 있습니다. 변경 사항이 있을 경우, 변경된 사항을 앱 내 또는 웹사이트를 통해 고지합니다.

7. 개인정보 보호 담당자
담당자: 인디라 
연락처: company@witworldwide.com

" },
                { "EN", @"Privacy Policy

Collection of Personal Information: We collect personal information for the purpose of providing our services. The following personal information is collected:
Mandatory Information: Photo files and related metadata.
Collection Method: Information generated through photo composites after the user selects an outfit at the kiosk and takes a photo.
Purposes of Collection and Use of Personal Information: Personal information we collect is used for the following purposes:
Service Provision: To provide users with photo composite data.
Notifications and Marketing: To deliver personalized information regarding events, promotions, advertisements, and other related services.
Retention and Use Period of Personal Information: User's personal information will be retained only for the duration necessary to achieve the purpose of collection. Once the purpose is fulfilled, the information will be securely destroyed immediately. In cases where the user requests account termination or service discontinuation, personal information will be retained for a legally required period and then securely destroyed.

Provision of Personal Information to Third Parties: We do not provide personal information to third parties without the user's consent, except in the following cases:

Legal Obligations: When required by law or through legal processes.
Service Providers and Affiliates: Personal information may be shared with trusted third-party companies as necessary to provide services, functions, and promotions, in which case only the minimum required information will be shared.
User Consent: Personal information will only be provided to third parties with the explicit consent of the user.
Measures to Ensure the Security of Personal Information We implement the following security measures to protect personal information:
Data Encryption: Personal information is encrypted both during storage and transmission.
Access Restrictions: Access to personal information is restricted to authorized personnel and affiliates who require access for the purpose of providing services.
Security Updates: Regular security checks and system updates are performed to address and mitigate any vulnerabilities.
Changes to the Privacy Policy This Privacy Policy may be updated or modified in response to legal requirements or changes in service offerings. Any modifications will be communicated to users via the app or website.

Personal Information Protection Officer
Officer: Indira
Contact: company@witworldwide.com
" },
                { "JP", @"Privacy Policy

Collection of Personal Information: We collect personal information for the purpose of providing our services. The following personal information is collected:
Mandatory Information: Photo files and related metadata.
Collection Method: Information generated through photo composites after the user selects an outfit at the kiosk and takes a photo.
Purposes of Collection and Use of Personal Information: Personal information we collect is used for the following purposes:
Service Provision: To provide users with photo composite data.
Notifications and Marketing: To deliver personalized information regarding events, promotions, advertisements, and other related services.
Retention and Use Period of Personal Information: User's personal information will be retained only for the duration necessary to achieve the purpose of collection. Once the purpose is fulfilled, the information will be securely destroyed immediately. In cases where the user requests account termination or service discontinuation, personal information will be retained for a legally required period and then securely destroyed.

Provision of Personal Information to Third Parties: We do not provide personal information to third parties without the user's consent, except in the following cases:

Legal Obligations: When required by law or through legal processes.
Service Providers and Affiliates: Personal information may be shared with trusted third-party companies as necessary to provide services, functions, and promotions, in which case only the minimum required information will be shared.
User Consent: Personal information will only be provided to third parties with the explicit consent of the user.
Measures to Ensure the Security of Personal Information We implement the following security measures to protect personal information:
Data Encryption: Personal information is encrypted both during storage and transmission.
Access Restrictions: Access to personal information is restricted to authorized personnel and affiliates who require access for the purpose of providing services.
Security Updates: Regular security checks and system updates are performed to address and mitigate any vulnerabilities.
Changes to the Privacy Policy This Privacy Policy may be updated or modified in response to legal requirements or changes in service offerings. Any modifications will be communicated to users via the app or website.

Personal Information Protection Officer
Officer: Indira
Contact: company@witworldwide.com
" },
                { "CN", @"Privacy Policy

Collection of Personal Information: We collect personal information for the purpose of providing our services. The following personal information is collected:
Mandatory Information: Photo files and related metadata.
Collection Method: Information generated through photo composites after the user selects an outfit at the kiosk and takes a photo.
Purposes of Collection and Use of Personal Information: Personal information we collect is used for the following purposes:
Service Provision: To provide users with photo composite data.
Notifications and Marketing: To deliver personalized information regarding events, promotions, advertisements, and other related services.
Retention and Use Period of Personal Information: User's personal information will be retained only for the duration necessary to achieve the purpose of collection. Once the purpose is fulfilled, the information will be securely destroyed immediately. In cases where the user requests account termination or service discontinuation, personal information will be retained for a legally required period and then securely destroyed.

Provision of Personal Information to Third Parties: We do not provide personal information to third parties without the user's consent, except in the following cases:

Legal Obligations: When required by law or through legal processes.
Service Providers and Affiliates: Personal information may be shared with trusted third-party companies as necessary to provide services, functions, and promotions, in which case only the minimum required information will be shared.
User Consent: Personal information will only be provided to third parties with the explicit consent of the user.
Measures to Ensure the Security of Personal Information We implement the following security measures to protect personal information:
Data Encryption: Personal information is encrypted both during storage and transmission.
Access Restrictions: Access to personal information is restricted to authorized personnel and affiliates who require access for the purpose of providing services.
Security Updates: Regular security checks and system updates are performed to address and mitigate any vulnerabilities.
Changes to the Privacy Policy This Privacy Policy may be updated or modified in response to legal requirements or changes in service offerings. Any modifications will be communicated to users via the app or website.

Personal Information Protection Officer
Officer: Indira
Contact: company@witworldwide.com
" }
            },

        };
    }

    // AR result 번역
    public void aRResultSceneInit()
    {
        scenes = new Dictionary<string, string[]>
        {
             {"aRResult" , new string[]
                 {
                   "Text01",
                   "Text02",
                   "Text03",
                   "ButtonText1",
                   "ButtonText2",
                 }
             }
        };

        scenesTxtArr = new Dictionary<string, string>[]
        {
           new Dictionary<string, string>
           {
                { "KR", @"한복 가상 착장 체험" },
                { "EN", @"Hanbok Virtual Fitting Experience" },
                { "JP", @"韓服バーチャル着用体験" },
                { "CN", @"韩服虚拟试穿体验" }
           },
           new Dictionary<string, string>
           {
                { "KR", @"사진을 저장하려면 <color=#FE6C50>QR을 찍어주세요!</color>" },
                { "EN", @"To save the photo, <color=#FE6C50>please scan the QR code!</color>" },
                { "JP", @"写真を保存するには、<color=#FE6C50>QRコードをスキャンしてください！</color>" },
                { "CN", @"要保存照片，请<color=#FE6C50>扫描二维码！</color>" }
           },
           new Dictionary<string, string>
           {
                { "KR", @"저장하기" },
                { "EN", @"Save" },
                { "JP", @"保存" },
                { "CN", @"保存" }
           },
             new Dictionary<string, string>
           {
                { "KR", @"저장하기" },
                { "EN", @"Save" },
                { "JP", @"保存" },
                { "CN", @"保存" }
           },
           new Dictionary<string, string>
           {
                { "KR", @"다시찍기" },
                { "EN", @"Retake" },
                { "JP", @"もう一度撮る" },
                { "CN", @"重新拍摄" }
           }
        };
    }

    // 호텔 번역
    public void hotelListSceneInit()
    {

        scenes = new Dictionary<string, string[]>
        {
             {"hotelList" , new string[]
                 {
                   "Text01",
                   "BtnText01",
                   "BtnText02",
                   "BtnText03",
                 }
             }
        };

        scenesTxtArr = new Dictionary<string, string>[]
        {
           new Dictionary<string, string>
           {
                { "KR", @"인사동 숙박안내" },
                { "EN", @"Insadong Accommodation Guide" },
                { "JP", @"仁寺洞宿泊案内" },
                { "CN", @"仁寺洞住宿指南" }
           },
           new Dictionary<string, string>
           {
                { "KR", @"호텔" },
                { "EN", @"Hotel" },
                { "JP", @"ホテル" },
                { "CN", @"酒店" }
           },
           new Dictionary<string, string>
           {
                { "KR", @"호스텔" },
                { "EN", @"Hostel" },
                { "JP", @"ホステル" },
                { "CN", @"旅舍" }
           },
           new Dictionary<string, string>
           {
                { "KR", @"게스트하우스" },
                { "EN", @"Guesthouse" },
                { "JP", @"ゲストハウス" },
                { "CN", @"客栈" }
           },
        };
    }

    // 언어 바꾸는 메서드 (전체 페이지에 적용 예정
    public Dictionary<string, Dictionary<string, string>> languageChange(string sceneName) {
        string currentStaticLanguage = LanguageService.getCurrentLanguage();
        resultDictionary = new Dictionary<string, Dictionary<string, string>>();

        for (int i = 0; i < scenes[sceneName].Length; i++)
        {            
            resultDictionary.Add(scenes[sceneName][i], scenesTxtArr[i]);

            //Debug.LogError($"sceneName][i] : {scenes[sceneName][i]}\n scenesTxtArr[i] : {scenesTxtArr[i]}");
        }

        return resultDictionary;
    }
    
}
