using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using cowsins.BulletHell;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace cowsins.BulletHell
{
    /// <summary>
    /// Manages the player damage and health system.
    /// </summary>
    public class PlayerHealth : Damageable
    {
        [SerializeField, Tooltip("Image used to display effects like hurt or heal.")] private Image playerStateUI;

        [SerializeField, Tooltip("playerStateUI color when damage.")] private Color damageColor;

        [SerializeField, Tooltip("playerStateUI color when heal.")] private Color healColor;

        [SerializeField, Tooltip("Speed of fade out effect for the playerStateUI color.")] private float fadeSpeed;

        [SerializeField, Tooltip("Audio Clip played on damage.")] private AudioClip damagedSFX;

        [SerializeField, Tooltip("Audio Clip played on healing.")] private AudioClip healSFX;

        [Range(-.2f, .2f), SerializeField, Tooltip("SFX pitch variation (random between negative and positive value)")] private float pitchVariationSFX;

        [SerializeField, Tooltip("CamShake amount on hit (damage)")] private float damageCamShakeAmount;

        [Tooltip("Set to true if you want the player to blink/flash on hit.")] public bool flashesOnDamage;

        [SerializeField, Tooltip("How long the effect is played.")] private float flashDuration;

        [SerializeField, Tooltip("How fast the effect is played.")] private float flashSpeed;

        private float blinkValue;

        [SerializeField, Tooltip("Image that displays health or shield. This must have a sprite assigned and set to Filled.")] protected Image healthSlider, shieldSlider;

        [SerializeField, Tooltip("Array that contains all the spriteRenderers that will blink on damage. These must have the Blink material assigned.")] protected SpriteRenderer[] sprites;

        private bool canReceiveDamage = true;


        public override void Update()
        {
            ManageControl();
            ManageUI();
            base.Update();
        }

        // Lose control if dead
        private void ManageControl()
        {
            if (isDead) LoseControl();
        }

        public override void TakeDamage(float damage)
        {
            // Return if we should not receive damage
            if (!canReceiveDamage || !GetComponent<PlayerMovement>().receiveDamageWhileDashing && GetComponent<PlayerMovement>().dashing) return;

            // Apply flash damage if required.
            if (flashesOnDamage)
                FlashDamage();

            // UI Effect
            playerStateUI.color = damageColor;

            // Base damage method
            base.TakeDamage(damage * GetComponent<PlayerModifiers>().damageReceivedModifier);

            // Play sound effects
            SoundManager.Instance.PlaySound(damagedSFX, 0, pitchVariationSFX, 0);

            // Play visual effects
            CamShake.instance.Shake(damageCamShakeAmount, 16, .8f, 17);
        }

        public override void TakeHeals(float heals)
        {
            // UI effect
            playerStateUI.color = healColor;

            // Base heal
            base.TakeHeals(heals * GetComponent<PlayerModifiers>().healReceivedModifier);

            // Play sound effects
            SoundManager.Instance.PlaySound(healSFX, 0, pitchVariationSFX, 0);
        }
        public void FlashDamage()
        {
            // Stop flashing and reset the flash value
            StopCoroutine(IFlashDamage());
            blinkValue = 0;

            // Restart flash
            StartCoroutine(IFlashDamage());
        }
        public IEnumerator IFlashDamage()
        {
            // Player should not receive damage while flashing
            canReceiveDamage = false;

            // While the time is not over, iterate through each object in the sprites array
            // Change the blink value for each of these.
            var elapsedTime = 0f;
            while (elapsedTime <= flashDuration)
            {
                for (int i = 0; i < sprites.Length; i++)
                {
                    var material = sprites[i].GetComponent<SpriteRenderer>().material;
                    material.SetFloat("_FlashAmount", blinkValue);
                    blinkValue = Mathf.PingPong(elapsedTime * flashSpeed, 1f);
                    elapsedTime += Time.deltaTime;
                }
                yield return null;
            }
            // re iterate through each object and reset the material value
            for (int i = 0; i < sprites.Length; i++)
            {
                var material = sprites[i].GetComponent<SpriteRenderer>().material;
                material.SetFloat("_FlashAmount", 0);
            }

            // We can keep receiving damage on hit after the effect is done.
            canReceiveDamage = true;
        }
        private void ManageUI()
        {
            // Manages the playerStateUI color and fade out effect on hit.
            if (playerStateUI.color.a > 0) playerStateUI.color -= new Color(0, 0, 0, Time.deltaTime * fadeSpeed);

            if (healthSlider != null) healthSlider.fillAmount = Mathf.Lerp(healthSlider.fillAmount, health / maxHealth, Time.deltaTime);

            if (shieldSlider != null) shieldSlider.fillAmount = Mathf.Lerp(shieldSlider.fillAmount, shield / maxShield, Time.deltaTime);

        }
        /// <summary>
        /// Returns true if the player can perform actions.
        /// </summary>
        public static bool controllable { get; private set; } = true;
        /// <summary>
        /// Allow the player to perform actions.
        /// </summary>
        public static void GrantControl() => controllable = true;
        /// <summary>
        /// Disallow the player to perform actions.
        /// </summary>
        public static void LoseControl() => controllable = false;

    }

#if UNITY_EDITOR

    [CustomEditor(typeof(PlayerHealth))]
    public class PlayerHealth2DEditor : Editor
    {
        private string[] tabs = { "Basic", "Effects", "UI", "Events" };
        private int currentTab = 0;

        override public void OnInspectorGUI()
        {
            serializedObject.Update();
            var myScript = target as PlayerHealth;

            Texture2D myTexture = Resources.Load<Texture2D>("CustomEditor/PlayerHealth_CustomEditor") as Texture2D;
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
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("health"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("shield"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("maxHealth"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("maxShield"));
                        break;
                    case "Effects":
                        EditorGUILayout.LabelField("EFFECTS CUSTOMIZATION", EditorStyles.boldLabel);
                        GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(8) });
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("damagedSFX"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("healSFX"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("pitchVariationSFX"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("damageCamShakeAmount"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("flashesOnDamage"));
                        if (myScript.flashesOnDamage)
                        {
                            EditorGUI.indentLevel++;
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("sprites"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("flashDuration"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("flashSpeed"));
                            EditorGUI.indentLevel--;
                        }
                        break;
                    case "UI":
                        EditorGUILayout.LabelField("USER INTERFACE", EditorStyles.boldLabel);
                        GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(8) });
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("healthSlider"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("shieldSlider"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("playerStateUI"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("damageColor"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("healColor"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("fadeSpeed"));
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