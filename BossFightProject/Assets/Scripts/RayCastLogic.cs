using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

enum PlayerState
{
    Idle,
    Moving,
    AttackMoving
}


public class RayCastLogic : MonoBehaviour
{
    NavMeshAgent m_navMeshAgent;

    [SerializeField]
    GameObject m_clickObj;
    [SerializeField]
    float m_clickObjSize = 0.0f;

    float m_meleeRange = 1.5f;

   // SwordLogic m_swordLogic;

    bool m_hasEnemyTarget = false;

    PlayerState m_playerState = PlayerState.Idle;

    GameObject m_enemyTarget;

    // Start is called before the first frame update
    void Start()
    {
        m_navMeshAgent = GetComponent<NavMeshAgent>();
        //m_swordLogic = GetComponentInChildren<SwordLogic>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            RaycastCameraToMouse();
        }
        if (m_clickObj && m_clickObjSize > 0)
            {
                m_clickObjSize -= Time.deltaTime;
                m_clickObj.transform.localScale = m_clickObjSize * (new Vector3(1,.1f,1));
        }

        if(m_enemyTarget && m_playerState == PlayerState.AttackMoving)
        {
            m_navMeshAgent.SetDestination(m_enemyTarget.transform.position);
            m_navMeshAgent.isStopped = false;
        }

        //CheckAttackRange();
    }

    void RaycastCameraToMouse()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit, 100.0f))
        {
            Debug.Log("We hit object: " + hit.collider.name);
            Debug.Log("Object hit at position: " + hit.point);

            m_navMeshAgent.SetDestination(hit.point);
            m_navMeshAgent.isStopped = false;

            DisplayClickObj(hit.point);

            if(hit.collider.gameObject.tag == "Enemy")
            {
                m_enemyTarget = hit.collider.gameObject;
                m_playerState = PlayerState.AttackMoving;
            }
            else
            {
                m_enemyTarget = null;
                m_playerState = PlayerState.Moving;
            }
        }
    }

    void DisplayClickObj(Vector3 pos)
    {
        if (m_clickObj)
        {
            m_clickObjSize = 1.0f;
            m_clickObj.transform.localScale =new  Vector3(1, .1f, 1);
            m_clickObj.transform.position = pos;
            Debug.Log("clickobject: " + m_clickObj.transform.position);
        }
    }

    /*
    void CheckAttackRange()
    {
        if (!m_enemyTarget || m_playerState != PlayerState.AttackMoving)
        {
            return;
        }

        Debug.DrawRay(transform.position, transform.forward * m_meleeRange, Color.red);

        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
        if(Physics.Raycast(ray, out hit, m_meleeRange))
        {
            if(hit.collider.gameObject.tag == "Enemy")
            {
                if (m_swordLogic)
                {
                    m_swordLogic.SetAttacking(true);
                }
                m_navMeshAgent.isStopped = true;
            }
        }

    }

    public EnemyLogic GetTargetEnemyLogic()
    {
        if (m_enemyTarget)
        {
            EnemyLogic enemyLogic = m_enemyTarget.GetComponent<EnemyLogic>();
            if (enemyLogic)
            {
                return enemyLogic;
            }
        }
        return null;
    }*/
}
