using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

// 전체 프로젝트에서 유닛 선택 매니저는 하나만 사용
// => 싱글톤으로 관리
public class UnitSelectionManager 
{
    // 모든 유닛이 생성될 시 m_AllUnitsList에 추가되어야 함
    public List<GameObject> m_AllUnitsList = new List<GameObject>();
    public List<GameObject> UnitList { get { return m_AllUnitsList; } } 

    // 모든 유닛 중 선택된 유닛 리스트
    public List<GameObject> m_UnitsSelected = new List<GameObject>();

    private LayerMask m_Clickable;
    private LayerMask m_Ground;
    private LayerMask m_Attackable;
    public bool m_AttackCursorVisible;

    private GameObject m_GroundMarker;

    private Camera m_Camera;

    //Vector3 m_CenterOfUnitSelected;
    //public Vector3 CenterOfUnitSelected { get { UpdateCenterOfUnitSelected(); return m_CenterOfUnitSelected; } }

    public float UnitSelectedCircumscriber = 0;

    public void Init()
    {
        m_Clickable = LayerMask.GetMask("Clickable");
        m_Ground = LayerMask.GetMask("GroundLayer");
        m_Attackable = LayerMask.GetMask("Attackable");

        
        m_GroundMarker = GameObject.Find("GroundMarker");
        m_GroundMarker.SetActive(false);

        m_Camera = Camera.main;

        DeSelectAll();
    }

    

