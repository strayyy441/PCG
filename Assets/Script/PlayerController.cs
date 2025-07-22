using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    private bool isGameStarted = false;
    private bool isInputDisabled = false; // 入力無効化フラグ
    public float moveSpeed = 5f;        // 移動速度
    public float jumpForce = 5f;        // ジャンプ力
    public float gravityMultiplier = 2f; // 重力の増幅率
    public float lookSensitivity = 2f;   // マウスの感度
    public Camera playerCamera;          // プレイヤーのカメラ
    private float verticalRotation = 0f; // 垂直回転の値
    private CharacterController characterController;
    private Vector3 velocity;            // 現在の速度

    public float knockbackForceToPlayer = 5f; // ノックバックの力
    public float knockbackDurationToPlayer = 0.5f; // ノックバックの持続時間
    private bool isKnockedBackToPlayer = false; // ノックバック中かどうか

    public GameObject currentSword; // 装備している剣への参照
    public bool isEquipSword = false;
    public GameObject currentGun; // 装備している銃への参照

    // currentGunの変更を通知するUnityEvent
    public UnityEvent<GameObject> OnCurrentGunChanged = new UnityEvent<GameObject>();

    private Animator anim;  //Animatorをanimという変数で定義する

    //銃に関する変数
    public float aimFOV = 30f;        // エイム時のカメラ視野角
    public float normalFOV = 60f;     // 通常時のカメラ視野角
    public float aimSpeed = 10f;      // エイム時の視野角変更速度
    private bool isAiming = false;    // エイム状態を保持

    public Transform gunHolderTransform; //通常時の銃の位置
    public Transform adsPosition;  // ADS時の銃の位置
    public float transitionSpeed = 10f; // 銃の移動速度
    BulletUIManager b_uiManager;

    void Start()
    {
        //DisableInput(); // 初期状態で入力を無効化

        Cursor.lockState = CursorLockMode.Locked; // マウスカーソルを中央に固定
        characterController = GetComponent<CharacterController>();

        //変数animに、Animatorコンポーネントを設定する
        anim = gameObject.GetComponent<Animator>();
        b_uiManager = FindObjectOfType<BulletUIManager>();
    }

    void Update()
    {

        if (!isKnockedBackToPlayer && !isInputDisabled)
        {
            MovePlayer(); // 通常の移動処理
        }
        else
        {
            // ノックバック中の処理はコルーチンで管理
        }

        // ゲーム開始後, インベントリが開いていない場合のみ視点操作を有効にする
        if(!isInputDisabled)
        {
            if(InventoryManager.Instance != null && !InventoryManager.Instance.IsInventoryOpen)
            {
                RotateCamera();
            }
        }

        if (!isInputDisabled) 
        {
            HandleAttack(); // 攻撃入力を処理するメソッドを追加
            HandleAiming();
            HandleShooting();
            HandleReloading();
        }
    }

    // ゲーム開始を通知する関数を追加
    public void StartGame()
    {
        if (!isGameStarted)
        {
        isGameStarted = true;
        Time.timeScale = 1f; // 時間を進める
        EnableInput(); // 入力を有効化
        }
    }

    // 入力を無効化するメソッド
    public void DisableInput()
    {
        isInputDisabled = true;
        Cursor.lockState = CursorLockMode.None; // マウスカーソルを解放
        Cursor.visible = true;                 // マウスカーソルを表示
    }

    // 入力を有効化するメソッド
    public void EnableInput()
    {
        isInputDisabled = false;
        Cursor.lockState = CursorLockMode.Locked; // マウスカーソルを固定
        Cursor.visible = false;                  // マウスカーソルを非表示
    }

    private void MovePlayer()
    {
        // 入力を取得
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // 移動ベクトルを作成
        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        // ジャンプ処理
        if (characterController.isGrounded)
        {
            // 地面にいるときは、Y軸の速度を0に設定
            velocity.y = 0f;

            if (Input.GetButtonDown("Jump"))
            {
                velocity.y = jumpForce; // ジャンプの力を適用
            }
        }

        // 重力を適用
        velocity.y += Physics.gravity.y * gravityMultiplier * Time.deltaTime;

        // プレイヤーを移動
        characterController.Move((move * moveSpeed + velocity) * Time.deltaTime);
    }

    private void RotateCamera()
    {
        // マウスの入力を取得
        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;

        // 垂直回転の計算
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f); // 上下の回転制限

        // カメラの回転を適用
        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX); // プレイヤーの水平回転
    }

    public void EquipSword(GameObject swordPrefab)
    {
        if (currentSword != null)
        {
            Destroy(currentSword);
            isEquipSword = false;
        }

        if(currentGun != null)
        {
            Destroy(currentGun);
        }

        // プレイヤーの子オブジェクト（例：Hand）の子の子オブジェクト（例：SwordHolder）を指定
        Transform swordHolderTransform = transform.Find("root/pelvis/Weapon"); // "Hand/SwordHolder"はそのパス

        if (swordHolderTransform != null) // 指定した子オブジェクトが存在するか確認
        {
            currentSword = Instantiate(swordPrefab, swordHolderTransform); // 剣をSwordHolderの子としてインスタンス化

            // 位置を設定
            currentSword.transform.localPosition = new Vector3(0f, 0f, 0f); // 必要に応じて位置を調整

            // 回転を設定
            currentSword.transform.localRotation = Quaternion.Euler(-180f, 0f, 0f); // 必要に応じて回転を調整

            isEquipSword = true;
        }
        else
        {
            Debug.LogError("SwordHolder transform not found!"); // SwordHolderが見つからない場合のエラーメッセージ
        }
    }

    public void EquipGun(GameObject gunPrefab)
    {
        if (currentGun != null)
        {
            Destroy(currentGun); // 既に装備している剣があれば削除
        }

        if (currentSword != null)
        {
            Destroy(currentSword);
            isEquipSword = false;
        }

        // プレイヤーの子オブジェクト（例：Hand）の子の子オブジェクト（例：SwordHolder）を指定
        //gunHolderTransform = transform.Find("root/pelvis/Weapon"); // "Hand/SwordHolder"はそのパス

        if (gunHolderTransform != null) // 指定した子オブジェクトが存在するか確認
        {
            currentGun = Instantiate(gunPrefab, gunHolderTransform); // 剣をSwordHolderの子としてインスタンス化

            // 位置を設定
            //currentGun.transform.localPosition = new Vector3(-0.486f, 0.915f, -0.311f); // 必要に応じて位置を調整

            // 回転を設定
            //currentGun.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f); // 必要に応じて回転を調整
        }
        else
        {
            Debug.LogError("GunHolder transform not found!"); // SwordHolderが見つからない場合のエラーメッセージ
        }
        b_uiManager.UpdateBulletUI(currentGun.GetComponent<Gun>()); // 所持数に基づいてUIを更新
    }

    public void DeleteWeapon()
    {
        if (currentGun != null)
        {
            Destroy(currentGun); // 既に装備している剣があれば削除
        }

        if (currentSword != null)
        {
            Destroy(currentSword);
        }
    }



    private void HandleAttack()
    {
        if (currentSword != null) // 攻撃ボタンの入力を確認
        {
            // 攻撃ロジックを実装
            if (Input.GetMouseButton(0))
            {
                anim.SetBool("IsLeftClick", true);
                Debug.Log("Left Click: True");
                // アニメーションを再生したり、ダメージを与えたりする処理をここに追加
            }
            // 左クリックが離されたとき
            else
            {
                anim.SetBool("IsLeftClick", false);
                Debug.Log("Left Click: False");
            }
        }
    }

    private void HandleAiming()
    {
        if (Input.GetMouseButton(1) && currentGun != null) // 右クリックでエイム
        {
            isAiming = true;
            //anim.SetBool("IsAiming", true);
            //Debug.Log("Aim: True");
        }
        else
        {
            isAiming = false;
            //anim.SetBool("IsAiming", false);
            //Debug.Log("Aim: False");
        }

        // カメラの視野角をエイム状態に応じて変更
        float targetFOV = isAiming ? aimFOV : normalFOV;
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, Time.deltaTime * aimSpeed);
        // 照準線の表示 (任意)
        UIManager.Instance.SetCrosshairVisibility(isAiming);

        // 銃を適切な位置へ補間移動
        if (currentGun != null)
        {
            Transform targetPosition = isAiming ? adsPosition : gunHolderTransform;
            currentGun.transform.position = Vector3.Lerp(currentGun.transform.position, targetPosition.position, Time.deltaTime * transitionSpeed);
            currentGun.transform.rotation = Quaternion.Slerp(currentGun.transform.rotation, targetPosition.rotation, Time.deltaTime * transitionSpeed);
        }
    }

    private void HandleShooting()
    {
        if (Input.GetMouseButtonDown(0) && currentGun != null) // 左クリックで射撃
        {
            // currentGunからGunスクリプトを取得
            Gun gunScript = currentGun.GetComponent<Gun>();
            
            if (gunScript != null) // Gunスクリプトが存在する場合にのみShootを呼び出す
            {
                gunScript.Shoot(); // 装備している銃から弾を発射
            }
            else
            {
                Debug.LogError("Gun script not found on currentGun!");
            }
        }
    }

    private void HandleReloading()
    {
        if (Input.GetKeyDown(KeyCode.R) && currentGun != null) // 左クリックで射撃
        {
            // currentGunからGunスクリプトを取得
            Gun gunScript = currentGun.GetComponent<Gun>();
            
            if (gunScript != null) // Gunスクリプトが存在する場合にのみShootを呼び出す
            {
                gunScript.Reload(); // 装備している銃に弾を装填
            }
            else
            {
                Debug.LogError("Gun script not found on currentGun!");
            }
        }
    }

    public void ApplyKnockbackToPlayer(Vector3 sourcePosition)
    {
        if (isKnockedBackToPlayer) return; // すでにノックバック中の場合は何もしない

        // ノックバックの方向を計算
        Vector3 knockbackDirectionToPlayer = (transform.position - sourcePosition).normalized;
        knockbackDirectionToPlayer.y = 0f; // 水平方向のみノックバックする

        // ノックバック処理を開始
        StartCoroutine(HandleKnockbackToPlayer(knockbackDirectionToPlayer));
    }

    private IEnumerator HandleKnockbackToPlayer(Vector3 direction)
    {
        isKnockedBackToPlayer = true;

        float elapsedTime = 0f;
        while (elapsedTime < knockbackDurationToPlayer)
        {
            // ノックバックの移動を適用
            characterController.Move(direction * knockbackForceToPlayer * Time.deltaTime);

            elapsedTime += Time.deltaTime;
            yield return null; // 次のフレームまで待機
        }

        isKnockedBackToPlayer = false; // ノックバック終了
    }

    //イベント管理
    public GameObject CurrentGun
    {
        get { return currentGun; }
        set
        {
            if (currentGun != value)
            {
                currentGun = value;
                OnCurrentGunChanged.Invoke(currentGun); // 値が変更されたときに通知
            }
        }
    }

//設定
// 外部から感度を変更するための関数
    public void SetLookSensitivity(float newLookSensitivity)
    {
        lookSensitivity = newLookSensitivity;
    }

}
