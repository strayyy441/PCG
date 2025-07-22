using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordHitboxController : MonoBehaviour
{
    private Collider swordCollider;

    void Start()
    {
        swordCollider = GetComponent<Collider>(); // 剣のColliderを取得
        swordCollider.enabled = false; // 初期状態では無効
    }

    // 攻撃の開始時にColliderを有効化
    public void EnableHitbox()
    {
        swordCollider.enabled = true;
        Debug.Log("Sword hitbox enabled");
    }

    // 攻撃の終了時にColliderを無効化
    public void DisableHitbox()
    {
        swordCollider.enabled = false;
        Debug.Log("Sword hitbox disabled");
    }
}

