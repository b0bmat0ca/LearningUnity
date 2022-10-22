using System;
using UniRx;
using UnityEngine;

public interface IInputEventProvider
{
    IReadOnlyReactiveProperty<Vector3> MoveDirection { get; }

    IReadOnlyReactiveProperty<bool> IsJump { get; }

    IReadOnlyReactiveProperty<bool> IsStop { get; }
}
