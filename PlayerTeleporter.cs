using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EvolveGames;
//プレイヤーを一定の時間になるとプレイヤーをテレポートするスクリプト
public class PlayerTeleporter : MonoBehaviour
{
    public Transform player; // プレイヤーのTransform
    public PlayerController playerController; // プレイヤーコントローラースクリプト

    // ターゲット位置にプレイヤーをテレポートするメソッド
    public void TeleporttoTarget(Vector3 targetPosition)
    {
        if (IsTargetPositionValid(targetPosition)) // ターゲット位置が有効かどうかをチェック
        {
            playerController.DisableMovement(); // 移動を無効化する
            player.transform.position = targetPosition; // プレイヤーをターゲット位置に移動する
            Debug.Log("Teleported"); 
            StartCoroutine(EnableMovementAfterDelay()); // 待機後に移動を有効化する
        }
        else
        {
            Debug.Log("Invalid position."); // ターゲット位置が無効な場合、デバッグログを出力する
        }
    }

    // 待機後に移動を有効化するコルーチン
    IEnumerator EnableMovementAfterDelay()
    {
        yield return new WaitForSeconds(0.1f); // 0.1秒待つ
        playerController.EnableMovement(); // 移動を有効化する
    }

    // ターゲット位置が有効かどうかをチェックするメソッド
    bool IsTargetPositionValid(Vector3 targetPosition)
    {
        return true; // 仮の実装として常に true を返す（実際のゲームでは位置のバリデーションを実装する）
    }
}
