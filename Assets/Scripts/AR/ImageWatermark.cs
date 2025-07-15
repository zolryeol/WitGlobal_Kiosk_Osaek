using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageWatermark : MonoBehaviour
{

    public Texture2D baseImage;     //  합성할 원본 이미지
    public Texture2D watermarkImage;//  워터마크 이미지

    public Vector2 watermarkPosition = new Vector2(10, 10);

    public Texture2D AddWatermark(Texture2D baseTexture, Texture2D watermarkTexture, float alpha = 1.0f)
    {
        // 워터마크 위치를 오른쪽 하단으로 설정
        Vector2 position = new Vector2(
            baseTexture.width - watermarkTexture.width - watermarkPosition.x,  
            watermarkPosition.y  
        );

        // 새로운 Texture2D 생성
        Texture2D resultTexture = new Texture2D(baseTexture.width, baseTexture.height, baseTexture.format, false);

        // 원본 텍스처 복사
        resultTexture.SetPixels(baseTexture.GetPixels());

        // 워터마크 합성
        for (int x = 0; x < watermarkTexture.width; x++)
        {
            for (int y = 0; y < watermarkTexture.height; y++)
            {
                // 워터마크의 현재 픽셀 위치
                int targetX = (int)position.x + x;
                int targetY = (int)position.y + y;

                // 텍스처 영역 벗어나면 스킵
                if (targetX >= baseTexture.width || targetY >= baseTexture.height)
                    continue;

                // 워터마크와 원본 픽셀 색상 가져오기
                Color baseColor = resultTexture.GetPixel(targetX, targetY);
                Color watermarkColor = watermarkTexture.GetPixel(x, y);

                // 알파값을 적용한 블렌딩
                float finalAlpha = watermarkColor.a * alpha; // 워터마크의 알파에 사용자 알파 값을 곱함
                Color blendedColor = Color.Lerp(baseColor, watermarkColor, finalAlpha);

                resultTexture.SetPixel(targetX, targetY, blendedColor);

            }
        }

        // 텍스처 적용
        resultTexture.Apply();

        return resultTexture;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (baseImage != null && watermarkImage != null)
        {
            // 워터마크 합성
            Texture2D result = AddWatermark(baseImage, watermarkImage, 1.0f);

            // 결과 텍스처를 적용 (예: UI 이미지 또는 메쉬 머터리얼)
            GetComponent<Renderer>().material.mainTexture = result; // 메쉬 머터리얼
                                                                    // GetComponent<UnityEngine.UI.RawImage>().texture = result; // UI RawImage
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
