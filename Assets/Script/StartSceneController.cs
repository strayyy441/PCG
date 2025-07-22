using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneController : MonoBehaviour
{
    // スタートボタンを押した時に呼び出される関数
    public void OnStartButtonClicked()
    {
        // 遷移先のシーン名を設定
        SceneManager.LoadScene("Action World");
    }
}
