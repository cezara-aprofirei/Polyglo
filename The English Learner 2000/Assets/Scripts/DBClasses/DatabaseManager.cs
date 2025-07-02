using UnityEngine;
using SQLite;
using System.IO;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance { get; private set; }

    public SQLiteConnection DB { get; private set; }

    private string dbPath;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeDatabase();
        
        
    }
    void InitializeDatabase()
    {
        dbPath = Path.Combine(Application.persistentDataPath, "TheEnglishLearner2000.db");

        // Optional: Copy from StreamingAssets if not present
        if (!File.Exists(dbPath))
        {
            string sourcePath = Path.Combine(Application.streamingAssetsPath, "TheEnglishLearner2000.db");
#if UNITY_ANDROID
            // Coroutine version for Android goes here if needed
#else
            if (File.Exists(sourcePath))
                File.Copy(sourcePath, dbPath);
#endif
        }

        DB = new SQLiteConnection(dbPath);
        Debug.Log("SQLite DB initialized at: " + dbPath);
    }

    void OnApplicationQuit()
    {
        DB?.Close();
    }
}
