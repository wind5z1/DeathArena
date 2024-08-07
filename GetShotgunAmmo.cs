using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//ショットガンタイプの銃を制御するスクリプト
public class GetShotgunAmmo : MonoBehaviour
{
    // プレイヤーと衝突したときに呼ばれる
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject player = GameObject.FindWithTag("Player"); // プレイヤーオブジェクトを見つける
            Shotgun shotgunscript = FindShotgunScript(player); // プレイヤーのショットガンスクリプトを取得
            StartCoroutine(Getshotgunammo(shotgunscript)); // ショットガンの弾薬を取得するコルーチンを開始
        }
    }

    // プレイヤーの子オブジェクトからショットガンスクリプトを見つける
    Shotgun FindShotgunScript(GameObject player)
    {
        Shotgun shotgunScript = player.GetComponentInChildren<Shotgun>();
        return shotgunScript;
    }

    // ショットガンの弾薬を取得するコルーチン
    IEnumerator Getshotgunammo(Shotgun shotgunscript)
    {
        yield return new WaitForSeconds(0.1f); // 0.1秒待つ
        shotgunscript.AddBullets(5); // ショットガンに5発の弾薬を追加
        Destroy(gameObject); // 弾薬オブジェクトを破壊
    }
}
