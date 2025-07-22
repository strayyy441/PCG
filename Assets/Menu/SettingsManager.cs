using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public GameObject settingsPanel; // 設定メニューのパネル
    public Slider sensitivitySlider; // 感度を調整するスライダー

    private PlayerController playerController;
    private bool isMenuOpen = false;
    public bool IsMenuOpen => isMenuOpen; // 外部からインベントリが開いているかどうかを確認できるように

    void Awake()
    {
        // プレイヤーの視点操作を管理しているスクリプトを取得
        playerController = FindObjectOfType<PlayerController>();

        // スライダーの初期設定
        if (sensitivitySlider != null && playerController != null)
        {
            sensitivitySlider.value = playerController.lookSensitivity; // スライダーを現在の感度に同期
            sensitivitySlider.onValueChanged.AddListener(UpdateSensitivity); // スライダーの値が変更されたときに呼び出す
        }
    }

    void UpdateSensitivity(float value)
    {
        if (playerController != null)
        {
            playerController.lookSensitivity = value; // スライダーの値を感度に反映
        }
    }

    void Update()
    {
        // Escキーで設定メニューを開閉
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleSettings();
        }
    }

    void ToggleSettings()
    {
        isMenuOpen = !isMenuOpen;
        bool isActive = !settingsPanel.activeSelf;
        settingsPanel.SetActive(isActive);

        // 設定メニューを開いている間はポーズ
        Time.timeScale = settingsPanel.activeSelf ? 0f : 1f;

        // インベントリ開いている間、視点操作を無効化
        if (playerController != null)
        {
            playerController.enabled = !isMenuOpen;
        }

        // マウスカーソルを制御
        Cursor.lockState = settingsPanel.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = settingsPanel.activeSelf;
    }
}


