using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLogic : MonoBehaviour
{
    CharacterController m_characterController;

    float m_movementSpeed = 5.0f;
    float m_horizontalInput;
    float m_verticalInput;

    Vector3 m_horizontalMovement;
    Vector3 m_verticalMovement;
    Vector3 m_heightMovement;

    float m_jumpHeight = .3f;
    float m_gravity = 0.01f;
    bool m_jump = false;

    Animator m_animator;

    [SerializeField]
    List<AudioClip> m_FootstepEarthSounds = new List<AudioClip>();
    [SerializeField]
    List<AudioClip> m_FootstepGrassSounds = new List<AudioClip>();

    AudioSource m_AudioSource;

    [SerializeField]
    Transform m_LeftFoot;

    [SerializeField]
    Transform m_RightFoot;


    GameObject m_Camera;
    CameraLogic m_CameraLogic;

    /*
    // Start is called before the first frame update
    void Start()
    {
        m_characterController = GetComponent<CharacterController>();
        m_animator = GetComponent<Animator>();
        m_AudioSource = GetComponent<AudioSource>();
        m_Camera = Camera.main.gameObject;
        if (m_Camera)
        {
            m_CameraLogic = m_Camera.GetComponent<CameraLogic>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        m_horizontalInput = Input.GetAxis("Horizontal");
        m_verticalInput = Input.GetAxis("Vertical");

        if (!m_jump && Input.GetButtonDown("Jump"))
        {
            m_jump = true;
        }

        if (m_animator) {
            m_animator.SetFloat("HorizontalInput", m_horizontalInput);
            m_animator.SetFloat("VerticalInput", m_verticalInput);
        }

    }

    void FixedUpdate()
    {

        if(m_CameraLogic && (Mathf.Abs(m_horizontalInput)>0.1f || Mathf.Abs(m_verticalInput) > 0.1f))
        {
            transform.forward = m_CameraLogic.GetForwardVector();
        }

        m_horizontalMovement = transform.right * m_horizontalInput * m_movementSpeed * Time.deltaTime;
        m_verticalMovement = transform.forward * m_verticalInput * m_movementSpeed * Time.deltaTime;

        if (!m_characterController.isGrounded)
        {
            m_heightMovement.y -= m_gravity;
        }
        else
        {
            m_heightMovement.y = 0;
        }
        if (m_jump)
        {
            m_heightMovement.y = m_jumpHeight;
            m_jump = false;
        }

        if (m_characterController)
        {
            m_characterController.Move(m_horizontalMovement + m_verticalMovement + m_heightMovement);
        }

    }

    public void PlayFootstepSound(int footIndex)
    {
        // 0 = left, 1 = right

        if (footIndex == 0)
        {
            RayCastTerrain(m_LeftFoot.position);
        }else if(footIndex == 1)
        {
            RayCastTerrain(m_RightFoot.position);
        }
    }

    void RayCastTerrain(Vector3 position)
    {
        LayerMask layerMask = LayerMask.GetMask("Terrain");
        Ray ray = new Ray(position, Vector3.down);
        RaycastHit hit;

        if(Physics.Raycast(ray,out hit, layerMask))
        {
            string hitTag = hit.collider.gameObject.tag;
            if(hitTag == "Earth")
            {
                PlayRandomSound(m_FootstepEarthSounds);
            }
            else if (hitTag == "Grass")
            {
                PlayRandomSound(m_FootstepGrassSounds);
            }
            else
            {
                PlayRandomSound(m_FootstepEarthSounds);
            }
            
        }
    }

    void PlayRandomSound(List<AudioClip> audioClips)
    {
        if (audioClips.Count > 0 && m_AudioSource)
        {
            int randomNum = Random.Range(0, audioClips.Count - 1);

            m_AudioSource.PlayOneShot(audioClips[randomNum]);
        }
    }*/

}
