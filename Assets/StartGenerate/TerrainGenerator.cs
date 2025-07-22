using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public int width = 10;       // 地形の幅
    public int height = 10;      // 地形の高さ

    public int Startwidth = 10;       // 始める位置
    public int Startheight = 0;   
    public float scale = 1.0f;   // ノイズのスケール
    public GameObject terrainPrefab; // 地形プレハブ

    //public GameObject switchPrefab; // 地形プレハブ

    void Start()
    {
        GenerateTerrain();
        //Instantiate(switchPrefab, new Vector3(5.0f, 4.5f, 5.0f), Quaternion.identity);
    }

    void GenerateTerrain()
    {
        // 既存の地形を削除（必要に応じて）
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        for (int x = Startwidth; x < Startwidth+width; x++)
        {
            for (int z = Startheight; z < Startheight+height; z++)
            {
                float sampleX = (float)x / width * scale;
                float sampleZ = (float)z / height * scale;
                float heightValue = Mathf.PerlinNoise(sampleX, sampleZ);

                // 地形を生成
                Vector3 position = new Vector3(x, heightValue * 5, z); // 高さを調整
                Instantiate(terrainPrefab, position, Quaternion.identity, transform);
            }
        }
    }
}


