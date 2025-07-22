using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject bulletPrefab;  // 発射する弾のPrefab
    private Transform firePoint;      // 弾の発射位置
    public float bulletSpeed = 20f;  // 弾の速度
    public int maxMagazine = 25;
    public int currentMagazine = 0; //マガジン残弾数
    public int bulletRemaining = 100; //総残弾数

    BulletUIManager b_uiManager;

    GameObject FPSReloadText;
    TextFadeController fpsReload_textfadecontroller;

    void Start()
    {
        firePoint = transform.Find("muzzle");
        currentMagazine = maxMagazine;
        b_uiManager = FindObjectOfType<BulletUIManager>();

        //リロード促しテキスト
        FPSReloadText = GameObject.Find("FPSReloadText");
        fpsReload_textfadecontroller = FPSReloadText.GetComponent<TextFadeController>();
    }

    public void Shoot()
    {
        if(currentMagazine != 0)
        {
            // 弾の生成と発射
            if (bulletPrefab != null && firePoint != null)
            {
                GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
                if (bulletRb != null)
                {
                    bulletRb.velocity = firePoint.forward * bulletSpeed;
                }
                
                currentMagazine--; //残弾数減らす
            }
            else
            {
                Debug.LogWarning("弾PrefabまたはfirePointが設定されていません");
            }
        }
        else
        {
            fpsReload_textfadecontroller.ShowMessage("弾薬切れで撃てません！\nRボタンを押して残弾から補充");
        }
    }

    public void Reload()
    {
        int reloadamount = maxMagazine - currentMagazine;

        if(currentMagazine < maxMagazine && bulletRemaining > 0) //マガジン最高数より現残弾数が少なく, かつ総残弾数が０でないとき
        {
            if(bulletRemaining >= reloadamount) //総残弾数がリロード量に足りている場合
            {
                currentMagazine += reloadamount; //Maxまでリロード
                bulletRemaining -= reloadamount; //リロード量を総残弾数からマイナス
                b_uiManager.UpdateBulletUI(this);
                Debug.Log("マガジン内弾数:" + currentMagazine + " 総残弾数:" + bulletRemaining);
            }
            else
            {
                currentMagazine += bulletRemaining; //足りない場合総残弾を装填
                bulletRemaining = 0; //総残弾数を0に
                b_uiManager.UpdateBulletUI(this);
                Debug.Log("マガジン内弾数:" + currentMagazine + " 総残弾数:" + bulletRemaining);
            }

        }
        else
        {
            Debug.Log("リロードできません");
        }
    }
}

