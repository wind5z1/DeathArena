using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//弾丸の当たり判定を制御するスクリプト
public class BulletCollisionHandler : MonoBehaviour
{
    public float destroyDelay = 2f; // 弾丸が消滅するまでの遅延時間
    public int damage = 15; // 弾丸が与えるダメージ量
    private bool hasDamaged = false; // ダメージが既に適用されたかどうかのフラグ

    // コライダーに他のオブジェクトが接触したときの処理
    private void OnTriggerEnter(Collider other)
    {
        if (!hasDamaged && other.CompareTag("Enemy"))
        {
            AIDamageHandler aIDamageHandler = other.GetComponent<AIDamageHandler>();
            if (aIDamageHandler != null)
            {
                aIDamageHandler.TakeDamage(damage); // 敵にダメージを与える
                Debug.Log("Damage applied to Enemy");

                hasDamaged = true; // ダメージ適用済みフラグを立てる
            }
        }
    }

    // 初期化処理
    private void Start()
    {
        StartCoroutine(DestroyAfterDelay()); // 一定時間後に弾丸を消滅させるコルーチンを開始
    }

    // 一定時間後に弾丸を消滅させるコルーチン
    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(destroyDelay); // 遅延時間を待つ
        Destroy(gameObject); // 弾丸を消滅させる
    }
}
