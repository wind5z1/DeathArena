using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EvolveGames;
//プレイヤーのHPに関するスクリプト
public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100; // 最大体力
    private int currentHealth; // 現在の体力
    public int healAmountPerSecond = 5; // 秒間回復量
    public Slider healthSlider; // 体力を表示するスライダー
    public GameObject deathPanel; // 死亡時に表示するパネル

    public delegate void HealthChangedDelegate(int newHealth);
    public event HealthChangedDelegate OnHealthChanged; // 体力が変更された際に呼び出されるイベント

    public delegate void PlayerDeathDelegate();
    public event PlayerDeathDelegate OnPlayerDeath; // プレイヤーが死亡した際に呼び出されるイベント

    public PlayerController playercontrollerScript; // プレイヤーコントローラースクリプト
    public AudioSource encounterAudio; // 敵に遭遇したときの音声
    public AudioSource unencounterAudio; // 敵に遭遇しなかったときの音声
    private bool isInvincible = false; // 無敵状態かどうかを示すフラグ

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth; // 初期体力を最大体力に設定
        UpdateHealthUI(); // 体力UIを更新
        deathPanel.SetActive(false); // 死亡パネルを非表示にする

        StartCoroutine(AutoHeal()); // 自動回復のコルーチンを開始
    }

    // 自動回復のコルーチン
    private IEnumerator AutoHeal()
    {
        while (true)
        {
            currentHealth = Mathf.Min(currentHealth + healAmountPerSecond, maxHealth); // 一定間隔で体力を回復する
            UpdateHealthUI(); // 体力UIを更新
            yield return new WaitForSeconds(10f); // 10秒待つ
        }
    }

    // ダメージを受けるメソッド
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount; // ダメージを適用
        UpdateHealthUI(); // 体力UIを更新
        if (currentHealth <= 0) // 体力が0以下になったら
        {
            Die(); // 死亡処理を実行
        }
    }

    // 短期間の無敵状態を開始するメソッド
    public void StartShortinvincibility()
    {
        StartCoroutine(ShortInvincibilityTimer());
    }

    // 無敵状態かどうかを返すメソッド
    public bool IsInvincible()
    {
        return isInvincible;
    }

    // 短期間の無敵状態を制御するコルーチン
    private IEnumerator ShortInvincibilityTimer()
    {
        isInvincible = true; // 無敵状態にする
        yield return new WaitForSeconds(5f); // 5秒待つ
        isInvincible = false; // 無敵状態を解除する
    }

    // 回復するメソッド
    public void Heal(int healAmount)
    {
        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth); // 体力を回復する
        UpdateHealthUI(); // 体力UIを更新
    }

    // 死亡処理を行うメソッド
    void Die()
    {
        OnPlayerDeath?.Invoke(); // プレイヤー死亡イベントを呼び出す
        playercontrollerScript.enabled = false; // プレイヤーコントローラーを無効にする
        encounterAudio.Stop(); // 遭遇音声を停止する
        unencounterAudio.Stop(); // 非遭遇音声を停止する
        Animator animator = deathPanel.GetComponent<Animator>(); // パネルのアニメーターを取得する
        deathPanel.SetActive(true); // 死亡パネルを表示する
        Cursor.lockState = CursorLockMode.None; // カーソルのロックを解除する
        Cursor.visible = true; // カーソルを表示する

        if (animator != null)
        {
            StartCoroutine(WaitForAnimationAndPause(animator)); // アニメーションの完了を待機して一時停止する
        }
        else
        {
            Time.timeScale = 0; // タイムスケールを0にする（ゲームを停止する）
        }
    }

    // アニメーションの完了を待機して一時停止するコルーチン
    private IEnumerator WaitForAnimationAndPause(Animator animator)
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length); // アニメーションの長さだけ待機する
        Time.timeScale = 0; // タイムスケールを0にする（ゲームを停止する）
    }

    // 体力UIを更新するメソッド
    private void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = (float)currentHealth / maxHealth; // スライダーの値を現在の体力の割合に設定する
        }

        OnHealthChanged?.Invoke(currentHealth); // 体力が変更されたことを通知する
    }

    // 現在の体力を返すメソッド
    public int GetCurrentHealth()
    {
        return currentHealth; // 現在の体力を返す
    }
}
