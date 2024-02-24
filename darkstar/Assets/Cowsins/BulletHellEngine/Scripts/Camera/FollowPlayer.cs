using UnityEngine;

namespace cowsins.BulletHell
{
/// <summary>
/// Attached on the parent of your camera, handles its movement.
/// </summary>
public class FollowPlayer : MonoBehaviour
{
    [SerializeField,Tooltip("The camera will follow this transform object.")] private Transform player;

    [SerializeField, Tooltip("Lerp velocity. The higher the value, the faster it will reach Player´s destination. ")] private float speed;

    [SerializeField, Tooltip("If true, the camera will point towards the player.")] private bool lookAtPlayer; 

    private Vector3 offset; 

    // Set the offset. We will use it to determine the camera position later on.
    private void Awake() => offset = transform.localPosition; 

    private void FixedUpdate()
    {
        // If there is no player, do not try to follow a player!
        if (player == null) return;

        // lerp the current position of the camera to follow the player position + offset.
        transform.position = Vector3.Lerp(transform.position, player.position + offset, speed * Time.deltaTime);
    }
}
}