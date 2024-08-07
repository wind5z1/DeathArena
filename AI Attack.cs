using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//AIの攻撃の機能を制御するスクリプト
public class AIAttack : MonoBehaviour
{
    public float attackRange = 2f; // 攻撃範囲
    public float attackCooldown = 1.5f; // 攻撃のクールダウン時間
    public int attackDamage = 10; // 攻撃ダメージ
    public float moveAnimationThreshold = 0.1f; // 移動アニメーションの閾値
    public bool canAttack = true; // 攻撃可能かどうか
    private PlayerHealth playerHealth; // プレイヤーのヘルス
    private Animator zombieAnimator; // ゾンビのアニメーター
    private NavMeshAgent agent; // ナビメッシュエージェント
    public AudioClip attackAudioClip; // 攻撃のオーディオクリップ
    private AudioSource audioSource; // オーディオソース

    // Startメソッドは初期化のために呼び出される
    void Start()
    {
        playerHealth = GameObject.FindWithTag("Player").GetComponent<PlayerHealth>(); // プレイヤーのヘルスを取得
        zombieAnimator = GetComponent<Animator>(); // アニメーターを取得
        agent = GetComponent<NavMeshAgent>(); // ナビメッシュエージェントを取得

        audioSource = gameObject.AddComponent<AudioSource>(); // オーディオソースを追加
        audioSource.clip = attackAudioClip; // オーディオクリップを設定
    }

    // Updateメソッドは毎フレーム呼び出される
    void Update()
    {
        if (CanAttackPlayer()) // プレイヤーを攻撃可能かどうかチェック
        {
            StartCoroutine(AttackPlayer()); // プレイヤーを攻撃するコルーチンを開始
        }
    }

    // プレイヤーを攻撃可能かどうかを判断する
    bool CanAttackPlayer()
    {
        if (playerHealth != null)
        {
            if (!playerHealth.IsInvincible()) // プレイヤーが無敵状態でないことをチェック
            {
                float distanceToPlayer = Vector3.Distance(transform.position, playerHealth.transform.position); // プレイヤーとの距離を計算
                return distanceToPlayer <= attackRange && canAttack; // 攻撃範囲内で攻撃可能かどうかを返す
            }
        }
        return false;
    }

    // プレイヤーを攻撃するコルーチン
    IEnumerator AttackPlayer()
    {
        canAttack = false; // 攻撃を一時停止
        agent.isStopped = true; // エージェントを停止
        zombieAnimator.SetTrigger("Attack1"); // 攻撃アニメーションを再生
        zombieAnimator.SetBool("isAttacking", true); // 攻撃中フラグを設定
        zombieAnimator.SetBool("isRunning", false); // 走るアニメーションを停止
        if (attackAudioClip != null)
        {
            audioSource.PlayOneShot(attackAudioClip); // 攻撃音を再生
        }
        yield return new WaitForSeconds(0.5f); // 0.5秒待機
        playerHealth.TakeDamage(attackDamage); // プレイヤーにダメージを与える

        zombieAnimator.SetTrigger("Attack1"); // 再度攻撃アニメーションを再生
        yield return new WaitForSeconds(0.5f); // 0.5秒待機
        playerHealth.TakeDamage(attackDamage); // 再度プレイヤーにダメージを与える
        StartCoroutine(AttackCooldown()); // 攻撃のクールダウンを開始
    }

    // 攻撃のクールダウンコルーチン
    IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown); // クールダウン時間待機
        zombieAnimator.ResetTrigger("Attack1"); // 攻撃トリガーをリセット
        zombieAnimator.SetBool("isAttacking", false); // 攻撃中フラグを解除
        zombieAnimator.SetBool("isRunning", true); // 走るアニメーションを再生
        agent.isStopped = false; // エージェントを再開
        canAttack = true; // 攻撃可能に設定
    }
}
