namespace FirstPersonMovement
{
    [System.Serializable] 
    public abstract class Stance
    {
        public float height;

        public float speedMultiplier;

        public readonly FPSStanceHandler handler;

        public Stance(FPSStanceHandler handler, float height, float speedMultiplier)
        {
            this.handler = handler;
            this.height  = height;
            this.speedMultiplier = speedMultiplier;
        }

        public abstract void Stand();

        public abstract void Crouch();
    }
}