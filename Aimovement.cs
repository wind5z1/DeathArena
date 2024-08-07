using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//AIの動きを制御するスクリプト
public class AiMovement : MonoBehaviour
{
    public float roamRadius = 10f; // ランダムに移動する半径
    public float roamInterval = 5f; // ランダム移動の間隔
    public float rotationSpeed = 5f; // 回転速度
    public float minDistanceBetweenPoints = 2f; // 訪れたポイント間の最小距離
    public LayerMask playerLayer; // プレイヤーのレイヤー
    public float detectionRadius = 10f; // プレイヤーを検出する半径
    public float chaseSpeed = 2f; // 追跡時の速度

    private NavMeshAgent agent; // ナビメッシュエージェント
    private float originalSpeed; // 元の速度
    private float timer; // タイマー
    private Animator animator; // アニメーター
    private const float speedThreshold = 0.1f; // 速度の閾値
    private List<Vector3> visitedPositions = new List<Vector3>(); // 訪れたポイントのリスト
    public AudioClip zombieScreamClip; // ゾンビの叫び声のオーディオクリップ
    private AudioSource audioSource; // オーディオソース

    // Startメソッドは初期化のために呼び出される
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = roamInterval;
        originalSpeed = agent.speed;
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        Roam(); // ランダム移動を開始         
    }

    // Updateメソッドは毎フレーム呼び出される
    void Update()
    {
        GameObject player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            if (CanSeePlayer(player))
            {
                agent.speed = chaseSpeed; // 追跡時の速度に設定
                agent.SetDestination(player.transform.position); // プレイヤーの位置を目的地に設定

                animator.SetBool("isRunning", true);
                if (audioSource.clip != zombieScreamClip || !audioSource.isPlaying)
                {
                    audioSource.Stop();
                    audioSource.clip = zombieScreamClip;
                    audioSource.Play();
                }
            }
            else
            {
                agent.speed = originalSpeed; // 元の速度に戻す
                timer -= Time.deltaTime;
                animator.SetBool("isRunning", false);

                if (timer <= 0f)
                {
                    Roam(); // ランダム移動を再開
                    timer = roamInterval;
                }
                SmoothRotation(); // スムーズに回転
            }
        }
    }

    // プレイヤーを視認できるかどうかを判定する
    bool CanSeePlayer(GameObject player)
    {
        if (player != null)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);
            foreach (Collider collider in hitColliders)
            {
                if (collider.gameObject == player)
                {
                    return true;
                }
            }
        }
        return false;
    }

    // ランダムに移動する
    void Roam()
    {
        Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, roamRadius, 1);

        Vector3 finalPosition = hit.position;

        while (IsTooCloseToVisited(finalPosition))
        {
            randomDirection = Random.insideUnitSphere * roamRadius;
            randomDirection += transform.position;

            NavMesh.SamplePosition(randomDirection, out hit, roamRadius, 1);
            finalPosition = hit.position;
        }
        visitedPositions.Add(finalPosition);
        if (visitedPositions.Count > 5)
        {
            visitedPositions.RemoveAt(0);
        }
        agent.SetDestination(finalPosition);

        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    // 訪れたポイントに近すぎるかどうかを判定する
    bool IsTooCloseToVisited(Vector3 position)
    {
        foreach (Vector3 visitedPos in visitedPositions)
        {
            if (Vector3.Distance(position, visitedPos) < minDistanceBetweenPoints)
            {
                return true;
            }
        }
        return false;
    }

    // スムーズに回転する
    void SmoothRotation()
    {
        Vector3 desiredDir = (agent.steeringTarget - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(desiredDir, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
    }
}
