using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Sliderを使うための名前空間

public class EnemyController : MonoBehaviour
{
    public float health = 100f; // エネミーの体力
    public float attackPower = 20f;
    public float attackRange = 2f; // 攻撃範囲
    public float attackCooldown = 2f; // 攻撃クールタイム
    public float attackStunDuration = 0.5f; // 攻撃後の硬直時間
    public float detectionRange = 10f; // 感知範囲
    public float moveSpeed = 2.0f; // 移動速度

    public GameObject bloodEffectPrefab; // 血エフェクトのプレハブ

    public float knockbackForce = 500f; //ノックバックの強さ
    public float knockbackDuration = 1.0f; //ノックバックの時間
    private bool isKnockedBack = false; //ノックバックしているかどうか
    private float knockbackEndTime = 0f; //ノックバックの終了時間

    private Transform player;
    private Rigidbody rb;
    private Animator anim;
    private bool isAttacking = false;
    private bool canAttack = true; // 攻撃可能かどうか
    public bool isAlive = true;

    public GameObject healthSliderPrefab; // 体力スライダーのプレハブ
    private GameObject healthSliderInstance; // 体力スライダーのインスタンス
    private Slider healthSlider; // スライダーコンポーネント

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // プレイヤーを見つける
        rb = GetComponent<Rigidbody>(); // Rigidbodyを取得
        anim = GetComponent<Animator>(); // アニメーターを取得

        // 体力スライダーのインスタンスを生成
        healthSliderInstance = Instantiate(healthSliderPrefab, transform.position + new Vector3(0, 2f, 0), Quaternion.identity, transform);
        healthSlider = healthSliderInstance.GetComponentInChildren<Slider>(); // 子のSliderコンポーネントを取得
        healthSlider.value = health; // 初期体力を設定
    }

    void Update()
    {
        
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        // プレイヤーが感知範囲内にいるか確認
        if (distanceToPlayer <= detectionRange)
        {
            if (!isKnockedBack) //ノックバックしていないなら
            {
                // プレイヤーに向かって移動する
                Vector3 direction = (player.position - transform.position).normalized;
                //rb.MovePosition(transform.position + direction * moveSpeed * Time.deltaTime);
                // 移動速度を設定
                rb.velocity = direction * moveSpeed;

                // プレイヤーの方向を向く
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f); // スムーズに回転

                // 攻撃範囲に入ったらアニメーションを切り替える
                if (distanceToPlayer <= attackRange && !isAttacking && canAttack)
                {
                    rb.velocity = Vector3.zero; //移動停止
                    StartAttack();
                }
                else if (distanceToPlayer > attackRange && isAttacking)
                {
                    StopAttack();
                }
            }
            else if (Time.time >= knockbackEndTime)
            {
                isKnockedBack = false; // ノックバック終了
            }
        }
        else
        {
            rb.velocity = Vector3.zero; // 感知範囲外では停止
        }

    // 体力スライダーの値を更新
    healthSlider.value = health; // 敵の体力に応じてスライダーの値を更新

    // HealthSliderをプレイヤーの方向に向ける
    Vector3 lookDirection = (player.position - healthSliderInstance.transform.position).normalized;
    healthSliderInstance.transform.rotation = Quaternion.LookRotation(lookDirection);

        if(health <= 0)
        {
            Die();
        }
    }

    void FixedUpdate()
    {
        // 手動で重力を強化
        rb.AddForce(Physics.gravity * rb.mass * 4f, ForceMode.Acceleration);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("bullet")) // プレイヤーと衝突したか確認
        {
            // 衝突した相手のゲームオブジェクトから PlayerHealth スクリプトを取得
            Bullet bullet = other.GetComponent<Bullet>();
            if (bullet != null)
            {
                TakeDamage(bullet.damage);
                Destroy(other.gameObject); // 衝突後に弾を消す
                Debug.Log("Take damage");
            }
        }

        //敵の攻撃がプレイヤーに当たったとき
        if (other.CompareTag("Player")) // プレイヤーと衝突したか確認
        {
            // 衝突した相手のゲームオブジェクトから PlayerController スクリプトを取得
            PlayerController playercontroller = other.GetComponent<PlayerController>();
            // 衝突した相手のゲームオブジェクトから PlayerHealth スクリプトを取得
            PlayerHealth playerhealth  = other.GetComponent<PlayerHealth>();

            if (playerhealth != null)
            {
                playerhealth.TakeDamage(this.attackPower);
            }

            if (playercontroller != null)
            {
                playercontroller.ApplyKnockbackToPlayer(transform.position);
            }
        }
    }

    // 攻撃アニメーションを開始する
    void StartAttack()
    {
        isAttacking = true;
        anim.SetBool("IsAttacking", true); // アニメーションブーリアンを変更
        Debug.Log("Started attacking player!");

        // 攻撃後に硬直
        StartCoroutine(AttackStun());
    }

    //攻撃後硬直
    IEnumerator AttackStun()
    {
        yield return new WaitForSeconds(attackStunDuration);
        isAttacking = false;

        // クールタイムが終了するまで待機
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true; // 攻撃可能になる
    }

    // 攻撃アニメーションを終了する
    void StopAttack()
    {
        isAttacking = false;
        anim.SetBool("IsAttacking", false); // 攻撃アニメーションを止める
        Debug.Log("Stopped attacking player!");
    }

    // エネミーがダメージを受けた時の処理
    public void TakeDamage(float damage)
    {
        //血のエフェクト
        Instantiate(bloodEffectPrefab, this.transform.position, Quaternion.identity);

        health -= damage;
        Debug.Log("Damage taken: " + damage + ", Remaining Health: " + health); // デバッグ用メッセージ

        // 体力バーを更新
        if (healthSlider != null)
        {
            healthSlider.value = health;
        }

        if (health <= 0)
        {
            Die();
        }
    }

    //エネミーが攻撃を受けた際のノックバック
    public void ApplyKnockback(Vector3 attackDirection)
    {
        isKnockedBack = true;
        knockbackEndTime = Time.time + knockbackDuration;

        // 高さ方向の動きを無視
        attackDirection.y = 0;
        attackDirection.Normalize();

        // ノックバック方向に力を加える
        rb.velocity = Vector3.zero; // 現在の速度をリセット
        rb.AddForce(attackDirection * knockbackForce, ForceMode.VelocityChange);
        Debug.Log("Addforceが呼ばれました");
    }


    // エネミーが倒された時の処理
    void Die()
    {
        Debug.Log("Enemy Died!");
        isAlive = false;
        Destroy(gameObject); // エネミーを削除する
    }
}


