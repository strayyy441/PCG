using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingItem : MonoBehaviour
{
    public float floatHeight = 0.5f; // 浮く高さ
    public float floatSpeed = 1f; // 浮遊速度
    private Vector3 startPosition; // 初期位置

    void Start()
    {
        startPosition = this.transform.position; // 初期位置を保存
    }

    void Update()
    {
        // Y軸の浮遊を実現
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        this.transform.position = new Vector3(this.transform.position.x, newY, this.transform.position.z);
    }
}

