using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject enemy;
    [SerializeField] private Transform patrolRoute; // 巡回地点を子オブジェクトに持つ親オブジェクト
    private readonly List<Transform> destinations = new();   // 巡回地点のリスト

    private string uiMessage;
    private CancellationTokenSource tokenSource;

    private bool enemyHit = false;
    public bool EnemyHit
    {
        get
        {
            return enemyHit;
        }
        set
        {
            enemyHit = value;
            uiMessage = "捕まりました";
            Time.timeScale = 0f;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        InitPatrolRoute();

        tokenSource = new CancellationTokenSource();
        try 
        {
            // こちらを利用すると、終了時に自動的にキャンセルされる
            // SpawnEnemyAsync(this.GetCancellationTokenOnDestroy()).Forget();

            SpawnEnemyAsync(tokenSource.Token).Forget();
        }
        catch (System.NullReferenceException ex)
        {
            Debug.Log(ex.ToString());
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 敵の増殖を止める
        if (Input.GetKeyDown(KeyCode.C))
        {
            tokenSource.Cancel();
        }
    }

    private void OnDestroy()
    {
        // 終了時にUniiTaskをキャンセルする
        tokenSource.Cancel();
    }

    private void OnGUI()
    {
        // 敵に捕まったら終了で、最初から始められるようにする
        if (enemyHit)
        {
            
            if (GUI.Button(new Rect(Screen.width / 2 - 64f, Screen.height / 2 - 32f, 128f, 64f), uiMessage))
            {
                SceneManager.LoadScene(0);
                Time.timeScale = 1.0f;
            }
        }
    }

    /// <summary>
    /// 敵の巡回地を初期化する
    /// </summary>
    private void InitPatrolRoute()
    {
        foreach (Transform destination in patrolRoute)
        {
            destinations.Add(destination);
        }
    }

    /// <summary>
    /// 10秒毎に敵を出現させる
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    private async UniTaskVoid SpawnEnemyAsync(CancellationToken token)
    {
        int spawnIndex = Random.Range(0, destinations.Count);
        GameObject obj = Instantiate(enemy, destinations[spawnIndex].position, Quaternion.identity);
        EnemyController ectl = obj.GetComponent<EnemyController>();
        ectl.DestIndex = spawnIndex;

        await UniTask.Delay(System.TimeSpan.FromSeconds(10), cancellationToken: token);

        SpawnEnemyAsync(token).Forget();
    }

    /// <summary>
    /// 敵に次の目的地を指示する
    /// </summary>
    /// <param name="index">現在の目的地番号</param>
    /// <returns></returns>
    public Transform EnemyDestination(ref int index)
    {
        index = (index + 1) % destinations.Count;
        return destinations[index];
    }
}
