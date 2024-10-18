
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChattingUI : UI_Base
{
    enum InputFields
    {
        ChattingInput,
    }

    public Action<string> ChatHandler;
    GameObject chatListContent;

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        Bind<InputField>(typeof(InputFields));

        InputField chatInput = Get<InputField>((int)InputFields.ChattingInput);
        chatInput.onEndEdit.AddListener(OnEndEdit);
        chatInput.interactable = false;

        chatListContent = Util.FindChild(gameObject, "Content", true);
    }

    public void BeInteractable()
    {
        Get<InputField>((int)InputFields.ChattingInput).interactable = true;    
    }

    public void DisplayChat(string name, string chat)
    {
        GameObject chatObj = new GameObject("ChatText");
        chatObj.transform.SetParent(chatListContent.transform);
        
        Text chatText = chatObj.AddComponent<Text>();
        chatText.text = " " + name + ": " + chat;
        chatText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        chatText.fontSize = 20;
        chatText.color = Color.white;   
    }

    private void OnEndEdit(string chat) {
        InputField chatInput = Get<InputField>((int)InputFields.ChattingInput);

        if (EventSystem.current.currentSelectedGameObject == chatInput.gameObject &&
            (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            ChatHandler.Invoke(chat);
            chatInput.text = "";
            chatInput.ActivateInputField();
        }
    }
}