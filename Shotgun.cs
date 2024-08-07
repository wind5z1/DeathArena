using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//シューティング（ショットガン）を制御するスクリプト
public class Shotgun : MonoBehaviour
{
    public Transform firePoint;                     // 発射位置
    public GameObject bulletPrefab;                 // 弾のプレハブ
    public int pelletsCount = 0;                    // ペレットの数
    public float spreadAngle = 3f;                  // 散布角度
    public float timebetweenShots = 1f;             // 射撃間隔
    public float bulletSpeed = 20f;                 // 弾の速度
    public TextMeshProUGUI bulletText;              // 弾数表示用のテキスト
    public Shooting shootingScript;                 // 射撃スクリプトへの参照
    private float elapsedTime = 0f;                 // 経過時間
    private int remainingBullets = 5;               // 残弾数
    public bool canShoot = true;                    // 射撃可能フラグ
    public bool isReloading = false;                // リロード中フラグ
    private int bulletsToReload = 5;                // 一度にリロードする弾数
    public static Shotgun Instance;                 // Shotgunのシングルトンインスタンス
    public ParticleSystem muzzleFlashParticleSystem1;   // マズルフラッシュのパーティクルシステム
    public AudioSource gunSound;                    // 射撃音
    public AudioSource reloadSound;                 // リロード音
    public AudioSource emptyreloadSound;            // 空のリロード音
    public TextMeshProUGUI gunNameText;             // 銃の名前表示用のテキスト

    private void Awake()
    {
        canShoot = true;
    }

    private void Start()
    {
        Instance = this;
        UpdateBulletText();                         // 弾数表示を更新
        shootingScript.SetBulletTextActive(false);  // 射撃スクリプトの弾数表示を非アクティブに設定
        gunNameText.text = "Shotgun";               // 銃の名前を設定
        gunNameText.gameObject.SetActive(false);   // 銃の名前表示を非アクティブに設定
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;

        // 左クリックが押され、射撃可能であり、射撃間隔を超えている場合
        if (Input.GetButtonDown("Fire1") && elapsedTime >= timebetweenShots && canShoot)
        {
            if (remainingBullets > 0)
            {
                Shoot();                            // 射撃する
                elapsedTime = 0f;                   // 経過時間をリセット
                remainingBullets--;                 // 残弾数を減らす
                UpdateBulletText();                 // 弾数表示を更新
            }
            else
            {
                if (!isReloading)
                {
                    StartCoroutine(Reload());       // リロードを開始
                }
            }
        }

        // Rキーが押され、射撃不可能であればリロードする
        if (Input.GetKeyDown(KeyCode.R) && !canShoot)
        {
            StartCoroutine(Reload());
        }

    }

    // 弾数を増やす
    public void AddBullets(int bulletCount)
    {
        pelletsCount += bulletCount;
        UpdateBulletText();
    }

    // 射撃処理
    void Shoot()
    {
        for (int i = 0; i < remainingBullets; i++)
        {
            if (canShoot)
            {
                float angle = Random.Range(-spreadAngle, spreadAngle);  // ランダムな角度を取得

                GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                bullet.transform.Rotate(Vector3.up, angle);            // 弾を散布角度で回転させる

                Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
                bulletRb.AddForce(bullet.transform.forward * 10f, ForceMode.Impulse);  // 弾に力を加える

                if (bulletRb != null)
                {
                    bulletRb.velocity = bullet.transform.forward * bulletSpeed;  // 弾に速度を与える
                    muzzleFlashParticleSystem1.Play();       // マズルフラッシュを再生する
                    GameManager.Instance.UpdateRecoil();     // リコイルを更新する

                    if (gunSound != null)
                    {
                        gunSound.Play();    // 射撃音を再生する
                    }
                }
                bullet.AddComponent<BulletCollisionHandler>();  // 弾に衝突処理を追加する
            }

        }
    }

    // 弾数表示を更新する
    void UpdateBulletText()
    {
        if (bulletText != null)
        {
            int displayBullets = remainingBullets;
            bulletText.text = "Shotgun Bullets: " + displayBullets + "/" + pelletsCount;
        }
    }

    // 弾数表示のアクティブ状態を設定する
    public void SetBulletTextActive(bool isActive)
    {
        if (bulletText != null)
        {
            bulletText.gameObject.SetActive(isActive);
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

        if (pelletsCount > 0 && reloadSound != null)
        {
            reloadSound.Play();     // リロード音を再生する
        }

        if (pelletsCount == 0 && emptyreloadSound != null)
        {
            emptyreloadSound.Play();    // 空のリロード音を再生する
        }

        if (remainingBullets < pelletsCount)
        {
            int reloadBullets = Mathf.Min(bulletsToReload, pelletsCount - remainingBullets);
            yield return new WaitForSeconds(2f);   // 2秒待つ（リロード時間）

            if (pelletsCount - reloadBullets >= 0)
            {
                pelletsCount -= reloadBullets;
                remainingBullets += reloadBullets;
                UpdateBulletText();    // 弾数表示を更新する
            }
            else
            {
                remainingBullets += Mathf.Min(bulletsToReload, pelletsCount);
            }
        }
        isReloading = false;
        canShoot = true;
    }

}
