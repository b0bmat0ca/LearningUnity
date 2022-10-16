using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TransformPlayer
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 8.0f;    // 移動速度
        [SerializeField] private float rotationSpeed = 32f; // 回転速度
        [SerializeField] private float jumpForce = 8.0f;  // ジャンプ力
        [SerializeField] private LayerMask groundLayer; // 地面のレイヤー

        private float axisH;
        private float axisV;

        private Vector3 velocity;

        private BoxCollider boxColider;

        // Start is called before the first frame update
        void Start()
        {
            velocity = Vector3.zero;
            boxColider = GetComponent<BoxCollider>();
        }

        // Update is called once per frame
        void Update()
        {
            axisH = Input.GetAxis("Horizontal");    // 矢印キーの左右入力を取得(-1 ～1)
            axisV = Input.GetAxis("Vertical");  // 矢印キーの上下入力を取得(-1 ～ 1)

            if (IsGrounded(out Vector3 normalVector))
            {
                velocity = Vector3.ProjectOnPlane(Vector3.forward, normalVector) * axisV * moveSpeed;
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    velocity.y += jumpForce;
                }
            }

            // 地面に接地していない場合は、重力を考慮する
            if (!IsGrounded(out _))
            {
                velocity.y += Physics.gravity.y * Time.deltaTime;
            }
            transform.Translate(velocity * Time.deltaTime);

            // プレイヤーの移動
            transform.Translate(Vector3.forward * (axisV * moveSpeed * Time.deltaTime));

            // プレイヤーの回転
            transform.Rotate(Vector3.up * (axisH * rotationSpeed * Time.deltaTime));
        }

        /// <summary>
        /// GroundLayerのColiderとの接地判定
        /// </summary>
        /// <param name="normalVector"></param>
        /// <returns></returns>
        private bool IsGrounded(out Vector3 normalVector)
        {
            Vector3 coliderCenter = boxColider.bounds.center;
            Vector3 direction = new Vector3(0, boxColider.bounds.size.y * -0.5f, boxColider.bounds.size.z * 0.5f);

            if (Physics.Raycast(coliderCenter, direction, out RaycastHit hit, 
                Vector3.Distance(coliderCenter, coliderCenter + direction), groundLayer))
            {
                normalVector = hit.normal;
                return true;
            }
            else if (Physics.BoxCast(coliderCenter, boxColider.bounds.size * 0.5f, Vector3.down,
                out hit, Quaternion.identity, 0f, groundLayer))
            {
                normalVector = hit.normal;
                return true;
            }

            normalVector = Vector3.zero;
            return false;
        }
    }
}
