using UnityEngine;
using cowsins.BulletHell;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace cowsins.BulletHell
{
/// <summary>
/// Manages the bullets instantiating through patterns (Pattern_SO). 
/// It also helps you defining the visuals of these. 
/// </summary>
public class PatternSpawner : MonoBehaviour
{
    #region variables

    [System.Serializable]
    public enum CollisionType
    {
        Simple,Accurate
    }
    [SerializeField,Tooltip("Set this to true if the pattern spawner belongs to the player.")] 
    private bool isPlayer;

    [Tooltip("Selects collision method. Simple collision types will result in a less accurate collision, but it will help with performance. " +
        "Accurate collision types will result in a more precise collision, but it will harm the performance, since it uses circular / spherical collisions." +
        "Note that collisions are handled through code, not through the system that Unity offers, to gain performance")]
    public CollisionType collisionType; 

    [SerializeField, Tooltip("Layers that this pattern spawner bullets can collide with. It is very important to highlight that this layer mask " +
        "ONLY AFFECTS the bullets instantiated by THIS pattern spawner.")] 
    private LayerMask hitLayer; 

    [Tooltip("Array that contains all the pattern_SO that this PatternSpawner is capable of shooting.")]
    public Pattern_SO[] patterns;

    [SerializeField, Tooltip("Graphic element for the bullet of each pattern in the Patterns array. There MUST be the same number of patterns and graphics elements.")] 
    private Transform[] graphics;

    // Returns true if the system is ready to shoot.
    public bool readyToShoot { get; private set; } = false;

    // Returns the current pattern set.
    public int currentPattern { get; set; }

    // Returns the current spin amount.
    public float spinAmount { get; private set; }

    private float timer;

    private float[] rotations;

    private int shotsRemaining;

    private float angleRotationMin,angleRotationMax; 

    private ShootingModes.Mode mode;

    #endregion

    #region methods

    #region shooting_methods
    /// <summary>
    /// Allows a pattern spawner to shoot a pattern. It shoots only once
    /// for defined amount shooting modes.
    /// </summary>
    ///  <param name="pattern">Order ID of the pattern in the patterns array of the PatternSpawner.</param>
    ///  <param name="_mode"> Shooting Mode required. Choose between DefinedAmount and Continuous shooting. Check ShootingModes.cs.</param>
    public void Shoot(int pattern, ShootingModes.Mode _mode)
    {
        if (_mode == ShootingModes.Mode.DefinedAmount)
        shotsRemaining = 1;

        currentPattern = pattern;   
        mode = _mode;

        Shoot(pattern);
    }

    /// <summary>
    /// Allows a pattern spawner to shoot a pattern. It also allows to
    /// set a finite number of times to shoot, in case the shooting mode selected is not continuous.
    /// </summary>
    ///  <param name="pattern">Order ID of the pattern in the patterns array of the PatternSpawner.</param>
    ///  <param name="_mode"> Shooting Mode required. Choose between DefinedAmount and Continuous shooting. Check ShootingModes.cs.</param>
    ///  <param name="times"> For defined amount shooting modes, amount of times to shoot. </param>
    public void Shoot(int pattern, ShootingModes.Mode _mode, int times)
    {
        if (times <= 0) Debug.LogError("ERROR: Can´t shoot 0 or negative times. Please assign a positive value above 0");
        shotsRemaining = times;
        currentPattern = pattern;

        Shoot(pattern);
    }
    /// <summary>
    /// Allows a pattern spawner to shoot a pattern. This the base shoot function.
    /// </summary>
    ///  <param name="pattern">Order ID of the pattern in the patterns array of the PatternSpawner.</param>
    private void Shoot(int pattern)
    {
        StopShooting(); 
        transform.localRotation = Quaternion.Euler(Vector3.zero); 
        timer = 0;
        rotations = new float[patterns[currentPattern].numberOfProjectiles];
        readyToShoot = true;

        angleRotationMin = patterns[currentPattern].initialMinimumRotation;
        angleRotationMax = patterns[currentPattern].initialMaximumRotation; 
    }
    /// <summary>
    /// Stops the shots no matter what.
    /// </summary>
    public void StopShooting() => readyToShoot = false;

    #endregion

    private void Update()
    { 
        // Check if we are ready to shoot
        if (timer <= 0 && readyToShoot)
        {
            SpawnBullets(currentPattern);
            // Reset timer
            timer = patterns[currentPattern].shootCooldown;
        }
        else
        {
            // keep the timer counting
            timer -= Time.deltaTime;

            // Update spin amount in case we use spin
            if (patterns[currentPattern].spinAccelerationAmount != 0 && (!patterns[currentPattern].useMaxSpinAmount || patterns[currentPattern].useMaxSpinAmount && spinAmount < patterns[currentPattern].maxSpinAmount))
                spinAmount += patterns[currentPattern].spinAccelerationAmount * Time.deltaTime;
            else spinAmount = patterns[currentPattern].spinAmount; 
        }
    }

    // Sets random rotations to the spawned bullets.
    // Note that it returns an array of floats, whose length matches the number of projectiles.
    private float[] RandomRotation(int patternSelected)
    {
        // Random rotation for manual angle patterns
        if (patterns[patternSelected].angleStyle == Pattern_SO.Angle.Manual)
        {
            for (int i = 0; i < patterns[patternSelected].numberOfProjectiles; i++)
                rotations[i] = Random.Range(patterns[patternSelected].initialMinimumRotation, patterns[patternSelected].initialMaximumRotation);
        }
        else
        {
            // Random rotation for automatic andle patterns
            for (int i = 0; i < patterns[patternSelected].numberOfProjectiles; i++)
                rotations[i] = Random.Range(0,360);
        }
        return rotations; // Return the array for later

    }

    // Handle rotation. Note that Random Rotation is handled in RamdonRotation function.
    // Also note that it returns an array of floats, whose length matches the number of projectiles.
    private float[] Rotate(int patternSelected)
    {
        float spread = (patterns[patternSelected].useSpread) ? Random.Range(patterns[patternSelected].minSpread, patterns[patternSelected].maxSpread) : 0;
        float parentRotation = 0;   
        // Rotation for manual patterns
        if (patterns[patternSelected].angleStyle == Pattern_SO.Angle.Manual)
        {
            if(patterns[patternSelected].increaseAngle)
            {
                angleRotationMin += patterns[patternSelected].minimumRotationStep;
                angleRotationMax += patterns[patternSelected].maximumRotationStep;

                angleRotationMin = Mathf.Clamp(angleRotationMin, -patterns[currentPattern].clampMinimumRotation, patterns[currentPattern].clampMinimumRotation);
                angleRotationMax = Mathf.Clamp(angleRotationMax, -patterns[currentPattern].clampMaximumRotation, patterns[currentPattern].clampMaximumRotation);
            }
            // Grab the parent rotation in case we need it
            if (patterns[patternSelected].influenceRotationByParent) parentRotation = transform.parent.rotation.eulerAngles.z; 

            // For each of the bullets, grab the appropriate rotation
            for (int i = 0; i < patterns[patternSelected].numberOfProjectiles; i++)
            {
                var fraction = (float)i / ((float)patterns[patternSelected].numberOfProjectiles - 1);
                var difference = angleRotationMax - angleRotationMin;
                var fractionOfDifference = fraction * difference;    
                rotations[i] = parentRotation +fractionOfDifference + angleRotationMin + spread + Time.realtimeSinceStartup * spinAmount;
            }
        }
        else
        {
            if (patterns[patternSelected].influenceRotationByParent) parentRotation = transform.parent.rotation.eulerAngles.z;

            // Non manual patterns rotation handling
            for (int i = 0; i < patterns[patternSelected].numberOfProjectiles; i++)
            {
                int multiplier = (i % 2 == 0 && patterns[patternSelected].invertSpinDirections) ? 1 : -1; 
                rotations[i] = (360 / patterns[patternSelected].numberOfProjectiles) * i + Time.realtimeSinceStartup * spinAmount * multiplier + spread + parentRotation;
            }
        }

        return rotations;
    }

    private void SpawnBullets(int patternSelected)
    {
        // Apply default or random rotation
        if (patterns[patternSelected].applyRandomness) RandomRotation(patternSelected);
        else Rotate(patternSelected); 

        // this is used to know how many times left we should spawn bullets.
        if(mode != ShootingModes.Mode.Continuous) shotsRemaining--;

        // Different shootings for different pattern methods
        switch(patterns[patternSelected].patternMethod)
        {
            // Default method shooting
            case Pattern_SO.PatternMethod.Default:
                for (int i = 0; i < patterns[patternSelected].numberOfProjectiles; i++)
                {
                    // Request bullet from the pool at the CURRENT position of the array, handled by i variable.
                    var bullet = PoolManager.Instance.RequestBullet();

                    // Set properties for the current "i" bullet.
                    SetBulletProperties(bullet, patternSelected, i, Mathf.Sin((patterns[patternSelected].vertexNumber * 2) * ((float)i / (float)patterns[patternSelected].numberOfProjectiles * Mathf.PI)));

                    // Set initial settings on the bullet.cs script each time we request a new bullet from the pool.
                    bullet.InitialSettings();
                }
                break;
            // Image pattern shooting
            case Pattern_SO.PatternMethod.ImagePattern:
                // Shoot Image Pattern Method. Note that we require Cowsins_ImagePattern_Spawner Component attached
                GetComponent<ImagePatternSpawner>().Shoot(); 
                break; 
        }

        // Play custom sound
        if(patterns[patternSelected].shotSFX != null)
        SoundManager.Instance.PlaySound(patterns[patternSelected].shotSFX, 0, patterns[patternSelected].pitchVariationSFX, 0);

        // If we no longer have enough remaining shots, Stop Shooting
        if (mode == ShootingModes.Mode.DefinedAmount && shotsRemaining <= 0) StopShooting();
    }

    public void SetBulletProperties(Bullet projectile, int patternSelected, int i, float vertexOffset)
    {   
        Pattern_SO bulletPattern = patterns[patternSelected]; 

        // Handling initial transforms for default shooting
        if (patterns[patternSelected].patternMethod == Pattern_SO.PatternMethod.Default)
        {
            projectile.transform.position = transform.position;

            projectile.transform.rotation = Quaternion.Euler(0, 0, rotations[i] / 2);
        }

        // Setting variables
        projectile.pattern = (bulletPattern.patternMethod == Pattern_SO.PatternMethod.Default) ? Bullet.PatternMethod.Default : Bullet.PatternMethod.ImagePattern;  
        // Extra speed is used to determine appropriate speed values, specially when using adaptative speed
        projectile.speed = (bulletPattern.adaptativeSpeed) 
            ? (bulletPattern.projectileSpeed + vertexOffset)
            : bulletPattern.projectileSpeed; 
        projectile.maxSpeed = bulletPattern.maxProjectileSpeed;
        projectile.duration = bulletPattern.projectileDuration;
        projectile.acceleration = bulletPattern.projectileAcceleration;
        projectile.setMaxProjectileSpeed = bulletPattern.setMaxProjectileSpeed;
        projectile.initialDistance = bulletPattern.spawnDistanceFromEmitter + vertexOffset; 
        projectile.idleTime = bulletPattern.idleTime;
        projectile.projectileSizeMultiplierOverTime = bulletPattern.projectileSizeMultiplierOverTime;
        projectile.extentsSize = bulletPattern.extentsSize;
        projectile.damage = isPlayer ? bulletPattern.damagePerHit * PlayerModifiers.Instance.damageDealtModifier : bulletPattern.damagePerHit;
        projectile.changeDirectionCooldown = (bulletPattern.changeDirections) ? bulletPattern.changeDirectionTimer : -1;

        projectile.emission = transform.position;
        projectile.hitLayer = hitLayer; 
        projectile.collisionType = collisionType; 

        // Instantiate our desired graphics. Note that graphics are destroyed each time a bullet returns to the pool.
        var graph = Instantiate(graphics[patternSelected], projectile.transform.position, Quaternion.identity , projectile.transform);
        if (bulletPattern.patternMethod == Pattern_SO.PatternMethod.Default)
        {
            Vector3 rotOrientation = transform.forward;
            graph.Rotate(rotOrientation, rotations[i]);
        }
        else
        {
            graph.right = graph.position - transform.position;
        }
 
    }

    #endregion
}
#if UNITY_EDITOR
[System.Serializable]
[CustomEditor(typeof(PatternSpawner))]
public class Cowsins_PatternSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        PatternSpawner myScript = target as PatternSpawner;
        Texture2D myTexture = Resources.Load<Texture2D>("CustomEditor/PatternSpawner_CustomEditor") as Texture2D;
        GUILayout.Label(myTexture);
        EditorGUILayout.BeginVertical();
        EditorGUILayout.Space(10f);
        GUILayout.Label("PATTERN SPAWNER", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Pattern spawner allows you to shoot bullets using Bullet Hell patterns with the `Shoot` function. " +
            "These patterns must be attached under `Patterns` and can be accessed through code depending on the order ID. (integer)", EditorStyles.helpBox);
        EditorGUILayout.Space(10f);
        EditorGUILayout.BeginVertical();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("isPlayer"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("patterns"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("graphics"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("collisionType"));
        if(myScript.collisionType == PatternSpawner.CollisionType.Simple)
            EditorGUILayout.LabelField("Simple collision types will result in a less accurate collision, but it will help with performance. " +
                "Note that collisions are handled through code, not through the system that Unity offers, in order to gain performance.", EditorStyles.helpBox); 
        else
            EditorGUILayout.LabelField("Accurate collision types will result in a more precise collision, but it will harm the performance, since it uses circular / spherical collisions. " +
                "Note that collisions are handled through code, not through the system that Unity offers, in order to gain performance.", EditorStyles.helpBox);
        EditorGUILayout.LabelField("You can adjust the collision extents for each pattern on their Pattern Scriptable Objects.", EditorStyles.helpBox);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("hitLayer"));

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(5f);

        EditorGUILayout.BeginHorizontal(GUI.skin.window);

        GUILayout.Label("These buttons should be used in play-mode");
        if (GUILayout.Button("Play One Shot")) myScript.Shoot(0,ShootingModes.Mode.DefinedAmount,1);
        if (GUILayout.Button("Play Continuous")) myScript.Shoot(0, ShootingModes.Mode.Continuous);

        EditorGUILayout.EndHorizontal();
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
}