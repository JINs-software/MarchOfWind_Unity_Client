using System;
using UnityEngine;
using static System.Runtime.CompilerServices.RuntimeHelpers;

public class Teleporter : MonoBehaviour {
    static private int CrtCode = 0;
    static private bool InitPosFlag = true;
    static private Vector3 InitPostion;
    static private float PosAngle;
    static private float PosRadius;

    const int Crt_Count_Marine = 1;
    const int Crt_Count_Firebat = 1;
    const int Crt_Count_Zergling = 1;
    const int Crt_Count_Hydra = 1;

    void OnTriggerEnter(Collider collider) {
        Teleportable teleportable = collider.transform.GetComponent<Teleportable>();
        if (teleportable != null) {
            OnEnter(teleportable);
        }
        collider.gameObject.SetActive(false);
    }

    public void OnEnter(Teleportable teleportable) {
        if (!teleportable.canTeleport) {
            return;
        }
        teleportable.canTeleport = false;

        enUNIT_TYPE unitType = enUNIT_TYPE.None;
        int crtCnt = 0;

        if (transform.parent.name == "port_marine")
        {
            unitType = enUNIT_TYPE.Terran_Marine;
            crtCnt = Crt_Count_Marine;
        }
        else if (transform.parent.name == "port_firebat")
        {
            unitType = enUNIT_TYPE.Terran_Firebat;
            crtCnt = Crt_Count_Firebat; 
        }
        //else if (transform.parent.name == "port_tank") unitType = enUNIT_TYPE.Terran_Tank;
        //else if (transform.parent.name == "port_robocop") unitType = enUNIT_TYPE.Terran_Robocop;
        else if (transform.parent.name == "port_zergling")
        {
            unitType = enUNIT_TYPE.Zerg_Zergling;
            crtCnt = Crt_Count_Zergling;
        }
        else if (transform.parent.name == "port_hydra")
        {
            unitType = enUNIT_TYPE.Zerg_Hydra;
            crtCnt = Crt_Count_Hydra;
        }
        //else if (transform.parent.name == "port_golem") unitType = enUNIT_TYPE.Zerg_Golem;
        //else if (transform.parent.name == "port_tarantula") unitType = enUNIT_TYPE.Zerg_Tarantula;

        for (int i = 0; i < crtCnt; i++)
        {
            GamaManager.Instance.AliveUnitCnt++;

            NetworkManager unitSession = RPC.Instance.AllocNewClientSession();
            if (unitSession == null)
            {
                Debug.Log("유닛 세션 연결 실패...!");
                return;
            }

            RPC.proxy.UNIT_CONN_TO_BATTLE_FIELD(GamaManager.Instance.BattleFieldID, unitSession);

            Vector3 initPostion = GetRandomCreatePosition(unitType);
            Vector3 initNorm = GamaManager.Instance.InitNorm;
            int crtCode = CrtCode++;
            RPC.proxy.UNIT_S_CREATE(crtCode, (byte)unitType, GamaManager.Instance.Team, initPostion.x, initPostion.z, initNorm.x, initNorm.z, unitSession);
            GamaManager.Instance.SetUnitSession(crtCode, unitSession);
        }

        GamaManager.Instance.SelectorCnt--;
        if (GamaManager.Instance.SelectorCnt == 0)
        {
            //RPC.proxy.ENTER_TO_BATTLE_FIELD();
            // => BattleField 초입에 호출
            // 기존 필드 유닛 crt 메시지를 BattleField 씬에서의 수신을 보장

            Manager.Scene.Clear();
            Manager.Scene.LoadScene(Define.Scene.BattleField);
        }
    }

    private Vector3 GetRandomCreatePosition(enUNIT_TYPE unitType)
    {
        Vector3 position = Vector3.zero;

        if (InitPosFlag)
        {
            InitPosFlag = false;
            PosAngle = 0f;
            PosRadius = GetUnitRadius(unitType);
            position = InitPostion = GamaManager.Instance.InitPosition;
        }
        else
        {
            position = InitPostion + new Vector3(
                Mathf.Cos(PosAngle) * (PosRadius + GetUnitRadius(unitType)),
                0.0f,
                Mathf.Sin(PosAngle) * (PosRadius + GetUnitRadius(unitType))
            );

            PosAngle += 45f;
            PosRadius += GetUnitRadius(unitType);
        }

        return position;
    }
    private float GetUnitRadius(enUNIT_TYPE unitType)
    {
        float radius = 0f;

        if (unitType == enUNIT_TYPE.Terran_Marine) radius = 1.5f;
        else if (unitType == enUNIT_TYPE.Terran_Firebat) radius = 1.5f;
        //else if (unitType == enUNIT_TYPE.Terran_Tank)
        //else if (unitType == enUNIT_TYPE.Terran_Robocop)
        else if (unitType == enUNIT_TYPE.Zerg_Zergling) radius = 4.0f;
        else if (unitType == enUNIT_TYPE.Zerg_Hydra) radius = 4.5f;
        //else if (unitType == enUNIT_TYPE.Zerg_Golem)
        //else if (unitType == enUNIT_TYPE.Zerg_Tarantula)

        return radius;
    }
}