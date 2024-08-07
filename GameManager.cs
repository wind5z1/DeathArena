using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//主にはゲーム内銃のカメラ（エイミングしてる場合とエイミングしてない場合）を管理するスクリプト
public class GameManager : MonoBehaviour
{
    public Camera mainCamera;        // ゲームのメインカメラ
    public Camera gunCamera;         // 銃のエイミング用カメラ
    public Camera gun1Camera;        // ショットガンのエイミング用カメラ
    public bool isAiming = false;    // エイミングモードがアクティブかどうかのフラグ
    public float recoilForce = 0.2f; // リコイルの力
    private float originalFieldOfView;  // メインカメラの元の視野
    private float originalRotationX;    // メインカメラの元のX軸回転
    private float recoilDuration = 0.1f;    // リコイルの効果時間
    private bool isRecoiling = false;   // 現在リコイル中かどうかのフラグ
    public static GameManager Instance; // GameManagerのシングルトンインスタンス

    // GameManagerのシングルトンインスタンスを返す
    public static GameManager GetInstance()
    {
        return Instance;
    }

    // エイミングが有効かどうかをチェックする
    public bool IsAimingEnable()
    {
        return !isAiming; // isAimingがfalseのときにエイミングが有効
    }

    // Startメソッドは最初のフレームが更新される前に呼び出されます
    void Start()
    {
        if (Instance == null)
        {
            Instance = this; // シングルトンインスタンスを割り当てる
        }
        else
        {
            Destroy(gameObject); // GameManagerの重複インスタンスを破棄する
        }

        mainCamera.enabled = true;      // メインカメラを有効にする
        gunCamera.enabled = false;      // 銃のカメラを無効にする
        gun1Camera.enabled = false;     // ショットガンのカメラを無効にする
        isAiming = false;               // エイミングフラグをfalseに初期化する

        // リコイル効果前のメインカメラの視野と回転Xを保存する
        originalFieldOfView = mainCamera.fieldOfView;
        originalRotationX = mainCamera.transform.rotation.eulerAngles.x;
    }

    // Updateは1フレームごとに呼び出されます
    void Update()
    {
        // 右マウスボタンが押されているかをチェック
        if (Input.GetMouseButton(1))
        {
            if (!isAiming)  // エイミング中でない場合
            {
                SwitchCamera(); // カメラを切り替えてエイミングモードを有効にする
                isAiming = true; // エイミングフラグをtrueにする
            }
        }
        else
        {
            isAiming = false;   // 右マウスボタンが離されたらエイミングフラグをfalseにする
            isRecoiling = false;    // リコイルフラグをリセットする
        }

        UpdateRecoil(); // リコイル効果を更新する
    }

    // リコイル効果を更新する
    public void UpdateRecoil()
    {
        // 左マウスボタンが押されており、射撃可能な場合
        if (!isRecoiling && Input.GetMouseButtonDown(0) && (Shooting.Instance.canShoot || Shotgun.Instance.canShoot))
        {
            mainCamera.fieldOfView += recoilForce;    // 視野を広げてリコイル効果を表現する
            mainCamera.transform.Rotate(Vector3.left * recoilForce * 10f); // リコイル時にカメラを上向きに回転させる
            isRecoiling = true; // リコイル中フラグをtrueにする
        }
        else
        {
            // リコイル後にカメラの回転をスムーズに元の位置に戻す
            mainCamera.transform.rotation = Quaternion.Slerp(mainCamera.transform.rotation,
                Quaternion.Euler(originalRotationX, mainCamera.transform.rotation.eulerAngles.y, mainCamera.transform.rotation.eulerAngles.z),
                Time.deltaTime * 5f);
        }
    }

    // カメラの切り替え
    void SwitchCamera()
    {
        mainCamera.enabled = !mainCamera.enabled; // メインカメラの有効状態を切り替える
        gunCamera.enabled = !gunCamera.enabled;   // 銃カメラの有効状態を切り替える
        gun1Camera.enabled = !gun1Camera.enabled; // ショットガンカメラの有効状態を切り替える

        isRecoiling = false; // リコイルフラグをリセットする
    }

}
