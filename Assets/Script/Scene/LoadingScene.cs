using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // ���� ���� ���� Ȯ�� �޽��� ����
        MSG_COM_REQUEST req = new MSG_COM_REQUEST();
        //Manager.Network.SetRequstMessage(req, enProtocolComRequest.REQ_ENTER_TO_SELECT_FIELD);
        req.type = (ushort)enPacketType.COM_REQUSET;
        req.type = (ushort)enProtocolComRequest.REQ_ENTER_TO_SELECT_FIELD;
        Manager.Network.SendPacket<MSG_COM_REQUEST>(req);
    }

    // Update is called once per frame
    void Update()
    {
        if (Manager.Network.ReceiveDataAvailable())
        {
            MSG_REPLY_ENTER_TO_SELECT_FIELD reply = new MSG_REPLY_ENTER_TO_SELECT_FIELD();    
            if(!Manager.Network.ReceivePacket<MSG_REPLY_ENTER_TO_SELECT_FIELD>(out reply))
            {
                //Debug.Log("�ε� ��, ���� ��� ���� ����");
                return;
            }
            else
            {
                Manager.GamePlayer.BattleFieldID = reply.fieldID;
                Manager.SceneTransfer.TransferToSelectField();
            }
        }
    }
}
