using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class WelcomeScriptController : MonoBehaviour
{
    public GameObject ScrollPopUpResumeButtonsGrid;
    public GameObject ScrollPopUpNewGameButtonsGrid;
    public GameObject LanguageButton;
    public GameObject WhatToPracticePopUpResume;
    public GameObject WhatToPracticePopUpNewGame;
    public GameObject StreakNumber;
    public GameObject XPValue;
    public GameObject LostStreak;
    public GameObject FirstTimeStreak;
    public GameObject ContinueStreak;
    public GameObject continueStreakValue;

    void Start()
    {
        DrawEnrolledLanguageButtons();
        DrawAvailableLanguageButtons();
        CheckDailyStreak();
        GeneralFunctions.UpdateStreakUI(StreakNumber);
        GeneralFunctions.UpdateXPUI(XPValue);
    }


    public void CheckDailyStreak()
    {
        //check if there is something in "last login" in the database, if not, then intialize it to 1
        var lastLogin = DatabaseController.GetLastLoginTime();
        var streak = DatabaseController.GetStreak();
        DateTime now = DateTime.UtcNow;

        if (lastLogin != DateTime.MinValue)
        {
            TimeSpan timeSinceLast = now.Date - lastLogin.Date;

            if (timeSinceLast.TotalDays == 1)
            {
                // Continued the streak
                streak++;
                continueStreakValue.GetComponent<TextMeshProUGUI>().text=streak.ToString();
                ContinueStreak.SetActive(true);
                DatabaseController.UpdatePlayerStreak(streak);
                //TODO: show him a pop-up to congratulate about the ongoing streak
                Debug.Log($"Streak continued! You're at {streak} days.");
            }
            else if (timeSinceLast.TotalDays > 1)
            {
                // Missed a day, reset streak
                streak = 1;
                LostStreak.SetActive(true);
                //TODO: show him a pop-up to let know they've lost the streak
                DatabaseController.UpdatePlayerStreak(streak);
                Debug.Log("Streak reset!");
            }
            else
            {
                // Same day login, do nothing
                Debug.Log("Already logged in today.");
            }
        }
        else
        {
            // First time login
            streak = 1;
            DatabaseController.UpdatePlayerStreak(streak);
            FirstTimeStreak.SetActive(true);
            //TODO: show him a pop-up to let him know about the streaks
            Debug.Log("Starting your streak!");
        }

        // Always update last login time
        GameData.dayStreak = streak;
        DatabaseController.UpdatePlayerLastLoginTime(now);
    }

    public void OnQuitButtonPressed()
    {
        Application.Quit();
        Debug.Log("game is quitting...");
    }

    public void DrawEnrolledLanguageButtons()
    {
        foreach (Transform child in ScrollPopUpResumeButtonsGrid.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < User.EnrolledLanguages.Count; i++)
        {
            string selectedLanguage = User.EnrolledLanguages[i];
            Sprite flag = Resources.Load<Sprite>("CountryFlags/" + selectedLanguage);
            GameObject newLanguageButton = Instantiate(LanguageButton, ScrollPopUpResumeButtonsGrid.transform);
            newLanguageButton.GetComponentInChildren<TextMeshProUGUI>().text = selectedLanguage;

            Transform flagImageTransform = newLanguageButton.transform.Find("FlagImage");
            Image flagImage = flagImageTransform.GetComponent<Image>();
            flagImage.sprite = flag;

            newLanguageButton.GetComponent<Button>().onClick.AddListener(() => OnLanguageSelected(WhatToPracticePopUpResume, selectedLanguage));
        } 
    }

    public void OnLanguageSelected(GameObject whatToPracticePopUp , string selectedLanguage)
    {
        Transform selectedLanguageTextTransformer = whatToPracticePopUp.transform.Find("LanguageSelectedText");
        TextMeshProUGUI text = selectedLanguageTextTransformer.GetComponent<TextMeshProUGUI>();
        text.text = selectedLanguage;

        whatToPracticePopUp.SetActive(true);
        GameData.selectedLanguage = selectedLanguage;
        //update the languages
        DatabaseController.AddLearningLanguage(selectedLanguage);
        
    }

    public void OnStudyButtonClicked()
    {

    }

    public void OnPracticeButtonClicked()
    {
        GameData.hasResumeBeenPressed = false;
        GameData.hasNewGameBeenPressed = false;
        SceneManager.LoadScene("LevelSelectorGames");
    }

    public void DrawAvailableLanguageButtons()
    {
        foreach (Transform child in ScrollPopUpNewGameButtonsGrid.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < User.AvailableLanguages.Count; i++)
        {
            //take the sprite of the current language flag
            string selectedLanguage = User.AvailableLanguages[i];
            Sprite flag = Resources.Load<Sprite>("CountryFlags/" + selectedLanguage);
            GameObject newLanguageButton = Instantiate(LanguageButton, ScrollPopUpNewGameButtonsGrid.transform);
            newLanguageButton.GetComponentInChildren<TextMeshProUGUI>().text = selectedLanguage;

            Transform flagImageTransform = newLanguageButton.transform.Find("FlagImage");
            Image flagImage = flagImageTransform.GetComponent<Image>();
            flagImage.sprite = flag;
            //newLanguageButton.GetComponent<Image>().sprite = flag;
            newLanguageButton.GetComponent<Button>().onClick.AddListener(() => OnLanguageSelected(WhatToPracticePopUpNewGame, selectedLanguage));
            newLanguageButton.GetComponent<Button>().onClick.AddListener(() => DrawEnrolledLanguageButtons());
            newLanguageButton.GetComponent<Button>().onClick.AddListener(() => DrawAvailableLanguageButtons());
        }
    }
        
    
}
