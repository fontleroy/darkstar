using UnityEngine;

namespace cowsins
{
    public struct PlayerInputs
    {
        public float HorizontalMovement;
        public float VerticalMovement;
        public float RHorizontalMovement;
        public float RVerticalMovement;
        public float MouseX;
        public float MouseY;
        public bool RightClick;
        public bool RightClickPress;

        public bool Dashing;
        public bool Attacking;

        public bool PauseToggle;

        public Vector2 MousePos;
    }
}

