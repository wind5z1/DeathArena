using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using EvolveGames;
using TMPro;
//AIのスポーン機能を制御するスクリプト
public class AISpawner : MonoBehaviour
{
    public GameObject aiPrefab; // スポーンするAIのプレハブ
    public PlayerTeleporter playerTeleporter; // プレイヤーのテレポートスクリプト
    public float spawnInterval; // スポーン間隔
    public int[] spawnCountsPerWave = { 10, 20, 30 }; // 各ウェーブのスポーン数
    public int maxWaves = 3; // 最大ウェーブ数
    public int currentWave = 0; // 現在のウェーブ
    public int enemyCount = 0; // 現在の敵の数
    public GameObject panel; // ゲームクリア時に表示するパネル
    public TextMeshProUGUI teleportText; // テレポートのカウントダウンテキスト
    public TextMeshProUGUI teleportoutsideText; // テレポート後のメッセージテキスト
    public AudioSource encounterAudio; // エンカウント時のオーディオソース
    public AudioSource notencounterAudio; // 非エンカウント時のオーディオソース
    public int allowedPrefabCount = 0; // 許可されたプレハブ数
    public Placeblock placeblockScript; // ブロック配置スクリプト
    public PlayerHealth playerHealth; // プレイヤーのヘルススクリプト

    // Startメソッドは初期化のために呼び出される
    void Start()
    {
        StartCoroutine(SpawnAICoroutine());
    }

    // AIをスポーンするコルーチン
    IEnumerator SpawnAICoroutine()
    {
        while (currentWave <= spawnCountsPerWave[currentWave])//今のウェーブがまだウェーブごとに設定された敵の数になってない場合
        {
            for (int i = 0; i < spawnCountsPerWave[currentWave]; i++)//ウェーブに設定された敵の数を生成する
            {
                yield return StartCoroutine(SpawnWaveCoroutine(spawnCountsPerWave[currentWave]));
                yield return StartCoroutine(CountdownCoroutine(3));
                encounterAudio.Play();
                notencounterAudio.Stop();
                playerTeleporter.TeleporttoTarget(new Vector3(-4.368393f, 3.837281f, 10.12f));//ウェーブのエネミーが完成に生成するたびにプレイヤーを特定の座標にテレポートする
                currentWave++;
                playerHealth.StartShortinvincibility();//プレイヤーが指定の座標にテレポートされたら短時間の無敵を与える
                allowedPrefabCount = currentWave + 1;//今のウェーブに応じて使えるブロックを一つに増える
                placeblockScript.UpdateStandCountText();
                yield return new WaitUntil(() => enemyCount == 0);

                if (currentWave < maxWaves) //ウェーブ完了した
                {
                    encounterAudio.Stop();
                    notencounterAudio.Play();
                    playerTeleporter.TeleporttoTarget(new Vector3(-20.34f, 3.837281f, 45.93f));//ウェーブ完了したら特定の座標にテレポートする
                    teleportoutsideText.text = "Wave " + currentWave + " Cleared. Be prepared for the next wave.";
                    yield return new WaitForSeconds(3f);
                    teleportoutsideText.text = "";
                }

                if (currentWave == maxWaves)//最大ウェーブが今のウェーブと同じだったら
                {
                    Animator animator = panel.GetComponent<Animator>();
                    encounterAudio.Stop();
                    notencounterAudio.Stop();
                    panel.SetActive(true);
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;

                    if (animator != null)
                    {
                        animator.SetBool("GameClearPanel", true);//勝ったパネルを呼び出す
                        StartCoroutine(WaitForAnimationAndPause(animator));
                    }
                    else
                    {
                        Time.timeScale = 0;
                    }
                }
            }
        }
    }

    // カウントダウンコルーチン
    IEnumerator CountdownCoroutine(int seconds)
    {
        for (int i = seconds; i > 0; i--)
        {
            teleportText.text = "Teleport in " + i;
            yield return new WaitForSeconds(1f);
        }
        teleportText.text = "";
    }

    // アニメーションの完了を待って一時停止するコルーチン
    IEnumerator WaitForAnimationAndPause(Animator animator)
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        Time.timeScale = 0;
    }

    // ウェーブごとにAIをスポーンするコルーチン
    IEnumerator SpawnWaveCoroutine(int spawnCount)
    {
        int spawnedCount = 0;

        while (spawnedCount < spawnCount)
        {
            SpawnAI();
            spawnedCount++;
            float elapsedTime = 0f;
            while (elapsedTime < spawnInterval)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    }

    // AIをスポーンするメソッド
    void SpawnAI()
    {
        if (aiPrefab != null)
        {
            Vector3 spawnPosition = transform.position + UnityEngine.Random.insideUnitSphere * 3f;
            GameObject newAI = Instantiate(aiPrefab, spawnPosition, Quaternion.identity);
            newAI.GetComponent<AIDamageHandler>().OnDeath += OnAIDeath;
            enemyCount++;
        }
    }

    // AIが死亡したときに呼ばれるメソッド
    void OnAIDeath()
    {
        enemyCount--;
    }
}
