using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{
    private bool isJumping = false;
    private float jumpCooldownTimer;
    private CharacterController controller;
    private InputAction moveAction;
    private InputAction jumpAction;

    private Animator animator;
    private AudioSource footstepSource;
    private AudioSource sfxSource;

    [SerializeField] private float jumpCooldown;
    [SerializeField] private float gravity;
    [SerializeField] private float characterSpeed;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float dampening;
    [SerializeField] private Transform cameraTransform;

    [SerializeField] private LayerMask platformLayerMask;
    private Vector3 platformVelocity;

    private Vector3 characterMovement;
    private Vector3 jumpVelocity;
    private Vector3 characterGravity;

    [SerializeField] private AudioClip footstepSound;
    [SerializeField] private AudioClip jumpSound;

    void Start()
    {
        this.controller = this.GetComponent<CharacterController>();
        this.moveAction = InputSystem.actions.FindAction("Move");
        this.jumpAction = InputSystem.actions.FindAction("Jump");
        this.jumpCooldownTimer = 0.0f;

        this.animator = this.GetComponent<Animator>();

        AudioSource[] audioSources = this.GetComponents<AudioSource>();
        this.footstepSource = audioSources[0];
        this.sfxSource = audioSources[1];
    }

    void SetAnimationState(Vector2 inputMovement)
    {
        this.animator.SetBool("IsJumping", this.isJumping);
        this.animator.SetBool("IsRunning", inputMovement != Vector2.zero);
        this.animator.SetFloat("MovementForward", inputMovement.magnitude);
    }

    void HandleJumping()
    {
        if (this.controller.isGrounded && this.isJumping && this.jumpCooldownTimer <= 0.0f)
        {
            this.jumpVelocity = Vector3.zero;
            this.isJumping = false;
        }
        if (this.controller.isGrounded && !this.isJumping && this.jumpAction.WasPressedThisFrame())
        {
            this.characterGravity = Vector3.zero;
            this.jumpVelocity = Vector3.zero;
            this.jumpVelocity.y = this.jumpSpeed;
            this.jumpCooldownTimer = this.jumpCooldown;
            this.isJumping = true;

            this.sfxSource.PlayOneShot(this.jumpSound);
        }
        if (this.jumpVelocity.y > 0.0f)
        {
            this.jumpVelocity.y -= Time.fixedDeltaTime;
        }
        else
        {
            this.jumpVelocity = Vector3.zero;
        }
        this.jumpCooldownTimer -= Time.fixedDeltaTime;
    }

    private void GetPlatformVelocity()
    {
        this.platformVelocity = Vector3.zero;
        if (Physics.Raycast(this.transform.position, Vector3.down, out RaycastHit hitInfo, 2.0f, this.platformLayerMask))
        {
            MovingPlatform platform = hitInfo.collider.GetComponent<MovingPlatform>();
            if (platform != null)
            {
                this.platformVelocity = platform.GetVelocity();
            }
        }
    }

    private void HandleFootsteps(Vector2 inputMovement)
    {
        bool isMoving = inputMovement != Vector2.zero;

        if (this.controller.isGrounded && isMoving && !this.isJumping)
        {
            if (!this.footstepSource.isPlaying)
            {
                this.footstepSource.clip = this.footstepSound;
                this.footstepSource.loop = true;
                this.footstepSource.Play();
            }
        }
        else
        {
            if (this.footstepSource.isPlaying && this.footstepSource.clip == this.footstepSound)
            {
                this.footstepSource.Stop();
            }
        }
    }

    void FixedUpdate()
    {
        this.HandleJumping();
        this.GetPlatformVelocity();

        var inputMovement = this.moveAction.ReadValue<Vector2>();

        this.HandleFootsteps(inputMovement);

        var inputRightDirection = this.cameraTransform.right;
        var inputForwardDirection = this.cameraTransform.forward;
        inputRightDirection.y = 0.0f;
        inputForwardDirection.y = 0.0f;
        inputRightDirection.Normalize();
        inputForwardDirection.Normalize();

        if (this.controller.isGrounded)
        {
            this.characterGravity.y = 0.0f;
        }

        this.characterGravity.y += this.gravity * Time.fixedDeltaTime;
        this.characterMovement += this.characterGravity * Time.fixedDeltaTime;
        this.characterMovement += this.jumpVelocity * Time.fixedDeltaTime;

        this.characterMovement += inputRightDirection * inputMovement.x * this.characterSpeed * Time.fixedDeltaTime;
        this.characterMovement += inputForwardDirection * inputMovement.y * this.characterSpeed * Time.fixedDeltaTime;
        this.characterMovement *= (1 - this.dampening);

        Vector3 characterForward = this.characterMovement;
        characterForward.y = 0.0f;
        if (characterForward.sqrMagnitude > 0.0f && characterForward != Vector3.zero)
        {
            this.transform.forward = characterForward.normalized;
        }

        if (!this.isJumping)
        {
            var combinedMovement = this.characterMovement + this.platformVelocity * Time.fixedDeltaTime;
            this.controller.Move(combinedMovement);
        }
        else
        {
            this.controller.Move(this.characterMovement);
        }

        this.SetAnimationState(inputMovement);
    }
}