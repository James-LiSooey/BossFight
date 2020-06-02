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
            //Debug.Log("Inside RightFoot OnTriggerEnter");
            //if(playerLogic) {
                if(GetComponentInParent<BossLogic>().dealDamage) {

                    if(GetComponentInParent<BossLogic>().bossAttackState == BossAttackState.Stomp) {
                        //Debug.Log("Boss Stomp Attack");
                        other.GetComponent<PlayerLogic>().GotHit(0);                    
                    } else if(GetComponentInParent<BossLogic>().bossAttackState == BossAttackState.Jump) {
                        //Debug.Log("Boss Jump Attack");
                        other.GetComponent<PlayerLogic>().GotHit(3);
                    }
                    GetComponentInParent<BossLogic>().dealDamage = false;
                }
            //}
        }   
    }
}
