using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactMaterial : MonoBehaviour
{
    public ImpactMaterialType Type;
}

public enum ImpactMaterialType
{
    Wood,
    Dirt,
    Tree
}