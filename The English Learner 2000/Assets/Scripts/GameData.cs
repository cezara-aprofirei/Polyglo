using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static string selectedLanguage;
    public static bool currentLanguagesChanged = false;
    public static bool hasResumeBeenPressed = false;
    public static bool hasNewGameBeenPressed = false;
    public static int lessonLevelSelected = 0;
    public static int numberOfLessons = 0;
    public static int dayStreak = 0;
    public static int XP = 0;
    public static bool freeplayMode = false;
    public static Queue<string> gameScenes;
}
