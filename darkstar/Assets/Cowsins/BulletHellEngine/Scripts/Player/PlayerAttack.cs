using UnityEngine;
using cowsins.BulletHell;
using UnityEngine.Events;
using cowsins; 
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace cowsins.BulletHell
{
/// <summary>
/// Handles player shooting. This system uses the Pattern_SO.
/// </summary>
public class PlayerAttack : MonoBehaviour
{
    [System.Serializable]
    public class Events
    {
        public UnityEvent OnShoot, OnShotRestarted;
    }

    [SerializeField,Tooltip("Attach a pattern spawner, we´ll use that to manually shoot.")] private PatternSpawner spawner;

    [SerializeField, Tooltip("Assign a fire rate for your patterns. For this system, we won´t use the fireRate in the Weapon_SO as we won´t shoot continuously.")] private float fireRate;

    [SerializeField, Tooltip("mount of camera shake effect on shot.")] private float camShakeAmount;

    [SerializeField, Tooltip("Array that contains all the points where muzzleFlashVFX will be instantiated. You can orientate the muzzlePoint so the muzzleFlashVFX will math that rotation.")] private Transform[] muzzle;

    [SerializeField, Tooltip("Visual effect played on each muzzle point on shot.")] private GameObject muzzleFlashVFX;

    [SerializeField, Tooltip("Audio Clip played on Shot.")] private AudioClip[] fireSFX;

    [Range(-.2f, .2f), SerializeField, Tooltip("Fire sfx pitch variation (random between negative value and positive value)")] private float pitchVariationFireSFX;

    [SerializeField] private Events events; 

    private bool canShoot = true;


    private void Update()
    {
        // If the player is not controllable, or cant shoot, return
        if (!PlayerHealth.controllable || !GetComponent<PlayerMovement>().canShootWhileDashing && GetComponent<PlayerMovement>().dashing) return;

        // Shoot if we can shoot and the input is detected
        if (InputManager.playerInputs.Attacking && canShoot) Shoot();
    }

    private void Shoot()
    {
        // Restart shoot logic
        canShoot = false;
        Invoke("RestartShot", fireRate);

        // Play custom method
        events.OnShoot?.Invoke(); 

        // Shoot method on our spawner
        // We won´t play continuously, so we won´t make use of the patter_SO fire rate
        spawner.Shoot(0, ShootingModes.Mode.DefinedAmount, 1);

        // Play audio clip on shot
        SoundManager.Instance.PlaySound(fireSFX[Random.Range(0,fireSFX.Length)], 0, pitchVariationFireSFX, 0);
        
        // If there are no muzzles, dont run this code
        // This iterates through each muzzle and spawns an appropriate muzzle flash VFX
        if(muzzle.Length != 0 && muzzleFlashVFX != null)
        {
            for (int i = 0; i < muzzle.Length; i++)
            {
                // Instantiate the muzzle flash effect and apply a matching scale
                var muz = Instantiate(muzzleFlashVFX, muzzle[i].position, muzzle[i].rotation, GetComponent<PlayerMovement>().Graphics);
                muz.transform.localScale = muzzle[i].localScale;
            }
        }

        // Camera shake effect
        CamShake.instance.ShootShake(camShakeAmount);
    }

    private void RestartShot()
    {
        events.OnShotRestarted?.Invoke();
        canShoot = true;
    }

}
#if UNITY_EDITOR

[CustomEditor(typeof(PlayerAttack))]
public class PlayerAttack2DEditor : Editor
{
    private string[] tabs = { "Basic", "Effects", "Events" };
    private int currentTab = 0;

    override public void OnInspectorGUI()
    {
        serializedObject.Update();
        var myScript = target as PlayerAttack;

        Texture2D myTexture = Resources.Load<Texture2D>("CustomEditor/PlayerAttack_CustomEditor") as Texture2D;
        GUILayout.Label(myTexture);

        EditorGUILayout.BeginVertical();
        currentTab = GUILayout.Toolbar(currentTab, tabs);
        EditorGUILayout.Space(5f);
        EditorGUILayout.EndVertical();

        if (currentTab >= 0 || currentTab < tabs.Length)
        {
            switch (tabs[currentTab])
            {
                case "Basic":
                    EditorGUILayout.LabelField("BASIC SETTINGS", EditorStyles.boldLabel); 
                    GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(8) });
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("spawner"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("fireRate"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("muzzle"));
                    break;
                case "Effects":
                    EditorGUILayout.LabelField("EFFECTS CUSTOMIIZATION", EditorStyles.boldLabel);
                    GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(8) });
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("camShakeAmount"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("muzzleFlashVFX")); 
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("fireSFX"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("pitchVariationFireSFX"));
                    break;
                case "Events":
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("events"));
                    break; 
            }
        }

        serializedObject.ApplyModifiedProperties();

    }
}
#endif
}