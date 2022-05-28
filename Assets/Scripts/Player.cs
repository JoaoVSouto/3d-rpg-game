using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public float gravity;
    public float smoothRotationTime;

    private float smoothTurnVelocity;
    private Animator animator;
    private Transform cam;
    private CharacterController characterController;
    private Vector3 moveDirection;

    private const int LEFT_CLICK_BUTTON_INDEX = 0;

    private enum AnimationStates
    {
        Idle,
        Walk,
        Attack,
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        cam = Camera.main.transform;
    }

    void Update()
    {
        Move();
        GetMouseInput();
    }

    void SetTransition(AnimationStates state)
    {
        animator.SetInteger("transition", (int)state);
    }

    void Move()
    {
        if (characterController.isGrounded)
        {
            // get horizontal input (right/left)
            float horizontal = Input.GetAxisRaw("Horizontal");
            // get vertical input (up/down)
            float vertical = Input.GetAxisRaw("Vertical");

            // vector pointing to horizontal and vertical direction result
            Vector3 direction = new Vector3(horizontal, 0f, vertical);

            if (direction.magnitude > 0)
            {
                // get player and camera angles
                float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                // a smoothy angle derivative
                float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, angle, ref smoothTurnVelocity, smoothRotationTime);
                // rotate the user based on smooth angle
                transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);
                // mount the move direction forwared based on angle
                moveDirection = Quaternion.Euler(0f, angle, 0f) * Vector3.forward * speed;

                SetTransition(AnimationStates.Walk);
            } else
            {
                moveDirection = Vector3.zero;
                SetTransition(AnimationStates.Idle);
            }
        }

        moveDirection.y -= gravity * Time.deltaTime;
        // move the player itself
        characterController.Move(moveDirection * Time.deltaTime);
    }

    void GetMouseInput()
    {
        if (characterController.isGrounded && Input.GetMouseButtonDown(LEFT_CLICK_BUTTON_INDEX))
        {
            SetTransition(AnimationStates.Attack);
        }
    }
}
