using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator2 : MonoBehaviour
{

    //public GameData gameData; //選択肢
    public bool shuffle = false;
    public GameObject player; //プレイヤー
    PlayerController playercontroller;
    CharacterController characterController;
    HealthItemUIManager hIUimanager; //HealthUI
    PlayerHealth playerHealth; 
    public GameObject instantiateSword;
    public GameObject instantiateGun;




    //音関係
    //アドベンチャー
    [SerializeField] private AudioClip correctSound; // 正解時の音
    [SerializeField] private AudioClip inCorrectSound; // 正解時の音
    public AudioClip[] bgmClips; // BGM用のAudioClip配列
    
    [SerializeField] private AudioSource audioSource; // AudioSourceコンポーネント

    public int width = 30;       // 地形の幅
    public int height = 30;      // 地形の高さ
    public int Startwidth = 10;  // 始める位置
    public int Startheight = -10;       
    public float scale = 0.9f;   // ノイズのスケール

    public int GenerateCount = 0; //生成回数

    //public Transform QueueParentObject; // 地形削除管理用の親オブジェクト（空のGameObject）
    public int maxBlocks = 3; // キューの最大ブロック数

    private Queue<GameObject> blockQueue = new Queue<GameObject>(); // 地形削除管理用のキュー

    public GameObject enemyManagerPrefab; //エネミーマネジャープレハブ

    public bool IsEnemyManagerCreated = false;
    public GameObject TimeManagerPrefab; //タイムマネジャープレハブ
    public GameObject terrainPrefab; // 地形プレハブ
    public GameObject wallPrefab; // 壁のプレハブ
    public GameObject frontwallPrefab; // 壁のプレハブ
    public GameObject frontwall; //生成後格納用
    public GameObject triggerAreaPrefab; // トリガーエリアのプレハブ

    public GameObject smokeEffectPrefab; // 煙エフェクトのプレハブ

    public GameObject swordPrefab; // 剣のプレハブ
    public GameObject gunPrefab; // 銃（ピストル）のプレハブ

    //アクションRPG敵プレハブ
    public GameObject enemyPrefab;
    //アクションRPGルール紹介用
    GameObject actionIntroductionCanvas; 
	TextFadeController ac_textfadecontroller;

    //FPS敵プレハブ
    public GameObject fpsEnemyPrefab;
    //FPSルール紹介用
    GameObject fpsIntroductionCanvas; 
	TextFadeController fps_textfadecontroller;

    //アドベンチャープレハブ
    public List<GameObject> itemPrefabs; //生成するアイテムのPrefabリスト
    private int itemIndex; //難易度ごとに生成するアイテムの量
    List<GameObject> spawnedItems; //生成されたアイテムのリスト
    int currentItemIndex; //現在回収すべきアイテムのインデックス
    public bool IsCreared = false; //アイテムが正しく回収された場合のフラグ
    bool IsFailed = false; //回収失敗時のフラグ
    //座標保存用
    public int ad_width;       // 地形の幅
    public int ad_height;      // 地形の高さ
    public int ad_Startwidth;  // 始める位置
    public int ad_Startheight;
    //正解不正解用
    GameObject correctCanvas; //正解オブジェクト
	TextFadeController correct_textfadecontroller;
    GameObject inCorrectCanvas; //不正解オブジェクト
	TextFadeController incorrect_textfadecontroller;
    //アドベンチャールール紹介用
    GameObject adventureIntroductionCanvas; 
	TextFadeController aI_textfadecontroller;

    public Vector3 playerInitialPosition;

    [Header("難易度を選択してください")]
    public LevelList level; //列挙型

    [Header("ジャンルを選択してください")]
    public GenreList genre; //列挙型

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

    public enum LevelList
    {
        easy = 1,
        normal = 2,
        hard = 3
    }

    public enum GenreList
    {
        actionRPG,
        FPS,
        Adventure
    }

    void Start()
    {
        /*if(gameData.selectedGenre.Equals("ActionRPG"))
        {
            genre = GenreList.actionRPG;
        }
        if(gameData.selectedGenre.Equals("FPS"))
        {
            genre = GenreList.FPS;
        }
        if(gameData.selectedGenre.Equals("Adventure"))
        {
            genre = GenreList.Adventure;
        }
        if(gameData.selectedDifficulty.Equals("easy"))
        {
            level = LevelList.easy;
        }
        if(gameData.selectedDifficulty.Equals("normal"))
        {
            level = LevelList.normal;
        }
        if(gameData.selectedDifficulty.Equals("hard"))
        {
            level = LevelList.hard;
        }*/
        player = GameObject.FindGameObjectWithTag("Player"); //プレイヤーオブジェクトを取得
        playercontroller = player.GetComponent<PlayerController>();
        characterController = player.GetComponent<CharacterController>();
        hIUimanager = FindObjectOfType<HealthItemUIManager>(); //HealthItemUIManagerオブジェクトを参照
        playerHealth = FindObjectOfType<PlayerHealth>(); //PlayerHealthオブジェクトを参照
        ReloadGeneratePosition();
        correctCanvas = GameObject.Find ("CorrectCanvas");
        correct_textfadecontroller = correctCanvas.GetComponent<TextFadeController>();
        inCorrectCanvas = GameObject.Find ("InCorrectCanvas");
        incorrect_textfadecontroller = inCorrectCanvas.GetComponent<TextFadeController>();

        actionIntroductionCanvas = GameObject.Find ("ActionIntroductionText");
        ac_textfadecontroller = actionIntroductionCanvas.GetComponent<TextFadeController>();
        fpsIntroductionCanvas = GameObject.Find ("FPSIntroductionText");
        fps_textfadecontroller = fpsIntroductionCanvas.GetComponent<TextFadeController>();
        adventureIntroductionCanvas = GameObject.Find ("AdventureIntroductionText");
        aI_textfadecontroller = adventureIntroductionCanvas.GetComponent<TextFadeController>();
    }

    void Update()
    {
        if(player.transform.position.y < -3)
        {
            ResetPlayerState();
        }
    }

    public void GenerateTerrain()
    {
        GameObject block = new GameObject("地形" + GenerateCount); //地形の親ブロックの生成

        // 既存の地形を削除（必要に応じて）
        /*foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }*/

        // 指定した範囲内で地形を生成
        for (int x = Startwidth; x < Startwidth + width; x++)
        {
            for (int z = Startheight; z < Startheight + height; z++)
            {
                float sampleX = (float)x / width * scale;
                float sampleZ = (float)z / height * scale;
                float heightValue = Mathf.PerlinNoise(sampleX, sampleZ);

                // 地形を生成
                Vector3 position = new Vector3(x, heightValue * 5, z); // 高さを調整
                Instantiate(terrainPrefab, position, Quaternion.identity, block.transform /*親指定*/);
            }
        }

        // 地形を生成
        Vector3 leftwallposition = new Vector3(((Startwidth + width)/2 + 5), 5f, (Startheight + height)); // 高さを調整
        Instantiate(wallPrefab, leftwallposition, Quaternion.identity, block.transform /*親指定*/); //左壁
        Vector3 rightwallposition = new Vector3(((Startwidth + width)/2 +5), 5f, Startheight); // 高さを調整
        Instantiate(wallPrefab, rightwallposition, Quaternion.identity, block.transform /*親指定*/); //左壁
        Vector3 frontwallposition = new Vector3((Startwidth + width), 5f, ((Startheight + height)/2)-5); // 高さを調整
        frontwall = Instantiate(frontwallPrefab, frontwallposition, Quaternion.Euler(0, 90f, 0), block.transform /*親指定*/); //左壁

        // 地形生成後にトリガーエリアを生成
        //CreateTriggerArea();
        CreateEnemy(); //敵を生成
        CreateEnemyManager(); //エネミーマネジャーを生成
        CreateTimeManager(); //タイムマネジャーを生成

        GenerateCount++;
        Debug.Log(GenerateCount + "回生成されました.");

        blockQueue.Enqueue(block);
        Debug.Log(blockQueue.Count + "回キューに追加されました.");

        if (blockQueue.Count > maxBlocks)
        {
            GameObject oldestBlock = blockQueue.Dequeue();
            Destroy(oldestBlock);
            Debug.Log((GenerateCount-2) + "番目の地形が削除されました.");
        }
    }

    public void CreateEnemy()
    {
        //一番最初の敵生成位置を設定
        Vector3 firstEnemyPosition = new Vector3((Startwidth + width / 2.0f)-1.0f, 5, (Startheight + height / 2.0f));
        Vector3 secondEnemyPosition = new Vector3((Startwidth + width / 2.0f)-1.0f, 5, (Startheight + height / 4.0f));
        Vector3 thirdEnemyPosition = new Vector3((Startwidth + width / 2.0f)-1.0f, 5, ((Startheight + height)*3.0f / 4.0f));
        Vector3 forthEnemyPosition = new Vector3((Startwidth + width / 2.0f)+1.0f, 5, (Startheight + height / 3.0f));
        Vector3 fifthEnemyPosition = new Vector3((Startwidth + width / 2.0f)+1.0f, 5, ((Startheight + height)*2.0f / 3.0f));

        //ジャンルアイテムの生成位置
        Vector3 itemPosition = new Vector3(Startwidth+3, 5, (Startheight + height / 2.0f));

        //HPリセット
        playerHealth.currentHealth = playerHealth.maxHealth;
        playerHealth.UpdatePlayerHealth();
        
        switch(genre)
        {
            case GenreList.actionRPG:
                //FPSルール紹介文
                ac_textfadecontroller.ShowMessage("剣で敵を全員倒そう！\n左クリックで攻撃, Qボタンで回復");
                playercontroller.EquipSword(instantiateSword);
                PlayBGM(0);
                switch(level)
                {
                    case LevelList.easy:

                    //回復アイテムの個数を設定
                    hIUimanager.itemCount = 5;
                    hIUimanager.UpdateUI();

                    //ジャンルアイテム生成
                    //Instantiate(swordPrefab, itemPosition, Quaternion.identity, transform);

                    //1体だけ生成, 何番目の敵かわかるように格納
                    GameObject enemy1_e = Instantiate(enemyPrefab, firstEnemyPosition, Quaternion.identity, transform);
                    Debug.Log("敵が生成されています");

                    //敵の早さ設定
                    enemy1_e.GetComponent<EnemyController>().moveSpeed = 2.0f;

                    Instantiate(smokeEffectPrefab, firstEnemyPosition, Quaternion.identity); // 敵の位置に煙エフェクトを追加
                    break;


                    case LevelList.normal:

                    //回復アイテムの個数を設定
                    hIUimanager.itemCount = 3;
                    hIUimanager.UpdateUI();

                    //ジャンルアイテム生成
                    //Instantiate(swordPrefab, itemPosition, Quaternion.identity, transform);

                    //1体だけ生成, 何番目の敵かわかるように格納
                    GameObject enemy1_n = Instantiate(enemyPrefab, firstEnemyPosition, Quaternion.identity, transform);
                    GameObject enemy2_n = Instantiate(enemyPrefab, secondEnemyPosition, Quaternion.identity, transform);
                    GameObject enemy3_n = Instantiate(enemyPrefab, thirdEnemyPosition, Quaternion.identity, transform);

                    //敵の早さ設定
                    enemy1_n.GetComponent<EnemyController>().moveSpeed = 3.5f;
                    enemy2_n.GetComponent<EnemyController>().moveSpeed = 3.5f;
                    enemy3_n.GetComponent<EnemyController>().moveSpeed = 3.5f;

                    // 各敵に煙エフェクトを追加
                    Instantiate(smokeEffectPrefab, firstEnemyPosition, Quaternion.identity);
                    Instantiate(smokeEffectPrefab, secondEnemyPosition, Quaternion.identity);
                    Instantiate(smokeEffectPrefab, thirdEnemyPosition, Quaternion.identity);
                    break;


                    case LevelList.hard:

                    //回復アイテムの個数を設定
                    hIUimanager.itemCount = 3;
                    hIUimanager.UpdateUI();

                    //ジャンルアイテム生成
                    //Instantiate(swordPrefab, itemPosition, Quaternion.identity, transform);

                    //1体だけ生成, 何番目の敵かわかるように格納
                    GameObject enemy1_h = Instantiate(enemyPrefab, firstEnemyPosition, Quaternion.identity, transform);
                    GameObject enemy2_h = Instantiate(enemyPrefab, secondEnemyPosition, Quaternion.identity, transform);
                    GameObject enemy3_h = Instantiate(enemyPrefab, thirdEnemyPosition, Quaternion.identity, transform);
                    GameObject enemy4_h = Instantiate(enemyPrefab, forthEnemyPosition, Quaternion.identity, transform);
                    GameObject enemy5_h = Instantiate(enemyPrefab, fifthEnemyPosition, Quaternion.identity, transform);

                    //敵の早さ設定
                    enemy1_h.GetComponent<EnemyController>().moveSpeed = 5.0f;
                    enemy2_h.GetComponent<EnemyController>().moveSpeed = 5.0f;
                    enemy3_h.GetComponent<EnemyController>().moveSpeed = 5.0f;
                    enemy4_h.GetComponent<EnemyController>().moveSpeed = 5.0f;
                    enemy5_h.GetComponent<EnemyController>().moveSpeed = 5.0f;

                    // 各敵に煙エフェクトを追加
                    Instantiate(smokeEffectPrefab, firstEnemyPosition, Quaternion.identity);
                    Instantiate(smokeEffectPrefab, secondEnemyPosition, Quaternion.identity);
                    Instantiate(smokeEffectPrefab, thirdEnemyPosition, Quaternion.identity);
                    Instantiate(smokeEffectPrefab, forthEnemyPosition, Quaternion.identity);
                    Instantiate(smokeEffectPrefab, fifthEnemyPosition, Quaternion.identity);
                    break;
                }
                break;
            case GenreList.FPS:

                //FPSルール紹介文
                fps_textfadecontroller.ShowMessage("銃で敵を全員倒そう！右クリックで狙いを定めて\n左クリックで射撃, Qで回復");
                playercontroller.EquipGun(instantiateGun);
                PlayBGM(1);

                switch(level)
                {
                    case LevelList.easy:

                    //回復アイテムの個数を設定
                    hIUimanager.itemCount = 5;
                    hIUimanager.UpdateUI();

                    //ジャンルアイテム生成
                    //Instantiate(gunPrefab, itemPosition, Quaternion.identity, transform);

                    //1体だけ生成, 何番目の敵かわかるように格納
                    GameObject enemy1_e = Instantiate(fpsEnemyPrefab, firstEnemyPosition, Quaternion.identity, transform);

                    Instantiate(smokeEffectPrefab, firstEnemyPosition, Quaternion.identity); // 敵の位置に煙エフェクトを追加

                    //敵の早さ設定
                    enemy1_e.GetComponent<FPSEnemyController>().moveSpeed = 2.0f;
                    break;


                    case LevelList.normal:

                    //回復アイテムの個数を設定
                    hIUimanager.itemCount = 3;
                    hIUimanager.UpdateUI();

                    //ジャンルアイテム生成
                    //Instantiate(gunPrefab, itemPosition, Quaternion.identity, transform);

                    //1体だけ生成, 何番目の敵かわかるように格納
                    GameObject enemy1_n = Instantiate(fpsEnemyPrefab, firstEnemyPosition, Quaternion.identity, transform);
                    GameObject enemy2_n = Instantiate(fpsEnemyPrefab, secondEnemyPosition, Quaternion.identity, transform);
                    GameObject enemy3_n = Instantiate(fpsEnemyPrefab, thirdEnemyPosition, Quaternion.identity, transform);

                    //敵の早さ設定
                    enemy1_n.GetComponent<FPSEnemyController>().moveSpeed = 3.5f;
                    enemy2_n.GetComponent<FPSEnemyController>().moveSpeed = 3.5f;
                    enemy3_n.GetComponent<FPSEnemyController>().moveSpeed = 3.5f;

                    // 各敵に煙エフェクトを追加
                    Instantiate(smokeEffectPrefab, firstEnemyPosition, Quaternion.identity);
                    Instantiate(smokeEffectPrefab, secondEnemyPosition, Quaternion.identity);
                    Instantiate(smokeEffectPrefab, thirdEnemyPosition, Quaternion.identity);
                    break;


                    case LevelList.hard:

                    //回復アイテムの個数を設定
                    hIUimanager.itemCount = 3;
                    hIUimanager.UpdateUI();

                    //ジャンルアイテム生成
                    //Instantiate(gunPrefab, itemPosition, Quaternion.identity, transform);

                    //1体だけ生成, 何番目の敵かわかるように格納
                    GameObject enemy1_h = Instantiate(fpsEnemyPrefab, firstEnemyPosition, Quaternion.identity, transform);
                    GameObject enemy2_h = Instantiate(fpsEnemyPrefab, secondEnemyPosition, Quaternion.identity, transform);
                    GameObject enemy3_h = Instantiate(fpsEnemyPrefab, thirdEnemyPosition, Quaternion.identity, transform);
                    GameObject enemy4_h = Instantiate(fpsEnemyPrefab, forthEnemyPosition, Quaternion.identity, transform);
                    GameObject enemy5_h = Instantiate(fpsEnemyPrefab, fifthEnemyPosition, Quaternion.identity, transform);

                    //敵の早さ設定
                    enemy1_h.GetComponent<FPSEnemyController>().moveSpeed = 5.0f;
                    enemy2_h.GetComponent<FPSEnemyController>().moveSpeed = 5.0f;
                    enemy3_h.GetComponent<FPSEnemyController>().moveSpeed = 5.0f;
                    enemy4_h.GetComponent<FPSEnemyController>().moveSpeed = 5.0f;
                    enemy5_h.GetComponent<FPSEnemyController>().moveSpeed = 5.0f;

                    // 各敵に煙エフェクトを追加
                    Instantiate(smokeEffectPrefab, firstEnemyPosition, Quaternion.identity);
                    Instantiate(smokeEffectPrefab, secondEnemyPosition, Quaternion.identity);
                    Instantiate(smokeEffectPrefab, thirdEnemyPosition, Quaternion.identity);
                    Instantiate(smokeEffectPrefab, forthEnemyPosition, Quaternion.identity);
                    Instantiate(smokeEffectPrefab, fifthEnemyPosition, Quaternion.identity);
                    break;
                }
                break;
            case GenreList.Adventure:
                
                //装備を破棄
                playercontroller.DeleteWeapon();

                //回復アイテムの個数を設定
                hIUimanager.itemCount = 0;
                hIUimanager.UpdateUI();

                //アドベンチャールール紹介文
                aI_textfadecontroller.ShowMessage("正しい順番でアイテムを回収しよう！\n失敗するとリセットされます");

                PlayBGM(2);

                switch(level)
                {
                    case LevelList.easy:
                    itemIndex = 3;
                    GenerateItems(itemIndex, shuffle);
                    shuffle = false;
                    break;


                    case LevelList.normal:
                    itemIndex = 5;
                    GenerateItems(itemIndex, shuffle);
                    shuffle = false;
                    break;


                    case LevelList.hard:
                    itemIndex = 7;
                    GenerateItems(itemIndex, shuffle);
                    shuffle = false;
                    break;
                }
                break;
        }
    }

    public void DeleteFrontWall()
    {
        Destroy(frontwall);
    }

    void CreateEnemyManager()
    {
        // エネミーマネジャーを生成
        Instantiate(enemyManagerPrefab, new Vector3(-1.0f, 0.0f, 0.0f), Quaternion.identity);
        IsEnemyManagerCreated = true;
    }

    void CreateTimeManager()
    {
        // タイムマネジャーを生成
        Instantiate(TimeManagerPrefab, new Vector3(-1.0f, 1.0f, 0.0f), Quaternion.identity);
    }

    public void GenerateItems(int itemAmount, bool shuffle)
    {
            spawnedItems = new List<GameObject>();
            /*foreach (GameObject prefab in itemPrefabs)
            {
                GameObject item = Instantiate(prefab, GetRandomPosition(), Quaternion.identity);
                spawnedItems.Add(item);
            }*/
            /*if(!shuffle)
            {*/
                for(int i = 0; i < itemAmount; i++)
                {
                    GameObject item = Instantiate(itemPrefabs[i], GetRandomPosition(), Quaternion.identity);
                    spawnedItems.Add(item);
                }
            /*}
            else if(shuffle)
            {
                for(int i = 0; i < itemAmount; i++)
                {
                    // ランダムにインデックスを選択
                    int randomIndex = Random.Range(0, itemPrefabs.Count);
                    GameObject item = Instantiate(itemPrefabs[randomIndex], GetRandomPosition(), Quaternion.identity);
                    spawnedItems.Add(item);
                }
            }*/
        
        currentItemIndex = 0;
        //IsCreared = false;
        //IsFailed = false;
    }

    //ランダムな位置を生成する関数
    private Vector3 GetRandomPosition()
    {
        //それぞれの座標をランダムに生成する
        float x = Random.Range(ad_Startwidth+5, (ad_Startwidth+ad_width-7)); //奥行
        float y = 4.5f;
        float z = Random.Range(ad_Startheight+7, (ad_Startheight+ad_height-7)); //横

        //Vector3型のPositionを返す
        return new Vector3(x,y,z);
    }

    public void ReloadGeneratePosition()
    {
        ad_width = width;       // 地形の幅
        ad_height = height;      // 地形の高さ
        ad_Startwidth = Startwidth;  // 始める位置
        ad_Startheight = Startheight;

        //失敗時のプレイヤーリスポジの更新
        playerInitialPosition = new Vector3(ad_Startwidth, 5, ((ad_Startheight+ad_height)/2));
    }

    public void CollectItem(GameObject item)
    {
        if (spawnedItems[currentItemIndex] == item)
        {
            //〇を出す
            correct_textfadecontroller.ShowMessage("〇");

            //正解音
            PlayOnceSound(correctSound);

            // 正しい順番で回収
            currentItemIndex++;
            Destroy(item); // アイテムを削除

            if (currentItemIndex >= spawnedItems.Count)
            {
                // 全て正しい順番で回収完了
                IsCreared = true;
            }
        }
        else
        {
            //×を出す
            incorrect_textfadecontroller.ShowMessage("×");

            //不正解音
            PlayOnceSound(inCorrectSound);

            // 順番間違い
            IsFailed = true;
            ResetItems();
        }
    }

    private void ResetItems()
    {
        foreach (GameObject item in spawnedItems)
        {
            if (item != null)
            {
                Destroy(item); // 現在のアイテムを削除
            }
        }

        // 元のアイテムを再生成
        GenerateItems(itemIndex, false);

        // 必要に応じてプレイヤーの状態もリセット
        ResetPlayerState();

        IsFailed = false;
    }

    // リストをランダムにシャッフルする関数
    public void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1); // ランダムなインデックスを取得
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public void ResetPlayerState()
    {
        // プレイヤーの位置や状態をリセットする処理を記述
        if (characterController != null)
        {
            characterController.enabled = false; // 一時的に無効化
            player.transform.position = playerInitialPosition;
            characterController.enabled = true;  // 再度有効化
        }
        else
        {
            player.transform.position = playerInitialPosition;
        }
    }



    // トリガーエリアを生成するメソッド
    public void CreateTriggerArea()
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

    //音を一度だけ鳴らす
    private void PlayOnceSound(AudioClip audio)
    {
        if (audio != null) // 音が設定されているか確認
        {
            audioSource.PlayOneShot(audio); // 音を再生
        }
    }

     // BGMを再生する関数
    public void PlayBGM(int index)
    {
        if (index < 0 || index >= bgmClips.Length)
        {
            Debug.LogError("Invalid BGM index: " + index);
            return;
        }

        if (audioSource.isPlaying && audioSource.clip == bgmClips[index])
        {
            Debug.Log("BGM is already playing: " + bgmClips[index].name);
            return;
        }

        audioSource.clip = bgmClips[index]; // 再生するBGMを設定
        audioSource.Play(); // 再生開始
    }

    // BGMを停止する関数
    public void StopBGM()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop(); // 再生中のBGMを停止
        }
    }
}

// トリガーエリアに入ったときの処理を管理するスクリプト
public class TriggerArea : MonoBehaviour
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
            //Destroy(this.gameObject);
        }
    }
}

