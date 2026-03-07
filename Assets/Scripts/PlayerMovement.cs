using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Camera cam;
    private Rigidbody2D rb;
    private InputAction moveAction;
    private InputAction jumpAction;
    private Vector2 velocity;

    public float moveSpeed = 8f;
    public float acceleration = 10f;
    public float deceleration = 15f;
    public float maxJump = 3.5f;
    public float maxJumpTime = 1f;
    public float JumpForce => (2f * maxJump) / (maxJumpTime / 2f);
    public float gravity => (-1.7f * maxJump) / Mathf.Pow((maxJumpTime / 2f), 2);

    // Public getter for other scripts, private setter
    public bool isGrounded { get; private set;}
    public bool isJumping { get; private set; }

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        moveAction = new InputAction("Move", InputActionType.Value);
        moveAction.AddCompositeBinding("1DAxis").With("Negative", "<Keyboard>/a").With("Positive", "<Keyboard>/d");
        jumpAction = new InputAction("Jump", InputActionType.Value);
        jumpAction.AddBinding("<Keyboard>/space");
        cam = Camera.main;
    }

    private void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
    }

    // Update is called once per frame
    private void Update()
    {
        HorizontalMovement();
        isGrounded = rb.Raycast(Vector2.down);

        if (isGrounded)
        {
            GroundedMovement();
        }
        
        ApplyGravity();
    }

    // Handles horizontal movement based on player input
    private void HorizontalMovement()
    {
        velocity.x = Mathf.MoveTowards(velocity.x, 
                moveAction.ReadValue<float>() * moveSpeed, 
                (moveAction.ReadValue<float>() != 0 ? acceleration : deceleration) * Time.deltaTime);
        
        // Stop Plumber movement if collision with wall
        // * velocity.x determines if collision is left or right
        if (rb.Raycast(Vector2.right * velocity.x))
        {
            velocity.x = 0f;
        }
    }

    private void GroundedMovement()
    {
        // Velocity will not keep decreasing while grounded
        velocity.y = Mathf.Max(velocity.y, 0f);
        isJumping = velocity.y > 0f;
        if (jumpAction.triggered && jumpAction.ReadValue<float>() > 0f)
        {
            velocity.y = JumpForce;
            isJumping = true;
        }
    }

    private void ApplyGravity()
    {
        // Gravity must become stronger if the player is not actively pressing jump
        bool falling = velocity.y < 0f || !jumpAction.IsPressed();
        float multiplier = falling ? 2.3f : 1f;
        velocity.y += gravity * Time.deltaTime * multiplier;
        velocity.y = Mathf.Max(velocity.y, gravity / 2f);
    }

    // FixedUpdate is called in a fixed time interval and is used for physics updates
    private void FixedUpdate()
    {
        Vector2 leftEdge = cam.ScreenToWorldPoint(Vector2.zero);
        Vector2 rightEdge = cam.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        Vector2 position = rb.position;
        if (position.x > leftEdge.x + 0.5f || velocity.x > 0f) {
            rb.linearVelocity = new Vector2(velocity.x, velocity.y);
        } else
        {
            rb.linearVelocity = new Vector2(0, velocity.y);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Transform refers to the Transform component attached to Plumber. Stores position, rotation, and scale. 
        if (collision.gameObject.layer != LayerMask.NameToLayer("PowerUp"))
        {
            if (transform.DotTest(collision.transform, Vector2.up))
            {
                velocity.y = 0f;
            }
        }
    }
}
