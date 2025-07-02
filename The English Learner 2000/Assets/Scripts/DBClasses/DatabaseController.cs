using UnityEngine;
using SQLite;
using System.Collections.Generic;
using System.Linq;
using System;
using Assets.Scripts.DBClasses;
using UnityEngine.InputSystem;
using static Unity.Burst.Intrinsics.X86;

public class DatabaseController : MonoBehaviour
{
    static SQLiteConnection db;
    static int userId;
    private bool hasRun = false;
    void Start()
    {
        if (hasRun == true)
        {
            return;
        }
        db = DatabaseManager.Instance.DB;
        
        PopulateDatabaseWithMandatoryTables();
        userId = db.Table<User>().First().ID;
        AddUserLanguagesToArray();
        RetainNumberOfLessons();
        hasRun = true;
    }

    void RetainNumberOfLessons()
    {
        GameData.numberOfLessons = db.Table<Lesson>().Count();
    }

    void AddUserLanguagesToArray()
    {
        var enrolledLangIDs = db.Table<UserLanguages>()
                                .Where(ul => ul.UserID == userId)
                                .Select(ul => ul.LanguageID)
                                .ToList();
        //for all ids, get the languages
        User.EnrolledLanguages = db.Table<Language>()
                                .Where(l => enrolledLangIDs.Contains(l.ID))
                                .Select(l => l.LanguageName)
                                .ToList();
        //apart from the ones the user is enrolled in
        User.AvailableLanguages = db.Table<Language>()
                                .Where(l => !enrolledLangIDs.Contains(l.ID))
                                .Select(l => l.LanguageName)
                                .ToList();
    }

