using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//プレイヤーがプラットフォームを作るために使うブロックに関するスクリプト
public class Placeblock : MonoBehaviour
{
    public GameObject blockPrefab; // プレハブとして使用するブロック
    public Camera playerCamera; // プレイヤーカメラ
    public AISpawner aISpawner; // AIスポーナー
    public float destructionDelay = 5f; // ブロックの破壊遅延時間
    public TextMeshProUGUI standCountText; // スタンドカウントのテキスト表示

    private void Start()
    {
        UpdateStandCountText(); // 初期化時にスタンドカウントを更新
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) // Pキーが押されたら
        {
            if (aISpawner.allowedPrefabCount > 0) // 許可されたプレハブ数が0より大きい場合
            {
                PlacePrefab(); // プレハブを配置する
                UpdateStandCountText(); // スタンドカウントを更新
            }
        }
    }

    // プレハブを配置するメソッド
    void PlacePrefab()
    {
        // プレイヤーカメラの画面中央からRayを発射する
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit)) // Rayがオブジェクトに当たった場合
        {
            // 当たった位置に新しいブロックをインスタンス化する
            GameObject newBlock = Instantiate(blockPrefab, hit.point, Quaternion.identity);
            aISpawner.allowedPrefabCount--; // 許可されたプレハブ数を減らす
            UpdateStandCountText(); // スタンドカウントを更新
            StartCoroutine(DestroyAfterDelay(newBlock)); // 一定時間後にブロックを破壊する
        }
    }

    // 一定時間後にオブジェクトを破壊するコルーチン
    IEnumerator DestroyAfterDelay(GameObject prefabInstance)
    {
        yield return new WaitForSeconds(destructionDelay); // destructionDelay秒待つ
        Destroy(prefabInstance); // オブジェクトを破壊する
    }

    // スタンドカウントのテキスト表示を更新するメソッド
    public void UpdateStandCountText()
    {
        standCountText.text = "Stand count: " + aISpawner.allowedPrefabCount; // ブロックのテキストを更新する
    }
}
