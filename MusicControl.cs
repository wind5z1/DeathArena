using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//全般的の音楽を調整するスクリプト
public class MusicControl : MonoBehaviour
{
    public AudioSource audioSource; // オーディオソース
    public string mainmenuSceneName = "MainMenu"; // メインメニューシーンの名前

    // Start is called before the first frame update
    void Start()
    {
        // 現在のシーンがメインメニューシーンの場合
        if (SceneManager.GetActiveScene().name == mainmenuSceneName)
        {
            PlayAudio(); // オーディオを再生する
        }
        else
        {
            PauseAudio(); // オーディオを一時停止する
        }
    }

    // オーディオを再生するメソッド
    void PlayAudio()
    {
        // オーディオソースが設定されており、再生中でない場合
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play(); // オーディオを再生する
        }
    }

    // オーディオを一時停止するメソッド
    void PauseAudio()
    {
        // オーディオソースが設定されており、再生中の場合
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Pause(); // オーディオを一時停止する
        }
    }
}