    void PopulateDatabaseWithMandatoryTables()
    {
        CreateAndPopulateLessonTable();
        CreateDefaultUserIfNeeded();
        CreateAndPopulateLanguageTable();
        CreateAndPopulateGamesTable();
        CreateAndPopulatePlayerLessonProgressTable();
        CreateAndPopulateVocabTable();
        CreateAndPopulateGameWordMappingTable();
        CreateAndPopulateLessonWordMappingTable();
        CreateAndPopulatePlayerGameProfile();
        CreateAndPopulateScoreHistory();
        db.CreateTable<UserLanguages>();     
    }
    void CreateAndPopulateScoreHistory()
    {
        db.CreateTable<ScoreHistory>();
    }
    void CreateAndPopulatePlayerGameProfile()
    {
        db.CreateTable<PlayerGameProfile>();
        if (db.Table<PlayerGameProfile>().Count() > 0) return;
        var gamesList = new List<PlayerGameProfile>()
        {
            new PlayerGameProfile {gameID = 1, currentLevel = 0},
            new PlayerGameProfile {gameID = 2, currentLevel = 0},
            new PlayerGameProfile {gameID = 3, currentLevel = 0},
            new PlayerGameProfile {gameID = 4, currentLevel = 0},
            new PlayerGameProfile {gameID = 5, currentLevel = 0},
            new PlayerGameProfile {gameID = 6, currentLevel = 0},
        };
        gamesList = gamesList.ToList();
        db.InsertAll(gamesList);
    }
    void CreateAndPopulateLessonTable()
    {
        db.CreateTable<Lesson>();
        AddLesson("Present Tense Simple", "The present simple tense is used to talk about habits, facts, and regular actions.",
    "Subject                  Verb Form      Example\r\nI/You/We/They       base verb\t   I eat apples.\r\n\r\nHe/She/It \t\tbase verb \t  She eats apples.\r\n\t\t\t+ s/es\r\n\r\nFor negative and interogative we use the helper verb : TO DO\r\n\r\n I/You/We/They     do not + base verb      I do not eat apples.\r\nHe/She/It \t         does not + base verb    She does not eat apples. \r\n\r\n Do       I/You/We/They    base verb\t    Do you eat apples?\r\n Does   He/She/It \t          base verb        Does she eat apples?"
    , "Add -s for most verbs\r\nAdd -es for verbs ending in -ch, -sh, -x, -s, -o (e.g., watches, goes)\r\nFor verbs ending in consonant + y, change y ? ies (e.g., studies)\r\nImportant: In negative form, the main verb stays in base form (no -s/es).\r\nFor questions, just like in negatives, the main verb stays in base form.\r\nDo not = Don't and Does not = Doesn't", "Conjugation\r\n", 0);
    }
    void CreateAndPopulateLessonWordMappingTable()
    {
        db.CreateTable<LessonWordMapping>();
        if (db.Table<LessonWordMapping>().Count() > 0) return;
        var vocabList = new List<LessonWordMapping>
        {
            new LessonWordMapping{wordID=1, lessonID=1},
            new LessonWordMapping{wordID=2, lessonID=1},
            new LessonWordMapping{wordID=3, lessonID=1},
            new LessonWordMapping{wordID=4, lessonID=1},
            new LessonWordMapping{wordID=5, lessonID=1},
            new LessonWordMapping{wordID=6, lessonID=1},
            new LessonWordMapping{wordID=7, lessonID=1},
            new LessonWordMapping{wordID=8, lessonID=1},
            new LessonWordMapping{wordID=9, lessonID=1},
            new LessonWordMapping{wordID=10, lessonID=1},
        };
        vocabList = vocabList.ToList();
        db.InsertAll(vocabList);

    }
    void CreateAndPopulateGameWordMappingTable()
    {
        db.CreateTable<GameWordMapping>();
        if (db.Table<GameWordMapping>().Count() > 0) return;
        var vocabList = new List<GameWordMapping>
        {
            new GameWordMapping{wordID=1, gameID=4},
            new GameWordMapping{wordID=2, gameID=4},
            new GameWordMapping{wordID=3, gameID=1},
            new GameWordMapping{wordID=3, gameID=5},
            new GameWordMapping{wordID=4, gameID=5},
            new GameWordMapping{wordID=4, gameID=1},
            new GameWordMapping{wordID=5, gameID=5},
            new GameWordMapping{wordID=6, gameID=6},
            new GameWordMapping{wordID=7, gameID=6},
            new GameWordMapping{wordID=8, gameID=2},
            new GameWordMapping{wordID=9, gameID=2},
            new GameWordMapping{wordID=10, gameID=3},
            //add the hangman words for adaptive difficulty
            //a1
            new GameWordMapping{wordID=11, gameID=1},
            new GameWordMapping{wordID=12, gameID=1},
            new GameWordMapping{wordID=13, gameID=1},
            new GameWordMapping{wordID=14, gameID=1},
            new GameWordMapping{wordID=15, gameID=1},
            new GameWordMapping{wordID=16, gameID=1},
            //a2
            new GameWordMapping{wordID=17, gameID=1},
            new GameWordMapping{wordID=18, gameID=1},
            new GameWordMapping{wordID=19, gameID=1},
            new GameWordMapping{wordID=20, gameID=1},
            new GameWordMapping{wordID=21, gameID=1},
            new GameWordMapping{wordID=22, gameID=1},
            //b1
            new GameWordMapping{wordID=23, gameID=1},
            new GameWordMapping{wordID=24, gameID=1},
            new GameWordMapping{wordID=25, gameID=1},
            new GameWordMapping{wordID=26, gameID=1},
            new GameWordMapping{wordID=27, gameID=1},
            new GameWordMapping{wordID=28, gameID=1},
        };
        vocabList = vocabList.ToList();
        db.InsertAll(vocabList);
    }
    void CreateAndPopulateVocabTable()
    {
        db.CreateTable<Vocab>();
        if (db.Table<Vocab>().Count() > 0) return;
        var vocabList = new List<Vocab>
        {
            new Vocab {LanguageID=1, Word="She eat apples .%She eats apples .", LessonID=1, Level=0},
            new Vocab {LanguageID=1, Word="Does it rains usually ?%Does it rain usually ?", LessonID=1, Level=0},
            new Vocab {LanguageID=1, Word="apple", LessonID=1, Level=0},
            new Vocab {LanguageID=1, Word="usually", LessonID=1, Level=0},
            new Vocab {LanguageID=1, Word="She eats apples .", LessonID=1, Level=0},
            new Vocab {LanguageID=1, Word="Does she usually eat apples ?", LessonID=1, Level=0},
            new Vocab {LanguageID=1, Word="We do not eat apples .", LessonID=1, Level=0},
            new Vocab {LanguageID=1, Word="apple%fruit%red%pie", LessonID=1, Level=0},
            new Vocab {LanguageID=1, Word="cherry%fruit%red%pie", LessonID=1, Level=0},
            new Vocab {LanguageID=1, Word="usually=de obicei%rarely=rareori%never=niciodata%always=mereu%sometimes=uneori", LessonID=1, Level=0},
            //id=11 from now on and they are words for the adaptive difficulty
            //hangman a1
            new Vocab {LanguageID=1, Word="dog", LessonID=0, Level=0},
            new Vocab {LanguageID=1, Word="sun", LessonID=0, Level=0},
            new Vocab {LanguageID=1, Word="table", LessonID=0, Level=0},
            new Vocab {LanguageID=1, Word="bread", LessonID=0, Level=0},
            new Vocab {LanguageID=1, Word="teacher", LessonID=0, Level=0},
            new Vocab {LanguageID=1, Word="student", LessonID=0, Level=0},
            //here we should do them for the other games as well, but after
            //id=17 hangman a2
            new Vocab {LanguageID=1, Word="pencil", LessonID=0, Level=1},
            new Vocab {LanguageID=1, Word="mirror", LessonID=0, Level=1},
            new Vocab {LanguageID=1, Word="airport", LessonID=0, Level=1},
            new Vocab {LanguageID=1, Word="village", LessonID=0, Level=1},
            new Vocab {LanguageID=1, Word="borrow", LessonID=0, Level=1},
            new Vocab {LanguageID=1, Word="travel", LessonID=0, Level=1},
            //id=23 hangman b1
            new Vocab {LanguageID=1, Word="excited", LessonID=0, Level=2},
            new Vocab {LanguageID=1, Word="nervous", LessonID=0, Level=2},
            new Vocab {LanguageID=1, Word="freedom", LessonID=0, Level=2},
            new Vocab {LanguageID=1, Word="history", LessonID=0, Level=2},
            new Vocab {LanguageID=1, Word="interview", LessonID=0, Level=2},
            new Vocab {LanguageID=1, Word="adventure", LessonID=0, Level=2},

        };
        vocabList = vocabList.Where(l => !string.IsNullOrEmpty(l.Word)).ToList();
        db.InsertAll(vocabList);
    }
    void CreateAndPopulatePlayerLessonProgressTable()
    {
        db.CreateTable<PlayerLessonProgress>();
        if (db.Table<PlayerLessonProgress>().Count() > 0) return;

        List<Lesson> lessons = db.Table<Lesson>().ToList();
        foreach (var lesson in lessons)
        {
            // Step 2: Insert a record for each lesson with status -1 (locked)
            PlayerLessonProgress progress = new PlayerLessonProgress
            {
                playerID = userId,
                lessonID = lesson.ID,
                status = -1 
            };
            db.Insert(progress);
        }
        UnlockLesson(userId, 1);
    }
    void CreateAndPopulateLanguageTable()
    {
        db.CreateTable<Language>();

        if (db.Table<Language>().Count() > 0) return; 

        var languageList = new List<Language>
        {
            new Language {LanguageName = "English"},
            new Language {LanguageName = "Spanish"},
            new Language {LanguageName = "French"},
            new Language {LanguageName = "German"},
            new Language {LanguageName = "Italian"}
        };
        languageList = languageList.Where(l => !string.IsNullOrEmpty(l.LanguageName)).ToList();
        db.InsertAll(languageList);
        
    }
    void CreateAndPopulateGamesTable()
    {
        db.CreateTable<Games>();

        if (db.Table<Games>().Count() > 0) return;

        var gamesList = new List<Games>
        {
            new Games {GameName = "Hangman"},
            new Games {GameName = "Taboo"},
            new Games {GameName = "MatchTheMeaning"},
            new Games {GameName = "GrammarPolice"},
            new Games {GameName = "PronouncePro"},
            new Games {GameName = "WordOrder"},
            
        };
        gamesList = gamesList.Where(l => !string.IsNullOrEmpty(l.GameName)).ToList();
        db.InsertAll(gamesList);
    }
    void CreateDefaultUserIfNeeded()
    {
        db.CreateTable<User>(); 

        var existingUser = db.Table<User>().FirstOrDefault();
        if (existingUser == null)
        {
            var defaultUser = new User
            {
                Username = "Player",
                LastLogin = DateTime.MinValue,
                Streak = 0
            };

            db.Insert(defaultUser);
            Debug.Log("Default user created.");
        }
        else
        {
            Debug.Log("User already exists: " + existingUser.Username);
        }
    }
    public static void AddLearningLanguage(string language)
    {
        var lang = db.Table<Language>().FirstOrDefault(l => l.LanguageName == language);
        if (lang == null) return;
        var exists = db.Table<UserLanguages>()
            .FirstOrDefault(ul => ul.UserID == userId && ul.LanguageID == lang.ID);

        if (exists == null)
        {
            var newUserLang = new UserLanguages
            {
                UserID = userId,
                LanguageID = lang.ID,
            };
            db.Insert(newUserLang);
            Debug.Log($"User is now learning {language}.");
            //update in the arrays
            User.EnrolledLanguages.Add(language);
            User.AvailableLanguages.Remove(language);
        }
    }
    public static void AddLesson(string title, string subtitle, string theory, string extraButtonText, string extraButtonTitle, int level)
    {
        if (db.Table<Lesson>().Count() > 0) return;
        Lesson newLesson = new Lesson { Title = title, Subtitle = subtitle, Theory = theory, ExtraButtonTitle = extraButtonTitle, ExtraButtonText = extraButtonText , Level=level};
        db.Insert(newLesson);
        Debug.Log("added new lesson");
    }


