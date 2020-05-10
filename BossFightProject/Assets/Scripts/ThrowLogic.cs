using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;
using Cinemachine;

public class ThrowLogic : MonoBehaviour
{
    Animator animator;
    PlayerLogic playerLogic;
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

    float throwPower = 90f;
    float cameraZoomOffset = -2f;

    bool walking = true;
    bool aiming = false;
    bool hasWeapon = true;
    bool pulling = false;

    [SerializeField]
    public Image reticle;

    CinemachineFreeLook virtualCamera;
    CinemachineImpulseSource impulseSource;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;

        animator = GetComponent<Animator>();
        playerLogic = GetComponent<PlayerLogic>();
        weaponRb = weapon.GetComponent<Rigidbody>();
        weaponLogic = weapon.GetComponent<WeaponLogic>();
        origLocPos = weapon.localPosition;
        origLocRot = weapon.localEulerAngles;
        reticle.DOFade(0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if(aiming) {
            playerLogic.RotateToCamera(transform);
        } else {
            transform.eulerAngles = new Vector3(Mathf.LerpAngle(transform.eulerAngles.x, 0, 0.2f), transform.eulerAngles.y, transform.eulerAngles.z);
        }

        animator.SetBool("Pulling", pulling);

        if(Input.GetButtonDown("Fire2") && hasWeapon) {
            Aim(true, true, 0);
        }
        
        if(Input.GetButtonUp("Fire2") && hasWeapon) {
            Aim(false, true, 0);
        }

        if(hasWeapon) {
            if(aiming && Input.GetButtonDown("Fire1")) {
                animator.SetTrigger("Throw");
            }
        } else {
            if(Input.GetButtonDown("Fire1")) {
                WeaponStartPull();
            }
        }

        if (pulling)
        {
            if(returnTime < 1)
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

    void Aim(bool state, bool changeCamera, float delay) {
        aiming = state;
        animator.SetBool("Aiming", aiming);

        float fade = state ? 1 : 0;
        reticle.DOFade(fade, 0.2f);

        if(!changeCamera) {
            return;
        }

        float newAim = state ? cameraZoomOffset : 0;
        float originalAim = !state ? cameraZoomOffset : 0;
        DOVirtual.Float(originalAim, newAim, 0.5f, CameraOffset).SetDelay(delay);

        //Particle handler
        //if(state) {
        //    glowParticle.Play();
        //} else {
        //    glowParticle.Stop();
        //}
    }

    public void WeaponThrow() {
        Aim(false, true, 1f);

        hasWeapon = false;
        weaponLogic.activated = true;
        weaponRb.isKinematic = false;
        weaponRb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        weapon.parent = null;
        weapon.eulerAngles = new Vector3(0, -90 +transform.eulerAngles.y, 0);
        weapon.transform.position += transform.right/5;
        weaponRb.AddForce(Camera.main.transform.forward * throwPower + transform.up * 2, ForceMode.Impulse);
        //Trail
        //trailRenderer.emitting = true;
        //trailParticle.Play();
    }

    public void WeaponStartPull() {
        pullPosition = weapon.position;
        weaponRb.Sleep();
        weaponRb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        weaponRb.isKinematic = true;
        weapon.DORotate(new Vector3(-90, -90, 0), 0.2f).SetEase(Ease.InOutSine);
        weapon.DOBlendableLocalRotateBy(Vector3.right * 90, 0.5f);
        weaponLogic.activated = true;
        pulling = true;
    }

    public void WeaponCatch() {
        returnTime = 0;
        pulling = false;
        weapon.parent = hand;
        weaponLogic.activated = false;
        weapon.localEulerAngles = origLocRot;
        weapon.localPosition = origLocPos;
        hasWeapon = true;

        //Particle and trail
        //catchParticle.Play();
        //trailRenderer.emitting = false;
        //trailParticle.Stop();

        //Shake
        impulseSource.GenerateImpulse(Vector3.right);
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
        virtualCamera.GetRig(0).GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset = new Vector3(offset, 1.5f, -2.0f);
        virtualCamera.GetRig(1).GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset = new Vector3(offset, 1.5f, -2.0f);
        virtualCamera.GetRig(2).GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset = new Vector3(offset, 1.5f, -2.0f);
    }
}
