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

        private Vector3 normalVector;

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

            if (IsGrounded())
            {
                velocity = Vector3.ProjectOnPlane(Vector3.forward, normalVector) * axisV * moveSpeed;
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    velocity.y += jumpForce;
                }
            }
            else
            {
                velocity.y += Physics.gravity.y * Time.deltaTime;
            }

            // プレイヤーの移動
            transform.Translate(velocity * Time.deltaTime);

            // プレイヤーの回転
            transform.Rotate(Vector3.up * (axisH * rotationSpeed * Time.deltaTime));
        }

        private void FixedUpdate()
        {
           Debug.Log( IsGrounded());

            //if (IsGrounded())
            //{
            //    if (Input.GetKeyDown(KeyCode.Space))
            //    {
            //        velocity = Vector3.ProjectOnPlane(Vector3.forward, normalVector) * axisV * moveSpeed;
            //        velocity.y += jumpForce;
            //    }
            //}
            //else
            //{
            //    velocity.y += Physics.gravity.y * Time.deltaTime;
            //}
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                normalVector = collision.contacts[0].normal;
            }
        }

        private bool IsGrounded()
        {
            Vector3 center = new(boxColider.bounds.center.x,
                boxColider.bounds.center.y - boxColider.bounds.size.y * 0.5f, boxColider.bounds.center.z);

            return Physics.CheckSphere(center, boxColider.bounds.size.x * 0.5f, groundLayer,
                QueryTriggerInteraction.Ignore);
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
