using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveToMuseumListScene : MonoBehaviour
{
    [SerializeField]
    private string museumListScene;
    [SerializeField]
    private CategoryButtonController museumCategory;
    [SerializeField]
    private TextMeshProUGUI searchText;

    public void MoveScene()
    {
        PlayerPrefs.SetInt("isCategorySearch", 1);
        PlayerPrefs.SetInt("museumCategory", museumCategory.currentIndex);
        SceneManager.LoadScene(museumListScene);
    }

    public void MoveSceneBySearchIcon()
    {
        PlayerPrefs.SetInt("isCategorySearch", 0);
        PlayerPrefs.SetString("museumSearchName", searchText.text);
        CurrentSearchKeyword.AddMuseumSearchKeyword(searchText.text);
        SceneManager.LoadScene(museumListScene);
    }
}
