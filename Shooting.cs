using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//シューティング（銃）を制御するスクリプト
public class Shooting : MonoBehaviour
{
    public GameObject bulletPrefab;             // 弾のプレハブ
    public Transform muzzleTransform;           // 発射位置のトランスフォーム
    public float bulletSpeed = 20f;             // 弾の速度
    public float shootCooldown = 0.1f;          // 射撃のクールダウン時間
    public int maxBullets = 20;                 // 最大弾数
    private int remainingBullets;               // 残弾数
    public bool canShoot = true;                // 射撃可能フラグ
    public bool isReloading = false;            // リロード中フラグ
    public ParticleSystem muzzleFlashParticleSystem;    // マズルフラッシュのパーティクルシステム
    public ParticleSystem muzzleFlashParticleSystem1;   // マズルフラッシュのパーティクルシステム（別パーティクル）
    private float lastShotTime = 0f;            // 最後の射撃時間
    public static Shooting Instance;            // Shootingのシングルトンインスタンス
    public AudioSource gunSound;                // 射撃音
    public AudioSource reloadSound;             // リロード音
    public AudioSource emptyreloadSound;        // 空のリロード音
    public TextMeshProUGUI bulletsText;         // 弾数表示用のテキスト
    public TextMeshProUGUI gunNameText;         // 銃の名前表示用のテキスト
    private int bulletsToReload = 10;           // 一度にリロードする弾数

    private void Awake()
    {
        canShoot = true;
    }

    private void Start()
    {
        Instance = this;
        remainingBullets = maxBullets;
        UpdateBulletsText();

        gunNameText.text = "Assault Riffle";    // 銃の名前を設定
        gunNameText.gameObject.SetActive(true);
    }

    // 弾数表示を更新する
    void UpdateBulletsText()
    {
        int displayBullets = remainingBullets;
        bulletsText.text = "Bullets: " + displayBullets + "/" + maxBullets;
    }

    // 弾を追加する
    public void AddBullets(int bulletCount)
    {
        maxBullets += bulletCount;
        UpdateBulletsText();
    }

    // Update is called once per frame
    void Update()
    {
        // 左マウスボタンが押されており、射撃可能であり、射撃クールダウン後であれば
        if (Input.GetMouseButton(0) && canShoot && Time.time - lastShotTime > shootCooldown)
        {

            if (remainingBullets > 0)
            {
                Shoot();                    // 射撃する
                remainingBullets--;         // 残弾数を減らす
                UpdateBulletsText();        // 弾数表示を更新する

                lastShotTime = Time.time;   // 最後の射撃時間を更新
            }
            else
            {
                if (!isReloading)
                {
                    StartCoroutine(Reload());   // リロードを開始する
                }
            }
        }

        // Rキーが押されており、射撃不可能であればリロードする
        if (Input.GetKeyDown(KeyCode.R) && !canShoot)
        {
            StartCoroutine(Reload());
        }

    }

    // 射撃処理
    void Shoot()
    {
        if (canShoot && remainingBullets > 0)
        {
            GameObject bullet = Instantiate(bulletPrefab, muzzleTransform.position, muzzleTransform.rotation);

            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                bulletRb.velocity = bullet.transform.forward * bulletSpeed;  // 弾に速度を与える
                GameManager.Instance.UpdateRecoil();   // リコイルを更新する
                muzzleFlashParticleSystem.Play();       // マズルフラッシュを再生する
                muzzleFlashParticleSystem1.Play();      // 別のマズルフラッシュも再生する

                if (gunSound != null)
                {
                    gunSound.Play();    // 射撃音を再生する
                }
            }

            bullet.AddComponent<BulletCollisionHandler>();  // 弾に衝突処理を追加する
        }

    }

    // リロード処理
    IEnumerator Reload()
    {
        if (isReloading)//リロードしてる場合は再びリロードすることはできないことにする
        {
            yield break;
        }
        isReloading = true;
        canShoot = false;

        if (maxBullets > 0 && reloadSound != null)
        {
            reloadSound.Play(); // リロード音を再生する
        }

        if (maxBullets == 0 && emptyreloadSound != null)
        {
            emptyreloadSound.Play();    // 空のリロード音を再生する
        }

        if (remainingBullets < maxBullets)
        {
            int reloadBullets = Mathf.Min(bulletsToReload, maxBullets - remainingBullets);
            yield return new WaitForSeconds(3f);   // 3秒待つ（リロード時間）

            if (maxBullets - reloadBullets >= 0)
            {
                maxBullets -= reloadBullets;
                remainingBullets += reloadBullets;
                UpdateBulletsText();    // 弾数表示を更新する
            }
            else
            {
                remainingBullets += Mathf.Min(bulletsToReload, maxBullets);
            }
        }
        isReloading = false;
        canShoot = true;
    }

    // 弾数表示のアクティブ状態を設定する
    public void SetBulletTextActive(bool isActive)
    {
        if (bulletsText != null)
        {
            bulletsText.gameObject.SetActive(isActive);
        }
    }

}
