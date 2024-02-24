using UnityEngine;

namespace cowsins.BulletHell
{ 
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class ScrollingBackground : MonoBehaviour
{

    [SerializeField,Tooltip("If true, the object will automatically move from start.")] private bool moveOnStart;

    [SerializeField, Tooltip("You MUST turn this on if your scroll works on the x axis. You MUST turn this off if it only works on the y axis.")] private bool xScroll; 

    [SerializeField, Tooltip("maximum velocity allowed.")] private Vector2 maxVelocity;

    [SerializeField,Tooltip("initial velocity of the object.")] private Vector2 initialVelocity;

    [SerializeField, Tooltip("How fast the velocity increases.")] private Vector2 acceleration;

    // Returns the current velocity vector of the object.
    public Vector2 currentVelocity { get; private set; }

    // Returns true if the object can be moved. False if not. You can change this value using AllowMovement() and DisallowMovement()
    public bool canMove { get; private set; }

    private BoxCollider2D col;

    private Rigidbody2D rb;

    private float size;

    public delegate void Scroll();

    public Scroll scroll; 

    void Start()
    {
        // Grab initial references
        col = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();  

        // Set initial velocity
        currentVelocity = initialVelocity;

        // Alow movement if required
        if (moveOnStart) canMove = true;

        // Determine scrol method
        if (xScroll)
        {
            scroll = XScroll; 
            size = col.size.x;
        }
        else
        {
            scroll = YScroll;
            size = col.size.y;
        }
    }

    private void Update()
    {
        // Prevent from moving if we do not want to move.
        if (!canMove) return;

        // Perform custom method
        scroll?.Invoke(); 

        // Prevent from exceeding max velocity values
        if (currentVelocity.magnitude >= maxVelocity.magnitude) return; 

        // Apply acceleration
        currentVelocity += acceleration * Time.deltaTime;
    }

    private void XScroll()
    {
        // Set rigidbody velocity to the current velocity.
        rb.velocity = currentVelocity;

        // Perform movement
        if (transform.position.x < -size)
        {
            Vector2 vector = new Vector2(size * 2, 0);
            transform.position = (Vector2)transform.position + vector;
        }
    }

    private void YScroll()
    {
        // Set rigidbody velocity to the current velocity.
        rb.velocity = currentVelocity;

        // Perform movement
        if (transform.position.y < -size)
        {
            Vector2 vector = new Vector2(0, size * 2);
            transform.position = (Vector2)transform.position + vector;
        }
    }
    /// <summary>
    /// Allows the object to start moving.
    /// </summary>
    public void AllowMovement() => canMove = true;
    /// <summary>
    /// Stops the object from moving.
    /// </summary>
    public void DisllowMovement() => canMove = false;
}
}