using UnityEngine;

namespace unity.Assets._Scripts.FPS.Movement.Types
{
    public class FrameState
    {
        public bool Grounded { get; set; }
        public bool Jumping  { get; set; }
        public bool Sprinting  { get; set; }
        public bool Falling  { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 InputVector { get; set; }
    }
}