using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public static class GetTerrainTexture 
{
    public static TextureToAlphaMapValue[] GetTextureAtPosition(Vector3 position)
    {
        Vector2 pos = ConvertPosition(position);

        Terrain terrain = Terrain.activeTerrain;
        TerrainLayer[] layers = terrain.terrainData.terrainLayers;

        float[,,] aMap = terrain.terrainData.GetAlphamaps((int)pos.x, (int)pos.y, 1, 1);
        TextureToAlphaMapValue[] textureValues = new TextureToAlphaMapValue[layers.Length];

        for (var i = 0; i < layers.Length; i++) {
            textureValues[i] = new TextureToAlphaMapValue{ alpha = aMap[0,0,i], texture = layers[i].diffuseTexture };
        }

        return textureValues;
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

public struct TextureToAlphaMapValue {
    public Texture texture;

    public float alpha;
}