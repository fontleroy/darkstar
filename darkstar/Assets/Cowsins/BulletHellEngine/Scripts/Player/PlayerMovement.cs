using UnityEngine;
using System;
using UnityEngine.Events;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace cowsins.BulletHell
{
    /// <summary>
    /// Manages the player movement, rotation, and advanced movement actions such as dashing.
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public class PlayerMovement : MonoBehaviour
    {
        [System.Serializable]
        public class Events
        {
            public UnityEvent OnMoving, OnDash, OnDashing, OnStopDashing, OnRegenDash;
        }
        public enum MovementStyle
        {
            Default, ClickToMove
        }

        [SerializeField] private Transform graphics;

        public Transform Graphics
        {
            get { return graphics; }
        }

        [SerializeField, Tooltip("Capacity of gaining velocity.")] private float acceleration;

        [SerializeField, Tooltip("movement speed for the click to move movement style.")] private float movementSpeed;

        [Tooltip("Select the style of the movement of your character")] public MovementStyle movementStyle;

        [SerializeField] private GameObject clickIndicator;

        [SerializeField, Tooltip("Set to true if the player should be able to move along the horizontal axis.")] private bool allowXAxisMovement;

        [SerializeField, Tooltip("Set to true if the player should be able to move along the vertical axis.")] private bool allowYAxisMovement;

        [SerializeField, Min(0), Tooltip("For gamepads, rotation speed.")] public float rotationSpeed;

        [Tooltip("Set to true if the player can aim towards the mouse (or joystick if using a gamepad).")] public bool rotates;

        [SerializeField, Tooltip("Assign the player camera.")] private Camera cam;

        [Tooltip("Set to true if the player can dash.")] public bool canDash;

        [SerializeField, Tooltip("Maximum amount of dashes you can perform / store. These will be regenerated after being used.")] private int amountOfDashes;

        [SerializeField, Tooltip("Regeneration time after dashing.")] private float dashCooldown;

        [Tooltip("If true, the player will be able to shoot while dashing.")] public bool canShootWhileDashing;

        [Tooltip("If true, you will be able to receive damage while dashing.")] public bool receiveDamageWhileDashing;

        [SerializeField, Tooltip("Player speed while dashing.")] private float dashSpeed;

        [SerializeField, Tooltip("Amount of time the player is in the dash state.")] private float dashTime;

        [SerializeField, Tooltip("Contains as a layout all the dash UI objects.")] private Transform dashUIContainer;

        [SerializeField, Tooltip("Used to display the number of dashes left.")] private GameObject dashUIObject;

        [SerializeField] private Events events;

        private int dashesRemaining;

        private Rigidbody2D rb;

        private PlayerModifiers playerModifiers;

        // Dash time remaining
        public float dashTimer { get; private set; }

        // Returns true if dashing
        public bool dashing { get; private set; } = false;

        private Vector2 dashDirection;

        private Action movement;

        private void Awake()
        {
            Cursor.visible = false;
            dashesRemaining = amountOfDashes;

            movement = (movementStyle == MovementStyle.Default) ? Movement : ClickToMoveMovement;

            rb = GetComponent<Rigidbody2D>();
            playerModifiers = GetComponent<PlayerModifiers>();
            // Spawn the input manager
            // The input manager is basic for the gameplay
            // Without the input manager, no inputs would be read, so you would not be able to perform any action.
            Instantiate(Resources.Load("InputManager"));

            // Instantiates the dash ui objects
            // Only do this if we can dash
            if (!canDash) return;
            GenerateDashUI();
        }

        private void Update()
        {
            if (!PlayerHealth.controllable) return; // If the character is dead, or cannot be controlled, do not perform any movement

            // If can dash, handle the logic
            if (canDash)
            {
                HandleDash();
                Dash();
            }

            // Movement related logic
            movement.Invoke();
            Rotation();
        }

        private void Movement()
        {
            // Do not move while dashing or if inputs are null
            if (dashTimer > 0 || InputManager.playerInputs.HorizontalMovement == 0 && InputManager.playerInputs.VerticalMovement == 0 || PauseMenu.pauseMenu.isPaused) return;

            // Invoke custom method
            events.OnMoving?.Invoke();

            // Get multipliers for the desired axis
            float xMovement = allowXAxisMovement ? InputManager.playerInputs.HorizontalMovement : 0;
            float yMovement = allowYAxisMovement ? InputManager.playerInputs.VerticalMovement : 0;

            // Apply movement
            rb.AddForce(new Vector2(xMovement, yMovement) * Time.deltaTime * acceleration * 100 * playerModifiers.movementSpeedModifier);
        }
        private Vector3 movePosition;
        private void ClickToMoveMovement()
        {
            rb.position = Vector3.Lerp(rb.position, movePosition, Time.deltaTime * movementSpeed * playerModifiers.movementSpeedModifier);
            if (InputManager.playerInputs.RightClick)
            {
                Vector3 mousePosition = Mouse.current.position.ReadValue();
                mousePosition.z = 10f; // Set a distance from the camera

                Vector3 targetPosition = cam.ScreenToWorldPoint(mousePosition);
                targetPosition.z = 0f;

                movePosition = targetPosition;
                if (InputManager.playerInputs.RightClickPress) Instantiate(clickIndicator, targetPosition, Quaternion.identity);
            }
        }

        private void Rotation()
        {
            // Do not move while dashing or paused or if the player cant rotate
            if (dashTimer > 0 || !rotates || PauseMenu.pauseMenu.isPaused) return;

            if (InputManager.playerInputs.MouseX == 0 && InputManager.playerInputs.MouseY == 0)
            {
                // Gamepad rotation
                if (InputManager.playerInputs.RHorizontalMovement != 0 || InputManager.playerInputs.RVerticalMovement != 0)
                    graphics.right = Vector3.Lerp(graphics.right,
                        new Vector2(InputManager.playerInputs.RHorizontalMovement, InputManager.playerInputs.RVerticalMovement)
                        , Time.deltaTime * rotationSpeed);
            }
            else
            {
                // Mouse rotation
                var dir = (Vector3)InputManager.playerInputs.MousePos - cam.WorldToScreenPoint(transform.position);
                var ang = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                graphics.rotation = Quaternion.AngleAxis(ang, Vector3.forward);
            }
        }
        private void HandleDash()
        {
            // If we are ready to dash and we perform the action, handle dash initial settings
            if (dashTimer <= 0 && InputManager.playerInputs.Dashing && dashesRemaining > 0)
            {
                // Custom method
                events.OnDash?.Invoke();

                // Use a dash
                dashesRemaining--;

                // Set dashing state
                dashing = true;

                // Regenerate the dash
                Invoke(nameof(DashCooldown), dashCooldown);

                // Grab desired dash direction depending on the player
                dashDirection = (InputManager.playerInputs.HorizontalMovement == 0 && InputManager.playerInputs.VerticalMovement == 0) ? transform.right : new Vector2(InputManager.playerInputs.HorizontalMovement, InputManager.playerInputs.VerticalMovement);

                // Update Dash UI
                Destroy(dashUIContainer.GetChild(0).gameObject);

                dashTimer = dashTime;
            }
        }

        private void Dash()
        {
            // Stop dashing
            if (dashTimer <= 0)
            {
                // Custom method
                events.OnStopDashing?.Invoke();

                dashing = false;

                return;
            }

            // Dash
            // Custom method
            events.OnDashing?.Invoke();

            // Keep the dash timer counting
            dashTimer -= Time.deltaTime;

            // Move the player in the desired direction at the desired speed
            GetComponent<Rigidbody2D>().AddForce(dashDirection * dashSpeed * Time.deltaTime * 100);
        }
        // Handles dash regeneration
        private void DashCooldown()
        {
            dashesRemaining++;
            events.OnRegenDash?.Invoke();

            // Update dash UI
            Instantiate(dashUIObject, dashUIContainer);
        }

        private void GenerateDashUI()
        {
            for (int i = 0; i < amountOfDashes; i++) Instantiate(dashUIObject, dashUIContainer);
        }
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(PlayerMovement))]
    public class PlayerMovement2DEditor : Editor
    {
        private string[] tabs = { "References", "Movement", "Advanced", "Events" };
        private int currentTab = 0;

        override public void OnInspectorGUI()
        {
            serializedObject.Update();
            var myScript = target as PlayerMovement;

            Texture2D myTexture = Resources.Load<Texture2D>("CustomEditor/PlayerMovement_CustomEditor") as Texture2D;
            GUILayout.Label(myTexture);

            EditorGUILayout.BeginVertical();
            currentTab = GUILayout.Toolbar(currentTab, tabs);
            EditorGUILayout.Space(5f);
            EditorGUILayout.EndVertical();

            if (currentTab >= 0 || currentTab < tabs.Length)
            {
                switch (tabs[currentTab])
                {
                    case "References":
                        EditorGUILayout.LabelField("REFERENCES", EditorStyles.boldLabel);
                        GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(8) });
                        GUILayout.Space(5);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("graphics"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("cam"));
                        if (myScript.canDash)
                        {
                            EditorGUI.indentLevel++;
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("dashUIContainer"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("dashUIObject"));
                            EditorGUI.indentLevel--;
                        }
                        break;
                    case "Movement":
                        EditorGUILayout.LabelField("MOVEMENT", EditorStyles.boldLabel);
                        GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(8) });
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("movementStyle"));
                        if (myScript.movementStyle == PlayerMovement.MovementStyle.Default)
                        {
                            EditorGUI.indentLevel++;
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("allowXAxisMovement"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("allowYAxisMovement"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("acceleration"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("rotates"));
                            if (myScript.rotates)
                            {
                                EditorGUI.indentLevel++;
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("rotationSpeed"));
                                EditorGUI.indentLevel--;
                            }
                            EditorGUI.indentLevel--;
                        }
                        else
                        {
                            EditorGUI.indentLevel++;
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("clickIndicator"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("movementSpeed"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("rotates"));
                            if (myScript.rotates)
                            {
                                EditorGUI.indentLevel++;
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("rotationSpeed"));
                                EditorGUI.indentLevel--;
                            }
                            EditorGUI.indentLevel--;
                        }
                        break;
                    case "Advanced":
                        EditorGUILayout.LabelField("ADVANCED MOVEMENT SETTINGS", EditorStyles.boldLabel);
                        GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(8) });
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("canDash"));
                        if (myScript.canDash)
                        {
                            EditorGUI.indentLevel++;
                            EditorGUILayout.LabelField("New settings under the `References` tab.", EditorStyles.helpBox);
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("amountOfDashes"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("dashCooldown"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("dashSpeed"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("dashTime"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("canShootWhileDashing"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("receiveDamageWhileDashing"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("dashUIContainer"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("dashUIObject"));
                            EditorGUI.indentLevel--;
                        }
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