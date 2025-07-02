using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class WordButton : MonoBehaviour
{
    public GameObject StreakNumber;
    public GameObject XPValue;
    private List<string> correctSentence = new List<string>();
    public List<string> playerSentence = new List<string>();
    public List<string> sentences = new List<string>();
    public List<string> scrambledWordsList = new List<string>();
    public GameObject sentencePanel; // unde bagi cuvintele pentru propozitie
    public GameObject wordsPanel; // unde stau cuvintele din propozitie
    public GameObject wordButtonPrefab;
    private int counter;
    public GameObject WinScreen;
    public GameObject LoseScreen;
    private int tries;
    Stopwatch stopwatch = new Stopwatch();

    private void Start()
    {
        if (GameData.freeplayMode == false)
        {//in a lesson
            //extract the words we need to play
            sentences = DatabaseController.GetWordsForGameAndLesson(GameData.lessonLevelSelected, 6);
            print(sentences);
        }
        else
        {
            //in freeplaymode
        }
        GeneralFunctions.UpdateStreakUI(StreakNumber);
        GeneralFunctions.UpdateXPUI(XPValue);
        counter = 0;
        InitialiseGame();
    }

    public void InitialiseGame()
    {
        stopwatch.Reset();
        stopwatch.Start();
        tries = 0;
        // Clear previous game state
        foreach (Transform child in wordsPanel.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in sentencePanel.transform)
        {
            Destroy(child.gameObject);
        }
        playerSentence.Clear();
        scrambledWordsList.Clear();

        // Reset sentence
        correctSentence = GenerateSentence();
        ScrambleWords();
        foreach (string word in scrambledWordsList)
        {
            CreateWordButton(word);
        }
    }

    public List<string> GenerateSentence()
    {
        //take the next sentence from sentences and break it into words and return it
        string currentSentence = sentences[counter++];
        return currentSentence.Split(" ").ToList();
    }

    public void ScrambleWords()
    {
        scrambledWordsList = correctSentence.OrderBy(word => Random.value).ToList();
    }

    public void CreateWordButton(string word)
    {   
        GameObject newButton = Instantiate(wordButtonPrefab, wordsPanel.transform);

        newButton.GetComponentInChildren<TextMeshProUGUI>().text = word;

        newButton.GetComponent<Button>().onClick.AddListener(() => OnWordButtonClick(newButton));
    }

    public void OnWordButtonClick(GameObject wordButton)
    {
        if (wordButton.transform.parent == sentencePanel.transform)
        {
            //move it from sentence panel to words panel and remove it from the player sentence
            wordButton.transform.SetParent(wordsPanel.transform);
            playerSentence.Remove(wordButton.GetComponentInChildren<TextMeshProUGUI>().text);

        }
        else
        {
            wordButton.transform.SetParent(sentencePanel.transform);
            playerSentence.Add(wordButton.GetComponentInChildren<TextMeshProUGUI>().text);
        }

        print("player sentence count" + playerSentence.Count);
        print("correct sentence count" + correctSentence.Count);


        if (playerSentence.Count == correctSentence.Count)
        {
            CheckSentence();
        }
    }

    public void CheckSentence()
    {
        tries++;
        
        if (!correctSentence.SequenceEqual(playerSentence))
        {
            //shake the words and put them back
            foreach(Transform child in sentencePanel.transform)
            {
                GameObject wordButton = child.gameObject;
                ShakeButton(wordButton, () => MoveButtonsToBottom(wordButton));
                playerSentence.Clear();

            }
        }
        else//win condition
        {
            stopwatch.Stop();
            foreach (Transform child in sentencePanel.transform)
            {
                GameObject wordButton = child.gameObject;
                Button buttonComponent = wordButton.GetComponent<Button>();

                ColorBlock colorBlock = buttonComponent.colors;
                colorBlock.normalColor = Color.green;  
                colorBlock.disabledColor = Color.green;
                buttonComponent.colors = colorBlock;

                buttonComponent.interactable = false;

            }
            //Invoke("InitialiseGame", 3f); //wait 3 seconds and initialise a new game
            GameData.XP += 100;
            GeneralFunctions.UpdateXPUI(XPValue);
            print("counter is " + counter);
            if (counter < sentences.Count)
            {
                print("it took you " + tries + " tries");
                print("it took you " + stopwatch.Elapsed.TotalSeconds + " seconds");
                Invoke("InitialiseGame", 1f);
            }
            else
            {
                print("it took you " + tries + " tries");
                print("it took you " + stopwatch.Elapsed.TotalSeconds + " seconds");
                WinScreen.SetActive(true);
            }
        }
    }

    public void MoveButtonsToBottom(GameObject wordButton)
    {
        wordButton.transform.SetParent(wordsPanel.transform);
    }
    void ShakeButton(GameObject button, System.Action onComplete)
    {
        float tiltAngle = 10f; // How much to tilt (in degrees)
        float duration = 0.1f; // Speed of each tilt
        int shakeTimes = 2; // Number of tilts

        // Rotate left and right with PingPong effect
        LeanTween.rotateZ(button, tiltAngle, duration)
            .setLoopPingPong(shakeTimes) // Moves back and forth
            .setEase(LeanTweenType.easeInOutSine) // Smooth movement
            .setOnComplete(() =>
            {
                button.transform.rotation = Quaternion.identity; // Reset rotation
                onComplete?.Invoke(); // Call the next function after shake
            });
    }


}
