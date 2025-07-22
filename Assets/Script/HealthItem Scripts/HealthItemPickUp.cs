using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro 用の名前空間

public class HealthItemPickup : MonoBehaviour
{
    //public HealthItem item; // 拾うアイテム
    public HealthItemUIManager uiManager; // UI管理スクリプト


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //InventoryManager.Instance.AddItem(item); // アイテムをインベントリに追加
            // アイテムを拾ったらインベントリUIを表示
            //InventoryManager.Instance.inventoryUI.SetActive(true);
            // インベントリを開く
            //InventoryManager.Instance.ToggleInventory(); // インベントリを開く処理を追加
            if (uiManager != null)
            {
                uiManager.AddItem();
            }
            Destroy(gameObject); // アイテムオブジェクトを削除
        }
    }
}
