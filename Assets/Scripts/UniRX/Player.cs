using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class Player : MonoBehaviour
{
    public IReadOnlyReactiveProperty<bool> IsDead => isDead;
    private readonly ReactiveProperty<bool> isDead = new();

    // Start is called before the first frame update
    void Start()
    {
        isDead.AddTo(this);

        this.OnCollisionEnterAsObservable()
            .Where(x => x.gameObject.TryGetComponent<Enemy>(out _))
            .Subscribe(onNext: _ => isDead.Value = true);
    }
}
