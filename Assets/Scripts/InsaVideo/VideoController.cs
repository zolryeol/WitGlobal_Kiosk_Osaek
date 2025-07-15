using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{


    /*
        1. 매개변수 
          - 씬
          - 엡쓰
          - 옵션 (바로 다음 뭔가 나와야한다) 
         
         
         사전형의 전체 타입은 씬 - 뎁쓰 - 언어 까지해서 나오는 객체는 해당 화면에서 지금 나와야하는 비디오와 자막이 나올 예정
         사전형 객체 키값 - 씬 -> 뎁쓰 로 찾음
         자막은 언어로 찾아옴
         
         
     */


    static Dictionary<string, Dictionary<string, List<VideoClip>>> clips;
    static Dictionary<string, Dictionary<string, List<string>>> subtitles;
    static string[] lang;
    int loopIndex;
    // 씬 별로 subCanvas 안에 있는 비디오 플레이어
    VideoPlayer VideoPlayer;
    TextMeshProUGUI subTitleText;

    [SerializeField] private string scene;
    [SerializeField] private string elDepth;
    int noTouchIndex = 0;
    static string curDepth = "";

    private bool isVideoPrepared = false; // 비디오 준비 상태 확인
    private float videoSwitchDelay = 0.5f; // 클립 변경 시 딜레이 (0.5초)

    void Start()
    {

        VideoPlayer = GameObject.Find("Insa").GetComponentInChildren<VideoPlayer>();
        subTitleText = GameObject.Find("subTitle").GetComponentInChildren<TextMeshProUGUI>();

        // 비디오 준비 완료 시 호출될 이벤트 리스너 추가
        VideoPlayer.prepareCompleted += OnVideoPrepared;


        loopIndex = 1;
        clipsInit();

        if (scene == "language") OnChangeVideo(elDepth == "" ? "1" : elDepth, scene, LanguageService.getCurrentLanguage());
        else if (scene == "weather") { }
        else OnChangeVideo(elDepth == "" ? "1" : elDepth, scene);
        lang = new string[] { "KR", "EN", "JP", "CN" };

    }


    void Update()
    {
        if (scene == "noTouch" && !VideoPlayer.isPlaying && isVideoPrepared)
        {
            loopNoTouchVideo();
        }       
        else if (scene != "noTouch" && !VideoPlayer.isPlaying && isVideoPrepared)
        {
            VideoPlayer.Play();
            //StartSubtitlesFromBeginning();
        }
    }

    //private void StartSubtitlesFromBeginning()
    //{
    //    isVideoPrepared = false;
    //    // 자막을 처음부터 보여주기 위해 초기화
    //    string[] splitSubtitles = subtitles[scene][curDepth][0].Split('@'); // 현재 장면과 깊이에 맞는 자막 분할
    //    float clipDuration = (float)VideoPlayer.clip.length; // 비디오 길이
    //    float subtitleDuration = clipDuration / splitSubtitles.Length; // 자막 표시 시간 계산

    //    // 자막 출력 코루틴 시작
    //    StartCoroutine(ShowSubtitles(splitSubtitles, subtitleDuration));
    //}

    private void loopNoTouchVideo()
    {
        int index = Array.IndexOf(lang, LanguageService.getCurrentLanguage());

        // 비디오 클립 변경
        loopIndex = (loopIndex + 1) % 8;
        VideoPlayer.clip = clips[scene][loopIndex + ""][0];
        VideoPlayer.Prepare(); // 비디오를 미리 준비
        isVideoPrepared = false; // 준비 상태를 false로 초기화

        // 자막 처리: 자막에 @가 포함되어 있다면 분할
        string[] subTitleArray = subtitles[scene][loopIndex + ""][0].Split('@');

        // 자막 배열 길이에 맞게 비디오 길이를 나누어 각 자막이 얼마 동안 보여야 하는지 계산
        float clipDuration = (float)VideoPlayer.clip.length;
        float subtitleDuration = clipDuration / subTitleArray.Length;

        // 코루틴을 이용해 자막을 순서대로 보여줌
        StartCoroutine(ShowSubtitles(subTitleArray, subtitleDuration));
    }

    // 비디오 준비가 완료되면 호출되는 이벤트 핸들러
    private void OnVideoPrepared(VideoPlayer source)
    {
        isVideoPrepared = true; // 비디오가 준비되었음을 표시
        VideoPlayer.Play(); // 비디오 재생
    }

    // 자막을 순서대로 보여주는 코루틴
    private IEnumerator ShowSubtitles(string[] subTitleArray, float subtitleDuration)
    {
        for (int i = 0; i < subTitleArray.Length; i++)
        {
            Debug.Log($"subTitleArray : {subTitleArray[i]}");
            subTitleText.text = subTitleArray[i];  // 현재 자막 표시

            // subtitleDuration 동안 대기 후 다음 자막 표시
            yield return new WaitForSeconds(subtitleDuration);
        }
    }

    public void OnChangeVideo(string depth, string scene, string e = null)
    {
        curDepth = depth;
        lang = new string[] { "KR", "EN", "JP", "CN" };
        VideoPlayer = GameObject.Find("Insa").GetComponentInChildren<VideoPlayer>();
        subTitleText = GameObject.Find("subTitle").GetComponentInChildren<TextMeshProUGUI>();

        // 비디오 플레이어 클립 설정
        string subtitleContent = "";
        if (scene == "language" && e != null)
        {
            Debug.Log(e);
            VideoPlayer.clip = clips[scene][depth][Array.IndexOf(lang, e)];
            // 언어 메뉴에서 실시간으로 수정된 언어를 사용할 경우
            subtitleContent = getSubTitleArr(scene, int.Parse(depth), Array.IndexOf(lang, LanguageService.getCurrentLanguage()) + 1);
        }      
        else {
            VideoPlayer.clip = clips[scene][depth][0];
            subtitleContent = subtitles[scene][depth][0];
        }

        // @를 기준으로 자막을 분할
        string[] splitSubtitles = subtitleContent.Split('@');

        if (splitSubtitles.Length > 1)
        {
            float clipDuration = (float)VideoPlayer.clip.length;
            float subtitleDuration = clipDuration / splitSubtitles.Length;
            // 자막 출력 코루틴 시작
            StartCoroutine(ShowSubtitles(splitSubtitles, subtitleDuration));
        }
        else
        {
            subTitleText.text = subtitleContent;
            // UI 강제 업데이트
            Canvas.ForceUpdateCanvases();
        }

        // 배경 이미지 설정
        Image backgroundImage = GameObject.Find("subTitleBackground").GetComponentInChildren<Image>();
        Color color = backgroundImage.color;
        color.a = 0.5f;
        backgroundImage.color = color;
    }


    // 모든 비디오 클립 초기화 
    // @ 로 나누자 
    private void clipsInit()
    {
        int depth = 1;
        int sceneCnt = 0;
        int i = 1;
        string[] scenes = getVideoScenes();
        Dictionary<string, int> maxDepth = getMaxDepth();

        clips = new Dictionary<string, Dictionary<string, List<VideoClip>>>();
        subtitles = new Dictionary<string, Dictionary<string, List<string>>>();
        Dictionary<string, List<VideoClip>> clipsDic;
        Dictionary<string, List<string>> subTitleDic;
        lang = new string[] { "KR", "EN", "JP", "CN" };
        VideoClip clip;
        int langIndex = 0;

        foreach (string scene in scenes)
        {
            clipsDic = new Dictionary<string, List<VideoClip>>();
            subTitleDic = new Dictionary<string, List<string>>();
            depth = 1;
            i = 1;
            bool sw = true;

            //Debug.Log(scene + " 설정 완료");
            while (sw)
            {
                // 각 씬과 depth에 맞는 비디오를 불러옴
                string videoPath = (scene != "language") ?
                                    $"Video/{scene}/depth{depth}/{i}" :
                                    $"Video/{scene}/depth{depth}/{lang[langIndex]}";

                clip = Resources.Load<VideoClip>(videoPath);

                if (scene == "noTouch") Debug.Log("path : " + videoPath + " // " + clip);

                if (clip != null)
                {
                    // 비디오 클립이 있으면 해당 depth에 추가
                    if (!clipsDic.ContainsKey(depth.ToString()))
                    {
                        clipsDic[depth.ToString()] = new List<VideoClip>();
                        subTitleDic[depth.ToString()] = new List<string>();
                    }

                    // 리스트에 비디오 클립과 자막을 추가
                    clipsDic[depth.ToString()].Add(clip);
                    subTitleDic[depth.ToString()].Add(getSubTitleArr(scene, depth, Array.IndexOf(lang, LanguageService.getCurrentLanguage()) + 1));

                    if (scene == "language")
                        Debug.Log(Array.IndexOf(lang, LanguageService.getCurrentLanguage()) + 1);

                    i++; // 다음 비디오를 찾기 위해 증가

                    if (scene == "language")
                    {

                        if (depth == 1)
                        {
                            if (langIndex == 3)
                            {
                                depth++;
                                langIndex = 0;
                                continue;
                            }
                            langIndex++;
                        }
                        else
                        {
                            if (langIndex == 3) break;
                            else langIndex++;
                        }


                    }
                }
                else
                {
                    // 언어 씬 2뎁스 안들어가고있음 이어서해야함@@@

                    // 최대 depth에 도달하면 루프 종료
                    if (depth == maxDepth[scene])
                    {
                        sw = false;
                        sceneCnt++;
                    }
                    else
                    {
                        // 다음 depth로 이동
                        depth++;
                        i = 1; // i 초기화
                    }
                }
            }


            // 씬에 대한 클립과 자막을 추가
            clips.Add(scene, clipsDic);
            subtitles.Add(scene, subTitleDic);
        }
    }




    private Dictionary<string, int> getMaxDepth()
    {
        string[] scenes = getVideoScenes();
        int[] dep = new int[] {
            3, 1, 3, 4, 1, 1, 5, 2, 4, 5, 3, 8, 2, 3, 3, 2, 3, 3, 4
        };

        Dictionary<string, int> result = new Dictionary<string, int>();
        int i = 0;
        foreach (string scene in scenes) {
            result.Add(scene, dep[i]);            
            i++;
        }        
        return result;
    }

    //case 1:
    //                    switch (i) // lang 배열의인덱스 순서 
    //                    {
    //                        case 1: // KO
    //                            result = @"
    //                                      ";
    //                            break;
    //                        case 2: // EN
    //                            result = @"";
    //                            break;
    //                        case 3: // JP
    //                            result = @"";
    //                            break;
    //                        case 4: // CN
    //                            result = @"";
    //                            break;
    //                    }


    private string getSubTitleArr(string scene, int depth, int i)
    {
        string result = string.Empty;
        switch (scene)
        {
            case "event":
                // event 씬 처리 로직

                switch (depth)
                {
                    case 1: // 맑음
                        switch (i) // lang 배열의인덱스 순서 
                        {
                            case 1: // KO
                                result = @"인사동에서는 매주 다양한 전통문화행사와 이벤트가 열려요.
우리 함께 인사동 이벤트를 즐기러 가볼까요?";
                                break;
                            case 2: // EN
                                result = @"Events are held in Insadong every week. Dive deeper into Korean culture through unique cultural events and exhibitions.";
                                break;
                            case 3: // JP
                                result = @"毎週インサドンでは多様なイベントが開催されます。多様な伝統文化イベントや展示会を通じて韓国をより深く体験してみましょう。";
                                break;
                            case 4: // CN
                                result = @"仁寺洞每周都会举办各种活动。
通过丰富的传统文化活动和展览，深入体验韩国文化吧！";
                                break;
                        }
                        break;
                    case 2: // 맑음
                        switch (i) // lang 배열의인덱스 순서 
                        {
                            case 1: // KO
                                result = @"인사동에는 매월 다양한 이벤트와 프로모션이 진행되고 있어요.
여러분도 함께 참여해보세요.";
                                break;
                            case 2: // EN
                                result = @"Insadong hosts various events and promotions every day. 
Join in and be a part of the fun!";
                                break;
                            case 3: // JP
                                result = @"インサドンでは毎日多様なイベントやプロモーションが行われています。皆さんもぜひ参加してみてください。";
                                break;
                            case 4: // CN
                                result = @"仁寺洞每天都有各种活动和促销在进行，欢迎大家一起来参与！";
                                break;
                        }
                        break;
                    case 3: // 맑음
                        switch (i) // lang 배열의인덱스 순서 
                        {
                            case 1: // KO
                                result = @"핸드폰으로 [QR] 코드를 찍으시면 길을 안내 받으실수 있어요.
                                          ";
                                break;
                            case 2: // EN
                                result = @"Scan the [QR] code with your phone to receive detailed directions.";
                                break;
                            case 3: // JP
                                result = @"携帯電話で[QR] コードを撮影すると道案内が受けられます。";
                                break;
                            case 4: // CN
                                result = @"用手机扫描[QR]码，您可以获得路线指引。";
                                break;
                        }
                        break;
                }

                break;
            case "exchange":
                // exchange 씬 처리 로직

                // info 씬 처리 로직
                switch (depth)
                {
                    case 1: // 맑음
                        switch (i) // lang 배열의인덱스 순서 
                        {
                            case 1: // KO
                                result = @"오늘의 환율을 알려 드릴께요. 
환전을 원하신다면 주변의 은행 검색을 통해서 안내해드릴께요.";
                                break;
                            case 2: // EN
                                result = @"Here is today's exchange rate. You can exchange currency 
at nearby banks. Use the search function to find nearby banks.";
                                break;
                            case 3: // JP
                                result = @"今日の為替レート情報です。近くの銀行で両替が可能です。";
                                break;
                            case 4: // CN
                                result = @"这是今天的汇率信息。您可以在附近的银行进行兑换。
请通过搜索功能查找周围的银行。";
                                break;
                        }
                        break;
                }
                break;
            case "food":
                // food 씬 처리 로직
                switch (depth)
                {
                    case 1: // 맑음
                        switch (i) // lang 배열의인덱스 순서 
                        {
                            case 1: // KO
                                result = @"인사동엔 유명한 맛집과 찻집이 정말 많이 있어요. 
방문하고 싶은 카테고리를 선택해주세요.";
                                break;
                            case 2: // EN
                                result = @"Insadong is home to many renowned restaurants and tea houses. Please choose the category you’d like to explore.";
                                break;
                            case 3: // JP
                                result = @"インサドンには有名なレストランや茶店が本当にたくさんありす。
訪れたいカテゴリーを選んでください。";
                                break;
                            case 4: // CN
                                result = @"仁寺洞有很多有名的美食店和茶馆。请选择您想去的类别。";
                                break;
                        }
                        break;
                    case 2: // 맑음
                        switch (i) // lang 배열의인덱스 순서 
                        {
                            case 1: // KO
                                result = @"매장을 클릭하시면 더욱더 상세한 정보를 얻을 수 있어요.";
                                break;
                            case 2: // EN
                                result = @"Tap for more details and directions";
                                break;
                            case 3: // JP
                                result = @"店舗をクリックすると、さらに詳しい情報が得られます。";
                                break;
                            case 4: // CN
                                result = @"点击店铺后，您可以获取更详细的信息。";
                                break;
                        }
                        break;
                    case 3: // 맑음
                        switch (i) // lang 배열의인덱스 순서 
                        {
                            case 1: // KO
                                result = @"핸드폰으로 [QR] 코드를 찍으시면 길을 안내 받으실수 있어요.";
                                break;
                            case 2: // EN
                                result = @"Scan the [QR] code with your phone to receive detailed directions";
                                break;
                            case 3: // JP
                                result = @"携帯電話で[QR] コードを撮影すると道案内が受けられます。";
                                break;
                            case 4: // CN
                                result = @"用手机扫描[QR]码，您可以获得路线指引。";
                                break;
                        }
                        break;
                }

                break;
            case "help":
                // help 씬 처리 로직                
                switch (depth)
                {
                    case 1: // 맑음
                        switch (i) // lang 배열의인덱스 순서 
                        {
                            case 1: // KO
                                result = @"도움이 필요하세요? 무엇을 도와 드릴까요?";
                                break;
                            case 2: // EN
                                result = @"Looking for help or essential services? 
Let me know how can I assist you?";
                                break;
                            case 3: // JP
                                result = @"こちらは本日の為替情報です。近くの銀行で両替が可能です。
検索機能を使って周辺の銀行を探してみてください。";
                                break;
                            case 4: // CN
                                result = @"需要帮助吗？请告诉我您需要什么帮助。";
                                break;
                        }
                        break;
                    case 2: // 맑음
                        switch (i) // lang 배열의인덱스 순서 
                        {
                            case 1: // KO
                                result = @"필요한신 내용을 클릭하시면 더욱 상세한 정보를 얻으실 수 있어요. ";
                                break;
                            case 2: // EN
                                result = @"Click on the information you need to get more detailed details.";
                                break;
                            case 3: // JP
                                result = @"必要な情報をクリックすると、
さらに詳しい情報を得ることができます。";
                                break;
                            case 4: // CN
                                result = @"点击所需的内容，您可以获取更详细的信息。";
                                break;
                        }
                        break;
                    case 3: // 맑음
                        switch (i) // lang 배열의인덱스 순서 
                        {
                            case 1: // KO
                                result = @"핸드폰으로 [QR] 코드를 찍으시면 길을 안내 받으실수 있어요.";
                                break;
                            case 2: // EN
                                result = @"Scan the [QR] code with your phone to receive detailed directions";
                                break;
                            case 3: // JP
                                result = @"携帯電話でQRコードを撮影すると道案内が受けられます。";
                                break;
                            case 4: // CN
                                result = @"用手机扫描二维码，您可以获得路线指引。";
                                break;
                        }
                        break;
                }
                break;
            case "hi":
                // hi 씬 처리 로직
                switch (depth)
                {
                    case 1: // 맑음
                        switch (i) // lang 배열의인덱스 순서 
                        {
                            case 1: // KO
                                result = @"안녕하세요. 인사동의 마스코트  '인사' 입니다.
저는 K-POP과 반려 동물을 좋아하는 20살 MZ 대학생입니다.@인사동에 대해서 궁금하신 것은 무엇이든 물어보세요.
인사동의 다양한 전통문화와 미션들을 알려드릴께요.";
                                break;
                            case 2: // EN
                                result = @"Hello, I'm Insa, the mascot of Insadong! I'm 20 and part of the MZ generation. I love K-pop, dancing, pets, and Korean food!@I'm passionate about taking photos, social media, 
finding trendy restaurants, and fashion. 
@I enjoy visiting Lotte World wearing school uniform 
and exploring palaces wearing a hanbok. 
@If you're ever curious about Insadong or anything else, 
feel free to ask me!";
                                break;
                            case 3: // JP
                                result = @"こんにちは、インサドン（インサドン）のマスコット「インサ」です
@私は今年20歳で、K-POPが大好きで、踊ることも好きですが、
韓国の伝統文化も好きなMZ世代の大学生です。
@インサドンについて何か知りたければ、
いつでも私に聞いてくださいね！";
                                break;
                            case 4: // CN
                                result = @"你好，我是仁寺洞的吉祥物“仁沙”。我今年20岁，是喜欢K-pop和跳舞的MZ世代大学生。如果你对有任何好奇，随时可以问我哦！";
                                break;
                        }
                        break;
                }
                break;
               
            case "info":
                // info 씬 처리 로직
                switch (depth)
                {
                    case 1: // 맑음
                        switch (i) // lang 배열의인덱스 순서 
                        {
                            case 1: // KO
                                result = @"인사동은 조선왕조때 부터 전통문화의 중심지였어요.
한국의 전통문화를 체험해볼수 있는 살아있는 박물관이예요@인사동에는 다양한 도자기와 고서화를 포함한 예술작품들과 
3.1운동 등의 유적지들도 볼수 있답니다.";
                                break;
                            case 2: // EN
                                result = @"Insadong is a living museum that has been at the heart of Seoul for nearly 600 years since the Joseon Dynasty. @It vividly showcases the life, history, and culture of the Korean people. In Insadong, visitors can experience a variety of antiques @such as pottery and old paintings, as well as diverse artworks.@The area is also home to historic sites, including locations tied to the March 1st Independence Movement.
";
                                break;
                            case 3: // JP
                                result = @"こんにちは、インサドン（インサドン）のマスコット「インサ」です@インサドンは朝鮮時代から約600年間、ソウルの中心部に位置しています@韓国人の生活、歴史、文化が生き生きと残っている、まるで生きた博物館のような場所です。@インサドンでは、陶磁器や古書画などの骨董品やさまざまなアート作品に触れることができ、歴史的な場所でもあります。
";
                                break;
                            case 4: // CN
                                result = @"仁寺洞是一个活生生的博物馆，自朝鲜王朝时期以来，近600年来一直坐落于首尔的中心。@这里生动地展现了韩国人的生活、历史和文化。@在仁寺洞，游客可以欣赏到陶瓷、古书画等各种古董和艺术作品，并且可以参观包括内的众多历史遗址
";
                                break;
                        }
                        break;
                }
                break;
            case "insaAr":
                // insaAr 씬 처리 로직                
                switch (depth)
                {
                    case 1:
                        switch (i) // lang 배열의인덱스 순서 
                        {
                            case 1: // KO
                                result = @"한복을 입은 여러분의 모습을 사진속에 담아 드릴께요.
마음에 드는 한복을 고르시고 촬영 버튼을 눌러주세요.@그리고 바닥의 표시된 부분에 서서 카메라를 봐주세요.
3,2,1 kimchi~ !!";
                                break;
                            case 2: // EN
                                result = @"I'll capture your beautiful appearance in hanbok! Please choose 
the hanbok you like and press the photo capture button.@Next, stand on the spot marked on the floor and look at the camera. 3, 2, 1—everyone say ""Kimchi~~~~~!!!!!!""";
                                break;
                            case 3: // JP
                                result = @"皆さんの美しい韓服姿を画面に収めますので、お好みの韓服を選んで、写真撮影ボタンを押してください。@そして、床に表示された場所に立ってカメラを見てください。3.2.1、みんなでキムチ~~~~~!!!!!!";
                                break;
                            case 4: // CN
                                result = @"请选择您喜欢的韩服，然后按下拍照按钮。接着站在地上标记的地方，看着相机。3.2.1，大家一起说 Kimchi~~~~~!!!!!!";
                                break;
                        }
                        break;
                    case 2:
                        switch (i) // lang 배열의인덱스 순서 
                        {
                            case 1: // KO
                                result = @"사진이 맘에 드시나요?
저장 버튼을 눌러주세요. 핸드폰에 저장해 드려요.@사진이 마음에 들지 않으세요? 그럼 다시 촬영 버튼을 눌러주세요.";
                                break;
                            case 2: // EN
                                result = @"Do you like your photo? If not,  let's try again. 
Press the Save button to download  it to your phone.";
                                break;
                            case 3: // JP
                                result = @"写真は気に入りましたか？気に入らなければ、
もう一度撮ってください。@気に入ったら、
QRコードを使用して携帯電話に保存してください。";
                                break;
                            case 4: // CN
                                result = @"照片您满意吗？如果不满意，我们可以再拍一次。如果满意，请点击保存按钮，您可以将照片保存在手机中。";
                                break;
                        }
                        break;
                    case 3:
                        switch (i) // lang 배열의인덱스 순서 
                        {
                            case 1: // KO
                                result = @"핸드폰으로 QR코드를 찍으시면 나의 핸드폰에 저장되요 ";
                                break;
                            case 2: // EN
                                result = @"Scan the [QR] code with your phone to receive detailed directions";
                                break;
                            case 3: // JP
                                result = @"携帯電話でQRコードを撮影すると自分の携帯電話に保存されます。";
                                break;
                            case 4: // CN
                                result = @"用手机扫描二维码，信息会保存在您的手机中。";
                                break;
                        }
                        break;
                    case 5:
                        switch (i) // lang 배열의인덱스 순서 
                        {
                            case 1: // KO
                                result = @"선택하신 옷을 입어보고 있어요. 조금만 기다려주세요^^";
                                break;
                            case 2: // EN
                                result = @"I'm trying on the clothes you selected. Please wait a moment^^";
                                break;
                            case 3: // JP
                                result = @"選んでいただいた服を試着しています。少々お待ちください^^";
                                break;
                            case 4: // CN
                                result = @"我正在试穿你选的衣服，请稍等一下^^";
                                break;
                        }
                        break;
                }

                break;
            case "language":
                // language 씬 처리 로직
                switch (depth)
                {
                    case 1:
                        switch (i) // lang 배열의인덱스 순서 
                        {
                            case 1: // KO
                                result = "'인사'는 여러분과 함께 소통하고 싶어요. 어떤 언어가 편하신가요?";
                                break;
                            case 2: // EN
                                result = "I can speak many languages. What language would you prefer?";
                                break;
                            case 3: // JP
                                result = "'インサ' は皆さんと話したいです！どの言語が一番お好きですか？";
                                break;
                            case 4: // CN
                                result = "'仁寺' 想和大家交流. 你们更习惯使用哪种语言呢？";
                                break;
                        }
                        break;
                    case 2:
                        switch (i) // lang 배열의인덱스 순서 
                        {
                            case 1: // KO
                                result = "안녕하세요, 지금부터는 저와 한국어로 대화할 수 있어요";
                                break;
                            case 2: // EN
                                result = "Let's continue our conversation in English";
                                break;
                            case 3: // JP
                                result = "はじめまして、今から私と日本語で会話できますよ。";
                                break;
                            case 4: // CN
                                result = "'仁寺' 想和大家交流. 你们更习惯使用哪种语言呢？";
                                break;
                        }
                        break;
                }

                break;
            case "lodgment":
                // lodgment 씬 처리 로직
                switch (depth)
                {
                    case 1:
                        switch (i) // lang 배열의인덱스 순서 
                        {
                            case 1: // KO
                                result = @"인사동 근처에는 다양한 유형의 숙소들이 있어요. 
검색을 통해 숙소정보를 알아보세요. ";
                                break;
                            case 2: // EN
                                result = @"Searching for places to stay near Insadong? There are plenty of options. Find the perfect accommodation by searching here.";
                                break;
                            case 3: // JP
                                result = @"インサドン近くには様々なタイプの宿泊施設があります。
検索して宿泊情報を調べてみてください";
                                break;
                            case 4: // CN
                                result = @"仁寺洞附近有各种类型的住宿。可以通过搜索了解更多住宿信息。";
                                break;
                        }
                        break;
                    case 2:
                        switch (i) // lang 배열의인덱스 순서 
                        {
                            case 1: // KO
                                result = "필요한신 내용을 클릭하면 더욱 상세한 정보를 얻으실 수 있어요. ";
                                break;
                            case 2: // EN
                                result = "Click on the information you need to get more detailed details.";
                                break;
                            case 3: // JP
                                result = @"必要な情報をクリックすると、
さらに詳しい情報を得ることができます。";
                                break;
                            case 4: // CN
                                result = "点击所需的内容，您可以获取更详细的信息。";
                                break;
                        }
                        break;
                    case 3:
                        switch (i) // lang 배열의인덱스 순서 
                        {
                            case 1: // KO
                                result = "핸드폰으로 [QR] 코드를 찍으시면 길을 안내 받으실수 있어요.";
                                break;
                            case 2: // EN
                                result = "Scan the [QR] code with your phone to receive detailed directions.";
                                break;
                            case 3: // JP
                                result = @"携帯電話でQRコードを撮影すると道案内が受けられます。";
                                break;
                            case 4: // CN
                                result = "用手机扫描二维码，您可以获得路线指引。";
                                break;
                        }
                        break;
                }
                break;
            case "mission":
                // mission 씬 처리 로직

                switch (depth)
                {
                    case 1:
                        switch (i) // lang 배열의인덱스 순서 
                        {
                            case 1: // KO
                                result = @"인사동에는 다양한 미션들이 진행되고 있어요.
“WIT” APP으로 도전해보세요. 다양한 선물과 혜택을 드립니다.";
                                break;
                            case 2: // EN
                                result = @"Take on the challenge by joining missions in Insadong for exciting gifts and experiences! Complete tasks through the 'WIT' app!";
                                break;
                            case 3: // JP
                                result = @"「インサドンには様々なミッションが進行中です。ミッションに参加して、様々なプレゼントや経験に挑戦してみましょう。@『WIT』アプリと連動して、様々な『ミッション』を進行してみましょう。」";
                                break;
                            case 4: // CN
                                result = @"仁寺洞有各种各样的任务正在进行中。参与任务，挑战各种礼物和体验吧。通过与 'WIT' 应用程序的联动，尝试各种 '任务' 吧。";
                                break;
                        }
                        break;
                    case 2:
                        switch (i) // lang 배열의인덱스 순서 
                        {
                            case 1: // KO
                                result = @"'인사'가 추천하는 미션이에요. 인사동에서 좋은 시간 보내세요.";
                                break;
                            case 2: // EN
                                result = @"I highly recommend this itinerary. Choose one of the three. 
Enjoy your time exploring Insadong!";
                                break;
                            case 3: // JP
                                result = @"インサがおすすめするコースです。3つの中から一つを選んでみましょう。 インサドンで良い時間を過ごしてください。";
                                break;
                            case 4: // CN
                                result = @"这是 '仁寺' 推荐的路线。请选择其中一个，
然后在仁寺洞度过美好的时光吧！";
                                break;
                        }
                        break;
                    case 3:
                        switch (i) // lang 배열의인덱스 순서 
                        {
                            case 1: // KO
                                result = @"핸드폰으로 [QR] 코드를 찍으시면 ‘미션’ 을 안내 받으실수 있어요.";
                                break;
                            case 2: // EN
                                result = @"Scan the [QR] code with your phone to receive detailed directions.";
                                break;
                            case 3: // JP
                                result = @"携帯電話で[QR] コードを撮影すると道案内が受けられます。
";
                                break;
                            case 4: // CN
                                result = @"用手机扫描[QR]码，您可以获得路线指引。";
                                break;
                        }
                        break;
                }

                break;
            case "museum":
                // museum 씬 처리 로직

                switch (depth)
                {
                    case 1:
                        switch (i) // lang 배열의인덱스 순서 
                        {
                            case 1: // KO
                                result = @"인사동에는  많은 갤러리들과 미술상점이 있어요.
우리 함께 다양한 작품들을 감상하러 가볼까요?
";
                                break;
                            case 2: // EN
                                result = @"Insadong is home to many galleries where you can view 
and purchase a variety of artworks. Take some time to explore.";
                                break;
                            case 3: // JP
                                result = "インサドンには非常に多くのギャラリーがあります。多くの作品を鑑賞し、購入もできるので、見て回ってはいかがでしょうか？";
                                break;
                            case 4: // CN
                                result = @"仁寺洞有很多画廊，您可以欣赏到许多作品，还可以购买。
不妨去逛一逛吧！";
                                break;
                        }
                        break;
                    case 2:
                        switch (i) // lang 배열의인덱스 순서 
                        {
                            case 1: // KO
                                result = @"갤러리를 클릭하면 갤러리 소개와 찾아가는 방법을 알려드릴게요.
";
                                break;
                            case 2: // EN
                                result = @"Tap for more details and directions";
                                break;
                            case 3: // JP
                                result = "ギャラリーをクリックするとギャラリーの紹介と行き方をお知らせします。";
                                break;
                            case 4: // CN
                                result = @"点击画廊后，我会为您提供画廊的介绍和前往的路线。";
                                break;
                        }
                        break;
                    case 3:
                        switch (i) // lang 배열의인덱스 순서 
                        {
                            case 1: // KO
                                result = @"핸드폰으로 [QR] 코드를 찍으시면 길을 안내 받으실수 있어요.";
                                break;
                            case 2: // EN
                                result = @"Scan the [QR] code with your phone to receive detailed directions";
                                break;
                            case 3: // JP
                                result = @"携帯電話で[QR] コードを撮影すると道案内が受けられます。";
                                break;
                            case 4: // CN
                                result = @"用手机扫描[QR]码，您可以获得路线指引。";
                                break;
                        }
                        break;
                }

                break;
            case "noTouch":
                // noTouch 씬 처리 로직

                switch (depth)
                {
                    case 1:
                        switch (i) // lang 배열의인덱스 순서 
                        {
                            case 1: // KO
                                result = @"안녕하세요^^ 인사동 홍보도우미 '인사'에요.
제가 요즘 배우는 댄스인데 잠깐 보여드릴게요~";
                                break;
                            case 2: // EN
                                result = @"Hello! I'm Insa, your guide to Insadong. 
Check out my new dance!";
                                break;
                            case 3: // JP
                                result = "こんにちは^^ インサドン（仁寺洞）のプロモーションアシスタント‘INSA’です。最近学んでいるダンスを少しだけお見せしますね~";
                                break;
                            case 4: // CN
                                result = "你好^^ 我是仁寺洞的宣传助手‘INSA’。我最近在学跳舞，给大家展示一下~";
                                break;
                        }
                        break;
                    case 2:
                        switch (i) // lang 배열의인덱스 순서 
                        {
                            case 1: // KO
                                result = @"한복은 한국의 역사와 문화가 담겨있는 전통 의복입니다. 
고유한 아름다움과 디자인을 통해 잘 표현되어 있습니다. @한국 전통 문화의 대표적인 유산으로서 세계적으로 
많은 이들에게 사랑받고 있어요";
                                break;
                            case 2: // EN
                                result = "‘Hanbok’ is a traditional Korean garment with a long history and deep-rooted traditions. Renowned for its inherent beauty@and unique design, ‘Hanbok’ is considered one of the symbols representing Korean culture@It is primarily worn during special occasions and holidays. Unlike the linear fashion of Western clothing, @Hanbok features soft, flowing lines and generous fabric. It is a significant cultural heritage @that symbolizes Korean tradition and culture, and its beauty is recognized worldwide.";
                                break;
                            case 3: // JP
                                result = "「ハンボク(韓服)は韓国の伝統的な衣服で、長い歴史と伝統を持つ服です。@ハンボク(韓服)は固有の美しさとユニークなデザインで韓国文化を代表する象徴の一つとされ、特別な行事や祝日に主に着用しています。@直線を主に表現する西洋服とは異なり、韓服は柔らかく流れるような線とゆったりとした服の裾が特徴です。@ハンボク(韓服)は単なる衣服を超えて、韓国の伝統と文化を象徴する重要な文化遺産であり、世界的にもその美しさが認められています。";
                                break;
                            case 4: // CN
                                result = "韩服是韩国的传统服饰，拥有悠久的历史和传统。韩服因其独特的美感和设计，被视为韩国文化的象征之一，通常在特别的活动或节日时穿着。@与直线的西方服饰不同，韩服的特点是柔和流畅的线条和宽松的衣摆。@韩服不仅仅是一种服饰，更是象征韩国传统和文化的重要文化遗产，全球也认可它的美丽。";
                                break;
                        }
                        break;
                    case 3:
                        switch (i) // lang 배열의인덱스 순서 
                        {
                            case 1: // KO
                                result = @"인사동에 오신걸 환영합니다. 키오스크 에서는 다양한 정보를 제공합니다.  
'인사'가 인사동을 자세히 알려드릴게요";
                                break;
                            case 2: // EN
                                result = @"Welcome to Insadong! 
You can find all the necessary information at the kiosk";
                                break;
                            case 3: // JP
                                result = @"インサドン（仁寺洞）へようこそ。キオスクでは様々な情報を提供しています。
'INSA'がインサドン（仁寺洞）について詳しくご案内します。";
                                break;
                            case 4: // CN
                                result = "欢迎来到仁寺洞！和我一起的话，您可以更轻松地享受仁寺洞^^";
                                break;
                        }
                        break;
                    case 4:
                        switch (i) // lang 배열의인덱스 순서 
                        {
                            case 1: // KO
                                result = @"한복은 한국의 역사와 문화가 담겨있는 전통 의복입니다. 고유한 아름다움과 디자인을 통해 잘 표현되어 있습니다. @ 한국 전통 문화의 대표적인 유산으로서 세계적으로 
많은 이들에게 사랑받고 있어요";
                                break;
                            case 2: // EN
                                result = "‘Hanbok’ is a traditional Korean garment with a long history and deep-rooted traditions. Renowned for its inherent beauty@and unique design, ‘Hanbok’ is considered one of the symbols representing Korean culture@It is primarily worn during special occasions and holidays. Unlike the linear fashion of Western clothing, @Hanbok features soft, flowing lines and generous fabric. It is a significant cultural heritage @that symbolizes Korean tradition and culture, and its beauty is recognized worldwide.";
                                break;
                            case 3: // JP
                                result = "「ハンボク(韓服)は韓国の伝統的な衣服で、長い歴史と伝統を持つ服です。 @ハンボク(韓服)は固有の美しさとユニークなデザインで韓国文化を代表する象徴の一つとされ、特別な行事や祝日に主に着用しています。@直線を主に表現する西洋服とは異なり、韓服は柔らかく流れるような線とゆったりとした服の裾が特徴です。@ハンボク(韓服)は単なる衣服を超えて、韓国の伝統と文化を象徴する重要な文化遺産であり、世界的にもその美しさが認められています。";
                                break;
                            case 4: // CN                여기부터 해야해 현우야 ###@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                                result = @"韩服是韩国的传统服饰，具有悠久的历史和传统。因其美感和设计成为了韩国文化的象征，通常在特别的活动或节日穿着。@作为韩国传统文化的代表性遗产，韩服在全球广受喜爱。
试着用AR体验穿着吧！@韩服不仅仅是一种服饰，更是象征韩国传统和文化的重要文化遗产，全球也认可它的美丽。";
                                break;
                        }
                        break;
                    case 5:
                        switch (i) // lang 배열의인덱스 순서 
                        {
                            case 1: // KO
                                result = @"WIT 앱을 통해서 다양한 미션들에 도전해 보세요.
푸짐한 선물과 혜택을 드립니다";
                                break;
                            case 2: // EN
                                result = "Take on the challenge by joining missions in Insadong for exciting gifts and experiences! Complete tasks through the 'WIT' app!";
                                break;
                            case 3: // JP
                                result = "「インサドン（仁寺洞）には様々なミッションが進行中です。@ミッションに参加して、様々なプレゼントや経験に挑戦してみましょう。 @『WIT』アプリと連動して、様々な『ミッション』を進行してみましょう。」";
                                break;
                            case 4: // CN
                                result = "仁寺洞有各种各样的任务正在进行中。参与任务，挑战各种礼物和体验吧。通过与 'WIT' 应用程序的联动，尝试各种 '任务' 吧。";
                                break;
                        }
                        break;
                    case 6:
                        switch (i) // lang 배열의인덱스 순서 
                        {
                            case 1: // KO
                                result = @"한복은 한국의 역사와 문화가 담겨있는 전통 의복입니다. 
고유한 아름다움과 디자인을 통해 잘 표현되어 있습니다.@한국 전통 문화의 대표적인 유산으로서 
세계적으로 많은 이들에게 사랑받고 있어요";
                                break;
                            case 2: // EN
                                result = "‘Hanbok’ is a traditional Korean garment with a long history and deep-rooted traditions. Renowned for its inherent beauty@and unique design, ‘Hanbok’ is considered one of the symbols representing Korean culture@It is primarily worn during special occasions and holidays. Unlike the linear fashion of Western clothing, @Hanbok features soft, flowing lines and generous fabric. It is a significant cultural heritage @that symbolizes Korean tradition and culture, and its beauty is recognized worldwide.";
                                break;
                            case 3: // JP
                                result = "「ハンボク(韓服)は韓国の伝統的な衣服で、長い歴史と伝統を持つ服です。 @ハンボク(韓服)は固有の美しさとユニークなデザインで韓国文化を代表する象徴の一つとされ、特別な行事や祝日に主に着用しています。@直線を主に表現する西洋服とは異なり、韓服は柔らかく流れるような線とゆったりとした服の裾が特徴です。@ハンボク(韓服)は単なる衣服を超えて、韓国の伝統と文化を象徴する重要な文化遺産であり、世界的にもその美しさが認められています。";
                                break;
                            case 4: // CN
                                result = @"韩服是韩国的传统服饰，具有悠久的历史和传统。因其美感和设计成为了韩国文化的象征，通常在特别的活动或节日穿着。@作为韩国传统文化的代表性遗产，韩服在全球广受喜爱。
试着用AR体验穿着吧！";
                                break;
                        }
                        break;
                    case 7:
                        switch (i) // lang 배열의인덱스 순서 
                        {
                            case 1: // KO
                                result = @"안녕하세요. 인사동의 마스코트  '인사'입니다.
저는 K-POP과 반려 동물을 좋아하는 20살 MZ 대학생입니다.@인사동에 대해서 궁금하신 것은 무엇이든 물어보세요.
인사동의 다양한 전통문화와 미션들을 알려드릴께요.";
                                break;
                            case 2: // EN
                                result = @"Hello, I'm Insa, the mascot of Insadong! I'm 20 and part of the MZ generation. 
I love K-pop, dancing, pets, and Korean food!@I'm passionate about taking photos, social media,
finding trendy restaurants, and fashion. @I enjoy visiting Lotte World wearing school uniform and exploring palaces wearing a hanbok. @If you're ever curious about Insadong or anything else, feel free to ask me!";
                                break;
                            case 3: // JP
                                result = @"「こんにちは、インサドン（仁寺洞）のマスコット『インサ』です。 @私は今年20歳で、K-POPとダンスが好きなMZ世代の大学生です！ 犬と猫が好きで、韓国料理をよく食べ、写真を撮るのが好きで、インスタグラムやTikTokを楽しんでいます。 @グルメを探し、ファッションを楽しみ、制服を着てロッテワールドに行くことや、韓服を着て宮殿に遊びに行くことが好きです。 @K-POPアイドルが好きです。 これからインサドン（仁寺洞）について知りたいことがあれば私に聞いてください！";
                                break;
                            case 4: // CN
                                result = @"你好，我是仁寺洞的吉祥物“仁寺”。 我今年20岁，是一名喜欢K-POP和跳舞的MZ世代大学生。 @我呢，喜欢狗和猫，喜欢韩食，喜欢拍照，
也喜欢用Instagram和TikTok。@我非常喜欢K-POP偶像。 今后如果对仁寺洞有什么好奇的，
随时可以问我哦！";
                                break;
                        }
                        break;
                    case 8:
                        switch (i) // lang 배열의인덱스 순서 
                        {
                            case 1: // KO
                                result = @"한복은 한국의 역사와 문화가 담겨있는 전통 의복입니다.
고유한 아름다움과 디자인을 통해 잘 표현되어 있습니다.@한국 전통 문화의 대표적인 유산으로서 세계적으로 
많은 이들에게 사랑받고 있어요";
                                break;
                            case 2: // EN
                                result = @"Hanbok’is a traditional Korean garment with a long history and deep-rooted traditions.
Renowned for its inherent beauty@and unique design,‘Hanbok is considered one of the symbols representing Korean culture@It is primarily worn during special occasions and holidays. Unlike the linear fashion of Western clothing, @Hanbok features soft, flowing lines and generous fabric. It is a significant cultural heritage @that symbolizes Korean tradition and culture, and its beauty is recognized worldwide.";
                                break;
                            case 3: // JP
                                result = "「ハンボク(韓服)は韓国の伝統的な衣服で、長い歴史と伝統を持つ服です。 @ハンボク(韓服)は固有の美しさとユニークなデザインで韓国文化を代表する象徴の一つとされ、特別な行事や祝日に主に着用しています。@直線を主に表現する西洋服とは異なり、韓服は柔らかく流れるような線とゆったりとした服の裾が特徴です。@ハンボク(韓服)は単なる衣服を超えて、韓国の伝統と文化を象徴する重要な文化遺産であり、世界的にもその美しさが認められています。";
                                break;
                            case 4: // CN
                                result = @"韩服是韩国的传统服饰，具有悠久的历史和传统。因其美感和设计成为了韩国文化的象征，通常在特别的活动或节日穿着。@作为韩国传统文化的代表性遗产，韩服在全球广受喜爱。
试着用AR体验穿着吧！";
                                break;
                        }
                        break;
                }
                break;
            case "palace":
                // palace 씬 처리 로직
                switch (depth)
                {
                    case 1: // 
                        switch (i) // lang 배열의인덱스 순서 
                        {
                            case 1: // KO
                                result = @"인사동 근처에는 조선시대 왕이 살았던 궁궐들이 있어요 
한국의 궁을 방문해보는건 어떠실까요?";
                                break;
                            case 2: // EN
                                result = @"Explore nearby palaces where the Joseon Dynasty kings once resided. Make sure to visit Korean palaces during your trip!";
                                break;
                            case 3: // JP
                                result = @"インサドン近くには朝鮮時代の王が住んでいた宮殿があります。韓国の宮殿を訪れてみてはいかがですか？";
                                break;
                            case 4: // CN
                                result = @"仁寺洞附近有朝鲜时代国王居住的宫殿。
要不要去参观一下韩国的这些宫殿呢？";
                                break;
                        }
                        break;
                    case 2: // 
                        switch (i) // lang 배열의인덱스 순서 
                        {
                            case 1: // KO
                                result = @"핸드폰으로 [QR] 코드를 찍으시면 길을 안내 받으실수 있어요.";
                                break;
                            case 2: // EN
                                result = @"Scan the [QR] code with your phone to receive detailed directions";
                                break;
                            case 3: // JP
                                result = @"携帯電話でQRコードを撮影すると道案内が受けられます。";
                                break;
                            case 4: // CN
                                result = @"用手机扫描二维码，您可以获得路线指引。";
                                break;
                        }
                        break;                
                }
                break;
            case "search":
                // search 씬 처리 로직

                switch (depth)
                {
                    case 1: // 맑음
                        switch (i) // lang 배열의인덱스 순서 
                        {
                            case 1: // KO
                                result = @"인사동은 곳곳에 예쁜 장소가 정말 많아요. 검색해보세요. 
저 '인사'가 빠르게 찾아드릴게요.";
                                break;
                            case 2: // EN
                                result = @"Insadong is filled with beautiful places. 
I can help you find them in no time. Let’s explore!";
                                break;
                            case 3: // JP
                                result = @"インサドンにはあちこちに素敵な場所がたくさんあります。
検索してみてください。「インサ」がすぐに見つけます。";
                                break;
                            case 4: // CN
                                result = @"仁寺洞到处都有很多漂亮的地方。快去搜索一下吧，
“仁寺”会快速帮你找到的。";
                                break;
                        }
                        break;
                    case 2: // 맑음
                        switch (i) // lang 배열의인덱스 순서 
                        {
                            case 1: // KO
                                result = @"매장을 클릭하시면 더욱더 상세한 정보를 얻을 수 있어요.
                                          ";
                                break;
                            case 2: // EN
                                result = @"Tap for more details and directions
";
                                break;
                            case 3: // JP
                                result = @"店舗をクリックすると、さらに詳しい情報が得られます。
";
                                break;
                            case 4: // CN
                                result = @"点击店铺后，您可以获取更详细的信息。
";
                                break;
                        }
                        break;
                    case 3: // 맑음
                        switch (i) // lang 배열의인덱스 순서 
                        {
                            case 1: // KO
                                result = @"핸드폰으로 [QR] 코드를 찍으시면 길을 안내 받으실수 있어요.";
                                break;
                            case 2: // EN
                                result = @"Scan the [QR] code with your phone to receive detailed directions";
                                break;
                            case 3: // JP
                                result = @"携帯電話で[QR] コードを撮影すると道案内が受けられます。";
                                break;
                            case 4: // CN
                                result = @"用手机扫描[QR]码，您可以获得路线指引。";
                                break;
                        }
                        break;
                }

                break;
            case "shopping":
                // shopping 씬 처리 로직
                switch (depth)
                {
                    case 1: // 맑음
                        switch (i) // lang 배열의인덱스 순서 
                        {
                            case 1: // KO
                                result = @"즐거운 추억이 될 쇼핑 아이템들을 찾으시나요?
인사동에는 다양한 전통문화 상점들이 있어요.@인사동에서 무엇을 사야할지 고민이 되신다면 인사가 추천하는 카테고리를 참고해 보세요~ 인사동에서 즐거운 쇼핑하세요.";
                                break;
                            case 2: // EN
                                result = @"Insadong is full of shops! Shop for unique items that create lasting memories. Not sure what to buy? Check these out!";
                                break;
                            case 3: // JP
                                result = @"インサドンには多様な店舗があります。旅行の思い出になる多様なインサドンの商品をショッピングしに行きませんか？@何を買うか悩んでいるなら、インサのおすすめカテゴリーを参考にしてみてください！";
                                break;
                            case 4: // CN
                                result = @"仁寺洞有许多不同的商店。要不要去逛逛，
买一些能留下旅行回忆的仁寺洞特色商品呢？@如果不知道买什么，可以参考“仁寺”的推荐类别！";
                                break;
                        }
                        break;
                    case 2: // 맑음
                        switch (i) // lang 배열의인덱스 순서 
                        {
                            case 1: // KO
                                result = @"매장을 클릭하시면 더욱더 상세한 정보를 얻을 수 있어요.";
                                break;
                            case 2: // EN
                                result = @"Tap for more details and directions";
                                break;
                            case 3: // JP
                                result = @"店舗をクリックすると、さらに詳しい情報を得ることができます。";
                                break;
                            case 4: // CN
                                result = @"点击店铺后，您可以获取更详细的信息。";
                                break;
                        }
                        break;
                    case 3: // 맑음
                        switch (i) // lang 배열의인덱스 순서 
                        {
                            case 1: // KO
                                result = @"핸드폰으로 [QR] 코드를 찍으시면 길을 안내 받으실수 있어요.";
                                break;
                            case 2: // EN
                                result = @"Scan the [QR] code with your phone to receive detailed directions";
                                break;
                            case 3: // JP
                                result = @"携帯電話で[QR] コードを撮影すると道案内が受けられます。";
                                break;
                            case 4: // CN
                                result = @"用手机扫描[QR]码，您可以获得路线指引。";
                                break;
                        }
                        break;
                }

                break;
            case "trans":
                // trans 씬 처리 로직

                switch (depth)
                {
                    case 1: // 맑음
                        switch (i) // lang 배열의인덱스 순서 
                        {
                            case 1: // KO
                                result = @"인사동 교통편이 궁금하시다고요? 
인사동에 올 수 있는 다양한 교통정보를 알아보세요.";
                                break;
                            case 2: // EN
                                result = @"Looking for transportation info in Insadong? Learn about 
all available transit options, including those beyond Insadong.";
                                break;
                            case 3: // JP
                                result = @"インサドンの交通手段が気になる？インサドンだけでなく様々な交通情報をチェックしてください。";
                                break;
                            case 4: // CN
                                result = @"您对仁寺洞的交通方式感兴趣吗？
不仅是仁寺洞，还有各种交通信息可以了解。";
                                break;
                        }
                        break;
                    case 2: // 맑음
                        switch (i) // lang 배열의인덱스 순서 
                        {
                            case 1: // KO
                                result = @"핸드폰으로 [QR] 코드를 찍으시면 길을 안내 받으실수 있어요.";
                                break;
                            case 2: // EN
                                result = @"Scan the [QR] code with your phone to receive detailed directions.
";
                                break;
                            case 3: // JP
                                result = @"携帯電話でQRコードを撮影すると道案内が受けられます。
";
                                break;
                            case 4: // CN
                                result = @"用手机扫描二维码，您可以获得路线指引。";
                                break;
                        }
                        break;                  
                }
                break;
            case "weather":
                // weather 씬 처리 로직
                switch (depth)
                {
                    case 1: // 맑음
                        switch (i) // lang 배열의인덱스 순서 
                        {
                            case 1: // KO
                                result = @"오늘은 맑고 화창한 하루가 될 거예요.
                                            인사동만의 다양한 전통 문화들을 즐겨보세요.";
                                break;
                            case 2: // EN
                                result = @"I'm Insa, your friendly weather fairy here in Insadong. 
Today, I’ve conjured up some beautiful sunshine just for you!";
                                break;
                            case 3: // JP
                                result = @"私はインサドンにいる皆さんの親しみやすい天気の妖精、インサです。美しい日差しと共にインサドンを楽しんでください。";
                                break;
                            case 4: // CN
                                result = @"我是位于仁寺洞的你们亲切的天气精灵“仁寺”。
在美丽的阳光下，尽情享受仁寺洞的美好时光吧！";
                                break;
                        }

                        break;
                    case 2: // 비                    
                        switch (i) // lang 배열의인덱스 순서 
                        {
                            case 1: // KO
                                result = @"우산을 챙기셨나요? 오늘은 비가 내려요.
빗소리와 함께 인사동을 즐겨보세요.";
                                break;
                            case 2: // EN
                                result = @"Hello from Insa! It’s raining today, so don’t forget your umbrella. Enjoy the sound of the rain while you explore Insadong!";
                                break;
                            case 3: // JP
                                result = @"今日は雨が降っていますが、傘を忘れませんでしたか？雨音を歌にしてインサドンを楽しんでください。";
                                break;
                            case 4: // CN
                                result = @"今天下雨了，你没忘记带伞吧？伴着雨声漫步仁寺洞，
享受这美好的时光吧！";
                                break;
                        }
                        break;
                    case 3: // 구름                    
                        switch (i) // lang 배열의인덱스 순서 
                        {
                            case 1: // KO
                                result = @"오늘은 흐린 하늘을 보실수 있어요.
흐린날이지만 우리 함께 빛나는 인사동의 보석들을 찾아볼까요.";
                                break;
                            case 2: // EN
                                result = @"The sky is cloudy today. Let’s take a look at Insadong Street, which shines even brighter in the cloudy weather!";
                                break;
                            case 3: // JP
                                result = @"今天下雨了，你没忘记带伞吧？
伴着雨声漫步インサドン，享受这美好的时光吧！";
                                break;
                            case 4: // CN
                                result = @"今天天空有些阴沉, 在这阴天里, 去欣赏更加明亮的仁寺洞街道吧！";
                                break;
                        }
                        break;
                }
                break;
            case "what":
                // what 씬 처리 로직

                switch (depth)
                {
                    case 1: // 맑음
                        switch (i) // lang 배열의인덱스 순서 
                        {
                            case 1: // KO
                                result = @"안녕하세요. ‘인사’ 예요.
저와함께 인사동의전통 문화를 즐겨보세요.@인사동에는 다양한 즐길거리들이 가득하답니다.
질문에 대답을 해주시면 추천코스를 알려 드릴께요.
";
                                break;
                            case 2: // EN
                                result = @"Are you visiting with family, friends, or a partner? Let me know how many are in your group, and I’ll suggest an itinerary!@How many people are in your group, and how much time will 
you be spending in Insadong? What would you like to do here?
";
                                break;
                            case 3: // JP
                                result = @"家族、友達、恋人と一緒に来ましたか？人数を教えてくれれば、
充実した一日を過ごせるおすすめコースを教えます！@何人で来ましたか？インサドンにどれくらい巡るつもりですか？
インサドンでやりたいことは何ですか？";
                                break;
                            case 4: // CN
                                result = @"您是和家人、朋友或恋人一起来的吗？告诉我人数，
我会为您推荐一个充实的一日游路线！@你们一共几个人？打算在仁寺洞待多久？
在仁寺洞想做些什么呢？请选择一个类别吧！";
                                break;
                        }

                        break;
                    case 2: // 비                    
                        switch (i) // lang 배열의인덱스 순서 
                        {
                            case 1: // KO
                                result = @"'인사'가 추천하는 코스에요. 인사동에서 좋은 시간 보내세요
3가지중에 하나를 선택해보세요. 인사동에서 좋은 시간 보내세요";
                                break;
                            case 2: // EN
                                result = @"I highly recommend this itinerary. Choose one of three routes.";
                                break;
                            case 3: // JP
                                result = @"「INSA」がおすすめするコースです。3つの中から1つを選んでください。
インサドンで素敵な時間をお過ごしください。";
                                break;
                            case 4: // CN
                                result = @"这是 '仁寺' 推荐的路线。请选择其中一个，
然后在仁寺洞度过美好的时光吧！";
                                break;
                        }
                        break;
                    case 3: // 구름                    
                        switch (i) // lang 배열의인덱스 순서 
                        {
                            case 1: // KO
                                result = @"핸드폰으로 [QR] 코드를 찍으시면 길을 안내 받으실수 있어요.
                                          ";
                                break;
                            case 2: // EN
                                result = @"Scan the [QR] code with your phone to receive detailed directions
";
                                break;
                            case 3: // JP
                                result = @"携帯電話で[QR] コードを撮影すると道案内が受けられます。";
                                break;
                            case 4: // CN
                                result = @"用手机扫描[QR]码，您可以获得路线指引。
";
                                break;
                        }
                        break;
                }
                break;
            case "aRExplain":
                // what 씬 처리 로직

                switch (depth)
                {
                    case 1:
                        switch (i) // lang 배열의인덱스 순서 
                        {
                            case 1: // KO
                                result = @"한복은 한국의 역사와 문화가 담겨있는 전통 의복입니다. 
고유한 아름다움과 디자인을 통해 잘 표현되어 있습니다.@한국 전통 문화의 대표적인 유산으로서 세계적으로 
많은 이들에게 사랑받고 있어요. AR로 착장해보세요.";
                                break;
                            case 2: // EN
                                result = @"‘Hanbok’ is a traditional Korean garment with a long history 
and deep-rooted traditions. Renowned for its inherent beauty@and unique design, ‘Hanbok’ is considered one of 
the symbols representing Korean culture@It is primarily worn during special occasions and holidays. 
Unlike the linear fashion of Western clothing, @Hanbok features soft, flowing lines and generous fabric. 
It is a significant cultural heritage @that symbolizes Korean tradition and culture, 
and its beauty is recognized worldwide.";
                                break;
                            case 3: // JP
                                result = @"「ハンボク(韓服)は韓国の伝統的な衣服で、長い歴史と伝統を持つ服です。@ハンボク(韓服)は固有の美しさとユニークなデザインで韓国文化を代表する
象徴の一つとされ、特別な行事や祝日に主に着用しています。@直線を主に表現する西洋服とは異なり、韓服は柔らかく流れるような線と
ゆったりとした服の裾が特徴です。@ハンボク(韓服)は単なる衣服を超えて、韓国の伝統と文化を象徴する
重要な文化遺産であり、世界的にもその美しさが認められています。";
                                break;
                            case 4: // CN
                                result = @"韩服是韩国的传统服饰，具有悠久的历史和传统。因其美感和设计成为了韩国文化的象征，通常在特别的活动或节日穿着。@作为韩国传统文化的代表性遗产，韩服在全球广受喜爱。
试着用AR体验穿着吧！@韩服不仅仅是一种服饰，更是象征韩国传统和文化的重要文化遗产，全球也认可它的美丽。";
                                break;
                        }
                        break;
                }
                break;
            default:
                // 기본 처리 로직
                break;
        }
        return result;
    }

    private string[] getVideoScenes()
    {
        return new string[] {
            "event", "exchange", "food", "help", "hi", "info", "insaAr",
            "language", "lodgment", "mission", "museum", "noTouch",
            "palace", "search", "shopping", "trans", "weather", "what","aRExplain"
        };
    }

    // 비디오 전환 
    public void clipTranslate(string scene, string depth, string opt) {
        // 클립 전환 
        // 클립에 맞는 자막 텍스트 뿌리기



    }


    private List<string> getVideoScenesDictionary()
    {
        return new Dictionary<string, string>()
        {
            { "event", "event" }, { "exchange", "exchange" }, { "food", "food" },
            { "help", "help" }, { "hi", "hi" }, { "info", "info" },
            { "insaAr", "insaAr" }, { "language", "language" }, { "lodgment", "lodgment" },
            { "mission", "mission" }, { "museum", "museum" }, { "noTouch", "noTouch" },
            { "palace", "palace" }, { "search", "search" }, { "shopping", "shopping" },
            { "trans", "trans" }, { "weather", "weather" }, { "what", "what" }
        }.Where(kv => kv.Key.Contains(SceneManager.GetActiveScene().name))
        .Select(kv => kv.Value)
        .ToList();
    }

}
