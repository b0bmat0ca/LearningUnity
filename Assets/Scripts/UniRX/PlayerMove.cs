using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    //private GameController controller;

    [SerializeField] private float moveForce = 24f; // 移動に加える力
    [SerializeField] private float rotationForce = 16f; // 回転に加える力
    [SerializeField] private float jumpForce = 8.0f;  // ジャンプ力
    [SerializeField] private LayerMask groundLayer; // 地面のレイヤー

    private float axisH;
    private float axisV;

    private Rigidbody _rigidbody;
    private BoxCollider boxColider;

    private Vector3 normalVector;

    private IInputEventProvider inputEventProvider;

    // Start is called before the first frame update
    void Start()
    {
        //controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        _rigidbody = GetComponent<Rigidbody>();
        boxColider = GetComponent<BoxCollider>();

        inputEventProvider = GetComponent<IInputEventProvider>();
    }

    // Update is called once per frame
    void Update()
    {
        // ジャンプ
        if (inputEventProvider.IsJump.Value && IsGrounded())
        {
            _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void FixedUpdate()
    {
        // 操作イベントの値を取得
        axisH = inputEventProvider.MoveDirection.Value.x;
        axisV = inputEventProvider.MoveDirection.Value.z;

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

