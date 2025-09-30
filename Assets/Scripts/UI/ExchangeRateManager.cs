using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ExchangeRateManager : MonoBehaviour
{
    public static ExchangeRateManager Instance;

    public Dictionary<string, string> RateStringList = new();

    public string[] currencyCodes = new string[]
    {
        "EUR", "USD", "CNH", "JPY(100)", "GBP", "CAD", "HKD", "THB", "SAR"
    };

    public string url = "https://oapi.koreaexim.go.kr/site/program/financial/exchangeJSON";
    public string authkey = "EfwPppev8zgn5XTpgQnLH1cthloyNItF";
    public string today;

    // 마지막으로 성공적으로 데이터를 가져온 날짜 (MMdd)
    public string lastSuccessfulDate = "";

    private Coroutine updateCoroutine;

    public void Init()
    {
        Instance = this;
        today = DateTime.Now.ToString("yyyyMMdd");

        // 키만 미리 넣고 값은 빈 문자열로 초기화
        foreach (var code in currencyCodes)
            RateStringList[code] = "";

        if (updateCoroutine != null)
            StopCoroutine(updateCoroutine);

        updateCoroutine = StartCoroutine(AutoUpdateCoroutine());
    }

    private IEnumerator AutoUpdateCoroutine()
    {
        while (true)
        {
            yield return StartCoroutine(UpdateAllExchangeRatesWithRetry());
            yield return new WaitForSeconds(1800f); // 30분마다 갱신
        }
    }

    private IEnumerator UpdateAllExchangeRatesWithRetry(int maxRetry = 3, float retryDelay = 1f)
    {
        int attempt = 0;
        bool success = false;

        while (attempt < maxRetry && !success)
        {
            yield return StartCoroutine(UpdateAllExchangeRates((isSuccess) => success = isSuccess));
            if (!success)
            {
                attempt++;
                if (attempt < maxRetry)
                    yield return new WaitForSeconds(retryDelay);
            }
        }

        if (!success)
            KioskLogger.Error("[ExchangeRateManager] 환율 전체 요청 최종 실패");
    }

    private IEnumerator UpdateAllExchangeRates(Action<bool> onComplete = null)
    {
        DateTime searchDate = DateTime.Now;
        string json = null;
        ExchangeRate[] rates = null;
        bool found = false;

        for (int tryDay = 0; tryDay < 10; tryDay++) // 최대 10일 전까지 시도
        {
            string searchDateStr = searchDate.ToString("yyyyMMdd");
            string fullUrl = $"{url}?authkey={authkey}&searchdate={searchDateStr}&data=AP01&type=json";
            using (UnityWebRequest request = UnityWebRequest.Get(fullUrl))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    json = request.downloadHandler.text;
                    rates = JsonHelper.FromJson<ExchangeRate>(json);

                    if (rates != null && rates.Length > 0)
                    {
                        found = true;
                        today = searchDateStr;
                        lastSuccessfulDate = searchDate.ToString("MMdd");
                        break;
                    }
                }
            }
            // 데이터가 없으면 하루 전으로 이동
            searchDate = searchDate.AddDays(-1);
        }

        if (found)
        {
            var codeSet = new HashSet<string>(currencyCodes);
            foreach (var code in currencyCodes)
                RateStringList[code] = "";

            foreach (var rate in rates)
            {
                if (codeSet.Contains(rate.cur_unit))
                    RateStringList[rate.cur_unit] = rate.deal_bas_r;
            }
            onComplete?.Invoke(true);
        }
        else
        {
            KioskLogger.Error("[ExchangeRateManager] 최근 10일간 환율 데이터가 없습니다.");
            onComplete?.Invoke(false);
        }
    }

    // 저장된 환율값을 반환
    public string GetRate(string code)
    {
        if (RateStringList.TryGetValue(code, out var rate))
            return rate;
        return "";
    }
}

[System.Serializable]
public class ExchangeRate
{
    public string cur_unit;
    public string cur_nm;
    public string deal_bas_r;
}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        string newJson = "{ \"array\": " + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.array;
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] array;
    }
}

