using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordController : MonoBehaviour
{
    public float damage = 25f; // 剣のダメージ量

    void OnTriggerEnter(Collider other)
    {
        // 敵と衝突したか確認
        Debug.Log("Enemy detected by sword!"); // デバッグ用メッセージ
        if (other.CompareTag("Enemy"))
        {
            EnemyController enemy = other.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage); // 敵にダメージを与える
                Vector3 attackDirection = (enemy.transform.position - transform.position).normalized;
                enemy.ApplyKnockback(attackDirection); //ノックバック処理
                Debug.Log("Enemy hit! Damage dealt: " + damage);
            }
            else
            {
                Debug.LogError("EnemyController component not found on the enemy.");
            }   
        }
    }
}


