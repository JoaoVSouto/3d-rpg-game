using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public float smoothRotationTime;

    private float smoothTurnVelocity;
    private Transform camera;
    private CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        camera = Camera.main.transform;
    }

    void Update()
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
            float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + camera.eulerAngles.y;
            // a smoothy angle derivative
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, angle, ref smoothTurnVelocity, smoothRotationTime);
            // rotate the user based on smooth angle
            transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);
            // mount the move direction forwared based on angle
            Vector3 moveDirection = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;
            // move the player itself
            characterController.Move(moveDirection * speed * Time.deltaTime);
        }
    }
}
