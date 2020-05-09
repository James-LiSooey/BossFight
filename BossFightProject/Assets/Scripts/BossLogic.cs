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
    NoAttack,
    JumpAttack,
    SwipeAttack,
    SlamAttack,
    KickAttack
}

public class BossLogic : MonoBehaviour
{
    BossState m_bossState = BossState.Idle;
    BossAttackState bossAttackState = BossAttackState.NoAttack;
    CharacterController m_characterController;
    Animator m_animator;
    GameObject m_player;

    Vector3 m_movementTarget;
    Vector3 m_horizontalMovement;
    Vector3 m_verticalMovement;
    Vector3 m_heightMovement;
    public float m_movementSpeed = 2.0f;
    public float m_distanceToTarget;
    float m_gravity = 0.01f;

    public float m_turnAngle = 20f;
    public float m_stoppingDistance = 3.0f;

    void Start()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_characterController = GetComponent<CharacterController>();
        m_animator = GetComponent<Animator>();
        m_movementTarget = transform.position;
    }

    void Update()
    {
        m_movementTarget = m_player.transform.position;

        var heading = m_movementTarget - transform.position;
        var distanceToTarget = heading.magnitude;
        var direction = heading / distanceToTarget;

        var correctedDirection = new Vector3(direction.x, 0f, direction.z);

        m_distanceToTarget = Vector3.Distance(m_movementTarget, transform.position);
        var angleDifference = Quaternion.Angle(transform.rotation, Quaternion.LookRotation(correctedDirection));

        var newLookAt = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(correctedDirection), 2.0F * Time.deltaTime);


        //Debug.Log("Angle difference: " + angleDifference);
        Debug.Log("Current Rotation: " + transform.rotation.eulerAngles.y);
        

        if (!m_characterController.isGrounded)
        {
            m_heightMovement.y -= m_gravity;
        }
        else
        {
            m_heightMovement.y = 0;
        }

        Vector3 movementVector = new Vector3(direction.x * m_movementSpeed * Time.deltaTime, m_heightMovement.y, direction.z * m_movementSpeed * Time.deltaTime);

        if(angleDifference > m_turnAngle)
        {
            m_bossState = BossState.Turning;
        }
        else if (m_distanceToTarget > 3)
        {
            m_bossState = BossState.Moving;
        }
        else
        {
            m_bossState = BossState.Idle;
        }

        switch (m_bossState)
        {
            case BossState.Turning:
                var currentAngle = transform.rotation.eulerAngles.y;

                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(correctedDirection), 1.2F * Time.deltaTime);
                var newAngle = transform.rotation.eulerAngles.y;

                if(currentAngle<90 && newAngle > 270)
                {
                    newAngle *= -1;
                }else if(currentAngle > 270 && newAngle<90)
                {
                    currentAngle *= -1;
                }

                if(currentAngle - newAngle < 0)
                {
                    m_animator.SetFloat("TurnInput", .5f);
                }
                else
                {
                    m_animator.SetFloat("TurnInput", -.5f);
                }
                m_animator.SetFloat("MovementInput", 0f);

                break;
            case BossState.Moving:
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(correctedDirection), 2F * Time.deltaTime);
                m_characterController.Move(movementVector);
                m_animator.SetFloat("TurnInput", 0f); 
                m_animator.SetFloat("MovementInput", 1f);
                break;
            case BossState.Idle:
                m_animator.SetFloat("TurnInput", 0f);
                m_animator.SetFloat("MovementInput", 0f);
                break;
            default:
                m_animator.SetFloat("TurnInput", 0f);
                m_animator.SetFloat("MovementInput", 0f);
                break;
        }

        

        
        
    }

    void MoveTowardsPoint(Vector3 pos)
    {
        if (m_characterController)
        {
            m_characterController.Move(pos* m_movementSpeed*Time.deltaTime);
        }
    }
}
