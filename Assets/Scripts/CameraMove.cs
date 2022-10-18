using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] Vector3 offset = new Vector3(0f, 4f, -6f);

    private Transform player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        // プレイヤーの位置からoffset分ズレた位置のワールド座標を算出して、カメラの位置に設定する
        transform.position = player.TransformPoint(offset);

        // カメラを常にプレイヤーの方向に向ける
        transform.LookAt(player);
    }
}

