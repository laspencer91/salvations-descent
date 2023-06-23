using UnityEngine;

namespace _Systems.Helpers
{
    public static class GameObjectHelper
    {
        public static bool IsPlayer(this GameObject gameObject)
        {
            return gameObject.CompareTag("Player");
        }
    }
}