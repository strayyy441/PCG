using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro 用の名前空間

public class HealthItemUIManager : MonoBehaviour
{
    public int itemCount = 0; // 所持アイテム数
    public int healAmount = 20; // 回復量
    public PlayerHealth playerHealth; // プレイヤーのHP管理スクリプト

    public Image itemImage; // 回復アイテムの画像
    public Sprite healingItemSprite; // PNG画像を割り当てる

    public TextMeshProUGUI itemCountText; // アイテム所持数表示

    void Start()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();
        if (itemImage != null && healingItemSprite != null)
        {
            itemImage.sprite = healingItemSprite;
            UpdateUI(); // 所持数に基づいてUIを更新
        }
        else
        {
            Debug.LogError("Item Image or Sprite is not assigned!");
        }
    }

    void Update()
    {
        Debug.Log("Item Count: " + itemCount);
        // 1ボタンで回復アイテムを使用
        if (Input.GetKeyDown(KeyCode.Q) && itemCount > 0)
        {
            UseHealingItem();
            Debug.Log("回復アイテムが使われました！");
        }
    }

    public void AddItem()
    {
        itemCount++;
        UpdateUI();
    }

    void UseHealingItem()
    {
        if (itemCount > 0)
        {
            playerHealth.Heal(healAmount);
            itemCount--;
            UpdateUI();
        }
    }

    public void UpdateUI()
    {
        // アイテム数をUIに反映
        itemCountText.text = itemCount.ToString();

        // アイテム画像を表示/非表示
        itemImage.enabled = itemCount > 0;
    }
}
