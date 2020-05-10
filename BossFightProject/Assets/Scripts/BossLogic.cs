using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

enum BossState
{
    Idle,
    Moving,
    Attacking,
    Turning
}
enum BossAttackState
{
    Jump,
    Swipe,
    Slam,
    Stomp,
    ready,
    notReady,
}


public class BossLogic : MonoBehaviour
{
    BossState m_bossState = BossState.Idle;
    BossAttackState bossAttackState = BossAttackState.notReady;
    CharacterController m_characterController;
    Animator m_animator;
    GameObject m_player;

    Vector3 m_movementTarget;
    Vector3 m_horizontalMovement;
    Vector3 m_verticalMovement;
    Vector3 m_heightMovement;
    public float m_movementSpeed = 2.0f;
    public float m_currentSpeed = 0f;
    public float m_acceleration = .01f;
    public Boolean m_closing = false;
    public float m_closingDistance = 0;

    public float m_distanceToTarget;
    float m_gravity = 0.01f;

    public float m_turnAngle = 20f;
    public float m_stoppingDistance = 3.0f;

    [SerializeField]
    Collider m_weapon;
    [SerializeField]
    Collider m_rightHand;
    [SerializeField]
    Collider m_rightFoot;

    const float MAXATTACKTIME = 5f;
    public float m_attackTimer = MAXATTACKTIME;


    void Start()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_characterController = GetComponent<CharacterController>();
        m_animator = GetComponent<Animator>();
        m_movementTarget = transform.position;
    }

      private void FixedUpdate()
    {
        if (m_attackTimer > 0  && bossAttackState == BossAttackState.notReady)
        {
            m_attackTimer -= Time.deltaTime;
        }
        else
        {
            bossAttackState = BossAttackState.ready;
        }

        m_movementTarget = m_player.transform.position;

        var heading = m_movementTarget - transform.position;
        var distanceToTarget = heading.magnitude;
        var direction = heading / distanceToTarget;

        var correctedDirection = new Vector3(direction.x, 0f, direction.z);

        m_distanceToTarget = Vector3.Distance(m_movementTarget, transform.position);
        var angleDifference = Quaternion.Angle(transform.rotation, Quaternion.LookRotation(correctedDirection));


        if(bossAttackState == BossAttackState.ready)
        {
            if (m_distanceToTarget < 8) {
                
                m_bossState = BossState.Attacking;

                if (m_distanceToTarget > 6)
                {
                    bossAttackState = BossAttackState.Jump;
                    BossTriggerAttack(correctedDirection,"JumpAttack");
                }
                else if(m_distanceToTarget < 3)
                {
                    bossAttackState = BossAttackState.Stomp;
                    BossTriggerAttack(correctedDirection, "StompAttack");
                }
                else if (angleDifference > m_turnAngle)
                {
                    bossAttackState = BossAttackState.Swipe;
                    BossTriggerAttack(correctedDirection, "SwipeAttack");
                }
                else
                {
                    bossAttackState = BossAttackState.Slam;
                    BossTriggerAttack(correctedDirection, "SlamAttack");
                }
            }
        }

        if (!m_characterController.isGrounded)
        {
            m_heightMovement.y -= m_gravity;
        }
        else
        {
            m_heightMovement.y = 0;
        }

        if (m_bossState == BossState.Moving)
        {
            if (m_distanceToTarget > 4)
            {
                m_bossState = BossState.Moving;
            }
            else
            {
                m_bossState = BossState.Idle;
            }
        }
        else if (m_bossState == BossState.Idle || m_bossState == BossState.Turning)
        {
            if (angleDifference > m_turnAngle)
            {
                m_bossState = BossState.Turning;
            }
            else if (m_distanceToTarget > 5)
            {
                m_bossState = BossState.Moving;
            }
            else
            {
                m_bossState = BossState.Idle;
            }
        }

        switch (m_bossState)
        {
            case BossState.Turning:
                var currentAngle = transform.rotation.eulerAngles.y;

                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(correctedDirection), 1.5F * Time.deltaTime);
                var newAngle = transform.rotation.eulerAngles.y;

                if (currentAngle < 90 && newAngle > 270)
                {
                    newAngle *= -1;
                }
                else if (currentAngle > 270 && newAngle < 90)
                {
                    currentAngle *= -1;
                }

                if (currentAngle - newAngle < 0)
                {
                    m_animator.SetFloat("TurnInput", .5f);
                }
                else
                {
                    m_animator.SetFloat("TurnInput", -.5f);
                }
                m_animator.SetFloat("MovementInput", 0f);
                m_currentSpeed = 0;
                break;

            case BossState.Moving:
                m_currentSpeed = Mathf.Lerp(m_currentSpeed, m_movementSpeed, m_acceleration);
                Vector3 movementVector = new Vector3(direction.x * m_currentSpeed * Time.deltaTime, m_heightMovement.y, direction.z * m_currentSpeed * Time.deltaTime);
               
                m_characterController.Move(movementVector);
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(correctedDirection), 2F * Time.deltaTime);

                m_animator.SetFloat("TurnInput", 0f);
                m_animator.SetFloat("MovementInput", m_currentSpeed / m_movementSpeed);
                break;

            case BossState.Idle:
                m_closing = false;
                m_currentSpeed = 0;
                m_animator.SetFloat("TurnInput", 0f);
                m_animator.SetFloat("MovementInput", 0f);
                break;

            case BossState.Attacking:
                m_closing = false;
                m_currentSpeed = 0;
                m_animator.SetFloat("TurnInput", 0f);
                m_animator.SetFloat("MovementInput", 0f);
                break;

            default:
                m_currentSpeed = 0;
                m_animator.SetFloat("TurnInput", 0f);
                m_animator.SetFloat("MovementInput", 0f);
                break;
        }
    }

    public void AttackAnimationEnd()
    {
        bossAttackState = BossAttackState.notReady;
        m_bossState = BossState.Idle;
        m_attackTimer = MAXATTACKTIME;
    }

    public void BossTriggerAttack(Vector3 attackDirection, string attackType)
    {
        m_bossState = BossState.Attacking;
        bossAttackState = BossAttackState.notReady;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(attackDirection), 2F * Time.deltaTime);
        m_animator.SetTrigger(attackType);
    }

    public void AttackStart(string attackType)
    {
        Debug.Log(attackType);
        if (attackType == "SwipeAttack")
        {
            m_weapon.enabled = true;
        }
        else if (attackType == "StompAttack")
        {
            m_rightFoot.enabled = true;
        }
        else if (attackType == "JumpAttack")
        {
            m_weapon.enabled = true;
            m_rightHand.enabled = true;
        }
        else if (attackType == "SlamAttack")
        {
            m_weapon.enabled = true;
            m_rightHand.enabled = true;
        }
    }
    public void AttackEnd(string attackType)
    {
        Debug.Log("End Attack");
        m_weapon.enabled = false;
        m_rightHand.enabled = false;
        m_rightFoot.enabled = false;
    }
    public void AttackEnd()
    {
        Debug.Log("End Attack");
        m_weapon.enabled = false;
        m_rightHand.enabled = false;
        m_rightFoot.enabled = false;
    }
}
