using TMPro;
using UnityEngine;
using System;
using System.Diagnostics;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class TabooController : MonoBehaviour
{
    public GameObject StreakNumber;
    public GameObject XPValue;
    public GameObject WordToGuess;
    public GameObject FordbiddenWord;
    public GameObject WordsPanel;
    public Button SubmitButton;
    public TMP_InputField InputField;


    private string wordDescription;
    private string wordToGuess;
    private string modelGuess;
    private List<string> forbiddenWords = new List<string>();
    private List<string> words = new List<string>();
    private int counter;

    public GameObject WinScreen;
    public GameObject LoseScreen;
    public TextMeshProUGUI AIGuess;

    void Start()
    {
        counter = 0;
        if (GameData.freeplayMode == false)
        {//in a lesson
            //extract the words we need to play
            words = DatabaseController.GetWordsForGameAndLesson(GameData.lessonLevelSelected, 2);
        }
        else
        {
            //in freeplaymode
        }
        GeneralFunctions.UpdateStreakUI(StreakNumber);
        GeneralFunctions.UpdateXPUI(XPValue);
        //reset
        SubmitButton.onClick.AddListener(OnSubmitButtonClicked);
        InitialiseGame();
        
    }

    void InitialiseGame()
    {
        foreach (Transform child in WordsPanel.transform)
        {
            Destroy(child.gameObject);
        }
        forbiddenWords.Clear();
        InputField.text = "";
        SubmitButton.interactable = true;
        GenerateWords();
        WordToGuess.GetComponentInChildren<TextMeshProUGUI>().text = wordToGuess;
        for (int i = 0; i < forbiddenWords.Count; i++)
        {
            GameObject newWord = Instantiate(FordbiddenWord, WordsPanel.transform);
            newWord.GetComponentInChildren<TextMeshProUGUI>().text = forbiddenWords[i];
        }
        
    }

    void GenerateWords()
    {
        string extractedWords = words[counter++];
        List<string> extractedWordsList = extractedWords.Split("%").ToList();
        wordToGuess = extractedWordsList[0];
        for (int i = 1; i < extractedWordsList.Count; i++) { 
            forbiddenWords.Add(extractedWordsList[i]);
        }
    }

    void OnSubmitButtonClicked()
    {
        wordDescription = InputField.text;
        SubmitButton.interactable = false;
        UnityEngine.Debug.Log("Word Description: " + wordDescription);
        try
        {
            // Create a new process to run the Python script
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "python";  // Or "python3" if needed, or use full path
            start.Arguments = $"\"C:\\Users\\cezar\\Desktop\\The English Learner 2000\\Assets\\Scripts\\ollama_taboo.py\" \"{wordDescription}\"";
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true; // Capture stderr as well
            start.CreateNoWindow = true;

            using (Process process = Process.Start(start))
            {
                // Read the output from the Python script
                using (System.IO.StreamReader reader = process.StandardOutput)
                {
                    modelGuess = reader.ReadToEnd();
                    UnityEngine.Debug.Log("Model's Guess: " + modelGuess); // Use Debug.Log instead of Console.WriteLine
                }

                // Capture any errors from stderr
                using (System.IO.StreamReader reader = process.StandardError)
                {
                    string errorResult = reader.ReadToEnd();
                    if (!string.IsNullOrEmpty(errorResult))
                    {
                        UnityEngine.Debug.LogError("Python script error: " + errorResult);
                    }
                }
            }
            
            //check if it went well or not
            if(modelGuess.Trim().ToLower() == wordToGuess.Trim().ToLower())
            {
                AIGuess.text = modelGuess.Trim().ToLower();
                GuessedWord();
            }
            else
            {
                AIGuess.text = modelGuess.Trim().ToLower();
                DidntGuessWord();
            }
                
            
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError("Error: " + ex.Message);
        }
    }

    void GuessedWord()
    {
        if (counter < words.Count)
        {
            Invoke("InitialiseGame", 1f);
            //give a win pop up and a next button
        }
        else
        {
            //GeneralFunctions.LoadNextGameScene();
            WinScreen.SetActive(true);
        }
        //AIGuess.text = "";
        GameData.XP += 100;
        GeneralFunctions.UpdateXPUI(XPValue);
    }

    void DidntGuessWord()
    {
        if (counter < words.Count)
        {
            Invoke("InitialiseGame", 1f);
            //give a lose pop up and a next button
        }
        else
        {
            WinScreen.SetActive(true);

        }
        GameData.XP -= 100;
        GeneralFunctions.UpdateXPUI(XPValue);
    }
}
