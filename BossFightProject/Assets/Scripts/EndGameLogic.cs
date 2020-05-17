using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndGameLogic : MonoBehaviour
{
    GameObject player;
    GameObject boss;

    PlayerLogic playerLogic;
    BossLogic bossLogic;

    [SerializeField]
    Button restartButton;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        boss = GameObject.FindGameObjectWithTag("Boss");
        playerLogic = player.GetComponent<PlayerLogic>();
        bossLogic = boss.GetComponent<BossLogic>();

        if(restartButton) {
            restartButton.Select();
        }
    }

    public void OnRestartClicked() {
        SceneManager.LoadScene("James Scene 2");
    }

    public void OnExitClicked() {
        Application.Quit();
    }
}
