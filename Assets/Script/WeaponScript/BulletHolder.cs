using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHolder : MonoBehaviour
{
    public int bullet = 25;

    PlayerController playercontroller;
    Gun gunScript;
    BulletUIManager b_uiManager;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playercontroller = other.GetComponent<PlayerController>();
            gunScript = playercontroller.currentGun.GetComponent<Gun>();
            b_uiManager = FindObjectOfType<BulletUIManager>();
            if(gunScript != null)
            {
                gunScript.bulletRemaining += bullet;
                b_uiManager.UpdateBulletUI(playercontroller.currentGun.GetComponent<Gun>()); // 所持数に基づいてUIを更新
                Debug.Log("総残弾数" + gunScript.bulletRemaining);
                Destroy(gameObject); // アイテムオブジェクトを削除
            }
        }
    }
}
