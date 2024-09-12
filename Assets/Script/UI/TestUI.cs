
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TestUI : UI_Base
{
    enum Buttons
    {
        DebugBtn,
        ConnectBtn,
    }

    enum Toggles
    {
        MarineToggle,
        FirebatToggle,
        ZerglingToggle,
        HydraToggle,
        MarineEnemyToggle,
        FirebatEnemyToggle,
        ZerglingEnemyToggle,
        HydraEnemyToggle,

        MarineDummyToggle,
        FirebatDummyToggle,
        ZerglingDummyToggle,
        HydraDummyToggle
    }

    Button debugBtn;
    Button connectBtn;
    Toggle MarineToggle;
    Toggle FirebatToggle;
    Toggle ZerglingToggle;
    Toggle HydraToggle;
    Toggle MarineEnemyToggle;
    Toggle FirebatEnemyToggle;
    Toggle ZerglingEnemyToggle;
    Toggle HydraEnemyToggle;
    Toggle MarineDummyToggle;
    Toggle FirebatDummyToggle;
    Toggle ZerglingDummyToggle;
    Toggle HydraDummyToggle;

    public Action DebugBtnHandler;
    public Action ConnectBtnHandler;
    public Action MarineToggleHandler;
    public Action FirebatToggleHandler;
    public Action ZerglingToggleHandler;
    public Action HydraToggleHandler;
    public Action MarineEnemyToggleHandler;
    public Action FirebatEnemyToggleHandler;
    public Action ZerglingEnemyToggleHandler;
    public Action HydraEnemyToggleHandler;
    public Action MarineDummyToggleHandler;
    public Action FirebatDummyToggleHandler;
    public Action ZerglingDummyToggleHandler;
    public Action HydraDummyToggleHandler;

    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        Bind<Toggle>(typeof(Toggles));

        debugBtn = GetButton((int)Buttons.DebugBtn);
        connectBtn = GetButton((int)Buttons.ConnectBtn);
        MarineToggle = Get<Toggle>((int)Toggles.MarineToggle);
        FirebatToggle = Get<Toggle>((int)Toggles.FirebatToggle);
        ZerglingToggle = Get<Toggle>((int)Toggles.ZerglingToggle);
        HydraToggle = Get<Toggle>((int)Toggles.HydraToggle);
        MarineEnemyToggle = Get<Toggle>((int)Toggles.MarineEnemyToggle);
        FirebatEnemyToggle = Get<Toggle>((int)Toggles.FirebatEnemyToggle);
        ZerglingEnemyToggle = Get<Toggle>((int)Toggles.ZerglingEnemyToggle);
        HydraEnemyToggle = Get<Toggle>((int)Toggles.HydraEnemyToggle);
        MarineDummyToggle = Get<Toggle>((int)Toggles.MarineDummyToggle);
        FirebatDummyToggle = Get<Toggle>((int)Toggles.FirebatDummyToggle);
        ZerglingDummyToggle = Get<Toggle>((int)Toggles.ZerglingDummyToggle);
        HydraDummyToggle = Get<Toggle>((int)Toggles.HydraDummyToggle);

        BindEvent(debugBtn.gameObject, OnDebugBtnClick);
        BindEvent(connectBtn.gameObject, OnConnectBtnClick);
        BindEvent(MarineToggle.gameObject, OnMarineToggleClick);
        BindEvent(FirebatToggle.gameObject, OnFirebatToggleClick);
        BindEvent(ZerglingToggle.gameObject, OnZerglingToggleClick);
        BindEvent(HydraToggle.gameObject, OnHydraToggleClick);
        BindEvent(MarineEnemyToggle.gameObject, OnMarineEnemyToggleClick);
        BindEvent(FirebatEnemyToggle.gameObject, OnFirebatEnemyToggleClick);
        BindEvent(ZerglingEnemyToggle.gameObject, OnZerglingEnemyToggleClick);
        BindEvent(HydraEnemyToggle.gameObject, OnHydraEnemyToggleClick);
        BindEvent(MarineDummyToggle.gameObject, OnMarineDummyToggleClick);
        BindEvent(FirebatDummyToggle.gameObject, OnFirebatDummyToggleClick);
        BindEvent(ZerglingDummyToggle.gameObject, OnZerglingDummyToggleClick);
        BindEvent(HydraDummyToggle.gameObject, OnHydraDummyToggleClick);

        TogglesOff();
    }

    private void Awake()
    {
        Init();
    }

    public void DebugOn()
    {
        debugBtn.interactable = false;

        connectBtn.interactable = true;
        TogglesOn();
    }
    public void DebugOff()
    {
        debugBtn.interactable = true;
        connectBtn.interactable = false;
        TogglesOff();
    }

    public void TogglesOn()
    {
        MarineToggle.isOn = false;
        FirebatToggle.isOn = false;
        ZerglingToggle.isOn = false;
        HydraToggle.isOn = false;
        MarineEnemyToggle.isOn = false;
        FirebatEnemyToggle.isOn = false;
        ZerglingEnemyToggle.isOn = false;
        HydraEnemyToggle.isOn = false;
        MarineDummyToggle.isOn = false;
        FirebatDummyToggle.isOn = false;
        ZerglingDummyToggle.isOn = false;
        HydraDummyToggle.isOn = false;

        MarineToggle.interactable = true;  
        FirebatToggle.interactable = true;
        ZerglingToggle.interactable = true;
        HydraToggle.interactable = true;
        MarineEnemyToggle.interactable = true;
        FirebatEnemyToggle.interactable = true;
        ZerglingEnemyToggle.interactable = true;
        HydraEnemyToggle.interactable = true;
        MarineDummyToggle.interactable = true;
        FirebatDummyToggle.interactable = true;
        ZerglingDummyToggle.interactable = true;
        HydraDummyToggle.interactable = true;
    }
    public void TogglesOff()
    {
        MarineToggle.isOn = false;
        FirebatToggle.isOn = false;
        ZerglingToggle.isOn = false;
        HydraToggle.isOn = false;
        MarineEnemyToggle.isOn = false;
        FirebatEnemyToggle.isOn = false;
        ZerglingEnemyToggle.isOn = false;
        HydraEnemyToggle.isOn = false;
        MarineDummyToggle.isOn = false;
        FirebatDummyToggle.isOn = false;
        ZerglingDummyToggle.isOn = false;
        HydraDummyToggle.isOn = false;

        MarineToggle.interactable = false;
        FirebatToggle.interactable = false;
        ZerglingToggle.interactable = false;
        HydraToggle.interactable = false;
        MarineEnemyToggle.interactable = false;
        FirebatEnemyToggle.interactable = false;
        ZerglingEnemyToggle.interactable = false;
        HydraEnemyToggle.interactable = false;
        MarineDummyToggle.interactable = false;
        FirebatDummyToggle.interactable = false;
        ZerglingDummyToggle.interactable = false;
        HydraDummyToggle.interactable = false;
    }

    void OnDebugBtnClick(PointerEventData evtdata) { 
        DebugBtnHandler.Invoke(); 
    }
    void OnConnectBtnClick(PointerEventData evtdata) { ConnectBtnHandler.Invoke(); }
    void OnMarineToggleClick(PointerEventData evtdata) { if (MarineToggle.isOn) MarineToggleHandler.Invoke(); }
    void OnFirebatToggleClick(PointerEventData evtdata) { if (FirebatToggle.isOn) FirebatToggleHandler.Invoke(); }
    void OnZerglingToggleClick(PointerEventData evtdata) { if (ZerglingToggle.isOn) ZerglingToggleHandler.Invoke(); }
    void OnHydraToggleClick(PointerEventData evtdata) { if (HydraToggle.isOn) HydraToggleHandler.Invoke(); }
    void OnMarineEnemyToggleClick(PointerEventData evtdata) { if (MarineEnemyToggle.isOn) MarineEnemyToggleHandler.Invoke(); }
    void OnFirebatEnemyToggleClick(PointerEventData evtdata) { if (FirebatEnemyToggle.isOn) FirebatEnemyToggleHandler.Invoke(); }
    void OnZerglingEnemyToggleClick(PointerEventData evtdata) { if (ZerglingEnemyToggle.isOn) ZerglingEnemyToggleHandler.Invoke(); }
    void OnHydraEnemyToggleClick(PointerEventData evtdata) { if (HydraEnemyToggle.isOn) HydraEnemyToggleHandler.Invoke(); }
    void OnMarineDummyToggleClick(PointerEventData evtdata) { if (MarineDummyToggle.isOn) MarineDummyToggleHandler.Invoke(); }
    void OnFirebatDummyToggleClick(PointerEventData evtdata) { if (FirebatDummyToggle.isOn) FirebatDummyToggleHandler.Invoke(); }
    void OnZerglingDummyToggleClick(PointerEventData evtdata) { if (ZerglingDummyToggle.isOn) ZerglingDummyToggleHandler.Invoke(); }
    void OnHydraDummyToggleClick(PointerEventData evtdata) { if (HydraDummyToggle.isOn) HydraDummyToggleHandler.Invoke(); }
}