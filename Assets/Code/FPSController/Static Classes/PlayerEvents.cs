using System;
using UnityEngine;

namespace EventManagement
{
    public class PlayerEvents
    {
        // Grounding
        public delegate void Land(float fallDistance);
        public Land OnLand;
        public void FireLandEvent(float fallDistance)
        {
            if (fallDistance < 0.4) return;
            
            OnLand?.Invoke(fallDistance);
        }
        
        
        // Jumping
        public delegate void Jump();
        public Jump OnJump;
        public void FireJumpEvent()
        {
            OnJump?.Invoke();
        }
    }
}