    public void Update()
    {
        // 마우스 좌클릭 유닛 선택
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = m_Camera.ScreenPointToRay(Input.mousePosition);

            // clickable 객체를 선택할 때 
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, m_Clickable))
            {
                // 다중 선택
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    MultiSelect(hit.collider.gameObject);
                }
                else
                {
                    SelectByClicking(hit.collider.gameObject);
                }
            }
            else
            {
                if (!Input.GetKey(KeyCode.LeftShift))
                {
                    // 선택 취소
                    DeSelectAll();
                }
            }
        }

        // 선택된 유닛이 존재한 상태에서 마우스 우클릭
        if (Input.GetMouseButtonDown(1) && m_UnitsSelected.Count > 0)
        {
            RaycastHit hit;
            Ray ray = m_Camera.ScreenPointToRay(Input.mousePosition);

            // clickable 객체를 선택할 때 
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, m_Ground))
            {
                m_GroundMarker.transform.position = hit.point;
                m_GroundMarker.SetActive(true);
                m_GroundMarker.GetComponent<GroundMarker>().SetGreenMark();
                m_GroundMarker.GetComponent<GroundMarker>().StayActive();

                foreach (var unit in m_UnitsSelected)
                {
                    if(unit.GetComponent<AttackController>() != null)
                    {
                        unit.GetComponent<AttackController>().m_TargetObject = null;
                        unit.GetComponent<UnitMovement>().TargetOnEnemy = false;
                    }
                }
            }
        }

        // 공격 가능 유닛이 선택된 상태
        // (공격 가능 유닛이란 AttackController를 컴포넌트로 갖고 있는 유닛)
        if (m_UnitsSelected.Count > 0 && AtLeastOneOffensiveUnit(m_UnitsSelected))
        {
            RaycastHit hit;
            Ray ray = m_Camera.ScreenPointToRay(Input.mousePosition);

            // clickable 객체를 선택할 때 
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, m_Attackable))
            {
                //Debug.Log("Enemy Hovered with mouse");

                m_AttackCursorVisible = true;

                // // 공격 가능 유닛이 선택된 상태에서 마우스 우클릭
                if (Input.GetMouseButtonDown(1))
                {
                    m_GroundMarker.GetComponent<GroundMarker>().SetRedMark();

                    //Transform target = hit.transform;
                    //
                    //foreach (GameObject unit in m_UnitsSelected)
                    //{
                    //    if (unit.GetComponent<AttackController>())
                    //    {
                    //        // 타겟 설정
                    //        // 타겟 설정 1. Trigger collider와 충돌 시
                    //        // 타겟 설정 2(현재). 직접 타겟을 지정할 때
                    //        unit.GetComponent<AttackController>().m_TargetToAttack = target;
                    //    }
                    //}

                    foreach (GameObject unit in m_UnitsSelected)
                    {
                        if(unit.GetComponent<AttackController>() != null)
                        {
                            unit.GetComponent<AttackController>().m_TargetObject = hit.collider.gameObject;

                            unit.GetComponent<UnitMovement>().TargetOnEnemy = true;
                        }
                    }
                }
            }
            else
            {
                m_AttackCursorVisible = false;
            }
        }

        CursorSelector();
    }

    private void CursorSelector()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, Mathf.Infinity, m_Clickable))
        {
            CursorManager.Instance.SetMarkerType(CursorManager.CursorType.Selectable);
        } 
        else if (Physics.Raycast(ray, out hit, Mathf.Infinity, m_Attackable)
            && m_UnitsSelected.Count > 0 && AtLeastOneOffensiveUnit(m_UnitsSelected))
        {
            CursorManager.Instance.SetMarkerType(CursorManager.CursorType.Attackable);
        }
        else if (Physics.Raycast(ray, out hit, Mathf.Infinity, m_Ground) 
            && m_UnitsSelected.Count > 0)
        {
            CursorManager.Instance.SetMarkerType(CursorManager.CursorType.Walkable);
        }
        else 
        {
            CursorManager.Instance.SetMarkerType(CursorManager.CursorType.None);
        }
    }

    private bool AtLeastOneOffensiveUnit(List<GameObject> m_UnitsSelected)
    {
        foreach (GameObject unit in m_UnitsSelected)
        {
            if (unit.GetComponent<AttackController>())
            {
                // 선택 유닛 목록 중 공격 가능 유닛이 하나라도 존재하는 경우 return true
                // (공격 가능 유닛이란 뜻은 AttackController를 들고 있는 유닛이란 뜻)
                return true;
            }
        }
        return false;
    }

    private void MultiSelect(GameObject unit)
    {
        if (m_UnitsSelected.Contains(unit))
        {
            SelectUnit(unit, false);
            m_UnitsSelected.Remove(unit);
        }
        else
        {
            m_UnitsSelected.Add(unit);
            SelectUnit(unit, true);
            UpdateUnitSelectedCircumscriber();   
        }
    }

    public void DeSelectAll()
    {
        foreach (var unit in m_UnitsSelected)
        {
            SelectUnit(unit, false);
        }

        if(m_GroundMarker == null)
        {
            m_GroundMarker = GameObject.Find("GroundMarker");
        }
        m_GroundMarker.SetActive(false);
        m_UnitsSelected.Clear();
    }

    public void SelectableUnitDestroyed(GameObject gameObject)
    {
        if(m_AllUnitsList.Contains(gameObject))
        {
            m_AllUnitsList.Remove(gameObject);  
        }
        if(m_UnitsSelected.Contains(gameObject))
        {
            SelectUnit(gameObject, false);
            m_UnitsSelected.Remove(gameObject); 
        }
    }

    private void SelectByClicking(GameObject unit)
    {
        DeSelectAll();

        m_UnitsSelected.Add(unit);
        SelectUnit(unit, true);
        UpdateUnitSelectedCircumscriber();
    }

    private void SelectUnit(GameObject unit, bool isSelected)
    {
        TriggerSeletionIndicator(unit, isSelected);
        EnableUnitMovement(unit, isSelected);
    }

    private void EnableUnitMovement(GameObject unit, bool shouldMove)
    {
        unit.GetComponent<UnitMovement>().enabled = shouldMove;
    }

    private void TriggerSeletionIndicator(GameObject unit, bool isVisible)
    {
        unit.transform.Find("Indicator").gameObject.SetActive(isVisible);
    }

    internal void DragSelect(GameObject unit)
    {
        if (!m_UnitsSelected.Contains(unit))
        {
            m_UnitsSelected.Add(unit);
            SelectUnit(unit, true);
            UpdateUnitSelectedCircumscriber();
        }
    }

    private void UpdateUnitSelectedCircumscriber()
    {
        List<float> radiusList = new List<float>();
        foreach (GameObject unit in m_UnitsSelected)
        {
            radiusList.Add(unit.GetComponent<NavMeshAgent>().radius * unit.transform.localScale.x);
        }

        radiusList.Sort((a, b) => b.CompareTo(a));

        UnitSelectedCircumscriber = radiusList[0];
        for (int i = 1; i < radiusList.Count;)
        {
            // 다음 반지름 
            int circumUnitCnt = (int)(2 * 3.14f * (UnitSelectedCircumscriber + radiusList[i]) / 2 * radiusList[i]);
            UnitSelectedCircumscriber += (radiusList[i] * 2);
            i += circumUnitCnt;
        }
    }
}