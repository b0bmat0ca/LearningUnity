using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private Enemy enemy;
    [SerializeField] private Transform patrolRoute; // 巡回地点を子オブジェクトに持つ親オブジェクト
    private readonly List<Transform> destinations = new();   // 巡回地点のリスト

    private IInputEventProvider inputEvetProvider;
    private CancellationTokenSource tokenSource;

    // Start is called before the first frame update
    void Start()
    {
        InitPatrolRoute();
        inputEvetProvider = GetComponent<IInputEventProvider>();

        tokenSource = new CancellationTokenSource();

        // こちらを利用すると、終了時に自動的にキャンセルされる
        // SpawnEnemyAsync(this.GetCancellationTokenOnDestroy()).Forget();
        SpawnEnemyAsync(tokenSource.Token).Forget();
    }

    // Update is called once per frame
    void Update()
    {
        // 敵の増殖を止める
        if (inputEvetProvider.IsStop.Value)
        {
            tokenSource.Cancel();
        }
    }

    private void OnDestroy()
    {
        // 終了時にUniiTaskをキャンセルする
        tokenSource.Cancel();
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
        Enemy obj = Instantiate(enemy, destinations[spawnIndex].position, Quaternion.identity);
        obj.DestIndex = spawnIndex;

        obj.ToNext
            .Where(x => x == true)
            .Subscribe(_ =>
            {
                int index = obj.DestIndex;
                obj.SetDestination(EnemyDestination(ref index));
                obj.DestIndex = index;
            }).AddTo(this);

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
