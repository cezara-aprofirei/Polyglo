using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GrammarPoliceController : MonoBehaviour
{
    private List<string> correctSentence = new List<string>();
    private List<string> incorrectSentence = new List<string>();

    public GameObject StreakNumber;
    public GameObject XPValue;

    public GameObject WinScreen;
    public GameObject LoseScreen;

    public GameObject sentencePanel;
    public GameObject wordButtonPrefab;
    public GameObject editPanelPrefab;
    public GameObject editPanelGrid;

    public UnityEngine.UI.Button submitButton;
    public TMP_InputField wordInputField;  
    private GameObject editPanel;
    private int lastClickedWordIndex;
    private GameObject selectedWord;
    private int correctedMistakesCounter = 0;
    private int mistakesCounter = 0;
    private List<string> sentences = new List<string>();
    private int counter;
    private int tries;
    Stopwatch stopwatch = new Stopwatch();

    void Start()
    {
        if (GameData.freeplayMode == false)
        {//in a lesson
         //extract the words we need to play
            sentences = DatabaseController.GetWordsForGameAndLesson(GameData.lessonLevelSelected, 4);
            foreach (var sentence in sentences)
            {
                print(sentence);
            }
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)) // or KeyCode.KeypadEnter for the keypad's Enter key
        {
            OnSubmitButtonClicked();
        }
    }

    public void InitialiseGame()
    {
        stopwatch.Reset();
        stopwatch.Start();
        correctedMistakesCounter = 0;
        mistakesCounter = 0;
        tries = 0;
        foreach (Transform child in sentencePanel.transform)
        {
            Destroy(child.gameObject);
        }
        correctSentence.Clear();
        incorrectSentence.Clear();

        (incorrectSentence,correctSentence) = GenerateSentence();
         for (int i=0; i<correctSentence.Count; i++)
             if (correctSentence[i] != incorrectSentence[i])
                {
                    mistakesCounter++;
                }
            
        
        foreach (string word in incorrectSentence)
        {
            CreateWordButton(word);
        }
        GameObject canvas = GameObject.Find("Canvas");
        editPanel = Instantiate(editPanelPrefab, editPanelGrid.transform);
        submitButton = editPanel.GetComponentInChildren<UnityEngine.UI.Button>();
        submitButton.onClick.AddListener(OnSubmitButtonClicked);
        editPanel.SetActive(false);
        print(mistakesCounter);
        
    }

    public (List<string> incorrectSentence, List<string> correctSentence) GenerateSentence()
    {//returns a tuple formed of a string which is the incorrect sentence and a list of strings which are potential
     //fixes for the incorrect one
        string currentSentencesString = sentences[counter++];
        print(currentSentencesString);
        List<string> currSentences = currentSentencesString.Split("%").ToList();

        correctSentence = currSentences[1].Split(" ").ToList();
        incorrectSentence = currSentences[0].Split(" ").ToList();
        return (incorrectSentence, correctSentence);
    }

    public void CreateWordButton(string word)
    {
        GameObject newButton = Instantiate(wordButtonPrefab, sentencePanel.transform);

        newButton.GetComponentInChildren<TextMeshProUGUI>().text = word;

        newButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnWordButtonClick(newButton));
    }

    public void OnWordButtonClick(GameObject wordButton)
    { 
        string clickedWordText = wordButton.GetComponentInChildren<TextMeshProUGUI>().text; 
        editPanel.SetActive(true);
        wordInputField = editPanel.transform.Find("WordInputField").GetComponent<TMP_InputField>();
        wordInputField.text = clickedWordText;
        lastClickedWordIndex = wordButton.transform.GetSiblingIndex();
        selectedWord = wordButton;
        wordInputField.Select();
        wordInputField.ActivateInputField();
    }

    public void OnSubmitButtonClicked()
    {
        tries++;
        string inputText = wordInputField.text;
        //daca textul din wordInput e la fel ca cel din propozitia corecta, atunci facem butonul initial verde
        //else, facem temporar butonul rosu si ii dam shake ca in partea cealalta
        if (correctSentence[lastClickedWordIndex] == inputText)
        {
            stopwatch.Stop();
            selectedWord.GetComponentInChildren<TextMeshProUGUI>().text = inputText;

            UnityEngine.UI.Button buttonComponent = selectedWord.GetComponent<UnityEngine.UI.Button>();
            ColorBlock colorBlock = buttonComponent.colors;
            colorBlock.normalColor = Color.green;
            colorBlock.disabledColor = Color.green;
            buttonComponent.colors = colorBlock;
            buttonComponent.interactable = false;
            if (correctSentence[lastClickedWordIndex] != incorrectSentence[lastClickedWordIndex])
            {
                correctedMistakesCounter++;
            }
            
            print(correctedMistakesCounter);
            editPanel.SetActive(false);
            if (correctedMistakesCounter == mistakesCounter)
            {
                //win condition
                GameData.XP += 100;
                GeneralFunctions.UpdateXPUI(XPValue);
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
        else
        {
            ShakeButton(selectedWord);
        }
    }

    void ShakeButton(GameObject button)
    {
        float tiltAngle = 10f; 
        float duration = 0.1f; 
        int shakeTimes = 2;

        UnityEngine.UI.Button buttonComponent = selectedWord.GetComponent<UnityEngine.UI.Button>();
        ColorBlock colorBlock = buttonComponent.colors;
        colorBlock.normalColor = Color.red;
        colorBlock.disabledColor = Color.red;
        buttonComponent.colors = colorBlock;

        LeanTween.rotateZ(button, tiltAngle, duration)
            .setLoopPingPong(shakeTimes) 
            .setEase(LeanTweenType.easeInOutSine) 
            .setOnComplete(() =>
            {
                button.transform.rotation = Quaternion.identity;
                colorBlock.normalColor = Color.gray;
                colorBlock.disabledColor = Color.gray;
                buttonComponent.colors = colorBlock;
            });
    }

}