    public static void AddScore(int gameID, float score)
    {
        ScoreHistory history = new ScoreHistory { gameID = gameID, score = score };
        db.Insert(history);
    }
    public static float ComputeAverage(int gameID, int limit=5)
    {
        List<ScoreHistory> recentScores = db.Table<ScoreHistory>()
            .Where(s => s.gameID == gameID)
            .OrderByDescending(s => s.ID)
            .Take(limit)
            .ToList();

        if (recentScores.Count == 0) return 1.0f; // Default neutral score

        float average = recentScores.Average(s => s.score);
        return average;
    }

    public static int CalculateDeltaUpdateLevel(int gameId) //return -1 for level downgrade, 1 for upgrade and 0 for no change
    {
        int count = db.Table<ScoreHistory>()
            .Count(s => s.gameID == gameId);

        if (count >= 5)
        {
            int delta = 0;
            float avg = ComputeAverage(gameId);
            if (avg > 1.5f) delta = 1;
            else if (avg < 0.9f) delta = -1;

            int currentLevel = GetCurrentLevel(gameId);
            //get currentLevel
            int newLevel = Mathf.Clamp(currentLevel + delta, 0, 5);
            UpdateGameLevel(gameId, newLevel);
            if (newLevel < currentLevel)
            {
                return -1;
            }
            else if (newLevel > currentLevel) { return 1; }
            return 0;
        }
        return -100;
        
    }

