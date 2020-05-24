using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenuLogic : MonoBehaviour
{

    [SerializeField]
    Button m_startButton;
    [SerializeField]
    Button m_exitButton;

    [SerializeField]
    GameObject m_TransitionCamera;
    [SerializeField]
    GameObject m_startMenu;

    [SerializeField]
    GameObject m_bossHealth;
    [SerializeField]
    GameObject m_playerHealth;
    // Start is called before the first frame update
     void Start()
    {
        if (m_startButton)
        {
            m_startButton.Select();
        }
    }

    public void OnStartClicked()
    {
        m_TransitionCamera.SetActive(true);
        m_bossHealth.SetActive(true);
        m_playerHealth.SetActive(true);
        m_startMenu.SetActive(false);
    }

    public void OnExitClicked()
    {
        Application.Quit();
    }

    public void DisableAllChildren(){

    }
}
