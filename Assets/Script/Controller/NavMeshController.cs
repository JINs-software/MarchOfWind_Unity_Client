using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshController : MonoBehaviour
{
    [SerializeField]
    private float CarvingTime = 0.5f;
    [SerializeField]
    private float CarvingMoveThreshold = 0.1f;

    private NavMeshAgent Agent;
    private NavMeshObstacle Obstacle;

    private float LastMoveTime;
    private Vector3 LastPostion;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        Obstacle = GetComponent<NavMeshObstacle>();

        Obstacle.enabled = false;
        Obstacle.carveOnlyStationary = false;
        Obstacle.carving = true;

        LastPostion = transform.position;   
    }

    private void Update()
    {
        if(Vector3.Distance(LastPostion, transform.position) > CarvingMoveThreshold)
        {
            LastMoveTime = Time.time;
            LastPostion = transform.position;
        }

        if (LastMoveTime + CarvingTime < Time.time)
        {
            Agent.enabled = false;
            Obstacle.enabled = true;
        }
    }

    public void SetDestination(Vector3 destination)
    {
        Obstacle.enabled = false;

        LastMoveTime = Time.time;
        LastPostion = transform.position;

        StartCoroutine(MoveAgent(destination));
    }

    public void MoveStop()
    {
        Agent.enabled = false;
        
        StartCoroutine(StopAgent());    
    }

    private IEnumerator MoveAgent(Vector3 destination)
    {
        yield return null;

        Agent.enabled = true;
        Agent.SetDestination(destination);      
    }

    private IEnumerator StopAgent()
    {
        yield return null;

        Obstacle.enabled = true;
    }
}
