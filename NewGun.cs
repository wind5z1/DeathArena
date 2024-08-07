using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EvolveGames;
using TMPro;
//エアドロップからのショットガンを拾ったらに関するスクリプト
public class NewGun : MonoBehaviour
{
    public float rotationSpeed = 5f; // 回転速度
    private bool isPickedUp = false; // アイテムが回収されたかどうかのフラグ
    public GameObject gun1; // ガン1
    public GameObject gun2; // ショットガン
    public GameObject itemDisplay; // アイテム表示
    private ItemChange itemChangeScript; // アイテム変更スクリプト
    private bool isCollided = false; // 衝突したかどうかのフラグ
    public GameObject airdropPrefab; // エアドロッププレハブ
    public Shotgun shotgunScript; // ショットガンスクリプト
    private GameObject currentAirDrop; // 現在のエアドロップ
    public TextMeshProUGUI pickedUpShotgunText; // ショットガンを拾ったテキスト

    private void Start()
    {
        // エアドロップの固定位置を設定して生成する
        Vector3 fixedPosition = new Vector3(-4.368393f, 1.5f, 10.21f);
        Instantiate(airdropPrefab, fixedPosition, Quaternion.identity);

        // 一定間隔でエアドロップを生成する
        InvokeRepeating("SpawnAirdrop", 100f, 100f);
    }

    // エアドロップを生成するメソッド
    void SpawnAirdrop()
    {
        if (currentAirDrop != null)
        {
            Destroy(currentAirDrop);
        }

        Vector3 fixedPosition = new Vector3(-4.368393f, 3.832181f, 10.21f);
        currentAirDrop = Instantiate(airdropPrefab, fixedPosition, Quaternion.identity);
    }

    // Update is called once per frame
    private void Update()
    {
        if (isCollided)
        {
            TryPickUpGun(); // ショットガンを拾う処理を試みる
        }

        // アイテムを回転させる
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    // 衝突時の処理
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Collided"); 
            isCollided = true; // 衝突フラグをtrueに設定
            shotgunScript.AddBullets(5); // ショットガンに弾薬を追加する

            pickedUpShotgunText.text = "Picked Up Shotgun"; // ショットガンを拾ったテキストを表示
            StartCoroutine(ClearMessageAfterDelay()); // 一定時間後にメッセージを消す
        }
    }

    // 一定時間後にメッセージを消すコルーチン
    IEnumerator ClearMessageAfterDelay()
    {
        yield return new WaitForSeconds(1.5f); // 1.5秒待つ
        pickedUpShotgunText.text = " "; // テキストを消す
    }

    // ショットガンガンを拾う処理
    public void TryPickUpGun()
    {
        if (!isPickedUp) // まだショットガンガンが拾われていない場合
        {
            // メッシュレンダラーを無効にする
            MeshRenderer meshRendedrer = GetComponent<MeshRenderer>();
            if (meshRendedrer != null)
            {
                meshRendedrer.enabled = false;
            }

            // ボックスコライダーを無効にする
            BoxCollider boxCollider = GetComponent<BoxCollider>();
            if (boxCollider != null)
            {
                boxCollider.enabled = false;
            }

            // 拾ったらガン1を無効にし、ガン2（ショットガン）を有効にする
            gun1.SetActive(false);
            gun2.SetActive(true);

            // アイテム変更スクリプトを取得して、ガンの初期化を行う
            itemChangeScript = FindObjectOfType<ItemChange>();
            if (itemChangeScript != null)
            {
                itemChangeScript.InitializeGuns(gun1, gun2);
            }

            isPickedUp = true; // エアドロップのショットガンが拾われたフラグをtrueに設定
        }
    }
}
