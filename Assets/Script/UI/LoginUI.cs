
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LoginUI : UI_Base
{
    enum InputFields
    {
        LoginServerIpInput,
        LoginServerPortInput,
        AccountIdInput,
        AccountPasswordInput
    }

    enum Buttons
    {
        LoginServerSetBtn,
        CreateBtn,
        LoginBtn
    }

    enum Texts
    {
        StatusText,
    }

    public Action<string, UInt16> LoginServerSetBtnHandler;
    public Action<string, string> CreateBtnHandler;
    public Action<string, string> LoginBtnHandler;

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        Bind<InputField>(typeof(InputFields));
        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));

        BindEvent(GetButton((int)Buttons.LoginServerSetBtn).gameObject, OnLoginServerSetBtnClicked);
        BindEvent(GetButton((int)Buttons.CreateBtn).gameObject, OnCreateAccountBtnClicked);
        BindEvent(GetButton((int)Buttons.LoginBtn).gameObject, OnLoginBtnClicked);
    }

    public void ResetStatusText(string status)
    {
        Get<Text>((int)Texts.StatusText).text = status; 
    }

    // Handlers
    private void OnLoginServerSetBtnClicked(PointerEventData data)
    {
        InputField loginServerIpInput = Get<InputField>((int)InputFields.LoginServerIpInput);
        InputField loginServerPortInput = Get<InputField>((int)InputFields.LoginServerPortInput);

        LoginServerSetBtnHandler.Invoke(loginServerIpInput.text, UInt16.Parse(loginServerPortInput.text));  
    }

    private void OnCreateAccountBtnClicked(PointerEventData data)
    {
        InputField accountIdInput = Get<InputField>((int)InputFields.AccountIdInput);
        InputField accoutPasswordInput = Get<InputField>((int)InputFields.AccountPasswordInput);

        CreateBtnHandler.Invoke(accountIdInput.text, accoutPasswordInput.text);
    }

    private void OnLoginBtnClicked(PointerEventData data)
    {
        InputField accountIdInput = Get<InputField>((int)InputFields.AccountIdInput);
        InputField accoutPasswordInput = Get<InputField>((int)InputFields.AccountPasswordInput);

        LoginBtnHandler.Invoke(accountIdInput.text, accoutPasswordInput.text);
    }
}