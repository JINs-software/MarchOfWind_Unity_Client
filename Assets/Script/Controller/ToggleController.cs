using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleController : MonoBehaviour
{
    Toggle m_Toggle;

    // Start is called before the first frame update
    void Start()
    {
        if(Manager.DebugManager.DebugMode)
        {
            m_Toggle = GetComponent<Toggle>();
            m_Toggle.onValueChanged.AddListener(delegate {
                ToggleValueChanged(m_Toggle);
            });
        }
        else
        {
            gameObject.SetActive(false);    
        }
    }

    void ToggleValueChanged(Toggle change)
    {
        if (m_Toggle.isOn)
        {
            if (m_Toggle.name == "DummyCrtToggle")
            {
                if (!Manager.DebugManager.OnDummyToggle())
                {
                    m_Toggle.isOn = false;
                }
            }
            else if (m_Toggle.name == "EnemyCrtToggle")
            {
                if (!Manager.DebugManager.OnEnemyToggle())
                {
                    m_Toggle.isOn = false;
                }
            }
        }
        else
        {
            if (m_Toggle.name == "DummyCrtToggle")
            {
                Manager.DebugManager.OffDummyToggle();
            }
            else if (m_Toggle.name == "EnemyCrtToggle")
            {
                Manager.DebugManager.OffEnemyToggle();
            }
        }
    }
}
