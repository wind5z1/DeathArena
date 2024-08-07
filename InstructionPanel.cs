using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EvolveGames;
//最初のインストラクションパネルを制御するスクリプト
public class InstructionPanel : MonoBehaviour
{
    public GameObject instructionPanel; // インストラクションパネルのゲームオブジェクト
    public PlayerController playerController; // プレイヤーコントローラー
    public AISpawner aISpawner; // AIスポーナー
    public Shotgun shotgun; // ショットガン
    public Shooting shooting; // シューティング

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(true); // ゲームオブジェクトをアクティブにする
        playerController.enabled = false; // プレイヤーコントローラーを無効にする
        aISpawner.enabled = false; // AIスポーナーを無効にする
        shotgun.enabled = false; // ショットガンを無効にする
        shooting.enabled = false; // シューティングを無効にする
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B)) // Bキーが押されたら
        {
            CloseInstructionPanel(); // インストラクションパネルを閉じる
        }
    }

    // インストラクションパネルを閉じるメソッド
    void CloseInstructionPanel()
    {
        gameObject.SetActive(false); // インストラクションパネルを非アクティブにする
        playerController.enabled = true; // プレイヤーコントローラーを有効にする
        aISpawner.enabled = true; // AIスポーナーを有効にする
        shotgun.enabled = true; // ショットガンを有効にする
        shooting.enabled = true; // シューティングを有効にする
    }
}
