using UnityEngine;

namespace unity.Assets._Scripts.FPS.Movement.Types
{
    public class FrameState
    {
        public bool Grounded { get; set; }
        public bool Jumping  { get; set; }
        public Vector3 Velocity  { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 InputVector { get; set; }
    }
}