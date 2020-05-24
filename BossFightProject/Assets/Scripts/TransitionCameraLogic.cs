using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionCameraLogic : MonoBehaviour
{

    [SerializeField]
    GameObject m_freelookCamera;
    [SerializeField]
    GameObject m_player;
    [SerializeField]
    GameObject m_boss;

    PlayerLogic m_playerLogic;

    public float m_transitionTimer = 0;
    float m_transitionTimelimit = 3f;
    // Start is called before the first frame update
    void Start()
    {
        m_boss.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if(m_transitionTimer<=m_transitionTimelimit){
            m_transitionTimer += Time.fixedDeltaTime;
        }
        if(m_transitionTimer>m_transitionTimelimit-.5f){
            m_freelookCamera.SetActive(true);
        }
        if(m_transitionTimer>m_transitionTimelimit){
            m_playerLogic = m_player.GetComponent<PlayerLogic>();
            m_playerLogic.enabled = true;
        }
    }
}
