﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

enum BossState
{
    Idle,
    Moving,
    Attacking,
    Turning
}
public enum BossAttackState
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
    public BossAttackState bossAttackState = BossAttackState.notReady;
    CharacterController m_characterController;
    Animator m_animator;
    GameObject m_player;

    Vector3 m_movementTarget;
    Vector3 m_horizontalMovement;
    Vector3 m_verticalMovement;
    Vector3 m_heightMovement;
    public float m_movementSpeed = 2.0f;
    float m_movementSpeedJump = 0;
    Vector3 m_jumpAttackDirection;
    
    public float m_currentSpeed = 0f;
    public float m_acceleration = .01f;
    public Boolean m_closing = false;
    public float m_closingDistance = 0;

    public float m_distanceToTarget;
    float m_gravity = 0.01f;

    public float m_turnAngle = 10f;
    public float m_stoppingDistance = 3.0f;

    [SerializeField]
    Collider m_weapon;
    [SerializeField]
    Collider m_rightHand;
    [SerializeField]
    Collider m_rightFoot;

    const float MAXATTACKTIME = 5f;
    public float m_attackTimer = MAXATTACKTIME;

    [SerializeField]
    int health = 100;

    [SerializeField]
    public int stompAttackDamage = 8;
    [SerializeField]
    public int swipeAttackDamage = 8;
    [SerializeField]
    public int slamAttackDamage = 16;
    [SerializeField]
    public int jumpAttackDamage = 20;

    [SerializeField]
    Text bossHealthText;

    [SerializeField]
    Slider slider;

    [SerializeField]
    AudioClip m_rightFootStepSound;

    [SerializeField]
    AudioClip m_leftFootStepSound;

    [SerializeField]
    AudioClip m_roarSound;

    [SerializeField]
    AudioClip m_attackSound;
    
    [SerializeField]
    AudioClip m_slamSound;

    AudioSource m_audioSource;

    

    bool isDead = false;
    public bool dealDamage = false;

    [SerializeField]
    TMP_Text endGameText;

    [SerializeField]
    GameObject endGameMenu;

    PlayerLogic m_playerLogic;


    void Start()
    {
        SetSliderMaxHealth();
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_characterController = GetComponent<CharacterController>();
        m_animator = GetComponent<Animator>();
        m_movementTarget = transform.position;
        m_audioSource = GetComponent<AudioSource>();
    }
    void OnEnable()
    {
        SetSliderMaxHealth();
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_playerLogic = m_player.GetComponent<PlayerLogic>();
        m_characterController = GetComponent<CharacterController>();
        m_animator = GetComponent<Animator>();
        m_movementTarget = transform.position;
    }
      private void FixedUpdate()
    {
        if(isDead) {
            return;
        }

        if (m_attackTimer > 0  && bossAttackState == BossAttackState.notReady)
        {
            m_attackTimer -= Time.deltaTime * UnityEngine.Random.Range(.5f, 2f);
        }
        else if(m_attackTimer <= 0 && bossAttackState == BossAttackState.notReady)
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
            if (m_distanceToTarget < 20) {
                
                m_bossState = BossState.Attacking;

                if (m_distanceToTarget > 8)
                {
                    m_movementSpeedJump = m_distanceToTarget/2.0f;
                    m_jumpAttackDirection = direction;
                    bossAttackState = BossAttackState.Jump;
                    BossTriggerAttack(correctedDirection,"JumpAttack");
                }
                else if(m_distanceToTarget < 4)
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
                m_currentSpeed = m_movementSpeedJump;
                Vector3 movementAttackVector = new Vector3(m_jumpAttackDirection.x * m_movementSpeedJump * Time.deltaTime, 0, m_jumpAttackDirection.z * m_movementSpeedJump * Time.deltaTime);
                m_characterController.Move(movementAttackVector);
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
        m_movementSpeedJump = 0;
        m_bossState = BossState.Idle;
        m_attackTimer = MAXATTACKTIME;
    }

    public void BossTriggerAttack(Vector3 attackDirection, string attackType)
    {
        m_bossState = BossState.Attacking;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(attackDirection), 2F * Time.deltaTime);
        m_animator.SetTrigger(attackType);
    }

    public void AttackStart(string attackType)
    {
        //Debug.Log(attackType);
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
            m_movementSpeedJump = 0;
            m_weapon.enabled = true;
            m_rightHand.enabled = true;
            m_rightFoot.enabled = true;
        }
        else if (attackType == "SlamAttack")
        {
            m_weapon.enabled = true;
            m_rightHand.enabled = true;
        }
    }
    public void AttackEnd(string attackType)
    {
        //Debug.Log("End Attack");
        m_weapon.enabled = false;
        m_rightHand.enabled = false;
        m_rightFoot.enabled = false;
    }
    public void AttackEnd()
    {
        //Debug.Log("End Attack");
        m_weapon.enabled = false;
        m_rightHand.enabled = false;
        m_rightFoot.enabled = false;
    }

    public void TakeDamage(int damage) {
        if (health <= 0  || m_playerLogic.gotHit)
        {
            return;
        }
        health -= damage;
        UpdateHealthSlider();
        if(health <= 0) {
            isDead = true;
            m_animator.SetTrigger("Die");
            m_characterController.enabled = false;
            SetEndGameText();
            endGameMenu.SetActive(true);
        }
    }

    public void UpdateHealthSlider() {
        if(slider) {
            slider.value = health;
        }
    }

    public void SetSliderMaxHealth() {
        if(slider) {
            slider.maxValue = health;
            slider.value = health;
        }
    }

    public void DealDamage() {
        dealDamage = true;
    }

    public void SetEndGameText() {
        endGameText.text = "You win.";
    }

    public void PlaySound(int index) {
        //0 = leftfoot; 1= rightfoot; 2 = slam; 3 = roar; 4 = attack
        if(index == 0) {
            m_audioSource.PlayOneShot(m_leftFootStepSound);
        } else if (index == 1) {
            m_audioSource.PlayOneShot(m_rightFootStepSound);
        } else if (index == 2) {
            m_audioSource.PlayOneShot(m_slamSound);
        } else if (index == 3) {
            m_audioSource.PlayOneShot(m_roarSound);
        } else if (index == 4) {
            m_audioSource.PlayOneShot(m_attackSound);
        }
    }
}
