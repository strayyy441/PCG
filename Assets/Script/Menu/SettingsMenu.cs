using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Slider sensitivitySlider; // スライダー
    public PlayerController playerController; // 感度を設定するプレイヤー

    void Start()
    {
        //GameObject player = GameObject.FindWithTag("Player");
        sensitivitySlider = GetComponent<Slider>();
        playerController = /*player.*/GetComponent<PlayerController>();
        // スライダーの初期値をプレイヤーの感度に合わせる
        sensitivitySlider.value = playerController.lookSensitivity;
        Debug.Log("感度が" + sensitivitySlider.value + "に設定しました.");

        // スライダーの値が変化したときに感度を更新
        sensitivitySlider.onValueChanged.AddListener(UpdateSensitivity);
        Debug.Log(playerController.lookSensitivity);
    }

    void UpdateSensitivity(float value)
    {
        // プレイヤーの感度をスライダーの値に更新
        playerController.SetLookSensitivity(value);
    }
}

