using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro 用の名前空間

public class BulletUIManager : MonoBehaviour
{
    public PlayerController playercontroller;
    Gun gunScript;

    public Image bulletImage; // 回復アイテムの画像
    public Sprite bulletSprite; // PNG画像を割り当てる

    public TextMeshProUGUI bulletCountText; // アイテム所持数表示

    void Start()
    {
        playercontroller = FindObjectOfType<PlayerController>();
        //gunScript = playercontroller.currentGun.GetComponent<Gun>(); //プレイヤーの装備しているcurrentGunのGunスクリプトを取得

        // PlayerControllerの通知を監視するリスナーを追加
        playercontroller.OnCurrentGunChanged.AddListener(HandleGunChanged);

        if (bulletImage != null && bulletSprite != null)
        {
            bulletImage.sprite = bulletSprite;
        }
        else
        {
            Debug.LogError("Item Image or Sprite is not assigned!");
        }
    }

    void Update()
    {
        //gunScript = playercontroller.currentGun.GetComponent<Gun>(); //プレイヤーの装備しているcurrentGunのGunスクリプトを取得
    }

    private void HandleGunChanged(GameObject newGun)
    {
        if (newGun != null)
        {
            // newGunに付いているスクリプトを取得
            gunScript = newGun.GetComponent<Gun>(); // ここで対象のスクリプト名に置き換え
            if (gunScript != null)
            {
                // 必要な処理をここに記述
                UpdateBulletUI(gunScript);
            }
        }
        else
        {
            Debug.Log("銃が解除されました");
            gunScript = null;
            UpdateBulletUI(gunScript); // UIをクリア
        }
    }

    public void UpdateBulletUI(Gun gunScript)
    {
        if (gunScript != null)
        {
            // アイテム数をUIに反映
            bulletCountText.text = gunScript.bulletRemaining.ToString();

            // アイテム画像を表示/非表示
            bulletImage.enabled = gunScript.bulletRemaining > 0;
        }
        else
        {
            // 銃がない場合、UIをクリア
            bulletCountText.text = "0";
            bulletImage.enabled = false;
        }
    }
}
