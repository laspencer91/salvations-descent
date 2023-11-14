using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "ImpactMaterialPartSystemDefinition", menuName = "Game/Create Impact Material Part System Definition")]
public class ImpactMaterialPartSystemDefinition : ScriptableObject
{
    [System.Serializable]
    public class ImpactMaterialPrefabPair
    {
        public ImpactMaterialType materialType;
        public GameObject prefab;
    }

    [Required]
    public GameObject DefaultEffect;

    public ImpactMaterialPrefabPair[] materialPrefabs;

    public GameObject GetPrefabForMaterialType(ImpactMaterialType materialType)
    {
        foreach (var pair in materialPrefabs)
        {
            if (pair.materialType == materialType)
            {
                return pair.prefab;
            }
        }

        Debug.LogWarning("Prefab not found for material type: " + materialType);
        return DefaultEffect;
    }

    public GameObject GetDefault()
    {
        return DefaultEffect;
    }
}