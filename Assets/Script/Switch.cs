using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    private bool isActive = false; // スイッチの状態

    void OnMouseDown()
    {
        // スイッチがクリックされたときの処理
        ToggleSwitch();
    }

    private void ToggleSwitch()
    {
        // スイッチの状態を切り替え
        isActive = !isActive;
        Debug.Log("スイッチが " + (isActive ? "オン" : "オフ") + " になりました。");

        // スイッチがオンのときの動作
        if (isActive)
        {
            // 例えば、キューブの色を変更
            GetComponent<Renderer>().material.color = Color.green;
        }
        else
        {
            // スイッチがオフのときの動作
            GetComponent<Renderer>().material.color = Color.red;
        }
    }
}

