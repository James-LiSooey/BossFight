﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLogic : MonoBehaviour
{
    float horizontalInput;
    float verticalInput;

    Vector3 movementInput;
    float movementSpeed = 4.0f;

    bool jump = false;
    float jumpHeight = 0.25f;
    float gravity = 0.981f;

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
    

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        camera = Camera.main.gameObject;
    }

    // Update is called once per frame
    void Update()
    {

        if(rolling) {
            return;
        }

        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        movementInput = new Vector3(horizontalInput, 0, verticalInput);

        var forward = camera.transform.forward;
		var right = camera.transform.right;

		forward.y = 0f;
		right.y = 0f;

		forward.Normalize ();
		right.Normalize ();

		desiredMoveDirection = forward * verticalInput + right * horizontalInput;

        if(Input.GetButtonDown("Jump")) {
            //Quaternion wantedRotation = Quaternion.LookRotation(movementInput);
            transform.rotation = Quaternion.LookRotation(desiredMoveDirection);
            animator.SetTrigger("Roll");
            rolling = true;
        }

        if (animator)
        {
            animator.SetFloat("HorizontalInput", horizontalInput);
            animator.SetFloat("VerticalInput", verticalInput);
        }
    }

    private void FixedUpdate() {
        if(rolling) {
            return;
        }

        if(Mathf.Abs(horizontalInput) > 0.1f || Mathf.Abs(verticalInput) > 0.1f) {
            //transform.forward = cameraLogic.GetForwardVector();
            transform.forward = camera.transform.forward;
        }

        heightMovement.y -= gravity * Time.deltaTime;

        //verticalMovement = transform.forward * verticalInput * movementSpeed * Time.deltaTime;
        //horizontalMovement = transform.right * horizontalInput * movementSpeed * Time.deltaTime;

        //characterController.Move(horizontalMovement + verticalMovement + heightMovement);

        var forward = camera.transform.forward;
		var right = camera.transform.right;

		forward.y = 0f;
		right.y = 0f;

		forward.Normalize ();
		right.Normalize ();

		desiredMoveDirection = forward * verticalInput + right * horizontalInput;
        characterController.Move(desiredMoveDirection * Time.deltaTime * 3);
        //transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (desiredMoveDirection), desiredRotationSpeed);
        

        if(characterController.isGrounded) {
            heightMovement.y = 0.0f;
        }
    }

    public void SetRollingState(bool isRolling) {    
        rolling = isRolling;
    }

    public void RotateToCamera(Transform t)
    {
        var forward = camera.transform.forward;
        var right = camera.transform.right;

        desiredMoveDirection = forward;

        t.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), desiredRotationSpeed);
    }
}
