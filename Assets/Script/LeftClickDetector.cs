using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftClickDetector : MonoBehaviour
{

    private Animator anim;  //Animatorをanimという変数で定義する

    void Start()
    {
        //変数animに、Animatorコンポーネントを設定する
        anim = gameObject.GetComponent<Animator>();
    }

    void Update()
    {
        // 左クリックが押されている間
        if (Input.GetMouseButton(0))
        {
            anim.SetBool("IsLeftClick", true);
            Debug.Log("Left Click: True");
        }
        // 左クリックが離されたとき
        else
        {
            anim.SetBool("IsLeftClick", false);
            Debug.Log("Left Click: False");
        }
    }

}

