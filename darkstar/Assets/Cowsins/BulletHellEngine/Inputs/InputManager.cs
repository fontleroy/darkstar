using UnityEngine;
using cowsins;
using UnityEngine.UI;

#if ENABLE_EDITOR
using UnityEditor;
#endif

namespace cowsins.BulletHell
{

    public class InputManager : MonoBehaviour
    {
        public static InputManager inputManager;

        public static PlayerActions inputActions;

        public static PlayerInputs playerInputs { get; private set; }

        private delegate PlayerInputs InputsMethod();

        private InputsMethod inputsMethod;

        private void Awake()
        {
            if (inputManager == null)
            {
                DontDestroyOnLoad(this);
                inputManager = this;
            }
            else Destroy(this.gameObject);


            if (inputActions == null) inputActions = new PlayerActions();

            inputActions.Enable();

        }
        private void OnDisable() => inputActions.Disable();

        private void Update() => playerInputs = ReceiveInputs();

        private PlayerInputs ReceiveInputs()
        {
            return new PlayerInputs
            {
                HorizontalMovement = inputActions.GameControls.Movement.ReadValue<Vector2>().x,
                VerticalMovement = inputActions.GameControls.Movement.ReadValue<Vector2>().y,
                RHorizontalMovement = inputActions.GameControls.Rotation.ReadValue<Vector2>().x,
                RVerticalMovement = inputActions.GameControls.Rotation.ReadValue<Vector2>().y,
                MouseX = inputActions.GameControls.Mouse.ReadValue<Vector2>().x,
                MouseY = inputActions.GameControls.Mouse.ReadValue<Vector2>().y,
                RightClick = inputActions.GameControls.RightClick.ReadValue<float>() > 0,
                RightClickPress = inputActions.GameControls.RightClick.WasPressedThisFrame(),
                Dashing = inputActions.GameControls.Dash.WasPressedThisFrame(),
                Attacking = inputActions.GameControls.Attack.IsPressed(),
                PauseToggle = inputActions.GameControls.PauseToggle.WasPressedThisFrame(),
                MousePos = inputActions.GameControls.MousePos.ReadValue<Vector2>()
            };
        }
    }

}