using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private GameController controller;

    private int destIndex;
    public int DestIndex
    {
        set
        {
            destIndex = value;
        }
    }

    private NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        agent = GetComponent<NavMeshAgent>();
        SetDestination(controller.EnemyDestination(ref destIndex));
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.remainingDistance < 0.1f && !agent.pathPending)
        {
            SetDestination(controller.EnemyDestination(ref destIndex));
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
            destIndex--; // プレイヤーを追いかけたことで、１つ目的地を飛ばしてしまうため
            SetDestination(controller.EnemyDestination(ref destIndex));
        }
    }

    private void SetDestination(Transform destination)
    {
        agent.destination = destination.position;
    }
}
