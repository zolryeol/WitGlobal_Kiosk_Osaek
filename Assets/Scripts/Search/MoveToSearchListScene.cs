using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveToSearchListScene : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI searchText;


    public void OnClickSearchButton()
    {
        Debug.LogError($"{searchText.text}");
        PlayerPrefs.SetString("searchKeyword", searchText.text);
        SceneManager.LoadScene("SearchList");
    }
}
