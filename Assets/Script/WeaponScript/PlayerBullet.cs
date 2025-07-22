using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float damage = 10;

    private void OnTriggerEnter(Collider other)
    {    
        if (other.CompareTag("Ground")) // 地面と衝突したか確認
        {
                Destroy(this.gameObject); // 衝突後に弾を消す
        //
        }
    }
}
