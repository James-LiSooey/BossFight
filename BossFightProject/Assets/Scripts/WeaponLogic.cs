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

    // private void OnCollisionEnter(Collision collision)
    // {
    //     Debug.Log("Layer: " + collision.gameObject.layer);
    //     if (collision.gameObject.layer == 9 || collision.gameObject.layer == 10 || collision.gameObject.layer == 11)
    //     {
    //         //print(collision.gameObject.name);
    //         GetComponent<Rigidbody>().Sleep();
    //         GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
    //         GetComponent<Rigidbody>().isKinematic = true;
    //         activated = false;
    //     }
    // }

    private void OnTriggerEnter(Collider other) {
        //Debug.Log("Layer: " + other.gameObject.layer);
        
        if(!player || !boss){
            return;
        }
        
        if(other.tag == "Boss") {
            Debug.Log("DealDamage: " + playerLogic.dealDamage);
            Debug.Log("AttackType: " + playerLogic.attackType);

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
                        bossLogic.TakeDamage(10);
                    } else if (playerLogic.attackType == AttackType.Strong) {
                        bossLogic.TakeDamage(20);
                    } else if(playerLogic.attackType == AttackType.Jump) {
                    bossLogic.TakeDamage(50);
                    }
                    else if(playerLogic.attackType == AttackType.Throw) {
                    bossLogic.TakeDamage(30);
                    }
                    playerLogic.dealDamage = false;
                }
            }
        }
    }