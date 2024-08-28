using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
using UnityEngine.XR;
using Text = UnityEngine.UI.Text;

public class CreateMatchUI : UI_Base
{
    enum InputFields
    {
        RoomNameInput,
    }

    enum Buttons
    {
        CreateBtn,
        CancelBtn,
    }

    enum Dropdowns
    {
        NumSelectDropdown,
    }

    enum Texts
    {
        StatusText,
    }

    public Action CancelBtnHandler;

    Button createBtn;
    Button cancelBtn;
    InputField roomNameInput;
    Dropdown numSelectDropdown;
    Text statusText;

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        Bind<InputField>(typeof(InputFields));
        Bind<Button>(typeof(Buttons));
        Bind<Dropdown>(typeof(Dropdowns));
        Bind<Text>(typeof(Texts));  

        createBtn = GetButton((int)Buttons.CreateBtn);
        cancelBtn = GetButton((int)Buttons.CancelBtn);
        roomNameInput = Get<InputField>((int)InputFields.RoomNameInput);
        numSelectDropdown = Get<Dropdown>((int)Dropdowns.NumSelectDropdown);
        statusText = Get<Text>((int)Texts.StatusText);

        BindEvent(createBtn.gameObject, OnCreateBtnClicked);
        BindEvent(cancelBtn.gameObject, OnCancelBtnClicked);
    }

    public void ResetUI()
    {
        roomNameInput.text = "";
        roomNameInput.interactable = true;
        createBtn.interactable = true;
        cancelBtn.interactable = true;
    }
    public void SetOnlyCancelBtn()
    {
        roomNameInput.text = "";
        roomNameInput.interactable = false;
        createBtn.interactable = false;
        cancelBtn.interactable = true;
    }
    public void SetStatusText(string text)
    {
        statusText.text = text; 
    }

    private void OnCreateBtnClicked(PointerEventData data)
    {
        string roomName = roomNameInput.text;
        if(string.IsNullOrEmpty(roomName))
        {
            return;
        }

        int selectedIndex = numSelectDropdown.value;
        string selectedOption = numSelectDropdown.options[selectedIndex].text;
        byte selectNum = byte.Parse(selectedOption);

        byte[] roomNameBytes = Encoding.ASCII.GetBytes(roomName);
        RPC.proxy.CREATE_MATCH_ROOM(roomNameBytes, (Byte)roomName.Length, selectNum);

        roomNameInput.interactable = false;
        createBtn.interactable = false;
        cancelBtn.interactable = false; 
    }

    private void OnCancelBtnClicked(PointerEventData data)
    {
        CancelBtnHandler.Invoke();
    }
}
