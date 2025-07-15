using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GogoongController : MonoBehaviour
{

    [SerializeField] private GameObject palaceItem;
    [SerializeField] private Transform parentsContent;

    private string[] palaceName;

    private Sprite[] palaceImgs;
    private string currentLanguage;
    Dictionary<string, Dictionary<string, Dictionary<string, string>>> result;

    void Start()
    {        
        // 현재 언제 
        currentLanguage = LanguageService.getCurrentLanguage();

        // 고궁 배열값 초기화
        palaceName = new string[] { "경복궁", "덕수궁", "창경궁", "창덕궁" };

        // 이미지 불러와서 배열값 초기화
        palaceImgs = new Sprite[] {
            Resources.Load<Sprite>("Image/Gogoong/Gyeongbokgung/Gyeongbokgung"),
            Resources.Load<Sprite>("Image/Gogoong/Deoksugung/Deoksugung"),
            Resources.Load<Sprite>("Image/Gogoong/Changgyeonggung/Changgyeonggung"),
            Resources.Load<Sprite>("Image/Gogoong/Changdeokgung/Changdeokgung")
        };

        result = palaceItemContentInit();

        for (int i = 0; i < palaceName.Length; i++)
        {
            GameObject palaceItemCopy = Instantiate(palaceItem, parentsContent);
            Image[] palaceImg = palaceItemCopy.GetComponentsInChildren<Image>();
            Button palaceItemBtn = palaceItemCopy.GetComponentInChildren<Button>();
            
            palaceItemBtn.onClick.AddListener(onSelectDetail);

            foreach (Image image in palaceImg)
            {
                if (image != null)
                {   // 궁궐 이미지 삽입
                    if (image.name == "PalaceImg") image.sprite = palaceImgs[i];
                }
            }

            // 프리펩 안에 있는 모든 Text 컴포넌트
            TextMeshProUGUI[] textComponents = palaceItemCopy.GetComponentsInChildren<TextMeshProUGUI>();
            Dictionary<string, string> res = result[palaceName[i]][currentLanguage];            
            foreach (TextMeshProUGUI text in textComponents) {            
                text.text = res[text.name] == null ? "고궁" : res[text.name];
            }
        }
    }

    public void onSelectDetail()
    {
        // EventSystem을 통해 클릭된 GameObject를 가져옵니다.
        GameObject clickedObject = EventSystem.current.currentSelectedGameObject;

        // 지정한 프리펩 안에 있는 자식 TextMeshProUGUI 객체들
        TextMeshProUGUI[] listTextItems = clickedObject.GetComponentsInChildren<TextMeshProUGUI>();
        string selectPalace;
        // 이름 확인해서 각 텍스트 영역에 DB 데이터 표현 처리
        foreach (var txtItem in listTextItems)
        {
            if (txtItem.name == "PalaceTitle") {                
                PlayerPrefs.SetString("PalaceTitle", txtItem.text);
                break;
            }
        }

        SceneManager.LoadScene("GogoongDetail");
    }

    private string getSelectPalace(string palaceTitle) {        
        return new Dictionary<string, string>
        {
            { "Gyeongbokgung", "경복궁" },
            { "경복궁", "경복궁" },
            { "景福宮", "경복궁" },
            { "けいふくきゅう", "경복궁" },
            { "景福宫", "경복궁" },
            { "Jǐngfúgōng", "경복궁" },

            { "Deoksugung", "덕수궁" },
            { "덕수궁", "덕수궁" },
            { "徳寿宮", "덕수궁" },
            { "とくじゅきゅう", "덕수궁" },
            { "德寿宫", "덕수궁" },
            { "Déshòugōng", "덕수궁" },

            { "Changgyeonggung", "창경궁" },
            { "창경궁", "창경궁" },
            { "昌慶宮", "창경궁" },
            { "しょうけいきゅう", "창경궁" },
            { "昌庆宫", "창경궁" },
            { "Chāngqìnggōng", "창경궁" },

            { "Changdeokgung", "창덕궁" },
            { "창덕궁", "창덕궁" },
            { "昌徳宮", "창덕궁" },
            { "しょうとくきゅう", "창덕궁" },
            { "昌德宫", "창덕궁" },
            { "Chāngdé gōng", "창덕궁" }
        }[palaceTitle];
    }

    // 고궁 정보 언어별 가져오기 
    // 언어 서비스에서 뺸 이유는 복잡하고 공통 메서드로 사용이 어려워서 리턴 타입이 다름
    private Dictionary<string, Dictionary<string, Dictionary<string, string>>> palaceItemContentInit()
    {
        return new Dictionary<string, Dictionary<string, Dictionary<string, string>>>()
        {
            { "경복궁", new Dictionary<string, Dictionary<string, string>>()
                {
                    { "KR", new Dictionary<string, string>()
                        {
                            { "PalaceTitle", "경복궁" },
                            { "PalaceAddress", "서울특별시 종로구 사직로 161" },
                            { "PalaceDt", "2024.09.26 ~ 10.20" },
                            { "PalaceTel", "02-3700-3900" },
                            { "PalaceTag", "조선 왕조의 첫 번째 궁궐" },
                            { "PalaceCategory", "고궁" }
                        }
                    },
                    { "EN", new Dictionary<string, string>()
                        {
                            { "PalaceTitle", "Gyeongbokgung" },
                            { "PalaceAddress", "161 Sajik-ro, Jongno-gu, Seoul" },
                            { "PalaceDt", "2024.09.26 ~ 10.20" },
                            { "PalaceTel", "+82-2-3700-3900" },
                            { "PalaceTag", "The first palace of the Joseon Dynasty" },
                            { "PalaceCategory", "Ancient Palace" }
                        }
                    },
                    { "JP", new Dictionary<string, string>()
                        {
                            { "PalaceTitle", "景福宮" },
                            { "PalaceAddress", "ソウル特別市鍾路区社稷路161" },
                            { "PalaceDt", "2024.09.26 ~ 10.20" },
                            { "PalaceTel", "02-3700-3900" },
                            { "PalaceTag", "朝鮮王朝の最初の宮殿" },
                            { "PalaceCategory", "古宮" }
                        }
                    },
                    { "CN", new Dictionary<string, string>()
                        {
                            { "PalaceTitle", "景福宫" },
                            { "PalaceAddress", "首尔特别市钟路区社稷路161号" },
                            { "PalaceDt", "2024.09.26 ~ 10.20" },
                            { "PalaceTel", "02-3700-3900" },
                            { "PalaceTag", "朝鲜王朝的第一座宫殿" },
                            { "PalaceCategory", "古宫" }
                        }
                    }
                }
            },
            { "덕수궁", new Dictionary<string, Dictionary<string, string>>()
                {
                    { "KR", new Dictionary<string, string>()
                        {
                            { "PalaceTitle", "덕수궁" },
                            { "PalaceAddress", "서울특별시 중구 세종대로 99" },
                            { "PalaceDt", "2024.09.26 ~ 10.20" },
                            { "PalaceTel", "02-771-9951" },
                            { "PalaceTag", "조선 말기 왕궁" },
                            { "PalaceCategory", "고궁" }
                        }
                    },
                    { "EN", new Dictionary<string, string>()
                        {
                            { "PalaceTitle", "Deoksugung" },
                            { "PalaceAddress", "99 Sejong-daero, Jung-gu, Seoul" },
                            { "PalaceDt", "2024.09.26 ~ 10.20" },
                            { "PalaceTel", "+82-2-771-9951" },
                            { "PalaceTag", "A royal palace from the late Joseon period" },
                            { "PalaceCategory", "Ancient Palace" }
                        }
                    },
                    { "JP", new Dictionary<string, string>()
                        {
                            { "PalaceTitle", "徳寿宮" },
                            { "PalaceAddress", "ソウル特別市中区世宗大路99" },
                            { "PalaceDt", "2024.09.26 ~ 10.20" },
                            { "PalaceTel", "02-771-9951" },
                            { "PalaceTag", "朝鮮時代後期の王宮" },
                            { "PalaceCategory", "古宮" }
                        }
                    },
                    { "CN", new Dictionary<string, string>()
                        {
                            { "PalaceTitle", "德寿宫" },
                            { "PalaceAddress", "首尔特别市中区世宗大路99号" },
                            { "PalaceDt", "2024.09.26 ~ 10.20" },
                            { "PalaceTel", "02-771-9951" },
                            { "PalaceTag", "朝鲜末期的王宫" },
                            { "PalaceCategory", "古宫" }
                        }
                    }
                }
            },
            { "창경궁", new Dictionary<string, Dictionary<string, string>>()
                {
                    { "KR", new Dictionary<string, string>()
                        {
                            { "PalaceTitle", "창경궁" },
                            { "PalaceAddress", "서울특별시 종로구 창경궁로 185" },
                            { "PalaceDt", "2024.09.26 ~ 10.20" },
                            { "PalaceTel", "02-762-4868" },
                            { "PalaceTag", "세종대왕이 건설한 궁궐" },
                            { "PalaceCategory", "고궁" }
                        }
                    },
                    { "EN", new Dictionary<string, string>()
                        {
                            { "PalaceTitle", "Changgyeonggung" },
                            { "PalaceAddress", "185 Changgyeonggung-ro, Jongno-gu, Seoul" },
                            { "PalaceDt", @"2024.09.26 ~ 10.20" },
                            { "PalaceTel", "+82-2-762-4868" },
                            { "PalaceTag", "A palace built by King Sejong" },
                            { "PalaceCategory", "Ancient Palace" }
                        }
                    },
                    { "JP", new Dictionary<string, string>()
                        {
                            { "PalaceTitle", "昌慶宮" },
                            { "PalaceAddress", "ソウル特別市鍾路区昌慶宮路185" },
                            { "PalaceDt", "2024.09.26 ~ 10.20" },
                            { "PalaceTel", "02-762-4868" },
                            { "PalaceTag", "世宗大王が建てた宮殿" },
                            { "PalaceCategory", "古宮" }
                        }
                    },
                    { "CN", new Dictionary<string, string>()
                        {
                            { "PalaceTitle", "昌庆宫" },
                            { "PalaceAddress", "首尔特别市钟路区昌庆宫路185号" },
                            { "PalaceDt", "2024.09.26 ~ 10.20" },
                            { "PalaceTel", "02-762-4868" },
                            { "PalaceTag", "世宗大王建造的宫殿" },
                            { "PalaceCategory", "古宫" }
                        }
                    }
                }
            },
            { "창덕궁", new Dictionary<string, Dictionary<string, string>>()
                {
                    { "KR", new Dictionary<string, string>()
                        {
                            { "PalaceTitle", "창덕궁" },
                            { "PalaceAddress", "서울특별시 종로구 율곡로 99" },
                            { "PalaceDt", "2024.09.26 ~ 10.20" },
                            { "PalaceTel", "02-762-8261" },
                            { "PalaceTag", "자연과 조화를 이루는 궁궐" },
                            { "PalaceCategory", "고궁" }
                        }
                    },
                    { "EN", new Dictionary<string, string>()
                        {
                            { "PalaceTitle", "Changdeokgung" },
                            { "PalaceAddress", "99 Yulgok-ro, Jongno-gu, Seoul" },
                            { "PalaceDt", "2024.09.26 ~ 10.20" },
                            { "PalaceTel", "+82-2-762-8261" },
                            { "PalaceTag", "A palace in harmony with nature" },
                            { "PalaceCategory", "Ancient Palace" }
                        }
                    },
                    { "JP", new Dictionary<string, string>()
                        {
                            { "PalaceTitle", "昌徳宮" },
                            { "PalaceAddress", "ソウル特別市鍾路区栗谷路99" },
                            { "PalaceDt", "2024.09.26 ~ 10.20" },
                            { "PalaceTel", "02-762-8261" },
                            { "PalaceTag", "自然と調和した宮殿" },
                            { "PalaceCategory", "古宮" }
                        }
                    },
                    { "CN", new Dictionary<string, string>()
                        {
                            { "PalaceTitle", "昌德宫" },
                            { "PalaceAddress", "首尔特别市钟路区栗谷路99号" },
                            { "PalaceDt", "2024.09.26 ~ 10.20" },
                            { "PalaceTel", "02-762-8261" },
                            { "PalaceTag", "与自然和谐的宫殿" },
                            { "PalaceCategory", "古宫" }
                        }
                    }
                }
            }
        };
    }

}
