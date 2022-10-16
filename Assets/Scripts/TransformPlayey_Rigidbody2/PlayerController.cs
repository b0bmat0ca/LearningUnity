using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TransformPlayey_Rigidbody2
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float moveForce = 24f; // 移動に加える力
        [SerializeField] private float rotationForce = 16f; // 回転に加える力
        [SerializeField] private float jumpForce = 8.0f;  // ジャンプ力

        private float axisH;
        private float axisV;

        private bool isGrounded;    // 地面に接しているかどうか

        private Rigidbody _rigidbody;

        private Vector3 normalVector;

        // Start is called before the first frame update
        void Start()
        {
            isGrounded = true;
            _rigidbody = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void Update()
        {
            // スペースキーを押したら、ジャンプ
            if (isGrounded && Input.GetKeyDown(KeyCode.Space))
            {
                _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                isGrounded = false;
            }
        }

        private void FixedUpdate()
        {
            axisH = Input.GetAxis("Horizontal");    // 矢印キーの左右入力を取得(-1 ～1)
            axisV = Input.GetAxis("Vertical");  // 矢印キーの上下入力を取得(-1 ～ 1)

            // プレイヤーの移動(斜面でも同じ力を加える)
            _rigidbody.AddForce(
                Vector3.ProjectOnPlane(transform.forward, normalVector) * (axisV * moveForce),
                ForceMode.Force);

            // プレイヤーの回転
            _rigidbody.AddTorque(transform.up * (axisH * rotationForce), ForceMode.Force);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                isGrounded = true;
                normalVector = collision.contacts[0].normal;
            }
        }
    }
}
