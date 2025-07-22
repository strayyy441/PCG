using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    public PlayerController playerController; // PlayerControllerの参照を設定する
    public GameObject startMenu; // スタートメニューのUIオブジェクト

    // スタートボタンが押されたときに呼び出される関数
    public void OnStartButtonPressed()
    {
        Debug.Log("OnStartButtonPressed called."); // デバッグ確認
        if (startMenu != null)
        {
            startMenu.SetActive(false); // スタートメニューを非表示
            Debug.Log("startMenu is now inactive.");
        }
        else
        {
            Debug.LogError("startMenu is not assigned!");
        }

        if (playerController != null)
        {
            playerController.StartGame(); // プレイヤーの操作を有効化
        }
    }

    void Start()
    {
        // ゲーム開始時は時間を停止
        Time.timeScale = 0f;
        
        if (startMenu != null)
        {
            startMenu.SetActive(true); // スタートメニューを確実に表示
        }
        else
        {
            Debug.LogError("startMenu is not assigned!");
        }

        if (playerController != null)
        {
            playerController.DisableInput();
        }
    }
}
