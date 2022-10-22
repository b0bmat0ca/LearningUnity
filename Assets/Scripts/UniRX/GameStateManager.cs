using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    private Player player;

    private readonly ReactiveProperty<GameState> gameState = new(GameState.Playing);

    private string uiMessage;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.IsDead
            .Where(x => x == true)
            .Subscribe(_ =>
            {
                uiMessage = "捕まりました";
                gameState.Value = GameState.Restart;
                Time.timeScale = 0f;
            }).AddTo(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnGUI()
    {
        // 敵に捕まったら終了で、最初から始められるようにする
        if (gameState.Value == GameState.Restart)
        {
            if (GUI.Button(new Rect(Screen.width / 2 - 64f, Screen.height / 2 - 32f, 128f, 64f), uiMessage))
            {
                Utilities.RestartGame(0);
            }
        }
    }
}
