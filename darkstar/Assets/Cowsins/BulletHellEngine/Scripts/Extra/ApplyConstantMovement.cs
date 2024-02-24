using UnityEngine;

namespace cowsins.BulletHell
{ 
public class ApplyConstantMovement : MonoBehaviour
{
    [SerializeField,Tooltip("If true, the object will automatically move from start.")] private bool moveOnStart;

    [SerializeField, Tooltip("Movement directions applied. It is recommendable to set a normalized vector.")] private Vector3 movement;

    [SerializeField, Tooltip("Rotations applied. (No need to normalize it)")] private Vector3 rotation; 

    [SerializeField, Tooltip("Maximum speed allowed.")] private float maxSpeed;

    [SerializeField, Tooltip("Initial speed of the object.")] private float initialSpeed;

    [SerializeField, Tooltip("Capacity of gaining speed.")] private float acceleration;

    //Returns true if the object can move.
    public bool canMove { get; private set; }

    //Returns the current speed.
    public float currentSpeed { get; private set; }

    private void Start()
    {
        // Set initial speed
        currentSpeed = initialSpeed;

        // Start moving if required
        if (moveOnStart) canMove = true; 
    }
    private void Update()
    {
        // Do not proceed if we should not move.
        if (!canMove) return;

        // Handle acceleration
        currentSpeed += acceleration * Time.deltaTime;

        // Clamp the current speed to the maximum allowed speed
        currentSpeed = Mathf.Clamp(currentSpeed,0, maxSpeed); 

        // Apply the movement
        transform.position += movement * Time.deltaTime * currentSpeed;

        // Do not rotate in case we do not need it
        if (rotation == Vector3.zero) return;

        // Apply rotation
        transform.Rotate(rotation * Time.deltaTime); 
    }
    /// <summary>
    /// Allows the object to start moving.
    /// </summary>
    public void AllowMovement() => canMove = true;
    /// <summary>
    /// Stops the object from moving.
    /// </summary>
    public void DisallowMovement() => canMove = false;
}
}