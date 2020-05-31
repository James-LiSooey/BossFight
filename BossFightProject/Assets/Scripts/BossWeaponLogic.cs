using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossWeaponLogic : MonoBehaviour
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
                    if(bossLogic.bossAttackState == BossAttackState.Jump) {
                        playerLogic.GotHit(3);
                    } else if(bossLogic.bossAttackState == BossAttackState.Slam) {
                        playerLogic.GotHit(2);
                    } else if(bossLogic.bossAttackState == BossAttackState.Swipe) {
                        playerLogic.GotHit(1);
                    }
                    bossLogic.dealDamage = false;
                }
            }
        }
    }
}
