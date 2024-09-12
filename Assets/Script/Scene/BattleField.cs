using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BattleField : BaseScene
{
    MOW_BATTLE_FIELD stub_MOW_BATTLE_FIELD;
    Dictionary<int, Unit> Units = new Dictionary<int, Unit>();
    Dictionary<int, UnitController> UnitControllers = new Dictionary<int, UnitController>();    

    protected override void Init()
    {
        base.Init();

        stub_MOW_BATTLE_FIELD = GetComponent<MOW_BATTLE_FIELD>();   
        if(stub_MOW_BATTLE_FIELD == null)
        {
            stub_MOW_BATTLE_FIELD = gameObject.AddComponent<MOW_BATTLE_FIELD>();    
        }
    }

    public override void Clear()
    {
        throw new System.NotImplementedException();
    }

    private void Start()
    {
        GamaManager.UnitSelection.Init();
        GamaManager.UnitSelection.Start = true;
        RPC.proxy.ENTER_TO_BATTLE_FIELD();
    }

    private void OnDestroy()
    {
        GamaManager.UnitSelection.Start = false;

        MSG_COM_REQUEST req = new MSG_COM_REQUEST();
        //Manager.Network.SetRequstMessage(req, enProtocolComRequest.REQ_MOVE_BATTLE_FIELD_TO_SELECT_FIELD);
        req.type = (ushort)enPacketType.COM_REQUSET;
        req.requestCode = (ushort)enProtocolComRequest.REQ_MOVE_BATTLE_FIELD_TO_SELECT_FIELD;
        Manager.Network.SendPacket<MSG_COM_REQUEST>(req);

        // REQ_MOVE_BATTLE_FIELD_TO_SELECT_FIELD 에 대한 응답을 받은 후 Battle 씬 파괴
        while (true)
        {
            if (Manager.Network.ReceiveDataAvailable())
            {
                byte[] payload;
                Manager.Network.ReceivePacket(out payload);
                if (payload == null)
                {
                    //Debug.Log("ReceiveDataAvailable, but ReceivePacket() => null");
                    return;
                }
                else
                {
                    enPacketType packetType = Manager.Network.GetMsgTypeInBytes(payload);
                    if (packetType == enPacketType.COM_REPLY)
                    {
                        MSG_COM_REPLY msg = Manager.Network.BytesToMessage<MSG_COM_REPLY>(payload);
                        if (msg.replyCode == (int)enProtocolComReply.REPLY_MOVE_BATTLE_FIELD_TO_SELECT_FIELD)
                        {
                            break;
                        }
                    }
                }

                //Debug.Log("REPLY_MOVE_BATTLE_FIELD_TO_SELECT_FIELD 메시지 수신 대기................");
            }
        }
    }


    public void S_PLAYER_CREATE(Int32 CRT_CODE, Int32 UNIT_ID, byte UNIT_TYPE, byte TEAM, float POS_X, float POS_Z, float NORM_X, float NORM_Z, float SPEED, Int32 MAX_HP, Int32 HP, float RADIUS, float ATTACK_DISTANCE, float ATTACK_RATE, float ATTACK_DELAY)
    {
        GameObject newUnitObj = CreateUnitObjectInScene(UNIT_ID, UNIT_TYPE, TEAM, POS_X, POS_Z, NORM_X, NORM_Z, SPEED, MAX_HP, HP, RADIUS, ATTACK_DISTANCE, ATTACK_RATE);
        Unit newUnit = newUnitObj.GetComponent<Unit>();
        Units.Add(newUnit.m_id, newUnit);

        if (GamaManager.Instance.Team == TEAM)
        {
            newUnitObj.tag = GamaManager.TEAM_TAG;
            newUnitObj.layer = LayerMask.NameToLayer(GamaManager.CLICKABLE_LAYER);

            // add 'UnitMovement' component
            UnitMovement unitMovement = newUnitObj.AddComponent<UnitMovement>();

            // add 'UnitController' component
            UnitController unitController = newUnitObj.AddComponent<UnitController>();
            unitController.Unit = newUnitObj.GetComponent<Unit>();
            unitController.UnitSession = GamaManager.Instance.GetUnitSession(CRT_CODE);

            unitMovement.MoveCmdHandler += unitController.OnMoveCmd;

            // add 'AttackController' component
            AttackController attackController = newUnitObj.AddComponent<AttackController>();
            attackController.Init(newUnit);

            UnitControllers.Add(newUnit.m_id, unitController);
        }
        else if(TEAM == (Byte)enPlayerTeamInBattleField.Team_Dummy)
            {
            newUnitObj.tag = GamaManager.DUMMY_TAG;
            newUnitObj.layer = LayerMask.NameToLayer(GamaManager.CLICKABLE_LAYER);

            // add 'UnitMovement' component
            UnitMovement unitMovement = newUnitObj.AddComponent<UnitMovement>();

            // add 'UnitController' component
            UnitController unitController = newUnitObj.AddComponent<UnitController>();
            unitController.Unit = newUnitObj.GetComponent<Unit>();
            unitController.UnitSession = GamaManager.Instance.GetUnitSession(CRT_CODE);

            unitMovement.MoveCmdHandler += unitController.OnMoveCmd;

            // add 'AttackController' component
            AttackController attackController = newUnitObj.AddComponent<AttackController>();
            attackController.Init(newUnit);

            UnitControllers.Add(newUnit.m_id, unitController);
        }
        else
        {
            if (TestManager.TestMode)
            {
                newUnitObj.tag = GamaManager.ENEMY_TAG;
                newUnitObj.layer = LayerMask.NameToLayer(GamaManager.ATTACKABLE_LAYER);

                // add 'UnitMovement' component
                UnitMovement unitMovement = newUnitObj.AddComponent<UnitMovement>();

                // add 'UnitController' component
                UnitController unitController = newUnitObj.AddComponent<UnitController>();
                unitController.Unit = newUnitObj.GetComponent<Unit>();
                unitController.UnitSession = GamaManager.Instance.GetUnitSession(CRT_CODE);

                unitMovement.MoveCmdHandler += unitController.OnMoveCmd;

                UnitControllers.Add(newUnit.m_id, unitController);

                // add 'AttackController' component
                AttackController attackController = newUnitObj.AddComponent<AttackController>();
                attackController.Init(newUnit);

                newUnit.AddComponent<Enemy>().ID = newUnit.m_id;
            }
            else
            {
                newUnitObj.tag = GamaManager.ENEMY_TAG;
                newUnitObj.layer = LayerMask.NameToLayer(GamaManager.ATTACKABLE_LAYER);

                newUnit.AddComponent<Enemy>().ID = newUnit.m_id;
                newUnit.AddComponent<Rigidbody>();
            }
        }

        newUnitObj.SetActive(true);
    }

    public void S_PLAYER_MOVE(Int32 UNIT_ID, byte TEAM, byte MOVE_TYPE, float POS_X, float POS_Z, float NORM_X, float NORM_Z, float SPEED, float DEST_X, float DEST_Z)
    {
        if (!Units.ContainsKey(UNIT_ID)) return;

        Unit unit = Units[UNIT_ID];
        if (MOVE_TYPE == (byte)enMOVE_TYPE.MOVE_START)
        {
            unit.Move_Start(new Vector3(POS_X, 0, POS_Z), new Vector3(DEST_X, 0, DEST_Z), new Vector3(NORM_X, 0 , NORM_Z) ,SPEED);
            //if(TEAM == GamaManager.Instance.Team)
            //{
            //    UnitController unitController = UnitControllers[UNIT_ID];
            //    unitController.OnMoving = true;
            //    if(unitController.State == enUNIT_STATUS.IDLE || unitController.State == enUNIT_STATUS.ATTACK)
            //    {
            //        unitController.State = enUNIT_STATUS.MOVE;
            //    }
            //}
        }
        else //if (MOVE_TYPE == (byte)enMOVE_TYPE.MOVE_STOP)
        {
            unit.Move_Stop(new Vector3(POS_X, 0, POS_Z), new Vector3(NORM_X, 0, NORM_Z));
            //if (TEAM == GamaManager.Instance.Team) {
            //    UnitController unitController = UnitControllers[UNIT_ID];
            //    unitController.OnMoving = false;
            //    unitController.State = enUNIT_STATUS.IDLE;
            //    UnitControllers[UNIT_ID].ServerPathFinding = false; 
            //}
        }
    }

    public void S_PLAYER_TRACE_PATH_FINDING_REPLY(Int32 UNIT_ID, Int32 SPATH_ID)
    {
        if (!Units.ContainsKey(UNIT_ID)) return;
        Unit unit = Units[UNIT_ID];
        unit.SPATH_REQ_Reply();
        if(unit.m_team == GamaManager.Instance.Team)
        {
            //unit.RecvJpsReqReply(); // <- 필수?
            UnitController unitController = UnitControllers[unit.m_id];
            unitController.SPATH_REPLY(SPATH_ID);
        }
    }
    public void S_PLAYER_TRACE_PATH(Int32 UNIT_ID, Int32 SPATH_ID, float POS_X, float POS_Z, byte SPATH_OPT)
    {
        if (!Units.ContainsKey(UNIT_ID)) return;
        Unit unit = Units[UNIT_ID];
        if (unit.m_team == GamaManager.Instance.Team)
        {
            UnitController unitController = UnitControllers[UNIT_ID];
            unitController.SPATH(SPATH_ID, POS_X, POS_Z, SPATH_OPT);
        }
    }

    public void S_PLAYER_LAUNCH_ATTACK(Int32 UNIT_ID, byte TEAM, float POS_X, float POS_Z, float NORM_X, float NORM_Z)
    {
        if (!Units.ContainsKey(UNIT_ID)) return;
        Unit unit = Units[UNIT_ID];
        unit.LauchAttack(POS_X, POS_Z, NORM_X, NORM_Z);
    }

    public void S_PLAYER_STOP_ATTACK(Int32 UNIT_ID, byte TEAM)
    {
        if (!Units.ContainsKey(UNIT_ID)) return;
        Unit unit = Units[UNIT_ID];
        unit.StopAttack();
    }

    public void S_PLAYER_ATTACK(Int32 UNIT_ID, byte TEAM, float POS_X, float POS_Z, float NORM_X, float NORM_Z, Int32 TARGET_ID, byte ATTACK_TYPE)
    {
        if (!Units.ContainsKey(UNIT_ID)) return;
        Unit unit = Units[UNIT_ID];
        unit.Attack(new Vector3(POS_X, 0, POS_Z), new Vector3(NORM_X, 0, NORM_Z), ATTACK_TYPE);
        //if (TEAM == GamaManager.Instance.Team)
        //{
        //    UnitControllers[UNIT_ID].OnMoving = false;
        //    UnitControllers[UNIT_ID].State = enUNIT_STATUS.ATTACK;
        //    UnitControllers[UNIT_ID].ServerPathFinding = false;
        //}
    }

    public void S_PLAYER_DAMAGE(Int32 UNIT_ID, Int32 HP)
    {
        if (!Units.ContainsKey(UNIT_ID)) return;
        Unit unit = Units[UNIT_ID];
        unit.RenewHP(HP);
    }

    public void S_PLAYER_DIE(Int32 UNIT_ID)
    {
        if (!Units.ContainsKey(UNIT_ID)) return;
        Unit unit = Units[UNIT_ID];
        unit.Die();
        Units.Remove(unit.m_id);

        if (TestManager.TestMode)
        {
            if (unit.m_team == GamaManager.Instance.Team)
            {
                UnitControllers.Remove(UNIT_ID);

                // 선택된 유닛이라면 제거
                GamaManager.UnitSelection.SelectableUnitDestroyed(unit.gameObject);

                GamaManager.Instance.AliveUnitCnt--;
                if (GamaManager.Instance.AliveUnitCnt == 0)
                {
                    // Select 씬으로 이동!
                    Manager.Scene.LoadScene(Define.Scene.SelectField);
                }
            }
            else
            {
                // 선택된 유닛이라면 제거
                GamaManager.UnitSelection.SelectableUnitDestroyed(unit.gameObject);
            }
        }
        else
        {
            if (unit.m_team == GamaManager.Instance.Team)
            {
                UnitControllers.Remove(UNIT_ID);

                // 선택된 유닛이라면 제거
                GamaManager.UnitSelection.SelectableUnitDestroyed(unit.gameObject);

                GamaManager.Instance.AliveUnitCnt--;
                if (GamaManager.Instance.AliveUnitCnt == 0)
                {
                    // Select 씬으로 이동!
                    Manager.Scene.LoadScene(Define.Scene.SelectField);
                }
            }
        }
    }

    private GameObject CreateUnitObjectInScene(Int32 UNIT_ID, byte UNIT_TYPE, byte TEAM, float POS_X, float POS_Z, float NORM_X, float NORM_Z, float SPEED, Int32 MAX_HP, Int32 HP, float RADIUS, float ATTACK_DISTANCE, float ATTACK_RATE)
    {
        string prefabName = string.Empty;
        enUNIT_TYPE unitType = (enUNIT_TYPE)UNIT_TYPE;
        switch (unitType)
        {
            //case enUNIT_TYPE.Terran_Marine: prefabName = "Marine_new"; break;
            //case enUNIT_TYPE.Terran_Firebat: prefabName = "Firebat_new"; break;
            //case enUNIT_TYPE.Zerg_Zergling: prefabName = "Zergling_new"; break;
            //case enUNIT_TYPE.Zerg_Hydra: prefabName = "Hydra_new"; break;
            case enUNIT_TYPE.Terran_Marine: prefabName = "Marine_new"; break;
            case enUNIT_TYPE.Terran_Firebat: prefabName = "Firebat_new"; break;
            case enUNIT_TYPE.Zerg_Zergling: prefabName = "Zergling_new"; break;
            case enUNIT_TYPE.Zerg_Hydra: prefabName = "Hydra_new"; break;


            //case enUNIT_TYPE.Terran_Tank: prefabName = "Tank"; break;
            //case enUNIT_TYPE.Terran_Robocop:  prefabName = "Robocop"; break;
            //case enUNIT_TYPE.Zerg_Golem: prefabName = "Golem"; break;
            //case enUNIT_TYPE.Zerg_Tarantula: prefabName = "Tarantula"; break;
            default: break;
        }

        GameObject unitObj = Manager.Resource.Instantiate($"Unit/{prefabName}");
        if (unitObj != null)
        {
            unitObj.transform.position = new Vector3(POS_X, 0, POS_Z);
            unitObj.transform.forward = new Vector3(NORM_X, 0, NORM_Z);

            Unit unit = unitObj.GetComponent<Unit>();
            if (unit == null)
            {
                unit.AddComponent<Unit>();
            }
            unit.Init(UNIT_ID, UNIT_TYPE, TEAM, new Vector3(POS_X, 0, POS_Z), new Vector3(NORM_X, 0, NORM_Z), SPEED, HP, MAX_HP, RADIUS, ATTACK_DISTANCE, ATTACK_RATE);
        }
        return unitObj;
    }
}
