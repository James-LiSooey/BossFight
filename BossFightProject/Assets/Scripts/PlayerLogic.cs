﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Cinemachine;
using Unity.Mathematics;
using System;
using TMPro;

public enum AttackType {
    Regular,
    Strong,
    Jump,
    Throw
}

public class PlayerLogic : MonoBehaviour
{
    float horizontalInput;
    float verticalInput;

    Vector3 movementInput;
    [SerializeField]
    float movementSpeed = 3.0f;

    bool jump = false;
    float jumpHeight = 0.25f;
    float gravity = 0.01f;

    Vector3 heightMovement;
    Vector3 verticalMovement;
    Vector3 horizontalMovement;

    CharacterController characterController;
    Animator animator;
    GameObject camera;

    float allowPlayerRotation = 0.1f;
    float desiredRotationSpeed = 0.1f;
    float magnitude;
    Vector3 desiredMoveDirection;

    bool rolling = false;

    Rigidbody weaponRb;
    WeaponLogic weaponLogic;
    float returnTime;

    Vector3 origLocPos;
    Vector3 origLocRot;
    Vector3 pullPosition;

    [SerializeField]
    Transform weapon;
    [SerializeField]
    Transform hand;
    Transform spine;
    [SerializeField]
    Transform curvePoint;

    [SerializeField]
    float throwPower = 90f;
    [SerializeField]
    float cameraZoomOffset = 2f;

    bool walking = true;
    bool aiming = false;
    public bool hasWeapon = true;
    bool pulling = false;

    public bool gotHit = false;

    public bool attacking = false;
    public bool attackCombo = false;
    public bool comboListener = false;

    public AttackType attackType;

    [SerializeField]
    public Image reticle;

    [SerializeField]
    CinemachineFreeLook virtualCamera;
    CinemachineImpulseSource impulseSource;

    public bool dealDamage = false;

    [SerializeField]
    int health = 100;

    [SerializeField]
    public int regularAttackDamage = 5;
    [SerializeField]
    public int strongAttackDamage = 7;
    [SerializeField]
    public int jumpAttackDamage = 10;

    [SerializeField]
    public int throwAttackDamage = 10;

    bool isDead = false;

    [SerializeField]
    Slider slider;

    [SerializeField]
    TMP_Text endGameText;
    
    [SerializeField]
    GameObject endGameMenu;

    GameObject boss;
    BossLogic bossLogic;


    ParticleSystem glowParticle;
    [SerializeField]
    TrailRenderer trail;
    ParticleSystem catchParticle;

    [SerializeField]
    AudioClip attackSound;
    [SerializeField]
    AudioClip catchSound;
    [SerializeField]
    AudioClip yellSound;
    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        SetSliderMaxHealth();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        camera = Camera.main.gameObject;
        //impulseSource = virtualCamera.GetComponent<CinemachineImpulseSource>();
        Cursor.visible = false;
        weaponRb = weapon.GetComponent<Rigidbody>();
        weaponLogic = weapon.GetComponent<WeaponLogic>();
        origLocPos = weapon.localPosition;
        origLocRot = weapon.localEulerAngles;
        reticle.DOFade(0, 0);

