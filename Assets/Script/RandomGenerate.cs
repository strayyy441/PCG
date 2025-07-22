using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGenerate : MonoBehaviour
{
    public int width = 10;       // 地形の幅
    public int height = 10;      // 地形の高さ
    public int Startwidth = 10;  // 始める位置
    public int Startheight = 0;       
    public float scale = 1.0f;   // ノイズのスケール
    public GameObject terrainPrefab; // 地形プレハブ
    public GameObject triggerAreaPrefab; // トリガーエリアのプレハブ

    private bool isGenerated = false; // 地形がすでに生成されたかどうか

    // プレイヤーが特定のエリアに入ったときにのみ呼ばれる
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isGenerated)
        {
            GenerateTerrain();
            isGenerated = true; // 一度だけ生成する
        }
    }

    public void GenerateTerrain()
    {
        // 既存の地形を削除（必要に応じて）
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // 指定した範囲内で地形を生成
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

        // 地形生成後にトリガーエリアを生成
        CreateTriggerArea();
    }

    // トリガーエリアを生成するメソッド
    void CreateTriggerArea()
    {
        // 新しいトリガーエリアの位置を設定
        Vector3 triggerPosition = new Vector3(Startwidth + width / 2.0f, 3, Startheight + height / 2.0f);
        GameObject triggerArea = Instantiate(triggerAreaPrefab, triggerPosition, Quaternion.identity);

        // トリガーエリアのサイズや位置を調整
        BoxCollider triggerCollider = triggerArea.GetComponent<BoxCollider>();
        triggerCollider.isTrigger = true;
        //triggerCollider.size = new Vector3(width, 1, height); // 必要に応じてサイズを調整

        triggerArea.transform.parent = transform; // 生成したエリアを親オブジェクトに設定
    }

    // 手動でリセット可能なメソッド
    public void ResetGeneration()
    {
        isGenerated = false;
    }
}

// トリガーエリアに入ったときの処理を管理するスクリプト
/*public class TriggerArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // TerrainGenerator2から新しい地形を生成するメソッドを呼び出し
            TerrainGenerator2 terrainGenerator = FindObjectOfType<TerrainGenerator2>();
            if (terrainGenerator != null)
            {
                terrainGenerator.GenerateTerrain();
            }
        }
    }
}*/
