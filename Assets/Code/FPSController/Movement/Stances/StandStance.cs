using UnityEngine;

namespace FirstPersonMovement
{
    [System.Serializable]
    public class StandStance : Stance
    {
        public StandStance(FPSStanceHandler handler, float height, float speedMultiplier) : base(handler, height, speedMultiplier)
        {}

        public override void Crouch()
        {
            handler.SetStance(handler.CrouchStance);
        }

        public override void Stand()
        {
		    handler.SetStance(handler.StandStance);
        }
    }
}