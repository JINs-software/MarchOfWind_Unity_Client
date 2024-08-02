using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // ���� ���� ���� Ȯ�� �޽��� ����
        MSG_COM_REQUEST req = new MSG_COM_REQUEST();
        Manager.Network.SetRequstMessage(req, enProtocolComRequest.REQ_ENTER_TO_SELECT_FIELD);
        if (!Manager.Network.SendPacket<MSG_COM_REQUEST>(req))
        {
            Debug.Log("�ε� ��, REQ_ENTER_TO_GAME_MODE �۽� ����");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Manager.Network.ReceiveDataAvailable())
        {
            MSG_REPLY_ENTER_TO_SELECT_FIELD reply = new MSG_REPLY_ENTER_TO_SELECT_FIELD();    
            if(!Manager.Network.ReceivePacket<MSG_REPLY_ENTER_TO_SELECT_FIELD>(out reply))
            {
                Debug.Log("�ε� ��, ���� ��� ���� ����");
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
