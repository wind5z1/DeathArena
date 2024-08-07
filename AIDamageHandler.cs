using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
//AIのダメージを受けるスクリプト
public class AIDamageHandler : MonoBehaviour
{
    public int maxHealth = 100; // 最大体力
    public int currentHealth; // 現在の体力
    private Animator aiAnimator; // AIキャラクターのアニメーター
    private NavMeshAgent agent; // ナビメッシュエージェント
    public AIAttack aIAttack; // 攻撃スクリプトの参照
    public bool isDead = false; // キャラクターが死んでいるかどうか
    public event Action OnDeath; // 死亡イベント
    public AudioClip deathAudioClip; // 死亡時のオーディオクリップ
    private AudioSource audioSource; // オーディオソース

    // Startメソッドは初期化のために呼び出される
    void Start()
    {
        currentHealth = maxHealth; // 現在の体力を最大体力に設定
        aiAnimator = GetComponent<Animator>(); // アニメーターを取得
        agent = GetComponent<NavMeshAgent>(); // ナビメッシュエージェントを取得

        audioSource = gameObject.AddComponent<AudioSource>(); // オーディオソースを追加
        audioSource.clip = deathAudioClip; // オーディオクリップを設定
    }

    // ダメージを受けたときに呼び出される
    public void TakeDamage(int damage)
    {
        if (currentHealth > 0)
        {
            currentHealth -= damage; // 体力を減少させる
        }
        if (currentHealth <= 0 && !isDead)
        {
            Die(); // 体力が0以下で、まだ死んでいない場合に死ぬ
        }
    }

    // キャラクターが死ぬときに呼び出される
    public void Die()
    {
        isDead = true; // 死亡フラグを立てる
        agent.isStopped = true; // エージェントを停止
        aIAttack.canAttack = false; // 攻撃を停止
        aiAnimator.SetTrigger("Death"); // 死亡アニメーションを再生
        if (deathAudioClip != null)
        {
            audioSource.PlayOneShot(deathAudioClip); // 死亡音を再生
        }
        StartCoroutine(DestroyAfterAnimation()); // アニメーション後にオブジェクトを破壊するコルーチンを開始
        OnDeath?.Invoke(); // 死亡イベントを発火
    }

    // 死亡アニメーション後にオブジェクトを破壊するコルーチン
    IEnumerator DestroyAfterAnimation()
    {
        yield return new WaitForSeconds(2f); // 2秒待機
        Destroy(gameObject); // オブジェクトを破壊
    }
}
