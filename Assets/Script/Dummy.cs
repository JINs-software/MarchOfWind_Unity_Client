using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Dummy : MonoBehaviour
{
    public NetworkManager Session;

    public bool OnMoveCmd = false;
    public bool OnMove = false;
    public Vector3 Destination;

    private Vector3 Position;

    // Start is called before the first frame update
    void Start()
    {
        Manager.UnitSelection.UnitList.Add(gameObject);
        Position = gameObject.transform.position;       
    }

    // Update is called once per frame
    void Update()
    {
        if (OnMoveCmd)
        {
            MSG_UNIT_S_MOVE moveMsg = new MSG_UNIT_S_MOVE();
            moveMsg.type = (ushort)enPacketType.UNIT_S_MOVE;
            moveMsg.moveType = (byte)enUnitMoveType.Move_Start;
            moveMsg.posX = gameObject.transform.position.x;
            moveMsg.posZ = gameObject.transform.position.z;
            Vector3 dirVec = (Destination - gameObject.transform.position).normalized;
            moveMsg.normX = dirVec.x;
            moveMsg.normZ = dirVec.z;
            moveMsg.destX = Destination.x;
            moveMsg.destZ = Destination.z;

            Session.SendPacket(moveMsg);

            OnMoveCmd = false;
            OnMove = true;

            gameObject.GetComponent<NavMeshAgent>().isStopped = false;
            gameObject.GetComponent<NavMeshAgent>().SetDestination(Destination);
            Position = gameObject.transform.position;
        }
        else if (OnMove)
        {
            if(gameObject.GetComponent<NavMeshAgent>().remainingDistance < gameObject.GetComponent<NavMeshAgent>().stoppingDistance)
            {
                MSG_UNIT_S_MOVE stopMsg = new MSG_UNIT_S_MOVE();
                stopMsg.type = (ushort)enPacketType.UNIT_S_MOVE;
                stopMsg.moveType = (byte)enUnitMoveType.Move_Stop;
                stopMsg.posX = gameObject.transform.position.x;
                stopMsg.posZ = gameObject.transform.position.z;
                stopMsg.normX = gameObject.transform.forward.normalized.x;
                stopMsg.normZ = gameObject.transform.forward.normalized.z;

                Session.SendPacket<MSG_UNIT_S_MOVE>(stopMsg);


                gameObject.GetComponent<NavMeshAgent>().isStopped = true;
                OnMove = false;
            }
        }
    }
}
