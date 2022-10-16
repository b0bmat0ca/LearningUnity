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

        private bool isJump;    // ジャンプ中か

        private BoxCollider boxColider;
        private Vector3 coliderHarfSize;

        private Vector3 normalVector;

        // Start is called before the first frame update
        void Start()
        {
            velocity = Vector3.zero;
            isJump = false;
            boxColider = GetComponent<BoxCollider>();
            coliderHarfSize = boxColider.bounds.size * 0.5f;
        }

        // Update is called once per frame
        void Update()
        {
            axisH = Input.GetAxis("Horizontal");    // 矢印キーの左右入力を取得(-1 ～1)
            axisV = Input.GetAxis("Vertical");  // 矢印キーの上下入力を取得(-1 ～ 1)

            // 坂を登った時の制御がうまくいかないので、一旦、保留
            /*
            if (!isJump)
            {
                velocity = Vector3.ProjectOnPlane(Vector3.forward, normalVector) * axisV * moveSpeed;
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    velocity.y += jumpForce;
                    isJump = true;
                }
            }

            // 地面に接地していない場合は、重力を考慮する
            if (!IsGrounded())
            {
                velocity.y += Physics.gravity.y * Time.deltaTime;
            }
            transform.Translate(velocity * Time.deltaTime);
            */

            // プレイヤーの移動
            transform.Translate(Vector3.forward * (axisV * moveSpeed * Time.deltaTime));

            // プレイヤーの回転
            transform.Rotate(Vector3.up * (axisH * rotationSpeed * Time.deltaTime));
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                isJump = false;
                normalVector = collision.contacts[0].normal;
            }
        }

        /// <summary>
        /// 地面に接地しているかどうか
        /// </summary>
        /// <returns></returns>
        private bool IsGrounded()
        {
            return Physics.CheckBox(boxColider.bounds.center, coliderHarfSize, Quaternion.identity, groundLayer);
        }
    }
}
