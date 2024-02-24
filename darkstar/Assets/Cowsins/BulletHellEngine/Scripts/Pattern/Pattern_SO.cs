using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace cowsins.BulletHell
{
[CreateAssetMenu(fileName = "New Pattern", menuName = "COWSINS/New Pattern", order = 1)]
public class Pattern_SO : ScriptableObject
{

    [Tooltip("You can assign a name to your pattern for better organization. This is optional though.")]public string patternName; 

    public enum PatternMethod
    {
        Default, ImagePattern
    };

    [Tooltip("Determines the style of the pattern. ")] public PatternMethod patternMethod;

    [Tooltip("Image that describes the shape of the pattern. ")] public Texture2D pattern;

    [Tooltip("Size of the pattern. Adaptative distance from the center.")] public int patternSize;

    [Tooltip("The step determines the amount of pixels that the system will check. " +
        "The higher this value is, the lower number of pixels it checks, so it´s more performant.")]
    [Range(15, 80)] public int xStep, yStep;

    public enum Angle
    {
        Manual, Automatic
    };

    [Tooltip("Determines the way of calculating the angle. Manual will grant you all the tools to adjust the bullets as you please, " +
        "while automatic will do that for you automatically"),SerializeField]
    public Angle angleStyle;

    [Tooltip("Set to true if you want to modify the angle during gameplay.")] public bool increaseAngle;

    [Tooltip("Set this to true if the pattern should depend on the parent rotation.")] public bool influenceRotationByParent;

    [Tooltip(" For manual angle styles, minimum  degree value for the angle.")] public float initialMinimumRotation;

    [Tooltip(" For manual angle styles, maximum degree value for the angle.")] public float initialMaximumRotation;

    [Tooltip("Amount in degrees that the minimum rotation changes each time you shoot.")] public float minimumRotationStep;

    [Tooltip("Amount in degrees that the maximum rotation changes each time you shoot.")] public float maximumRotationStep;

    [Tooltip("Limit value for the minimum rotation")] public float clampMinimumRotation;

    [Tooltip("Limit value for the maximum rotation.")] public float clampMaximumRotation;

    [Tooltip("Projectiles shot (per fire)")] [Min(1)]public int numberOfProjectiles;

    [Tooltip("For continuous shooting, shooting interval.")] [Min(.1f)] public float shootCooldown;

    [Tooltip("Distance from the center of the emitter. " +
        "(PatternSpawner location, since it acts as the emitter)")] public float spawnDistanceFromEmitter;

    [Tooltip("Bullet spin amount per fire. Set to 0 for null spin.")] public float spinAmount;

    [Tooltip("Set this to true if the pattern spawner should invert spin directions. " +
        "(Used to create effects such as flowers etc…) ")] public bool invertSpinDirections;

    [Tooltip("Bullet spin acceleration.")] public float spinAccelerationAmount;

    public bool useMaxSpinAmount;

    [Tooltip("limit spin velocity.")] public float maxSpinAmount;

    [Tooltip("Projectile lifetime. Keep this as low as possible for better performance results.")] public float projectileDuration;

    [Tooltip("If true, random rotations will be applied, always respecting the limits set.")] public bool applyRandomness;

    [Tooltip("Set to true if you want to use spread.")] public bool useSpread;

    public float minSpread,maxSpread;

    [Tooltip("Time for the bullet to start moving once it gets spawned.")] [Min(0)]public float idleTime;

    [Tooltip("Damage dealt to the IDamageable that it collides with. ")] public float damagePerHit;

    [Tooltip("Velocity of the bullet. Set to negative speed to invert velocity values.")] public float projectileSpeed;

    [Tooltip("Set this to true for the system to calculate the appropriate speed based on the distance from the emitter." +
        " This is most used for image-based patterns or patterns with vertex numbers greater than 0.")] public bool adaptativeSpeed;

    [Tooltip("Limit the projectile velocity.")] public bool setMaxProjectileSpeed;

    [Tooltip("Maximum allowed projectile velocity.")] public float maxProjectileSpeed;

    [Tooltip("Set projectile acceleration. 0 for null acceleration = constant speed.")] public float projectileAcceleration;

    [Tooltip("Amount of growth of the player per frame. " +
        "Positive values make the bullet bigger, while negative values make it smaller.")] public float projectileSizeMultiplierOverTime;

    [Tooltip("Set this to true if the pattern should change directions based on the timer.")] public bool changeDirections;

    [Tooltip("Amount of time before the directions change. " +
        "Note that, this value is divided by two for the first direction change, " +
        "since it comes from the emitter and the distance required to travel is half.")] [Min(.1f)]public float changeDirectionTimer;

    [Tooltip("Allows to simulate polygons. 0 means no vertex (circular)," +
        " 3 vertex number is a triangle, 4 vertex number is a square etc… Notice that it is based on sinus shapes, so it will not be a perfect polygon shape")] public int vertexNumber;

    [Tooltip("Collision amount. Enable Gizmos in play mode to see the size of the collisions. " +
        "Notice that the collisions are handled through code to improve performance. " +
        "Bullet Hell Engine does not use the Collider system by Unity for the bullets of the pattern spawner.")] [Min(.05f)] public float extentsSize;

    [Tooltip("Audio clip played when shooting. Set to null if you do not want to play an audio.")] public AudioClip shotSFX;

    [Tooltip("Pitch variation of the shotSFX.")] [Range(-.2f,.2f)]public float pitchVariationSFX;
}

#if UNITY_EDITOR
#region customEditor
/// <summary>
/// CUSTOM EDITOR STUFF
/// </summary>
[System.Serializable]
[CustomEditor(typeof(Pattern_SO))]
public class Cowsins_Pattern_SO_Editor : Editor
{
    private string[] tabs = { "Pattern", "Stats", "Others"};
    private int currentTab = 0;

    override public void OnInspectorGUI()
    {
        serializedObject.Update();
        Pattern_SO myScript = target as Pattern_SO;

        Texture2D myTexture = Resources.Load<Texture2D>("CustomEditor/WeaponObject_CustomEditor") as Texture2D;
        GUILayout.Label(myTexture);

        EditorGUILayout.BeginVertical();
        GUILayout.Label("*Optional*",EditorStyles.helpBox);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("patternName"));
        EditorGUILayout.Space(10f);
        currentTab = GUILayout.Toolbar(currentTab, tabs);
        EditorGUILayout.Space(10f);
        EditorGUILayout.EndVertical();

        if (currentTab >= 0 || currentTab < tabs.Length)
        {
            switch (tabs[currentTab])
            {
                case "Pattern": 
                    ClassicVariables(myScript);
                    break; 

                    case "Others":
                        GUILayout.Label("COLLISIONS", EditorStyles.boldLabel);
                        GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(8) });
                        EditorGUILayout.Space(3f);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("extentsSize"));

                        EditorGUILayout.Space(10f);
                        GUILayout.Label("AUDIO",EditorStyles.boldLabel);
                        GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(8) });
                        EditorGUILayout.Space(3f);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("shotSFX"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("pitchVariationSFX"));
                    break; 
                case "Stats":
                    GUILayout.Label("PATTERN STATISTICS", EditorStyles.boldLabel);
                    GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(8) });
                    EditorGUILayout.Space(3f);

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("damagePerHit"));
                    break; 
            }
        }
        serializedObject.ApplyModifiedProperties();
    }

    private void ClassicVariables(Pattern_SO myScript)
    {
        
        GUILayout.Label("PATTERN CREATION", EditorStyles.boldLabel);
        GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(8) });
        EditorGUILayout.Space(3f);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("patternMethod"));

        if(myScript.patternMethod == Pattern_SO.PatternMethod.ImagePattern)
        {
            EditorGUI.indentLevel++;
            EditorGUI.indentLevel++;
            GUILayout.Label("WARNING: BETA FEATURE! Work in progress, it might not be 100% optimized.", EditorStyles.helpBox);
            EditorGUI.indentLevel--;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("pattern"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("patternSize"));

            if(myScript.pattern != null && ( myScript.xStep < 25 || myScript.yStep < 25 ) && ( myScript.pattern.width >= 1920 || myScript.pattern.width >= 1080 )) GUILayout.Label("WARNING:LOW STEP VALUES FOR HEAVY IMAGES WILL CAUSE PERFORMANCE ISSUES.", EditorStyles.helpBox);
            if (myScript.pattern != null && (myScript.xStep > 20 || myScript.yStep > 20) && (myScript.pattern.width < 1000 || myScript.pattern.width < 800)) GUILayout.Label("WARNING: WITH HIGH STEP VALUES FOR LIGHT IMAGES, MANY IMPORTANT SPAWNING POINTS WILL BE IGNORED.", EditorStyles.helpBox);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("xStep"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("yStep")); 

            EditorGUI.indentLevel--;
        }
        else
        {
            myScript.pattern = null; 

            EditorGUILayout.PropertyField(serializedObject.FindProperty("numberOfProjectiles"));

            if (myScript.numberOfProjectiles < 14)
                myScript.vertexNumber = 0; 
            else  EditorGUILayout.PropertyField(serializedObject.FindProperty("vertexNumber"));

            if (myScript.numberOfProjectiles != 1) EditorGUILayout.PropertyField(serializedObject.FindProperty("angleStyle"));

            if(myScript.numberOfProjectiles == 1 ) EditorGUILayout.PropertyField(serializedObject.FindProperty("influenceRotationByParent"));

            if (myScript.numberOfProjectiles == 1) myScript.angleStyle = Pattern_SO.Angle.Automatic;
            if (myScript.angleStyle == Pattern_SO.Angle.Manual)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("influenceRotationByParent"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("increaseAngle"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("initialMinimumRotation"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("initialMaximumRotation"));
                if(myScript.increaseAngle)
                {
                    EditorGUI.indentLevel++;
                    GUILayout.Label("DISCLAIMER: If your rotation step is positive, add a value higher than the initial one for the clamp rotation ( for both minimum and rotation ).", EditorStyles.helpBox);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("minimumRotationStep"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("maximumRotationStep"));  
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("clampMinimumRotation"));
                    if (myScript.minimumRotationStep > 0 && myScript.clampMinimumRotation < myScript.initialMinimumRotation) myScript.clampMinimumRotation = myScript.initialMinimumRotation;
                    if (myScript.minimumRotationStep < 0 && myScript.clampMinimumRotation > myScript.initialMinimumRotation) myScript.clampMinimumRotation = myScript.initialMinimumRotation;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("clampMaximumRotation"));
                    if (myScript.maximumRotationStep > 0 && myScript.clampMaximumRotation < myScript.initialMaximumRotation) myScript.clampMaximumRotation = myScript.initialMaximumRotation;
                    if (myScript.maximumRotationStep < 0 && myScript.clampMaximumRotation > myScript.initialMaximumRotation) myScript.clampMaximumRotation = myScript.initialMaximumRotation;
                    EditorGUI.indentLevel--;
                }
                EditorGUI.indentLevel--;
            }
            else
            {
                if(myScript.numberOfProjectiles > 1) myScript.influenceRotationByParent = false;
                myScript.increaseAngle = false; 
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("applyRandomness"));
            if (!myScript.applyRandomness) EditorGUILayout.PropertyField(serializedObject.FindProperty("useSpread"));
            else myScript.useSpread = false; 

            if(myScript.useSpread)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("minSpread"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("maxSpread"));
                EditorGUI.indentLevel--;
            }
        }


        EditorGUILayout.PropertyField(serializedObject.FindProperty("idleTime"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("shootCooldown"));

        if (myScript.patternMethod != Pattern_SO.PatternMethod.ImagePattern) EditorGUILayout.PropertyField(serializedObject.FindProperty("spawnDistanceFromEmitter"));
        else myScript.spawnDistanceFromEmitter = 0; 

        EditorGUILayout.PropertyField(serializedObject.FindProperty("projectileSpeed"));

        if (myScript.patternMethod != Pattern_SO.PatternMethod.ImagePattern) EditorGUILayout.PropertyField(serializedObject.FindProperty("adaptativeSpeed"));

        if (myScript.patternMethod == Pattern_SO.PatternMethod.ImagePattern) myScript.adaptativeSpeed = false; 

        EditorGUILayout.PropertyField(serializedObject.FindProperty("setMaxProjectileSpeed"));

        if (myScript.setMaxProjectileSpeed)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxProjectileSpeed"));
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("projectileAcceleration")); 

        EditorGUILayout.PropertyField(serializedObject.FindProperty("projectileSizeMultiplierOverTime"));

        if (myScript.patternMethod == Pattern_SO.PatternMethod.Default)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("spinAmount"));

            if(myScript.spinAmount != 0)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("invertSpinDirections"));
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.PropertyField(serializedObject.FindProperty("spinAccelerationAmount"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("useMaxSpinAmount"));

            if (myScript.useMaxSpinAmount)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("maxSpinAmount"));
                EditorGUI.indentLevel--;
            }
        }       


        EditorGUILayout.PropertyField(serializedObject.FindProperty("projectileDuration"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("changeDirections"));

        if(myScript.changeDirections)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("changeDirectionTimer"));
            EditorGUI.indentLevel--;
        }

    }
}
    #endregion
#endif
}