using UnityEngine;

namespace FirstPersonMovement
{
    [System.Serializable]
    public class CrouchStance : Stance
    {
        public CrouchStance(FPSStanceHandler handler, float height, float speedMultiplier) : base(handler, height, speedMultiplier)
        {}

        public override void Crouch()
        {
            if (handler.WouldCollide(Vector3.up, handler.StandStance.height - height)) return;
			    handler.SetStance(handler.StandStance);
        }

        public override void Stand()
        {
            if (handler.WouldCollide(Vector3.up, handler.StandStance.height - height)) return;
			
            handler.SetStance(handler.StandStance);
        }
    }
}