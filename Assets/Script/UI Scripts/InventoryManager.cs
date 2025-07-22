using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance; // シングルトンインスタンス

    public GameObject inventoryUI; // インベントリUI
    private bool isInventoryOpen = false;
    public bool IsInventoryOpen => isInventoryOpen; // 外部からインベントリが開いているかどうかを確認できるように
    private PlayerController playerController;
    

    //public List<HealthItem> items = new List<HealthItem>(); // インベントリ内のアイテムリスト

    void Awake()
    {
        // シングルトンのインスタンス設定
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // 既にインスタンスがある場合は新しく作成したインスタンスを破棄
        }
        // プレイヤーの視点操作を管理しているスクリプトを取得
        playerController = FindObjectOfType<PlayerController>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        inventoryUI.SetActive(isInventoryOpen);
        Time.timeScale = isInventoryOpen ? 0 : 1; // インベントリ開閉時に時間を止める/進める

        // インベントリ開いている間、視点操作を無効化
        if (playerController != null)
        {
            playerController.enabled = !isInventoryOpen;
        }

        // インベントリが開いている場合はカーソルを表示、閉じる場合は非表示に
        if (isInventoryOpen)
        {
            Cursor.lockState = CursorLockMode.None; // カーソルを自由に動かせるように
            Cursor.visible = true; // カーソルを表示
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked; // カーソルをロックして隠す
            Cursor.visible = false; // カーソルを非表示
        }
    }

    /*public void AddItem(HealthItem item)
    {
        items.Add(item);
        // インベントリUIに反映する処理をここに追加
    }

    public void UseItem(HealthItem item)
    {
        items.Remove(item);
        PlayerHealth.Instance.Heal(item.healAmount); // プレイヤーを回復
        // インベントリUIからアイテムを削除する処理をここに追加
    }*/
}


