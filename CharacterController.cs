using UnityEngine;
using UnityEngine.Events;

public class CharacterController : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Amount of force added when the player jumps.")]
    private float m_JumpForce = 400f;

    [SerializeField]
    [Range(0,1)]
    [Tooltip("// Amount of maxSpeed applied to crouching movement. 1 = 100%")]
    private float m_CrouchSpeed = .36f;

    [SerializeField]
    [Range(0, .3f)]
    [Tooltip("How much to smooth out the movement.")]
    private float m_MovementSmoothing = .05f;

    [SerializeField]
    [Tooltip("Whether or not a player can steer while jumping;")]
    private bool m_AirControl = false;

    [SerializeField]
    [Tooltip("A mask determining what is ground to the character")]
    private LayerMask m_WhatIsGround;

    [SerializeField]
    [Tooltip("A position marking where to check if the player is grounded.")]
    private Transform m_GroundCheck;

    [SerializeField]
    [Tooltip("A position marking where to check for ceilings")]
    private Transform m_CeilingCheck;

    [SerializeField]
    [Tooltip("A collider that will be disabled when crouching")]
    private Collider2D m_CrouchDisableCollider;

    const float k_GroundedRadius = .2f;
    private bool m_Grounded;
    const float k_CeilingRadius = .2f;
    private Rigidbody2D m_RigidBody2d;
    private bool m_FacingRight = true;
    private Vector3 m_Velocity = Vector3.zero;

    [Header("Events")]
    [Space]
    public UnityEvent OnLandEvent;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    public BoolEvent OnCrouchEvent;
    private bool m_WasCrouching = false;


    private void Awake()
    {
        m_RigidBody2d = GetComponent<Rigidbody2D>();
        
        if (OnLandEvent == null)
        {
            OnLandEvent = new UnityEvent();
        }

        if (OnCrouchEvent == null)
        {
            OnCrouchEvent = new BoolEvent();
        }
    }

    private void FixedUpdate()
    {
        bool wasGrounded = m_Grounded;
        m_Grounded = false;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for(int i = 0; i < colliders.Length; i++)
        {
            m_Grounded = true;
            if(!wasGrounded)
            {
                OnLandEvent.Invoke();
            }
        }
    }

    public void Move(float move, bool crouch, bool jump)
    {
        // If crouching, check to see if the character can stand up
        if (!crouch)
        {
            // If the character has a ceiling preventing them from standing up, keep them crouching
            if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
            {
                crouch = true;
            }
        }

        // Only control the player if grounded or airControl is turned on
        if (m_Grounded || m_AirControl)
        {
            // If crouching
            if (crouch)
            {
                if (!m_WasCrouching)
                {
                    m_WasCrouching = true;
                    OnCrouchEvent.Invoke(true);
                }

                // Reduce the speed by the crouchSpeed multiplier
                move *= m_CrouchSpeed;

                // Disable one of the colliders when crouching
                if (m_CrouchDisableCollider != null)
                {
                    m_CrouchDisableCollider.enabled = false;
                }
            }
            else
            {
                // Enable the collider when not crouching
                if (m_CrouchDisableCollider != null)
                {
                    m_CrouchDisableCollider.enabled = true;
                }

                if (m_WasCrouching)
                {
                    m_WasCrouching = false;
                    OnCrouchEvent.Invoke(false);
                }
            }

            // Move the character by finding the target velocity
            Vector3 targetVelocity = new Vector2(move * 10f, m_RigidBody2d.velocity.y);
            // And then smoothing it out and applying it to the character
            m_RigidBody2d.velocity = Vector3.SmoothDamp(m_RigidBody2d.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

            // If the input is moving the player right and the player is facing left...
            if (move > 0 && !m_FacingRight)
            {
                // ... flip the player.
                Flip();
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (move < 0 && m_FacingRight)
            {
                // ... flip the player.
                Flip();
            }
        }
        // If the player should jump...
        if (m_Grounded && jump)
        {
            // Add a vertical force to the player.
            m_Grounded = false;
            m_RigidBody2d.AddForce(new Vector2(0f, m_JumpForce));
        }
    }
    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
