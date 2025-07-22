using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{
    private float intervalTime = 5.0f; //ステージ生成までの時間

    GameObject generator1; //generator1そのものが入る変数
	TerrainGenerator2 script; //TerrainGeneraotr2Scriptが入る変数

    TimeManager timemanager; //タイムマネジャーオブジェクト
    public float crearTime = 0;
    GameObject IntervalText;
    TextFadeController crear_textfadecontroller;
    public GameObject[] enemyBox;

    GameObject EmotionsReceiver;
    EmotionsReceiver esscript;

    public int emotionsScore;

    // Start is called before the first frame update
    void Start()
    {
        generator1 = GameObject.Find ("Generator1");
		script = generator1.GetComponent<TerrainGenerator2>();
        timemanager = FindObjectOfType<TimeManager>();
        IntervalText = GameObject.Find ("IntervalText");
        crear_textfadecontroller = IntervalText.GetComponent<TextFadeController>();

        EmotionsReceiver = GameObject.Find ("EmotionsReceiver"); //emotionsreceiverObject
		esscript = EmotionsReceiver.GetComponent<EmotionsReceiver>(); //EmotionsReceiverスクリプト取得
    }

    // Update is called once per frame
    
    void Update()
    {
        enemyBox = GameObject.FindGameObjectsWithTag("Enemy");
        print("敵の数：" + enemyBox.Length);
        if(enemyBox.Length == 0 && script.genre != TerrainGenerator2.GenreList.Adventure)
        {
            script.StopBGM();
            script.DeleteFrontWall();
            crearTime = timemanager._timeElapsed;
            emotionsScore = esscript.CalculateScore(); //感情スコアを計算
            esscript.ResetScore(); //スコアをリセット
            esscript.HandleEmotion(emotionsScore); //難易度, ジャンル調整
            Debug.Log("全ての敵が倒されました");
            script.Startwidth = script.Startwidth + script.width;
            script.ReloadGeneratePosition();
            script.Invoke("GenerateTerrain", intervalTime); // ｎ秒後にHeal()を呼び出し
            crear_textfadecontroller.ShowMessage(intervalTime + "秒後に次のステージが生成されます！\nクリアタイム:" + timemanager._timeElapsed.ToString("F2"));
            Debug.Log(intervalTime + "秒後に次のステージが生成されます！");
            //script.GenerateTerrain();
            Destroy( timemanager ); //タイムマネジャーを削除
            Destroy( this.gameObject );
            script.IsEnemyManagerCreated = false;
        }

        //アドベンチャーステージ
        if(script.IsCreared == true)
        {
            script.StopBGM();
            script.DeleteFrontWall();
            crearTime = timemanager._timeElapsed;
            emotionsScore = esscript.CalculateScore(); //感情スコアを計算
            esscript.ResetScore(); //スコアをリセット
            esscript.HandleEmotion(emotionsScore); //難易度, ジャンル調整
            Debug.Log("アイテムが正しく回収されました");
            script.Startwidth = script.Startwidth + script.width;
            script.ReloadGeneratePosition();
            script.shuffle = true;
            script.ShuffleList(script.itemPrefabs);
            script.Invoke("GenerateTerrain", intervalTime); // ｎ秒後にHeal()を呼び出し
            crear_textfadecontroller.ShowMessage(intervalTime + "秒後に次のステージが生成されます！\nクリアタイム:" + timemanager._timeElapsed.ToString("F2"));
            Debug.Log(intervalTime + "秒後に次のステージが生成されます！");
            script.IsCreared = false;
            Destroy( timemanager ); //タイムマネジャーを削除
            Destroy( this.gameObject );
            script.IsEnemyManagerCreated = false;
        }
    }
}
