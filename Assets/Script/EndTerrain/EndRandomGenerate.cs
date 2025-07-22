using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndrandomGenerate : MonoBehaviour
{
    public int width = 10;       // 地形の幅
    public int height = 10;      // 地形の高さ
    public int Startwidth = 10;  // 始める位置
    public int Startheight = 0;       
    public float scale = 1.0f;   // ノイズのスケール
    public GameObject terrainPrefab; // 地形プレハブ

    private bool isGenerated = false; // 地形がすでに生成されたかどうか

    // 特定のエリアに入ったときに呼ばれる
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isGenerated)
        {
            GenerateTerrain();
            //isGenerated = true; // 一度だけ生成する
        }
    }

    void GenerateTerrain()
    {
        // 既存の地形を削除（必要に応じて）
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        for (int x = Startwidth; x < Startwidth + width; x++)
        {
            for (int z = Startheight; z < Startheight + height; z++)
            {
                if(z == Random.Range(Startheight, Startheight+height+1))
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
}
