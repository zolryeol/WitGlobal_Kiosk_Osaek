using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QRFocusController : MonoBehaviour
{
    [SerializeField]
    private GameObject QRFocus;

    // 저장버튼 클릭시
    public void OnClickSaveButton()
    {   
        // QR보이기
        QRFocus.SetActive(true);
    }

    // X버튼 클릭시
    public void OnClickCloseButton()
    {
        // QR숨기기
        QRFocus.SetActive(false);
    }
}
