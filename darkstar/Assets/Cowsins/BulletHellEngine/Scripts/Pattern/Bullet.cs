using UnityEngine;
using System.Collections;

namespace cowsins.BulletHell
{
/// <summary>
/// Attached to the Cowsins_Bullet.prefab. You can create new bullet prefabs though, 
/// but it is highly recommended to use this prefab, since it is already made and there is basically no reason to make a new prefab. 
/// The bullet is just the object that moves and handles the logic for both player and enemy Bullets.
/// </summary>
public class Bullet : MonoBehaviour
{
    public enum PatternMethod
    {
        Default,ImagePattern
    };
    [HideInInspector] public PatternMethod pattern;

    [HideInInspector] public float speed,acceleration, changeDirectionCooldown,maxSpeed,duration, initialDistance,idleTime, projectileSizeMultiplierOverTime,damage, extentsSize;

    [HideInInspector] public Vector3 emission;

    [HideInInspector] public bool setMaxProjectileSpeed;

    [HideInInspector] public LayerMask hitLayer;

    [HideInInspector] public PatternSpawner.CollisionType collisionType;

    private float currentSpeed;

    private Vector3 direction;

    private bool canMove;

    private float dirCd;

    // Do Initial settings for first-time-spawning bullets
    private void OnEnabled() => InitialSettings();

    public virtual void Update()
    {
        HandleCollisions();

        // If we are idle, we won´t move
        if (!canMove) return;

        // Handle ReSizing
        if (projectileSizeMultiplierOverTime != 0) SizeOverTime(); 

        // Handling acceleration.
        // Speed is increased each frame
        currentSpeed += acceleration * Time.deltaTime;

        // Handle maximum speed
        if(setMaxProjectileSpeed) currentSpeed = Mathf.Clamp(speed , -maxSpeed, maxSpeed);

        // Moving the bullet.
        if (pattern == PatternMethod.Default) transform.Translate(direction.normalized * currentSpeed * Time.deltaTime);
        else transform.position += -transform.right * Time.deltaTime * currentSpeed * initialDistance;


        // Destroying the bullet and avoiding problems with the collisions when the size is null
        if (transform.localScale.x <= .05f && transform.localScale.y <= .05f && transform.localScale.z <= .05f) DestroyMe(); 

    }

    private void ChangeDirections()
    {
        // Setting inverse speed and accelerations to switch directions.
        currentSpeed = -currentSpeed;
        acceleration = -acceleration;
        dirCd = changeDirectionCooldown; 
    }
    public void DestroyMe()
    {
        // Destroy any graphics we may have
        if(transform.childCount != 0) Destroy(transform.GetChild(0).gameObject);

        // Disabling the object
        this.gameObject.SetActive(false);
        GetComponent<Bullet>().enabled = false;
    }

    private void CanMove() => canMove = true; 

    private void SizeOverTime() => transform.localScale += new Vector3(1,1,1) * projectileSizeMultiplierOverTime * Time.deltaTime; 

    private IEnumerator HandleDestruction()
    {
        yield return new WaitForSeconds(duration);
        DestroyMe(); 
    }

    private IEnumerator HandleMovementAllowance()
    {
        yield return new WaitForSeconds(idleTime);
        CanMove();
    }
    
    private IEnumerator HandleDirection()
    {
        if (changeDirectionCooldown == -1) yield break;

        yield return new WaitForSeconds(dirCd);
        ChangeDirections();

        StartCoroutine(HandleDirection()); 
    }

    public void InitialSettings()
    {
        canMove = false; 

        currentSpeed = speed;

        direction = transform.right;

        transform.Translate(direction * initialDistance);

        initialDistance = Vector3.Distance(transform.position, emission);

        transform.localScale = new Vector3(1, 1, 1);

        StartCoroutine(HandleDestruction());
        StartCoroutine(HandleMovementAllowance());
        dirCd = changeDirectionCooldown / 2; 
        StartCoroutine(HandleDirection()); 
    }

    private void HandleCollisions()
    {
        RaycastHit2D hit = collisionType == PatternSpawner.CollisionType.Simple
            ? Physics2D.BoxCast(transform.position, new Vector2(extentsSize * transform.localScale.x, extentsSize * transform.localScale.y), 0, transform.up, extentsSize, hitLayer)
            : Physics2D.CircleCast(transform.position, extentsSize * transform.localScale.x, transform.up, extentsSize, hitLayer);

        if (hit)
        {
            if (hit.transform.GetComponent<IDamageable>() != null) hit.transform.GetComponent<IDamageable>().TakeDamage(damage);
            DestroyMe();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if (collisionType == PatternSpawner.CollisionType.Simple)
            Gizmos.DrawWireCube(transform.position, new Vector3(extentsSize * transform.localScale.x, extentsSize * transform.localScale.y, extentsSize * transform.localScale.z));
        else
            Gizmos.DrawWireSphere(transform.position, extentsSize * transform.localScale.x);
    }
}
}