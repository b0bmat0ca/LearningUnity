using System;
using UniRx;
using UnityEngine;

public class InputEventProviderImpl : MonoBehaviour, IInputEventProvider
{
    #region IInputEventProvider

    public IReadOnlyReactiveProperty<bool> IsJump => isJump;
    public IReadOnlyReactiveProperty<Vector3> MoveDirection => moveDirection;

    public IReadOnlyReactiveProperty<bool> IsStop => isStop;

    #endregion

    private readonly ReactiveProperty<bool> isJump = new(false);
    private readonly ReactiveProperty<Vector3> moveDirection = new();
    private readonly ReactiveProperty<bool> isStop = new(false);

    // Start is called before the first frame update
    void Start()
    {
        isJump.AddTo(this);
        moveDirection.AddTo(this);
        isStop.AddTo(this);
    }

    // Update is called once per frame
    void Update()
    {
        isJump.Value = Input.GetKeyDown(KeyCode.Space);
        moveDirection.SetValueAndForceNotify(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")));
        isStop.Value = Input.GetKeyDown(KeyCode.C);
    }
}
