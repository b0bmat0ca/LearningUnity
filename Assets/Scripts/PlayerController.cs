using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private GameController controller;

    [SerializeField] private float moveForce = 24f; // 移動に加える力
    [SerializeField] private float rotationForce = 16f; // 回転に加える力
    [SerializeField] private float jumpForce = 8.0f;  // ジャンプ力
    [SerializeField] private LayerMask groundLayer; // 地面のレイヤー

    private float axisH;
    private float axisV;

    private Rigidbody _rigidbody;
    private BoxCollider boxColider;

    private Vector3 normalVector;

    // Start is called before the first frame update
    void Start()
    {
        controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        _rigidbody = GetComponent<Rigidbody>();
        boxColider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        axisH = Input.GetAxis("Horizontal");    // 矢印キーの左右入力を取得(-1 ～1)
        axisV = Input.GetAxis("Vertical");  // 矢印キーの上下入力を取得(-1 ～ 1)

        // スペースキーを押したら、ジャンプ
        if (IsGrounded() && Input.GetKeyDown(KeyCode.Space))
        {
            _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void FixedUpdate()
    {
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
            normalVector = collision.contacts[0].normal;
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            controller.EnemyHit = true;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            normalVector = collision.contacts[0].normal;
        }
    }

    /// <summary>
    /// 地面に接地しているか
    /// </summary>
    /// <returns></returns>
    private bool IsGrounded()
    {
        // BoxColiderの底面の中心点
        Vector3 center = new(boxColider.bounds.center.x,
            boxColider.bounds.center.y - boxColider.bounds.size.y * 0.5f, boxColider.bounds.center.z);

        return Physics.CheckSphere(center, boxColider.bounds.size.x * 0.5f, groundLayer, 
            QueryTriggerInteraction.Ignore);
    }
}

