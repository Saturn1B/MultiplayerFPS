using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : NetworkBehaviour
{
    [Header("Camera Control")]
    [SerializeField] private float minPitch;
    [SerializeField] private float maxPitch;
    [SerializeField] private float lookSensitivity;

    [Header("Player Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float jumpHeight;

    [Header("Crouch")]
    [SerializeField] private float standingHeight;
    [SerializeField] private float crouchingHeight;


    private CharacterController playerController;
    private Camera playerCamera;

    private float yaw;
    private float pitch;

    private Vector3 velocity;
    private float gravity = 9.81f;
    private bool isCrouching;
    private float crouchTransitionSpeed = .1f;

    private bool isAttemptingToJump;

	public override void OnNetworkSpawn()
	{
        if(playerCamera == null) playerCamera = GetComponentInChildren<Camera>();
        playerCamera.enabled = IsLocalPlayer;
	}

	private void Start()
    {
        playerController = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (!IsOwner) return;

        HandleMovement();
        HandleMouseLook();
        HandleJump();
        HandleCrouch();
    }

    private void HandleMovement()
    {
        float currentSpeed = isCrouching ? crouchSpeed : Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontal, 0.0f, vertical).normalized;
        Vector3 moveVelocity = transform.TransformDirection(moveDirection) * currentSpeed;

        velocity = Vector3.Lerp(velocity, moveVelocity, Time.deltaTime * 20f);

        // Apply gravity
        velocity.y -= gravity * Time.deltaTime * 15;

        playerController.Move(velocity * Time.deltaTime);
    }

    private void HandleMouseLook()
    {
        // Camera Look
        yaw += Input.GetAxisRaw("Mouse X") * lookSensitivity;
        pitch += Input.GetAxisRaw("Mouse Y") * lookSensitivity;

        pitch = ClampAngle(pitch, minPitch, maxPitch);

        transform.eulerAngles = new Vector3(0.0f, yaw, 0.0f);
        playerCamera.transform.localEulerAngles = new Vector3(pitch, 0.0f, 0.0f);
    }

    private void HandleJump()
    {
        if (playerController.isGrounded && Input.GetButtonDown("Jump"))
        {
            if (isCrouching)
            {
                StartCoroutine(CrouchStand());
            }

            velocity.y = Mathf.Sqrt(15f * jumpHeight * gravity);
        }
    }

    private void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            StartCoroutine(CrouchStand());
        }
    }

    private IEnumerator CrouchStand()
    {
        float targetHeight = isCrouching ? standingHeight : crouchingHeight;
        float initialHeight = playerController.height;

        float timeElapsed = 0f;

        while (timeElapsed < crouchTransitionSpeed)
        {
            playerController.height = Mathf.Lerp(initialHeight, targetHeight, timeElapsed / crouchTransitionSpeed);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        playerController.height = targetHeight;
        isCrouching = !isCrouching;
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;

        return Mathf.Clamp(angle, min, max);
    }
}
