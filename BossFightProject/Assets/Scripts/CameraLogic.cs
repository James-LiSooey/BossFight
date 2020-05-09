using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLogic : MonoBehaviour
{

    float m_cameraMovementOffset = 0.15f;
    public float m_cameraZOffset = 5.0f;
    GameObject m_Player;

    // Start is called before the first frame update
    void Start()
    {
        m_Player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCameraPosition();

        if (Input.GetKey(KeyCode.Space))
        {
            CenterCamera();
        }

    }

    void CenterCamera()
    {
        transform.position = new Vector3(m_Player.transform.position.x, transform.position.y, m_Player.transform.position.z - m_cameraZOffset);
    }

    void UpdateCameraPosition()
    {
        if(Input.mousePosition.x >= Screen.width)
        {
            transform.position = new Vector3(transform.position.x + m_cameraMovementOffset, transform.position.y, transform.position.z);
        }else if (Input.mousePosition.x <= 0.0f)
        {
            transform.position = new Vector3(transform.position.x - m_cameraMovementOffset, transform.position.y, transform.position.z);
        }

        if (Input.mousePosition.y >= Screen.height)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + m_cameraMovementOffset);
        }
        else if (Input.mousePosition.y <= 0.0f)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y , transform.position.z - m_cameraMovementOffset);
        }
    }

}
