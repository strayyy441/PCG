using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance; // シングルトンインスタンス
    //public Transform playerTransform; //プレイヤー自身の位置

    public float maxHealth = 100f; // プレイヤーの最大体力
    public float currentHealth;
    public int deathCount = 0; //死んだ回数記録用　難易度調整に使う

    public Slider playerHealthSlider; // 感度を調整するスライダー

    private bool isInvincible = false; //無敵時間中かどうか
    public float invincibleDuration = 1f; //無敵時間

    GameObject TerrainGenerator2;
    TerrainGenerator2 terrainscript;

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

    }

    void Start()
    {
        currentHealth = maxHealth; // ゲーム開始時に体力を最大に設定

        TerrainGenerator2 = GameObject.Find ("Generator1"); //Unityちゃんをオブジェクトの名前から取得して変数に格納する
		terrainscript = TerrainGenerator2.GetComponent<TerrainGenerator2>(); //unitychanの中にあるUnityChanScriptを取得して変数に格納する

        //playerTransform = GameObject.FindGameObjectWithTag("Player").transform; //プレイヤーオブジェクトを参照
        //Debug.Log(playerTransform.position);

        // HPバーの初期設定
        if (playerHealthSlider != null)
        {
            playerHealthSlider.maxValue = maxHealth;
            playerHealthSlider.value = currentHealth;
        }
    }

    public void UpdatePlayerHealth()
    {
        if (playerHealthSlider != null)
        {
            playerHealthSlider.value = currentHealth; // 現在のHPをスライダーに反映
        }
    }

    // ダメージを受ける処理
    public void TakeDamage(float damage)
    {
        if (isInvincible) return; // 無敵中はダメージを無視

        currentHealth -= damage;
        Debug.Log("Player took damage: " + damage + " Current health: " + currentHealth);

        // HPバーを更新
        UpdatePlayerHealth();

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvincibilityCoroutine());
        }
    }

    // プレイヤーが死亡した時の処理
    void Die()
    {
        CharacterController controller = GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = false; // 一時的に無効化
            terrainscript.ResetPlayerState();
            controller.enabled = true;  // 再度有効化
        }
        else
        {
            terrainscript.ResetPlayerState();
        }

        Debug.Log("Player Died!");
        //gameObject.SetActive(false);

        //実験用に死んだら体力MAXまで回復
        currentHealth = maxHealth;
        UpdatePlayerHealth();
        deathCount++;

        // 無敵状態を付与
        StartCoroutine(InvincibilityCoroutine());
    }

    // プレイヤーが回復する処理
    public void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        Debug.Log("Player healed: " + amount + " Current health: " + currentHealth);

        // HPバーを更新
        UpdatePlayerHealth();
    }

    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleDuration);
        isInvincible = false;
    }
}