    public static void UpdateGameLevel(int gameId, int newLevel)
    {
        var game = db.Table<PlayerGameProfile>().FirstOrDefault(u => u.gameID == gameId);
        game.currentLevel = newLevel;
        db.Update(game);
    }

    public static int GetCurrentLevel(int gameId)
    {
        var profile = db.Table<PlayerGameProfile>()
            .FirstOrDefault(p => p.gameID == gameId);

        return profile?.currentLevel ?? 0; // Default to level 0 if not found
    }


    public static Lesson GetLessonData(int lessonNumber)
    {
        return db.Table<Lesson>().FirstOrDefault(l => l.ID == lessonNumber);
    }

    public static DateTime GetLastLoginTime()
    {
        return db.Table<User>().FirstOrDefault(u => u.ID == userId).LastLogin;
    }

    public static void UpdatePlayerStreak(int streak)
    {
        var user = db.Table<User>().FirstOrDefault(u => u.ID == userId);
        user.Streak = streak;
        db.Update(user);
    }

    public static void UpdatePlayerLastLoginTime(DateTime lastLoginTime)
    {
        var user = db.Table<User>().FirstOrDefault(u => u.ID == userId);
        user.LastLogin = lastLoginTime;
        db.Update(user);
    }

    public static int GetStreak()
    {
        return db.Table<User>().FirstOrDefault(u => u.ID == userId).Streak;
    }

    public static void UnlockLesson(int playerID, int lessonID)
    {
        // Find the player's progress record for the specified lesson
        var progress = db.Table<PlayerLessonProgress>()
                         .FirstOrDefault(p => p.playerID == playerID && p.lessonID == lessonID);

        if (progress != null)
        {
            // Update the status to 0 (unlocked)
            progress.status = 0;
            db.Update(progress);
            Debug.Log($"Lesson {lessonID} unlocked for Player {playerID}.");
        }
        else
        {
            Debug.LogWarning($"No progress record found for Player {playerID} and Lesson {lessonID}.");
        }
    }

    public static List<string> GetWordsForGameAndLesson(int lessonId, int gameId)
    {
        List<string> words = new List<string>();
        var lessonWords = db.Table<LessonWordMapping>()
                                .Where(lwm => lwm.lessonID == lessonId)
                                .ToList();

        var gameWords = db.Table<GameWordMapping>()
                              .Where(gwm => gwm.gameID == gameId)
                              .ToList();

        var wordIDs = new HashSet<int>();
        foreach (var lw in lessonWords)
        {
            foreach (var gw in gameWords)
            {
                if (lw.wordID == gw.wordID)
                {
                    wordIDs.Add(lw.wordID);
                }
            }
        }

        foreach (int wordID in wordIDs)
        {
            var word = db.Table<Vocab>().FirstOrDefault(w => w.ID == wordID);
            if (word != null)
            {
                words.Add(word.Word);
                Debug.Log($"Found word: {word.Word}");
            }
        }
        return words;
    }

    public static List<string> GetWordsForGameAndCurrentLevel(int gameID)
    {
        int currentLevel = GetCurrentLevel(gameID);

        // Get word IDs for the game
        var mappedWordIDs = db.Table<GameWordMapping>()
                              .Where(gwm => gwm.gameID == gameID)
                              .Select(gwm => gwm.wordID)
                              .ToList();

        // Get vocab words matching those IDs and the current level
        var words = db.Table<Vocab>()
                      .Where(v => mappedWordIDs.Contains(v.ID) && v.Level == currentLevel)
                      .Select(v => v.Word)
                      .ToList();

        return words;
    }

}
