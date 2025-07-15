using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour
{
    // ���� ��ü
    [SerializeField]
    private Image[] starImages;

    // ���� �ؽ�Ʈ
    [SerializeField]
    private TextMeshProUGUI scoreText;

    // ���� �̹���
    [SerializeField]
    private Sprite starSprite;

    // ���� ������Ʈ
    public void UpdateScore(string scoreStr)
    {
        // ���ھ� �����
        if (!string.IsNullOrEmpty(scoreStr))
        {
            float score = float.Parse(scoreStr);

            // �� �̹��� ������Ʈ
            int starQt = (int)score;
            for (int i = 0; i < starQt; i++)
            {
                starImages[i].sprite = starSprite;
            }

            // �����ؽ�Ʈ ������Ʈ
            scoreText.text = score.ToString();
        }
        // ���ھ� X��
        else
        {
            scoreText.text = "-";
        }

    }
}
