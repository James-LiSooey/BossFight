using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponLogic : MonoBehaviour
{
    public bool activated;
    public float rotationSpeed;

    GameObject player;
    GameObject boss;
    PlayerLogic playerLogic;
    BossLogic bossLogic;

    Rigidbody rigidBody;

    [SerializeField]
    TrailRenderer trail;
    [SerializeField]
    GameObject bossRoot;
    
    private void Start() {
        player = GameObject.FindGameObjectWithTag("Player");
        boss = GameObject.FindGameObjectWithTag("Boss");
        playerLogic = player.GetComponent<PlayerLogic>();
        bossLogic = boss.GetComponent<BossLogic>();
        rigidBody = GetComponent<Rigidbody>();
    }

    void OnEnable() {
        player = GameObject.FindGameObjectWithTag("Player");
        boss = GameObject.FindGameObjectWithTag("Boss");
        playerLogic = player.GetComponent<PlayerLogic>();
        bossLogic = boss.GetComponent<BossLogic>();
    }

    void Update()
    {
        if(!player){
            player = GameObject.FindGameObjectWithTag("Player");
            if(player){
            playerLogic = player.GetComponent<PlayerLogic>();
            }
        }
        if(!boss){
            boss = GameObject.FindGameObjectWithTag("Boss");
            if(boss){
            bossLogic = boss.GetComponent<BossLogic>();
            }
        }

        if (activated)
        {
            transform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other) {
        //Debug.Log("Layer: " + other.gameObject.layer);
        
        if(!player || !boss){
            return;
        }

        if(other.tag == "Boss") {
             if(bossLogic) {
                 if(!playerLogic.hasWeapon) {
                    trail.emitting = false;

                    rigidBody.Sleep();
                    rigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                    rigidBody.isKinematic = true;
                    activated = false;
                    
                    transform.parent = bossRoot.transform;
                 }
                 
            }
                if(playerLogic.dealDamage) {
                    if(playerLogic.attackType == AttackType.Regular) {

                        other.GetComponent<BossLogic>().TakeDamage(playerLogic.regularAttackDamage);
                    } else if (playerLogic.attackType == AttackType.Strong) {
                        other.GetComponent<BossLogic>().TakeDamage(playerLogic.strongAttackDamage);
                    } else if(playerLogic.attackType == AttackType.Jump) {
                        other.GetComponent<BossLogic>().TakeDamage(playerLogic.jumpAttackDamage);
                    } else if(playerLogic.attackType == AttackType.Throw) {
                        other.GetComponent<BossLogic>().TakeDamage(playerLogic.throwAttackDamage);
                    }
                    playerLogic.dealDamage = false;
                }
            }
        }
    }