using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // 게임 시작 여부 확인 메시지 전송
        MSG_COM_REQUEST req = new MSG_COM_REQUEST();
        Manager.Network.SetRequstMessage(req, enProtocolComRequest.REQ_ENTER_TO_SELECT_FIELD);
        if (!Manager.Network.SendPacket<MSG_COM_REQUEST>(req))
        {
            Debug.Log("로딩 씬, REQ_ENTER_TO_GAME_MODE 송신 실패");
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
                Debug.Log("로딩 씬, 게임 모드 접근 실패");
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
