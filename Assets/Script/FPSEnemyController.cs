using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSEnemyController : MonoBehaviour
{
    public float health = 100f;
    public float attackPower = 20f;
    public float attackRange = 5f; // 攻撃範囲
    public float detectionRange = 10f; // 感知範囲
    public float moveSpeed = 2.0f; // 移動速度

    public GameObject bulletPrefab; // 弾のプレハブ
    public float bulletSpeed = 20f; // 弾の速度
    public int maxMagazine = 5;
    public int currentMagazine = 0; //マガジン残弾数
    private bool isReloading = false;
    public float reloadTime = 3.0f; //リロード時間

    public float _attackSpan = 1;    //攻撃間隔
    private float _timeElapsed;   //経過時間

    public GameObject bloodEffectPrefab;
    public GameObject healthSliderPrefab;
    private GameObject healthSliderInstance;
    private Slider healthSlider;

    private Transform player;
    private Rigidbody rb;
    private Animator anim;
    private bool isAttacking = false;
    private bool isAttackAnimation = false;
    private bool canAttack = true;
    private bool isAlive = true;

    private Vector3 randomTargetPosition;
    private float randomMoveCooldown = 3f; // ランダム移動のクールタイム
    private float nextRandomMoveTime = 0f; // ランダム移動の次のタイミング

    GameObject generator1; //generator1そのものが入る変数
	TerrainGenerator2 terrainscript; //TerrainGeneraotr2Scriptが入る変数

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        // 体力スライダーのインスタンスを生成
        healthSliderInstance = Instantiate(healthSliderPrefab, transform.position + new Vector3(0, 2f, 0), Quaternion.identity, transform);
        healthSlider = healthSliderInstance.GetComponentInChildren<Slider>();
        healthSlider.value = health;

        currentMagazine = maxMagazine;

        _timeElapsed = 0;   //経過時間をリセット

        generator1 = GameObject.Find ("Generator1");
		terrainscript = generator1.GetComponent<TerrainGenerator2>(); 
    }

    void Update()
    {

        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        // プレイヤーが感知範囲内にいるか
        if (distanceToPlayer <= detectionRange)
        {
            // プレイヤーに向かって移動する
            Vector3 direction = (player.position - transform.position).normalized;

            // プレイヤーの方向を向く
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f); // スムーズに回転

            if (distanceToPlayer <= attackRange && !isAttacking && canAttack && currentMagazine > 0)
            {
                _timeElapsed += Time.deltaTime;     //時間をカウントする

                if (_timeElapsed >= _attackSpan)
                {
                    // プレイヤーに向けて射撃
                    StartAttackAnimation();
                    ShootAtPlayer();
                    _timeElapsed = 0;   //経過時間をリセットする
                }
            }
            else if (distanceToPlayer <= attackRange && currentMagazine == 0)
            {
                StartReloadAnimation();
                // 一定時間リロード
                Invoke("EnemyReload", reloadTime);
                StopReloadAnimation();
                Debug.Log(currentMagazine);
            }
        }
        else
        {
            // プレイヤーが範囲外の場合、ランダムに移動
            RandomMove();
            StopAttackAnimation();
        }

        // 体力スライダーの更新
        healthSlider.value = health;

        // HealthSliderをプレイヤーの方向に向ける
        Vector3 lookDirection = (player.position - healthSliderInstance.transform.position).normalized;
        healthSliderInstance.transform.rotation = Quaternion.LookRotation(lookDirection);

        if (health <= 0)
        {
            Die();
        }
    }

    void FixedUpdate()
    {
        // 手動で重力を強化
        rb.AddForce(Physics.gravity * rb.mass * 4f, ForceMode.Acceleration);
    }

    void RandomMove()
    {
        if (Time.time >= nextRandomMoveTime)
        {
            // ランダムなターゲット位置を設定
            randomTargetPosition = new Vector3(Random.Range(terrainscript.ad_Startwidth, terrainscript.ad_Startwidth+terrainscript.ad_width), transform.position.y, Random.Range(terrainscript.ad_Startheight, terrainscript.ad_Startheight+terrainscript.ad_height));

            // ランダム移動を開始
            nextRandomMoveTime = Time.time + randomMoveCooldown;
        }

        // ランダム移動
        Vector3 direction = (randomTargetPosition - transform.position).normalized;

        // Y軸方向を無視した平面方向のみに修正
        direction.y = 0;
        direction = direction.normalized; // 正規化し直す

        rb.velocity = direction * moveSpeed;

        // 回転処理
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    // プレイヤーに向けて弾を発射
    void ShootAtPlayer()
    {
        // プレイヤーの中心を狙う
        Vector3 playerCenter = player.position;
        if (player.TryGetComponent<Collider>(out Collider playerCollider))
        {
            playerCenter = playerCollider.bounds.center; // プレイヤーの中心座標
        }

        // 敵の発射位置を調整
        Vector3 firingPosition = transform.position;
        if (TryGetComponent<Collider>(out Collider enemyCollider))
        {
            firingPosition = enemyCollider.bounds.center; // 敵の中心から発射
        }

        // 弾の飛ぶ方向を計算
        Vector3 direction = (playerCenter - firingPosition).normalized;
        //Vector3 direction = (player.position - transform.position).normalized;
        GameObject bullet = Instantiate(bulletPrefab, transform.position + new Vector3(0, 1.5f, 0), Quaternion.LookRotation(direction));
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        bulletRb.velocity = direction * bulletSpeed;

        currentMagazine--;

        Debug.Log("Firing at player!");
    }

    /*void StartAttack()
    {
        isAttacking = true;
        anim.SetBool("IsAttacking", true);

        StartCoroutine(AttackStun());
    }*/

    IEnumerator AttackStun()
    {
        yield return new WaitForSeconds(0.5f); // 攻撃アニメーションの後硬直時間
        isAttacking = false;

        // クールタイムが終了するまで待機
        yield return new WaitForSeconds(2f); // 攻撃後の待機時間
        canAttack = true;
    }

    // 攻撃アニメーションを開始する
    void StartAttackAnimation()
    {
        isAttackAnimation = true;
        anim.SetBool("IsAttackAnimation", true); // アニメーションブーリアンを変更
        Debug.Log("Started attacking player!");
    }

    // 攻撃アニメーションを終了する
    void StopAttackAnimation()
    {
        isAttackAnimation = false;
        anim.SetBool("IsAttackAnimation", false); // 攻撃アニメーションを止める
        Debug.Log("Stopped attacking player!");
    }

    // リロードアニメーションを開始する
    void StartReloadAnimation()
    {
        isReloading = true;
        anim.SetBool("IsReloading", true); // アニメーションブーリアンを変更
        Debug.Log("Enemy is Reloading");
    }

    // リロードアニメーションを終了する
    void StopReloadAnimation()
    {
        isReloading = false;
        anim.SetBool("IsReloading", false); // リロードアニメーションを止める
        Debug.Log("Stopped Reloading");
    }


    public void EnemyReload()
    {
        currentMagazine = maxMagazine; //Maxまでリロード
    }

    public void TakeDamage(float damage)
    {
        Instantiate(bloodEffectPrefab, transform.position, Quaternion.identity);
        health -= damage;
        healthSlider.value = health;

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Enemy Died!");
        isAlive = false;
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("playerbullet"))
        {
            PlayerBullet playerbullet = other.GetComponent<PlayerBullet>();
            if (playerbullet != null)
            {
                TakeDamage(playerbullet.damage);
                Destroy(other.gameObject);
            }
        }
    }
}
