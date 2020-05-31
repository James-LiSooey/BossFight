using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRightFootLogic : MonoBehaviour
{
     GameObject player;
    GameObject boss;
    PlayerLogic playerLogic;
    BossLogic bossLogic;

    Animator playerAnimator;
    
    private void Start() {
        player = GameObject.FindGameObjectWithTag("Player");
        boss = GameObject.FindGameObjectWithTag("Boss");
        playerLogic = player.GetComponent<PlayerLogic>();
        bossLogic = boss.GetComponent<BossLogic>();
        playerAnimator = player.GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player") {
            if(playerLogic) {
                if(bossLogic.dealDamage) {
                    playerLogic.GotHit(0);
                    bossLogic.dealDamage = false;
                }
            }
        }
    }
}
