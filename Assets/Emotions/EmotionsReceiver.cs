using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class EmotionsReceiver : MonoBehaviour
{
    private TcpListener server;
    private TcpClient client;
    private NetworkStream stream;
    private const int port = 5555; // Pythonと一致させる必要あり
    private string previousEmotion = "";
    GameObject generator1; //generator1そのものが入る変数
	TerrainGenerator2 t2script; //TerrainGeneraotr2Scriptが入る変数
    GameObject enemymanager;
    EnemyManager emscript;
    private float neutralTimeCounter = 0f; // neutralが続いた時間を記録
    private float neutralThreshold = 90f; // 1分半 (90秒)
    float playerPerformance = 100; //仮のプレイヤースキル指標
    private List<string> emotionHistory = new List<string>(); //1ステージ分の感情リスト　ポジティブネガティブのみ
    private readonly object emotionHistoryLock = new object();


    void Start()
    {
        // サーバーを開始
        server = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
        server.Start();
        Debug.Log("Emotion Receiver started. Waiting for connection...");

        // クライアントを非同期で接続
        server.BeginAcceptTcpClient(OnClientConnected, null);

        generator1 = GameObject.Find ("Generator1");
		t2script = generator1.GetComponent<TerrainGenerator2>();
    }

    private void OnClientConnected(IAsyncResult result)
    {
        client = server.EndAcceptTcpClient(result);
        Debug.Log("Client connected!");
        stream = client.GetStream();
        Debug.Log("Stream created: " + (stream != null));
    }

    void Update()
    {
        if(t2script.IsEnemyManagerCreated == true)
        {
            Debug.Log("EnemyManagerが生成されました");
            enemymanager = GameObject.Find("EnemyManager(Clone)");
            emscript = enemymanager.GetComponent<EnemyManager>();
        }

        ReadDataAsync();
        // ゲーム内で感情データを利用
        //HandleEmotion(emotionData.emotion);
        //emotionHistory.Add(emotionData.emotion); // 感情を履歴に追加

        /*if (stream != null)
        {
            Debug.Log("Stream is initialized.");
            Debug.Log("DataAvailable: " + stream.DataAvailable);
            if (stream.DataAvailable)
            {
                Debug.Log("Stream data available: " + stream.DataAvailable);

                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string jsonData = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                // JSONデータをパース
                EmotionData emotionData = JsonUtility.FromJson<EmotionData>(jsonData);
                Debug.Log("Received Emotion: " + emotionData.emotion);

                // 感情が前回と異なる場合にログを出力
                //emotion: neutral, happy, surprised, sad, angry
                if (emotionData.emotion != previousEmotion)
                {
                    Debug.Log("New Emotion: " + emotionData.emotion);
                    previousEmotion = emotionData.emotion;
                }

                // ゲーム内で感情データを利用
                //HandleEmotion(emotionData.emotion);
                emotionHistory.Add(emotionData.emotion); // 感情を履歴に追加
            }
        }*/
    }

    private async void ReadDataAsync()
    {
        if (stream != null)
        {
            byte[] buffer = new byte[1024];
            try
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    string jsonData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Debug.Log("Received Data: " + jsonData);

                    EmotionData emotionData = JsonUtility.FromJson<EmotionData>(jsonData);
                    Debug.Log("Emotion: " + emotionData.emotion);

                    lock (emotionHistoryLock) 
                    {
                        emotionHistory.Add(emotionData.emotion); // 感情を履歴に追加
                    }
                    
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error reading data: " + ex.Message);
            }
        }
    }


    public void HandleEmotion(int emotionsScore)
    {
        Debug.Log("Handle Emotionが呼ばれました.");
        if (emotionHistory.Count == 0)
        {
            Debug.Log("emotionHistory is empty.");
        }
        else
        {
            Debug.Log("emotionHistory: " + string.Join(", ", emotionHistory));
        }
        Debug.Log("emotionsScore = " + emotionsScore);

        if (emotionsScore > 5)
        {
            Debug.Log("Player is happy! No changes required.");
            neutralTimeCounter = 0f; // neutral時間をリセット

            // CSVファイルへの出力
            WriteToCsv("Player is happy", emotionsScore, t2script.level, t2script.genre);
        }
        else if (emotionsScore < -5)
        {
            Debug.Log($"Player is uncomfortable. Adjusting difficulty...");
            AdjustDifficulty();
            neutralTimeCounter = 0f;

            // CSVファイルへの出力
            WriteToCsv("Player is uncomfortable", emotionsScore, t2script.level, t2script.genre);
        }
        /*else if (emotion == "surprised")
        {
            Debug.Log("Player is surprised. Adjusting difficulty based on performance...");
            AdjustDifficulty();
            neutralTimeCounter = 0f;
        }*/
        else if (emotionsScore <= 5 && emotionsScore >= -5)
        {
            neutralTimeCounter += Time.deltaTime;
            /*if (neutralTimeCounter >= neutralThreshold)
            {*/
                Debug.Log("Player seems bored. Changing game mode...");
                ChangeGameModeOnBoredom();

                // CSVファイルへの出力
                WriteToCsv("Player seems bored", neutralTimeCounter, t2script.level, t2script.genre);
            //}
        }
    }

    private void AdjustDifficulty()
    {
        if(emscript != null)
        {
            Debug.Log("emscript != null");
            // 仮のクリアタイムやスコア基準（実際のゲームに合わせて変更）
            //float playerPerformance = GetPlayerPerformance(); // スコアやタイムを取得

            if ((int)emscript.crearTime < 20) // 20秒以内にクリアなら一段階難しく（簡単すぎ）
            {
                // CSVファイルへの出力
                WriteToCsv("CrearTime :", emscript.crearTime, t2script.level, t2script.genre);

                if (t2script.level < TerrainGenerator2.LevelList.hard)
                {
                    t2script.level++;
                    Debug.Log($"Difficulty increased to: {t2script.level}");

                    // CSVファイルへの出力
                    WriteToCsv("Difficulty increased to:", emscript.crearTime, t2script.level, t2script.genre);
                }
            }
            else if ((int)emscript.crearTime > 80) // 80秒以上かかってるなら一段階簡単に（難しすぎ）
            {
                // CSVファイルへの出力
                WriteToCsv("CrearTime :", emscript.crearTime, t2script.level, t2script.genre);

                if (t2script.level > TerrainGenerator2.LevelList.easy)
                {
                    t2script.level--;
                    Debug.Log($"Difficulty decreased to: {t2script.level}");

                    // CSVファイルへの出力
                    WriteToCsv("Difficulty decreased to:", emscript.crearTime, t2script.level, t2script.genre);
                }
            }
            else
            {
                ChangeGameModeOnBoredom();

                // CSVファイルへの出力
                WriteToCsv("Level is not changed.", emscript.crearTime, t2script.level, t2script.genre);
            }

            // CSVファイルへの出力
            WriteToCsv("Level is not changed.", emscript.crearTime, t2script.level, t2script.genre);
            
        }
    }

    private void ChangeGameModeOnBoredom()
    {
        if (t2script.genre == TerrainGenerator2.GenreList.actionRPG)
        {
            t2script.genre = TerrainGenerator2.GenreList.Adventure; 
        }
        else if (t2script.genre == TerrainGenerator2.GenreList.Adventure)
        {
            t2script.genre = TerrainGenerator2.GenreList.FPS;
        }
        else if (t2script.genre == TerrainGenerator2.GenreList.FPS)
        {
            t2script.genre = TerrainGenerator2.GenreList.actionRPG;
        }
        Debug.Log($"Game mode changed to: {t2script.genre}");

        // CSVファイルへの出力
        WriteToCsv("Game mode changed to:", neutralTimeCounter, t2script.level, t2script.genre);
        neutralTimeCounter = 0f; // neutral時間をリセット
    }

    public int CalculateScore()
    {
        Dictionary<string, int> emotionWeights = new Dictionary<string, int>
        {
            { "happy", 1 },
            { "sad", -1 },
            { "angry", -1 },
            { "fear", -1 },
            { "surprised", 0 },
            { "neutral", 0 }
        };

        int score = 0;
        foreach (var emotion in emotionHistory)
        {
            if (emotionWeights.ContainsKey(emotion))
            {
                score += emotionWeights[emotion]; //ポジティブな感情を+1, ネガティブな感情を-1としてそれの合計が>0なら楽しめてる,<0なら楽しめてないとする
            }
        }

        // 経過時間をスコアに反映
        //score += Mathf.FloorToInt(elapsedTime * 0.1f); // 例えば経過時間1秒につき+0.1スコア
        return score;
    }

    public void ResetScore()
    {
        emotionHistory.Clear();
    }

    private void WriteToCsv(string message, float value, TerrainGenerator2.LevelList level, TerrainGenerator2.GenreList genre)
    {
        // CSVファイルの保存先を指定
        string filePath = Path.Combine(Application.dataPath, "Unity_EmotionLog.csv");

        try
        {
            // ファイルが存在しない場合、ヘッダーを追加
            if (!File.Exists(filePath))
            {
                using (var writer = new StreamWriter(filePath, false)) // 上書きモード
                {
                    writer.WriteLine("Timestamp,Message,Value,Level,Genre"); // ヘッダー行
                }
            }

            // ログを追記
            using (var writer = new StreamWriter(filePath, true)) // 追記モード
            {
                string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                writer.WriteLine($"{timestamp},{message},{value},{level},{genre}");
            }

            Debug.Log($"Log written to {filePath}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to write log: {ex.Message}");
        }
    }



    void OnApplicationQuit()
    {
        // クライアントとサーバーを閉じる
        stream?.Close();
        client?.Close();
        server?.Stop();
    }
}

[Serializable]
public class EmotionData
{
    public string emotion;
}
