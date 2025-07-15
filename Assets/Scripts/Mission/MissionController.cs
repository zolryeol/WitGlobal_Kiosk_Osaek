using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionController : MonoBehaviour
{
    [SerializeField] private GameObject listItem;
    [SerializeField] private int page;
    [SerializeField] private int size;
    [SerializeField] private string type;
    [SerializeField] private Transform content;


    private WITAPI witApi;

    private string currentLanguage;
    private LanguageService ls;
    // Start is called before the first frame update
    void Start()
    { 
        currentLanguage = LanguageService.getCurrentLanguage();
        ls = new LanguageService();
        ls.missionSceneInit();
        Dictionary<string, Dictionary<string, string>> res = ls.languageChange("Mission");

        string[] keysArray = res.Keys.ToArray();
        foreach (string key in keysArray)
        {
            
            // key와 동일한 이름의 GameObject를 찾음
            GameObject targetObject = GameObject.Find(key);
            
            if (targetObject != null)
            {
                // GameObject에서 TextMeshProUGUI 컴포넌트를 찾음
                TextMeshProUGUI textComponent = targetObject.GetComponentInChildren<TextMeshProUGUI>();

                if (textComponent != null)
                {
                    // res[key]에서 언어에 맞는 텍스트를 가져와서 TextMeshProUGUI에 설정
                    // 예를 들어 "KR"에 해당하는 텍스트를 설정 (사용할 언어에 맞게 수정 가능)
                    textComponent.text = res[key][currentLanguage];  // 필요에 따라 "KR" 대신 "EN" 등 사용   
                }
                else
                {
                    Debug.LogWarning($"TextMeshProUGUI component not found in {key}");
                }
            }
            else
            {
                Debug.LogWarning($"GameObject with name {key} not found");
            }
        }        
        
        getMsgList();


    }

    async void getMsgList() {
        witApi = new WITAPI();
        page = UnityEngine.Random.Range(1, 6);  // 1부터 5까지의 숫자를 반환

        List<ResponseData> data = await witApi.getMsnList(page, size);
        int i = 1;
        foreach (var vo in data)
        {
            GameObject obj = Instantiate(listItem, content);
            TextMeshProUGUI[] textComponants = obj.GetComponentsInChildren<TextMeshProUGUI>();
            RawImage qrCode = obj.GetComponentInChildren<RawImage>(); 

            foreach (TextMeshProUGUI text in textComponants) {
                switch (text.name)
                {
                    case "MissionTitle":
                        text.text = "미션 " + i;
                        break;
                    case "MissionContent":
                        text.text = vo.PST_CN;
                        break;
                    case "MissionTime":
                        // 문자열을 DateTime으로 변환
                        DateTime startDate = DateTime.Parse(vo.MSN_TB.MSN_BGNG_DT);
                        DateTime endDate = DateTime.Parse(vo.MSN_TB.MSN_END_DT);
                        text.text = $"{startDate.ToString("tt hh:mm", new System.Globalization.CultureInfo("ko-KR"))} ~ {endDate.ToString("tt hh:mm", new System.Globalization.CultureInfo("ko-KR"))}"; 
                        break;
                    case "PK":
                        text.text = vo.PK+"";
                        break;
                }                
            }
            i++;

            //QRCreator.CreateQR("https://wit.page.link/29hQ?pstPk="+vo.PK, qrCode);
            //QRCreator.CreateQR($"https://wit.page.link/?link=https://wit.page.link/Rdse?pstPk={vo.PK}&apn=com.witdiocianapp", qrCode);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
