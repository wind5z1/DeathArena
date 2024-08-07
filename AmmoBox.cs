using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//弾薬箱を制御するスクリプト
public class AmmoBox : MonoBehaviour
{
    public int ammoAmount = 10; // 弾薬の量
    private bool isPlayerNear = false; // プレイヤーが近くにいるかどうか
    private Animator boxAnimator; // アニメーターコンポーネント
    public Shooting shootingScript; // 射撃スクリプト
    public TextMeshProUGUI riffleBulletText; // ライフルの弾薬テキスト
    public TextMeshProUGUI pickupText; // ピックアップメッセージのテキスト
    public AudioClip opensound; // 開ける音
    private AudioSource audioSource; // オーディオソース

    // 初期化処理
    private void Start()
    {
        boxAnimator = GetComponent<Animator>();
        boxAnimator.enabled = false;

        audioSource = gameObject.AddComponent<AudioSource>();
    }

    // プレイヤーが弾薬箱に接触したときの処理
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerNear = true;
        }
    }

    // プレイヤーが弾薬箱から離れたときの処理
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerNear = false;
        }
    }

    // 毎フレーム更新処理
    private void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            if (!shootingScript.enabled)//プレイヤーがシューティングスクリプトがなかったら文字で提示する
            {
                pickupText.text = "Please press 'G' to riffle for picking up riffle ammo.";
                StartCoroutine(ClearTextAfterDelay(1f));
            }
            else
            {
                StartCoroutine(OpenAmmoBox(shootingScript));//今シューティングスクリプトが持ってば弾薬を増やす
            }
        }
    }

    // 弾薬箱を開けるコルーチン
    IEnumerator OpenAmmoBox(Shooting shootingScript)
    {
        if (opensound != null)
        {
            audioSource.PlayOneShot(opensound);
        }
        boxAnimator.enabled = true;
        boxAnimator.Play("opened_closed");
        yield return new WaitForSeconds(1f);
        boxAnimator.enabled = false;
        shootingScript.AddBullets(ammoAmount);
        riffleBulletText.text = "Picked Up Riffle Bullet";
        yield return new WaitForSeconds(1f);
        riffleBulletText.text = " ";
        gameObject.SetActive(false);
    }

    // 一定時間後にテキストをクリアするコルーチン
    private IEnumerator ClearTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        riffleBulletText.text = "";
    }
}
