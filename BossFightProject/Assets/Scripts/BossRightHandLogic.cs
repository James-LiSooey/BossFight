using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRightHandLogic : MonoBehaviour
{
    GameObject player;
    GameObject boss;
    PlayerLogic playerLogic;
    BossLogic bossLogic;
    
    private void Start() {
        player = GameObject.FindGameObjectWithTag("Player");
        boss = GameObject.FindGameObjectWithTag("Boss");
        playerLogic = player.GetComponent<PlayerLogic>();
        bossLogic = boss.GetComponent<BossLogic>();
    }

    // private void OnEnable() {
    //     player = GameObject.FindGameObjectWithTag("Player");
    //     boss = GameObject.FindGameObjectWithTag("Boss");
    //     playerLogic = player.GetComponent<PlayerLogic>();
    //     bossLogic = boss.GetComponent<BossLogic>();
    // }

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player") {
            //Debug.Log("Inside RightHand OnTriggerEnter");
            if(playerLogic) {
                if(GetComponentInParent<BossLogic>().dealDamage) {
                    if(GetComponentInParent<BossLogic>().bossAttackState == BossAttackState.Jump) {
                        //Debug.Log("Boss Jump Attack");
                        other.GetComponent<PlayerLogic>().GotHit(3);
                    }
                    else if(GetComponentInParent<BossLogic>().bossAttackState == BossAttackState.Slam) {
                        //Debug.Log("Boss Slam Attack");
                        other.GetComponent<PlayerLogic>().GotHit(2);
                    }
                    GetComponentInParent<BossLogic>().dealDamage = false;
                }
            }
        }
    }
}
