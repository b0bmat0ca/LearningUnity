using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public IReadOnlyReactiveProperty<bool> ToNext => toNext;
    private ReactiveProperty<bool> toNext = new(false);

    public int DestIndex { get; set; }

    private NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        toNext.AddTo(this);

        agent = GetComponent<NavMeshAgent>();
        toNext.Value = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.remainingDistance < 0.1f && !agent.pathPending)
        {
            toNext.Value = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SetDestination(other.gameObject.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            toNext.Value = true;
        }
    }

    public void SetDestination(Transform destination)
    {
        agent.destination = destination.position;
        toNext.Value = false;
    }
}
