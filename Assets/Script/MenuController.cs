using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public Dropdown genreDropdown;
    public Dropdown difficultyDropdown;
    public GameData gameData;

    void Start()
    {
        // 自動的にDropdownを取得（オブジェクト名が"GenreDropdown"の場合）
        genreDropdown = GameObject.Find("Genre").GetComponent<Dropdown>();
        difficultyDropdown = GameObject.Find("Difficulty").GetComponent<Dropdown>();
    }
    public void OnStartGame()
    {
        // gameData が null でないかチェック
        if (gameData != null)
        {
            gameData.selectedGenre = genreDropdown.options[genreDropdown.value].text;
            gameData.selectedDifficulty = difficultyDropdown.options[difficultyDropdown.value].text;

            UnityEngine.SceneManagement.SceneManager.LoadScene("Non Adjusted");
        }
        else
        {
            Debug.LogError("gameData is not assigned in the inspector!");
        }
    }
}

