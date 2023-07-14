using System;
using System.Collections;
using System.Collections.Generic;
using _Systems.Audio;
using UnityEngine;
using UnityEngine.ProBuilder;

public static class TextureUtility
{
    public static RaycastHitTextureResult GetTextureFromRaycastHit(RaycastHit hit)
    {
        MeshRenderer hitRenderer = hit.transform.GetComponent<MeshRenderer>();

        ProBuilderMesh pbMesh = hit.collider.GetComponent<ProBuilderMesh>();
        if (pbMesh)
        {
            // Probuilder Collisions
            return new RaycastHitTextureResult(FindSubmeshTexture(pbMesh.GetComponent<MeshCollider>(), hit));
        }
        else if (hitRenderer != null) 
        {
            // Standard Mesh Collisions
            return new RaycastHitTextureResult(hitRenderer.sharedMaterial.mainTexture);
        } 
        else
        {
            // Terrain Collisions
            TerrainCollider terrainCollider = hit.collider.GetComponent<TerrainCollider>();
            return new RaycastHitTextureResult(GetTextureAtPosition(hit.point));
        }
    }

	/** Used to detect submesh texture for probuilder. Should probably go to a Utility class. **/
	public static Texture FindSubmeshTexture(MeshCollider collider, RaycastHit hit)
	{
		Mesh mesh = collider.sharedMesh;

		// There are 3 indices stored per triangle
		int limit = hit.triangleIndex * 3;
		int submesh;
		for (submesh = 0; submesh < mesh.subMeshCount; submesh++)
		{
			int numIndices = mesh.GetTriangles(submesh).Length;
			if (numIndices > limit)
				break;

			limit -= numIndices;
		}

		Texture myTexture = hit.collider.GetComponent<MeshRenderer>().sharedMaterials[submesh].mainTexture;

		return myTexture;
    }

    public static TextureToAlphaMapResult GetTextureAtPosition(Vector3 position)
    {
        Vector2 pos = ConvertPosition(position);

        Terrain terrain = Terrain.activeTerrain;
        TerrainLayer[] layers = terrain.terrainData.terrainLayers;

        float[,,] aMap = terrain.terrainData.GetAlphamaps((int)pos.x, (int)pos.y, 1, 1);
        TextureAndAlpha[] textureValues = new TextureAndAlpha[layers.Length];
        TextureAndAlpha mainTexture = textureValues[0];

        for (var i = 0; i < layers.Length; i++) 
        {
            textureValues[i] = new TextureAndAlpha{ alpha = aMap[0,0,i], texture = layers[i].diffuseTexture };
            if (textureValues[i].alpha > mainTexture.alpha)
            {
                mainTexture = textureValues[i];
            }
        }

        return new TextureToAlphaMapResult(textureValues, mainTexture);
    }
    static Vector2 ConvertPosition(Vector3 playerPosition)
    {
        Terrain terrain = Terrain.activeTerrain;
        Vector3 terrainPosition = playerPosition - terrain.transform.position;
        Vector3 mapPosition = new Vector3
        (terrainPosition.x / terrain.terrainData.size.x, 0,
        terrainPosition.z / terrain.terrainData.size.z);
        float xCoord = mapPosition.x * terrain.terrainData.alphamapWidth;
        float zCoord = mapPosition.z * terrain.terrainData.alphamapHeight;

        return new Vector2(xCoord, zCoord);
    }
}

public class RaycastHitTextureResult 
{
    public Texture GeometryCollision { get; }
    public TextureToAlphaMapResult TerrainCollision { get; }

    public RaycastHitTextureResult(Texture standardResultTexture)
    {
        GeometryCollision = standardResultTexture;
        TerrainCollision = null;
    }

    public RaycastHitTextureResult(TextureToAlphaMapResult terrainTextureResults)
    {
        GeometryCollision = null;
        TerrainCollision = terrainTextureResults;
    }
}

public struct TextureAndAlpha 
{
    public Texture texture;

    public float alpha;
}

public class TextureToAlphaMapResult
{
    public TextureAndAlpha[] Values;

    public TextureAndAlpha MainTexture;

    public TextureToAlphaMapResult(TextureAndAlpha[] values, TextureAndAlpha mainTexture)
    {
        Values = values;
        MainTexture = mainTexture;
    }
}