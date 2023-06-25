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
        
        
        // SPRINTING
        public delegate void StartSprint();
        public StartSprint OnStartSprint;
        public void FireStartSprintEvent()
        {
            OnStartSprint?.Invoke();
        }
        
        
        public delegate void EndSprint();
        public EndSprint OnEndSprint;
        public void FireEndSprintEvent()
        {
            OnEndSprint?.Invoke();
        }
       
        
        // ADS
        public delegate void ADSToggle(bool _isAds);
        public ADSToggle OnADSToggle;
        public void FireADSToggleEvent(bool _isAds)
        {
            OnADSToggle?.Invoke(_isAds);
        }
    }
}