using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public float gravity;
    public float smoothRotationTime;
    public float colliderRadius;
    public List<Transform> enemies = new List<Transform>();

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
                if (!animator.GetBool("attacking"))
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

                    animator.SetBool("walking", true);
                } else
                {
                    animator.SetBool("walking", false);
                    moveDirection = Vector3.zero;
                }
            } else if (animator.GetBool("walking"))
            {
                animator.SetBool("walking", false);
                SetTransition(AnimationStates.Idle);
                moveDirection = Vector3.zero;
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
            if (animator.GetBool("walking"))
            {
                animator.SetBool("walking", false);
                SetTransition(AnimationStates.Idle);
            }

            if (!animator.GetBool("walking"))
            {
                StartCoroutine("Attack");
            }
        }
    }

    IEnumerator Attack()
    {
        animator.SetBool("attacking", true);
        SetTransition(AnimationStates.Attack);

        yield return new WaitForSeconds(0.4f);

        GetEnemiesList();

        foreach (Transform enemy in enemies)
        {
            // inflict damage
            print(enemy.name);
        }

        yield return new WaitForSeconds(1f);

        animator.SetBool("attacking", false);
    }

    void GetEnemiesList()
    {
        enemies.Clear();

        foreach (Collider collider in Physics.OverlapSphere((transform.position + transform.forward * colliderRadius), colliderRadius))
        {
            if (collider.gameObject.CompareTag("Enemy"))
            {
                enemies.Add(collider.transform);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward, colliderRadius);
    }
}
