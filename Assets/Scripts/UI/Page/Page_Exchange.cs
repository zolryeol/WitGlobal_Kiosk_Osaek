using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Page_Exchange : MonoBehaviour
{
    [SerializeField] Transform contentParent;
    [SerializeField] List<TextMeshProUGUI> ExchangeMoneyTextList = new();

    private void Awake()
    {
        contentParent = transform.Find("Body").transform.Find("Contents").transform;

        for (int i = 0; i < contentParent.childCount; ++i)
        {
            ExchangeMoneyTextList.Add(contentParent.GetChild(i).Find("ExchangeMoney").GetComponent<TextMeshProUGUI>());
        }
    }

    public void FetchExchangeRate()
    {
        for (int i = 0; i < ExchangeMoneyTextList.Count; i++)
        {
            string code = ExchangeRateManager.Instance.currencyCodes[i];
            if (ExchangeRateManager.Instance.RateStringList.TryGetValue(code, out string rate))
            {
                ExchangeMoneyTextList[i].text = $"{rate} KRW";
                //ExchangeMoneyTextList[i].text = $"{code}: {rate} KRW";
            }
            else
            {
                Debug.LogError("불러오기 실패");
                ExchangeMoneyTextList[i].text = $"{code}: 불러오기 실패";
            }
        }
    }

}
