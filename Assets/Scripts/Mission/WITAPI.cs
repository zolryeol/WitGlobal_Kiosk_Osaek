using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

public class WITAPI
{

    private static readonly HttpClient client = new HttpClient();

    public class TokenResponse
    {
        public Data data { get; set; }
    }

    public class Data
    {
        public string token { get; set; }
    }

    private string getJWT() {
        string url = "http://wit.inno-t.shop/api/auth/login";
        var request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "POST";
        request.ContentType = "application/json";  // JSON 데이터 전송 시 ContentType 설정
        request.Accept = "application/json";       // 응답을 JSON으로 받을 경우

        // 요청 본문에 보낼 데이터를 JSON 형식으로 준비
        var requestBody = new
        {
            EML = "changyeongim1@gmail.com",
            PW = "1"
        };
        string json = JsonConvert.SerializeObject(requestBody);

        // Body에 JSON 데이터를 작성
        using (var streamWriter = new StreamWriter(request.GetRequestStream()))
        {
            streamWriter.Write(json);
            streamWriter.Flush();
            streamWriter.Close();
        }

        string results = string.Empty;
        HttpWebResponse response;

        try
        {
            using (response = request.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                results = reader.ReadToEnd();

                // JSON 응답 데이터를 처리 (역직렬화하여 Token 추출)
                TokenResponse tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(results);                
                return tokenResponse.data.token;
            }
        }
        catch (WebException ex)
        {
            // 오류 처리
            using (var errorResponse = (HttpWebResponse)ex.Response)
            {
                using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                {
                    string error = reader.ReadToEnd();
                    Debug.LogError("Error: " + error);
                    return null;
                }
            }
        }
    }

    // 이거 내일 해야함 꼭 

    public async Task<List<ResponseData>> getMsnList(int page, int size)
    {
        string url = "http://wit.inno-t.shop/api/pst/getMsnList?page=" + page + "&size=" + size + "&type=ALL";

        // Authorization 헤더 추가 (Bearer 토큰 예시)
        string token = getJWT();                
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        try
        {
            // 비동기 요청 보내기
            var response = await client.GetAsync(url);
            //response.EnsureSuccessStatusCode(); // 예외 발생시키기

            // 응답 스트림 읽기
            var results = await response.Content.ReadAsStringAsync();      
            // JSON 데이터를 역직렬화하여 C# 객체로 변환
            WITMissionVO mission = JsonConvert.DeserializeObject<WITMissionVO>(results);            
            return mission.Data;
        }
        catch (Exception ex)
        {
            Debug.Log(ex.StackTrace);
            return null;
        }
    }

}