        boss = GameObject.FindGameObjectWithTag("Boss");
        bossLogic = boss.GetComponent<BossLogic>();
    }

    void Awake() {
        SetSliderMaxHealth();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        camera = Camera.main.gameObject;
        //impulseSource = virtualCamera.GetComponent<CinemachineImpulseSource>();
        Cursor.visible = false;
        weaponRb = weapon.GetComponent<Rigidbody>();
        weaponLogic = weapon.GetComponent<WeaponLogic>();
        origLocPos = weapon.localPosition;
        origLocRot = weapon.localEulerAngles;
        reticle.DOFade(0, 0);

        boss = GameObject.FindGameObjectWithTag("Boss");
        bossLogic = boss.GetComponent<BossLogic>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rolling || isDead || gotHit)
        {
            return;
        }

        if(comboListener) {
            if(!attackCombo && ((attackType == AttackType.Regular && Input.GetButtonDown("RegularAttack")) || (attackType == AttackType.Strong && Input.GetButtonDown("StrongAttack")))) {
                attackCombo = true;
            }
            return;
        }

        animator.SetBool("Pulling", pulling);

        if (Input.GetButtonDown("Fire2") && hasWeapon)
        {
            Aim(true, true, 0);
        }

        if (Input.GetButtonUp("Fire2") && hasWeapon)
        {
            Aim(false, true, 0);
        }

        if (hasWeapon)
        {
            if (aiming && Input.GetButtonDown("Fire1"))
            {
                animator.SetTrigger("Throw");
            }

            if(Input.GetButtonDown("RegularAttack")) {
                animator.SetTrigger("RegularAttack1");
                attackType = AttackType.Regular;
                attacking = true;
                comboListener = true;
            }

            if(Input.GetButtonDown("StrongAttack")) {
                animator.SetTrigger("StrongAttack1");
                attackType = AttackType.Strong;
                comboListener = true;
                attacking = true;
            }

            if(Input.GetButtonDown("JumpAttack")) {
                animator.SetTrigger("JumpAttack");
                attackType = AttackType.Jump;
                attacking = true;
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1"))
            {
                WeaponStartPull();
            }
        }

        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Math.Max(Input.GetAxis("Vertical"), -.5f);

        movementInput = new Vector3(horizontalInput, 0, verticalInput);

        var forward = camera.transform.forward;
        var right = camera.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        desiredMoveDirection = forward * verticalInput + right * horizontalInput;

        if (aiming)
        {
            return;
        }

        if (Input.GetButtonDown("Jump"))
        {
            transform.rotation = Quaternion.LookRotation(desiredMoveDirection);
            animator.SetTrigger("Roll");
            rolling = true;
        }

        if (animator)
        {
            animator.SetFloat("HorizontalInput", horizontalInput);
            animator.SetFloat("VerticalInput", verticalInput);
        }

        if (pulling)
        {
            if (returnTime < 1)
            {
                weapon.position = GetQuadraticCurvePoint(returnTime, pullPosition, curvePoint.position, hand.position);
                returnTime += Time.deltaTime * 1.5f;
            }
            else
            {
                WeaponCatch();
            }
        }
    }

    private void FixedUpdate()
    {

        if (rolling || attacking || isDead || gotHit)
        {
            return;
        }

        if (aiming)
        {
            transform.forward = camera.transform.forward;
            return;
        }
        else
        {
            if (Mathf.Abs(horizontalInput) > 0.1f || Mathf.Abs(verticalInput) > 0.1f)
            {
                var forwardTransform = camera.transform.forward;
                forwardTransform.y = 0f;
                transform.forward = forwardTransform;
            }
        }

        var forward = camera.transform.forward;
        var right = camera.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        if (!characterController.isGrounded)
        {
            desiredMoveDirection.y -= gravity;
        }
        else
        {
            desiredMoveDirection.y = 0;
        }

        desiredMoveDirection = forward * verticalInput + right * horizontalInput;
        characterController.Move(desiredMoveDirection * Time.deltaTime * movementSpeed);
    }

    public void SetRollingState(bool isRolling)
    {
        rolling = isRolling;
    }

    public void SetAttackingState(bool isAttacking)
    {
        attacking = isAttacking;
        attackCombo = false;
        trail.emitting = false;
    }

    public void DealDamage() {
        dealDamage = true;
    }

    public void UpdateComboListener(string status)
    {
        if(status == "Sleep")
        {
            comboListener = false;
        }
        else
        {
            comboListener = true;
        }
    }

    public void EnableTrail() {
        trail.emitting = true;
    }

    public void DisableTrail() {
        trail.emitting = false;
    }

    public void TakeDamage(int damage) {
        //Debug.Log("Inside PlayerLogic.TakeDamage");
        health -= damage;
        UpdateHealthSlider();
        
        if(health <= 0) {
            isDead = true;
            animator.SetTrigger("Die");
            characterController.enabled = false;
            SetEndGameText();
            endGameMenu.SetActive(true);
        }
    }

    public void GotHit(int index) {
        //Debug.Log("Inside GotHit");
        //Debug.Log("GotHit index: " + index);
        //0 = Stomp; 1 = Swipe; 2 = Slam; 3 = Jump;
        gotHit = true;
        attacking = false;
        aiming = false;
        pulling = false;
        attackCombo = false;
        comboListener = false;
        dealDamage = false;

        if(index == 0) {
            animator.SetTrigger("StompHit");
            //Debug.Log("Taking damamge" + GameObject.FindGameObjectWithTag("Boss").GetComponent<BossLogic>().stompAttackDamage);
            //TakeDamage(GameObject.FindGameObjectWithTag("Boss").GetComponent<BossLogic>().stompAttackDamage);
            TakeDamage(20);
        } else if (index == 1) {
            animator.SetTrigger("SwipeHit");
        //     Debug.Log("Taking damamge" + GameObject.FindGameObjectWithTag("Boss").GetComponent<BossLogic>().swipeAttackDamage);
        //     TakeDamage(GameObject.FindGameObjectWithTag("Boss").GetComponent<BossLogic>().swipeAttackDamage);
            TakeDamage(15);
        } else if (index == 2) {
            animator.SetTrigger("SlamOrJumpHit");
        //     Debug.Log("Taking damamge" + GameObject.FindGameObjectWithTag("Boss").GetComponent<BossLogic>().slamAttackDamage);
        //     TakeDamage(GameObject.FindGameObjectWithTag("Boss").GetComponent<BossLogic>().slamAttackDamage);
            TakeDamage(25);
        } else if (index == 3) {
            animator.SetTrigger("SlamOrJumpHit");
        //     Debug.Log("Taking damamge" + GameObject.FindGameObjectWithTag("Boss").GetComponent<BossLogic>().jumpAttackDamage);
        //     TakeDamage(GameObject.FindGameObjectWithTag("Boss").GetComponent<BossLogic>().jumpAttackDamage);
            TakeDamage(40);
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

    public void SetEndGameText() {
        endGameText.text = "You lose.";

    }

    public void RotateToCamera(Transform t)
    {
        var forward = camera.transform.forward;
        var right = camera.transform.right;

        desiredMoveDirection = forward;

        t.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), desiredRotationSpeed);
    }

    void Aim(bool state, bool changeCamera, float delay)
    {
        aiming = state;
        animator.SetBool("Aiming", aiming);

        float fade = state ? 1 : 0;
        reticle.DOFade(fade, 0.2f);

        if (!changeCamera)
        {
            return;
        }

        float newAim = state ? cameraZoomOffset : 0;
        float originalAim = !state ? cameraZoomOffset : 0;
       // DOVirtual.Float(originalAim, newAim, 0.1f, CameraOffset).SetDelay(delay);
    }

    public void WeaponThrow()
    {
        Aim(false, true, 1f);
        attackType = AttackType.Throw;
        hasWeapon = false;
        weaponLogic.activated = true;
        weaponRb.isKinematic = false;
        weaponRb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        weapon.parent = null;
        //weapon.eulerAngles = new Vector3(0, -90 + transform.eulerAngles.y, 0);
        weapon.transform.position += transform.right / 5;
        weaponRb.AddForce(Camera.main.transform.forward * throwPower + transform.up * 2, ForceMode.Impulse);

        // ParticleSystem.EmissionModule em = trailParticle.emission;
        // em.enabled = true;
        //trailParticle.Play();

        trail.emitting = true;

    }

    public void WeaponStartPull()
    {
        pullPosition = weapon.position;
        weaponRb.Sleep();
        weaponRb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        weaponRb.isKinematic = true;
        weapon.DORotate(new Vector3(-90, -90, 0), 0.2f).SetEase(Ease.InOutSine);
        weapon.DOBlendableLocalRotateBy(Vector3.right * 90, 0.5f);
        weaponLogic.activated = true;
        pulling = true;
        EnableTrail();
    }

    public void WeaponCatch()
    {
        returnTime = 0;
        pulling = false;
        weapon.parent = hand;
        weaponLogic.activated = false;
        weapon.localEulerAngles = origLocRot;
        weapon.localPosition = origLocPos;
        hasWeapon = true;

        //Shake
        virtualCamera.GetComponent<CinemachineImpulseSource>().GenerateImpulse(Vector3.right);

        trail.emitting = false;
    }

    public Vector3 GetQuadraticCurvePoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        return (uu * p0) + (2 * u * t * p1) + (tt * p2);
    }

    void CameraOffset(float offset)
    {
        //virtualCamera.GetRig(0).GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset = new Vector3(offset, 1.5f, -2.0f);
     //virtualCamera.GetRig(1).GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset = new Vector3(offset, 1.5f, -2.0f);
     //   virtualCamera.GetRig(2).GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset = new Vector3(offset, 1.5f, -2.0f);
    }

    public void PlaySound(int index) {
        //0 = leftfoot; 1= rightfoot; 2 = attack; 3 = catch; 4 = yell
        if (index == 2) {
            audioSource.PlayOneShot(attackSound);
        } else if (index == 3) {
            audioSource.PlayOneShot(catchSound);
        } else if (index == 4) {
            audioSource.PlayOneShot(yellSound);
        }
    }
}
