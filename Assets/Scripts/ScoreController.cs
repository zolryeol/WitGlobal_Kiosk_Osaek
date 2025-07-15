using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour
{
    // 별점 객체
    [SerializeField]
    private Image[] starImages;

    // 별점 텍스트
    [SerializeField]
    private TextMeshProUGUI scoreText;

    // 별점 이미지
    [SerializeField]
    private Sprite starSprite;

    // 별점 업데이트
    public void UpdateScore(string scoreStr)
    {
        // 스코어 존재시
        if (!string.IsNullOrEmpty(scoreStr))
        {
            float score = float.Parse(scoreStr);

            // 별 이미지 업데이트
            int starQt = (int)score;
            for (int i = 0; i < starQt; i++)
            {
                starImages[i].sprite = starSprite;
            }

            // 별점텍스트 업데이트
            scoreText.text = score.ToString();
        }
        // 스코어 X면
        else
        {
            scoreText.text = "-";
        }

    }
}
