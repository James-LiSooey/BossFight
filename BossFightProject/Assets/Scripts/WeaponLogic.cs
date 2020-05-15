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
    
    private void Start() {
        player = GameObject.FindGameObjectWithTag("Player");
        boss = GameObject.FindGameObjectWithTag("Boss");
        playerLogic = player.GetComponent<PlayerLogic>();
        bossLogic = boss.GetComponent<BossLogic>();
    }

    void Update()
    {
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
        Debug.Log("Layer: " + other.gameObject.layer);

        if (other.gameObject.layer == 9 || other.gameObject.layer == 10 || other.gameObject.layer == 11)
        {
            //print(collision.gameObject.name);
            GetComponent<Rigidbody>().Sleep();
            GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            GetComponent<Rigidbody>().isKinematic = true;
            activated = false;
        }

        if(other.tag == "Boss") {
            if(bossLogic) {
                 if(playerLogic.attackType == playerLogic.AttackType.Regular) {
                     bossLogic.TakeDamage(10);
                 } else if (playerLogic.attackType == playerLogic.AttackType.Strong) {
                     bossLogic.TakeDamage(20);
                 } else if(playerLogic.attackType == playerLogic.AttackType.Jump) {
                    bossLogic.TakeDamage(50);
                }
            }
        }
    }

    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.CompareTag("Breakable"))
    //     {
    //         if(other.GetComponent<BreakBoxScript>() != null)
    //         {
    //             other.GetComponent<BreakBoxScript>().Break();
    //         }
    //     }
    // }
}
