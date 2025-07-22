using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // プレイヤーと衝突したか確認
        {
            // 衝突した相手のゲームオブジェクトから bullet スクリプトを取得
            PlayerHealth playerhealth = other.GetComponent<PlayerHealth>();
            if (playerhealth != null)
            {
                playerhealth.TakeDamage(damage);
                playerhealth.UpdatePlayerHealth();
                Destroy(this.gameObject); // 衝突後に弾を消す
                Debug.Log(damage + "ダメージ受けました");
            }
        }
        
        if (other.CompareTag("Ground")) // 地面と衝突したか確認
        {
                Destroy(this.gameObject); // 衝突後に弾を消す
        }
    }
}
