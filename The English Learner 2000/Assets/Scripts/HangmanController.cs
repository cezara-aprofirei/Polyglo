using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HangmanController : MonoBehaviour
{
    public GameObject StreakNumber;
    public GameObject XPValue;
    [SerializeField] GameObject wordContainer;
    [SerializeField] GameObject keyboardContainer;
    [SerializeField] GameObject letterContainer;
    [SerializeField] GameObject letterButton;
    [SerializeField] TextAsset possibleWord;
    [SerializeField] GameObject[] hangmanStages;

    public GameObject WinScreen;
    public GameObject LoseScreen;

    private string word;
    private List<string> words = new List<string>();
    private int incorrectGuesses;
    private int correctGuesses;
    private int counter=0;
    bool isHard = false;
    int tries;
    Stopwatch stopwatch = new Stopwatch();
    //here tries is correctGuessees+IncorrectGuesses
    void Start()
    {
        if (GameData.freeplayMode==false)
        {//in a lesson
            //extract the words we need to play
            words = DatabaseController.GetWordsForGameAndLesson(GameData.lessonLevelSelected, 1);
        }
        else
        {
            //in freeplaymode
            //identify the level of the player and extract those words for the level
            //after each level check if the delta got bigger and modify the words array depending on their performance
            words = DatabaseController.GetWordsForGameAndCurrentLevel(1);
        }
        GeneralFunctions.UpdateStreakUI(StreakNumber);
        GeneralFunctions.UpdateXPUI(XPValue);
        InitialiseKeyboardButtons();
        InitialiseGame(); 
    }

    private void InitialiseKeyboardButtons()
    {
        for (int i=65; i<=90; i++)
        {
            CreateKeyboardButton(i);
        }
    }

    private void InitialiseGame()
    {
        stopwatch.Reset();
        stopwatch.Start();
        //reset data back to original state
        incorrectGuesses = 0;
        correctGuesses = 0;
        tries = 0;
        foreach (Button child in keyboardContainer.GetComponentsInChildren<Button>())
        {
            child.interactable = true;
        }

        foreach (Transform child in wordContainer.GetComponentInChildren<Transform>())
        {
            Destroy(child.gameObject);

        }
        foreach (GameObject stage in hangmanStages)
        {
            stage.SetActive(false); //stop displaying the hangman
        }
        //generate new word
        word = GenerateWord().ToUpper();

        if (isHard == false)
        {
            correctGuesses += 2;
            for (int i = 0; i < word.Length; i++)
            {
                var temp = Instantiate(letterContainer, wordContainer.transform);
                TextMeshProUGUI letterText = temp.GetComponentInChildren<TextMeshProUGUI>();

                // Reveal first and last letters at the start
                if (i == 0 || i == word.Length - 1)
                {
                    letterText.text = word[i].ToString();
                }
                else
                {
                    letterText.text = ""; // Placeholder for unguessed letters
                }
            }
        }
        else
        {
            for (int i = 0; i < word.Length; i++)
            {
                var temp = Instantiate(letterContainer, wordContainer.transform);
            }
        }
    }

    private void CreateKeyboardButton(int i)
    {
        string letter = ((char)i).ToString();
        GameObject temp = Instantiate(letterButton, keyboardContainer.transform);
        temp.GetComponentInChildren<TextMeshProUGUI>().text = ((char)i).ToString();
        temp.GetComponent<Button>().onClick.AddListener(delegate { CheckLetter(((char)i).ToString()); });//when we click the button it calls the function with the parameter as what was in the button
    }

    private string GenerateWord()
    {
        return words[counter++].ToString();
    }

    private void CheckLetter(string inputLetter)
    {//check if the pressed letter is in the word
        bool letterInWord = false;
        int loopStart = 0;
        int loopEnd = word.Length;
        if (isHard == false)
        {
            loopStart = 1;
            loopEnd = word.Length - 1;

        }
        for ( int i=loopStart; i<loopEnd; i++)
        {
            if (inputLetter == word[i].ToString())
            {
                letterInWord = true;
                correctGuesses++;
                wordContainer.GetComponentsInChildren<TextMeshProUGUI>()[i].text = inputLetter;
            }
        }

        if (letterInWord == false)
        {
            incorrectGuesses++;
            hangmanStages[incorrectGuesses - 1].SetActive(true);
        }
        CheckOutcome();
    }

    private void CheckOutcome()
    {
        tries = correctGuesses + incorrectGuesses;
        if ( correctGuesses == word.Length)//win
        {
            if (isHard == false)
            {
                tries -= 2;
            }
            stopwatch.Stop();
            for (int i=0; i<word.Length; i++)
            {
                wordContainer.GetComponentsInChildren<TextMeshProUGUI>()[i].color = Color.green;
            }
            
            if (counter < words.Count)//daca mai avem cuvinte, mai jucam
            {
                float timeTaken = (float)stopwatch.Elapsed.TotalSeconds;
                print("it took you " + tries + " tries");
                print("it took you " + stopwatch.Elapsed.TotalSeconds + " seconds");

                //calculate and add the score
                float score = GeneralFunctions.calculateScoreForLevel(timeTaken, 30, correctGuesses, tries);
                DatabaseController.AddScore(1, score);
                int feedback = DatabaseController.CalculateDeltaUpdateLevel(1);
                if (feedback != 0)
                {
                    //update the words
                    counter = 0;
                    words = DatabaseController.GetWordsForGameAndCurrentLevel(1);

                }
                Invoke("InitialiseGame", 1f);
            }
            else
            {
                print("it took you " + tries + " tries");
                print("it took you " + stopwatch.Elapsed.TotalSeconds + " seconds");
                //if I'm in freeplay, I should go back to the main menu, but realistically right now, I should only exit if I want to
                if (GameData.freeplayMode == false)
                {
                    WinScreen.SetActive(true);
                }
                else
                {
                    SceneManager.LoadScene("LevelSelectorGames");
                }
                
            }
            GameData.XP += 100;
            GeneralFunctions.UpdateXPUI(XPValue);
        }

        if (incorrectGuesses == hangmanStages.Length)//lose
        {
            for (int i = 0; i < word.Length; i++)
            {
                wordContainer.GetComponentsInChildren<TextMeshProUGUI>()[i].color = Color.red;
                wordContainer.GetComponentsInChildren<TextMeshProUGUI>()[i].text = word[i].ToString();
            }
            GameData.XP -= 100;
            GeneralFunctions.UpdateXPUI(XPValue);
            if (counter < words.Count)
            {
                float timeTaken = (float)stopwatch.Elapsed.TotalSeconds;
                //calculate and add the score
                float score = GeneralFunctions.calculateScoreForLevel(timeTaken, 20, correctGuesses, tries);
                DatabaseController.AddScore(1, score);
                int feedback = DatabaseController.CalculateDeltaUpdateLevel(1);
                if (feedback!=0)
                {
                    //update the words
                    counter = 0;
                    words = DatabaseController.GetWordsForGameAndCurrentLevel(1);

                }
                Invoke("InitialiseGame", 1f);
            }
            else
            {
                if (GameData.freeplayMode == false)
                {
                    GeneralFunctions.LoadNextGameScene();
                }
                else
                {
                    SceneManager.LoadScene("LevelSelectorGames");
                }
                //LoseScreen.SetActive(true);
            }
        }
    }

}
