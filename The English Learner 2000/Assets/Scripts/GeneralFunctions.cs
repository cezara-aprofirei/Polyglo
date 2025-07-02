using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GeneralFunctions : MonoBehaviour
{
    public static void UpdateStreakUI(GameObject StreakNumber)
    {
        StreakNumber.GetComponentInChildren<TextMeshProUGUI>().text = GameData.dayStreak.ToString();
    }
    public static void UpdateXPUI(GameObject StreakNumber)
    {
        StreakNumber.GetComponentInChildren<TextMeshProUGUI>().text = GameData.XP.ToString();
    }

    public static void StartLesson()
    {
        GameData.freeplayMode = false;
    }
    public static void FreePlay()
    {
        GameData.freeplayMode=true;
    }
    public static void LoadNextGameScene()
    {
        if (GameData.gameScenes.Count > 0)
        {
            //await Task.Delay(2000);//TOREMOVE
            string nextScene = GameData.gameScenes.Dequeue();
            Debug.Log($"Loading next game scene: {nextScene}");
            SceneManager.LoadScene(nextScene);
        }
        else
        {
            Debug.Log("All game scenes completed!");
            SceneManager.LoadScene("LessonSelector");
        }
    }

    public static float calculateScoreForLevel(float timeTakenSeconds, int maxTimeSeconds, int correctAttempts, int totalAttempts)
    {
        
        // Clamp to avoid divide by zero
        if (totalAttempts == 0) totalAttempts = 1;

        // Accuracy: between 0 and 1
        float accuracy = Mathf.Clamp01((float)correctAttempts / totalAttempts);

        // Speed: 1 if completed within max time, drops below 1 otherwise
        float speedFactor = Mathf.Clamp01((float)maxTimeSeconds / timeTakenSeconds);

        // Try penalty: encourage fewer tries (1.0 = perfect first try, lower if more tries)
        float tryPenalty = Mathf.Clamp01(1f - ((float)(totalAttempts - correctAttempts) / 5f));

        // Weighted score
        float score = (0.5f * accuracy) + (0.3f * speedFactor) + (0.2f * tryPenalty);

        // Normalize to 0.0 – 2.0 (or leave as 0.0 – 1.0 if you prefer)
        print("correct attempts : " + correctAttempts + "\n");
        print("total attempts : " + totalAttempts + "\n");
        print("accuracy : " + accuracy + "\n");
        print("time taken : " + timeTakenSeconds + "\n");
        print("speed : " + speedFactor + "\n");
        print("score : " + score + "\n");
   
        return Mathf.Clamp(score * 2.0f, 0f, 2.0f);
    }
}
