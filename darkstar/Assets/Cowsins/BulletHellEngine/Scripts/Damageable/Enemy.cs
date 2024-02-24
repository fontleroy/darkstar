using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using cowsins.BulletHell;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace cowsins.BulletHell
{
    /// <summary>
    /// Handles basic Enemy behaviour (related to damaging ONLY). Inherits from Damageable.cs.
    /// </summary>
    public class Enemy : Damageable
    {
        [SerializeField, Min(0), Tooltip("Minimum amount to add to the global score on dead." +
            " Notice that this will only work if ScoreManage exists in the scene (The player prefab brings it already).")]
        private int deathScoreMin;

        [SerializeField, Min(0), Tooltip("Maximum amount to add to the global score on dead. " +
            "Notice that this will only work if ScoreManage exists in the scene (The player prefab brings it already).")]
        private int deathScoreMax;

        [SerializeField, Tooltip("Assign a damage pop up prefab here. " +
            "Bullet Hell Engine brings some examples that you can use.")]
        protected Transform damagePopUp;

        private Transform popUp;

        [SerializeField, Tooltip("Array that stores audio clips for taking damage.")] private AudioClip[] hitSFX;

        [SerializeField, Tooltip("Visual effect played on death.")] private GameObject deathVFX;

        [SerializeField, Tooltip("Damage pop up lifetime.")] private float popUpDestroyTime;

        [SerializeField, Tooltip("Variation on the x axis ( locally ) each time the pop up is instantiated.")] protected float xVariation;

        [Tooltip("If true, blink / flash effect will be applied on take damage.")] public bool flashesOnDamage;

        [SerializeField, Tooltip("How long it takes for the effect to finish. Notice that the material goes back to its original state when finished.")] protected float flashDuration;

        [SerializeField, Tooltip("How fast the blink effect is performed.")] protected float flashSpeed;

        private float blinkValue;

        [Tooltip(" if true, the enemy will display states through the user interface.")]
        public bool displayUI;

        [SerializeField, Tooltip("Image that displays the amount of health. It must have a sprite assigned and set to Filled.")] protected Image healthSlider, shieldSlider;

        [SerializeField, Tooltip("Array that contains every sprite on which the effect will be applied. These sprites must have the Blink material assigned.")] protected SpriteRenderer[] sprites;

        public delegate void OnDeath();

        public OnDeath onDeath;

        public override void Update()
        {
            // Handle UI
            ManageUI();

            // Perform basic Update functions from Damageable.cs
            base.Update();
        }
        public override void TakeDamage(float damage)
        {
            // Perform Flash damage effect
            if (flashesOnDamage)
                FlashDamage();
            // Perform damage pop up
            DamagePopUp(damage);

            // Play SFX
            if (hitSFX.Length != 0) SoundManager.Instance.PlaySound(hitSFX[Random.Range(0, hitSFX.Length)], 0, .05f, 0);

            // take Damage
            base.TakeDamage(damage);
        }
        public override void Die()
        {
            // Invoke a custom method if necessary
            onDeath?.Invoke();

            // Add Score if the manager is not null
            if (ScoreManager.Instance != null) ScoreManager.Instance.AddScore(Random.Range(deathScoreMin, deathScoreMax));

            // Instantiate visual effects
            if (deathVFX != null) Instantiate(deathVFX, transform.position, Quaternion.identity);

            // Perform base function
            base.Die();
        }
        private void DamagePopUp(float damage)
        {
            // If there is no damage pop up object, do not try to instantiate it.
            if (damagePopUp == null) return;

            if (popUp == null) // If thre is not a pop up in the scene ( for each enemy ), continue: 
            {
                // Create the pop up
                popUp = Instantiate(damagePopUp, transform.position, Quaternion.identity);

                // Perform transform modifications ( x variation )
                popUp.position += transform.right * Random.Range(-xVariation, xVariation);
            }

            // If there is a pop up already, assign the current damage + the damage it just took.
            // Parse transforms text into a number, so we can get to sum "text" with the new damage value.
            popUp.GetComponentInChildren<TextMeshProUGUI>().text = (float.Parse(popUp.GetComponentInChildren<TextMeshProUGUI>().text) + damage).ToString("F0");

            //Destroy the pop up after its lifetime.
            Destroy(popUp.gameObject, popUpDestroyTime);
        }
        public void FlashDamage()
        {
            // Stop any possible effect running and reset it.
            StopCoroutine(IFlashDamage());
            blinkValue = 0;

            // Start Effect from zero.
            StartCoroutine(IFlashDamage());
        }
        public IEnumerator IFlashDamage()
        {
            var elapsedTime = 0f;
            // While the elapsed time is less than the actual duration of the effect, keep going.
            while (elapsedTime <= flashDuration)
            {
                // FOr each assigned sprites, change the flash amount.
                for (int i = 0; i < sprites.Length; i++)
                {
                    var material = sprites[i].GetComponent<SpriteRenderer>().material;
                    material.SetFloat("_FlashAmount", blinkValue);
                    blinkValue = Mathf.PingPong(elapsedTime * flashSpeed, 1f);
                    elapsedTime += Time.deltaTime;
                }
                yield return null;
            }
            // After the effect is done, set the flash amounts back to default.
            for (int i = 0; i < sprites.Length; i++)
            {
                var material = sprites[i].GetComponent<SpriteRenderer>().material;
                material.SetFloat("_FlashAmount", 0);
            }
        }
        private void ManageUI()
        {
            // If we do not display UI, do not try to access it.
            if (!displayUI) return;

            // Handles slider values. 
            if (healthSlider != null) healthSlider.fillAmount = Mathf.Lerp(healthSlider.fillAmount, health / maxHealth, Time.deltaTime);
            // Handles slider values. 
            if (shieldSlider != null) shieldSlider.fillAmount = Mathf.Lerp(shieldSlider.fillAmount, shield / maxShield, Time.deltaTime);
        }

    }
#if UNITY_EDITOR

    [CustomEditor(typeof(Enemy))]
    public class EnemyEditor : Editor
    {
        private string[] tabs = { "Basic", "Effects", "UI", "Events" };
        private int currentTab = 0;

        override public void OnInspectorGUI()
        {
            serializedObject.Update();
            var myScript = target as Enemy;

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
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("health"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("shield"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("maxHealth"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("maxShield"));
                        break;
                    case "Effects":
                        EditorGUILayout.LabelField("EFFECTS CUSTOMIIZATION", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField("SCORE", EditorStyles.toolbarButton);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("deathScoreMin"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("deathScoreMax"));
                        EditorGUILayout.LabelField("BLINK EFFECT", EditorStyles.toolbarButton);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("flashesOnDamage"));
                        if (myScript.flashesOnDamage)
                        {
                            EditorGUI.indentLevel++;
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("flashSpeed"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("flashDuration"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("sprites"));
                            EditorGUI.indentLevel--;
                        }
                        EditorGUILayout.LabelField("OTHERS", EditorStyles.toolbarButton);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("deathVFX"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("hitSFX"));
                        break;
                    case "UI":
                        EditorGUILayout.LabelField("BASIC UI", EditorStyles.toolbarButton);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("displayUI"));
                        if (myScript.displayUI)
                        {
                            EditorGUI.indentLevel++;
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("healthSlider"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("shieldSlider"));
                            EditorGUI.indentLevel--;
                        }
                        EditorGUILayout.LabelField("DAMAGE POP UP", EditorStyles.toolbarButton);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("damagePopUp"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("xVariation"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("popUpDestroyTime"));
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