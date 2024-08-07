using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//フラッシュライトを制御するスクリプト
public class Flashlight : MonoBehaviour
{
    [SerializeField] GameObject FlashlightLight; // フラッシュライトの光オブジェクト
    private bool FlashlightActive = false; // フラッシュライトがアクティブかどうかのフラグ
    public AudioSource torchlightSound; // フラッシュライトの音源

    // Startは初めに呼び出される
    void Start()
    {
        FlashlightLight.gameObject.SetActive(false); // フラッシュライトをオフにする
    }

    // Updateはフレームごとに呼び出される
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (FlashlightActive == false)
            {
                FlashlightLight.gameObject.SetActive(true); // フラッシュライトをオンにする
                FlashlightActive = true; // フラッシュライトがアクティブになったことを記録
                if (torchlightSound != null)
                {
                    torchlightSound.Play(); // フラッシュライトの音を再生
                }
            }
            else
            {
                FlashlightLight.gameObject.SetActive(false); // フラッシュライトをオフにする
                FlashlightActive = false; // フラッシュライトが非アクティブになったことを記録
                if (torchlightSound != null)
                {
                    torchlightSound.Play(); // フラッシュライトの音を再生
                }
            }
        }
    }
}
