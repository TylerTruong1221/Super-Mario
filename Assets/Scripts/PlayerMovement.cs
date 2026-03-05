using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private InputAction moveAction;
    private Vector2 velocity;

    public float moveSpeed = 8f;
    public float acceleration = 20f;
    public float deceleration = 30f;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        moveAction = new InputAction("Move", InputActionType.Value);
        moveAction.AddCompositeBinding("1DAxis").With("Negative", "<Keyboard>/a").With("Positive", "<Keyboard>/d");
    }

    private void OnEnable()
    {
        moveAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
    }

    // Update is called once per frame
    private void Update()
    {
        HorizontalMovement();
    }

    // Handles horizontal movement based on player input
    private void HorizontalMovement()
    {
        velocity.x = Mathf.MoveTowards(velocity.x, 
                moveAction.ReadValue<float>() * moveSpeed, 
                (moveAction.ReadValue<float>() != 0 ? acceleration : deceleration) * Time.deltaTime);
    }

    // FixedUpdate is called in a fixed time interval and is used for physics updates
    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(velocity.x, rb.linearVelocity.y);
    }
}
