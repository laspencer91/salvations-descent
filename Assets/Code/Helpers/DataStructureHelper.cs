using System.Collections.Generic;
using UnityEngine;

namespace _Systems.Helpers
{
    public static class DataStructureHelper
    {
        public static T GetRandomElement<T>(this List<T> list)
        {
            return list[Random.Range(0, list.Count - 1)];
        }
    }
}