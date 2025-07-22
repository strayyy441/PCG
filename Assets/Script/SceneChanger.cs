using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  // シーン遷移用

public class SceneChanger : MonoBehaviour
{
    // プレイヤーがこのブロックに触れたときに呼び出される
    private void OnTriggerEnter(Collider other)
    {
        // タグが"Player"のオブジェクトか確認
        if (other.CompareTag("Player"))
        {
            // 別のシーンに遷移する
            SceneManager.LoadScene("AdventureWorld");  // "NextScene" は遷移したいシーン名
        }
    }

    //スタートボタンが押されたとき
    public void OnPressStartButton()
    {
        SceneManager.LoadScene("StartMenu");
    }
}

