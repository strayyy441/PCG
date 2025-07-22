using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickup : MonoBehaviour
{
    public GameObject gunPrefab; // 剣のPrefabへの参照
    public float rotationSpeed = 50f; // 回転速度

    public AudioClip pickupSound; // ピックアップ時の音
    private AudioSource audioSource; // AudioSourceコンポーネント

    BulletUIManager b_uiManager;

    void Start()
    {
        audioSource = GetComponent<AudioSource>(); // AudioSourceを取得
    }

    void Update()
    {
        // Y軸を中心に回転させる
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // プレイヤーと衝突したか確認
        {
            PlayPickupSound(); // 音を鳴らす
            // プレイヤーに剣を装備させる
            PlayerController playercontroller = other.GetComponent<PlayerController>();
            b_uiManager = FindObjectOfType<BulletUIManager>();
            if (playercontroller != null)
            {
                playercontroller.EquipGun(gunPrefab);
                b_uiManager.UpdateBulletUI(playercontroller.currentGun.GetComponent<Gun>()); // 所持数に基づいてUIを更新
                Destroy(gameObject); // シーンから剣を削除
            }
        }
    }

    private void PlayPickupSound()
    {
        if (pickupSound != null) // 音が設定されているか確認
        {
            audioSource.PlayOneShot(pickupSound); // 音を再生
        }
    }
}
