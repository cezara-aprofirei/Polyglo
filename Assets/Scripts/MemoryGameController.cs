using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MemoryGameController : MonoBehaviour
{
    public GameObject StreakNumber;
    public GameObject XPValue;
    private List<string> puzzleSolution = new List<string>(); //primele 5 sunt cuvintele in romana, urmatoarele 5 sunt cuvintele in engleza, perechile sunt pe pozitiile i si i%5
    private List<string> puzzleDisplayed = new List<string>();
    [SerializeField]
    private Transform puzzleField;
    [SerializeField]
    private GameObject card;
    private bool isHard = false;

    private List<Button> buttons = new List<Button>();
    private List<string> words = new List<string>();
    private int counter;

    private bool firstGuess;
    private bool secondGuess;

    private int countGuesses;
    private int countCorrectGuesses;
    private int gameGuesses;

    private int firstGuessIndex, secondGuessIndex;

    private string firstGuessPuzzle, secondGuessPuzzle;

    private UnityEngine.Color originalColor;
    private UnityEngine.Color selectedColor;

    public GameObject WinScreen;
    public GameObject LoseScreen;

    private int tries;

    Stopwatch stopwatch = new Stopwatch();

    void InitializeWords2()
    {
        string cuvinte = words[counter++];
        List<string> perechiCuvinte = cuvinte.Split("%").ToList();
        List<string> cuvinteRomana = new List<string>();
        List<string> cuvinteEngleza = new List<string>();
        foreach (var pereche in perechiCuvinte)
        {
            List<string> cuvintePereche = pereche.Split("=").ToList();
            cuvinteEngleza.Add(cuvintePereche[0]);
            cuvinteRomana.Add(cuvintePereche[1]);
        }

        foreach (var cuvant in cuvinteRomana)
        {
            puzzleSolution.Add(cuvant);
        }

        foreach (var cuvant in cuvinteEngleza)
        {
            puzzleSolution.Add(cuvant);
        }

        foreach (var cuvant in puzzleSolution)
        {
            print(cuvant.ToString());
        }

    }

    private void Awake()
    {
        for(int i=0; i<10; i++) {
            GameObject _card = Instantiate(card);
            _card.name = "" + i;
            _card.transform.SetParent(puzzleField, false);
        }
        
    }
    void Start()
    {
        counter = 0;
        if (GameData.freeplayMode == false)
        {//in a lesson
         //extract the words we need to play
            words = DatabaseController.GetWordsForGameAndLesson(GameData.lessonLevelSelected, 3);
        }
        else
        {
            //in freeplaymode
        }
        GeneralFunctions.UpdateStreakUI(StreakNumber);
        GeneralFunctions.UpdateXPUI(XPValue);
        InitialiseGame();
        originalColor = card.GetComponent<Image>().color;
        ColorUtility.TryParseHtmlString("#825656", out selectedColor);
        gameGuesses = puzzleDisplayed.Count / 2;
        AddListenersOnCards();
    }

    private void InitialiseGame()
    {
        stopwatch.Reset();
        stopwatch.Start();
        //reset data
        tries = 0;
        firstGuess = secondGuess = false;
        firstGuessIndex = -1;
        secondGuessIndex = -1;
        firstGuessPuzzle = secondGuessPuzzle = null;
        countCorrectGuesses = 0;
        countGuesses = 0;
        //reactivate all buttons
        foreach (Button button in buttons)
        {
            button.interactable = true;
            button.GetComponent<Image>().color = originalColor;
        }

        InitializeWords2();
        if (isHard == true)
        {
            GetButtonsAdvanced();
            AddpuzzleDisplayed();
            Shuffle(puzzleDisplayed);
        }
        else
        {
            AddpuzzleDisplayed();
            Shuffle(puzzleDisplayed);
            GetButtonsBeginner();
        }
        
    }

    public void GetButtonsAdvanced()
    {
            GameObject[] objects = GameObject.FindGameObjectsWithTag("cardButton");
            for (int i = 0; i < objects.Length; i++)
            {
                buttons.Add(objects[i].GetComponent<Button>());
                buttons[i].transform.Find("backImage").gameObject.SetActive(true);
                buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = "";
                
            }
    }
    public void GetButtonsBeginner()
    {
            GameObject[] objects = GameObject.FindGameObjectsWithTag("cardButton");
            for (int i = 0; i < objects.Length; i++)
            {
                buttons.Add(objects[i].GetComponent<Button>());
                buttons[i].transform.Find("backImage").gameObject.SetActive(false);
                buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = puzzleDisplayed[i];

                ColorBlock cb = buttons[i].colors;
                cb.disabledColor = UnityEngine.Color.green; 
                buttons[i].colors = cb;
        }
            
    }

    void AddpuzzleDisplayed() 
    {
        puzzleDisplayed.AddRange(puzzleSolution);
    }

    void AddListenersOnCards()
    {
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() => PickCard(isHard));
        }
    }

    public void PickCard(bool isHard)
    {   
        if (isHard == true)
        {
            if (!firstGuess && (firstGuessIndex = int.Parse(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name)) < 5)
            {
                firstGuess = true;

                firstGuessPuzzle = puzzleDisplayed[firstGuessIndex];
                buttons[firstGuessIndex].GetComponentInChildren<TextMeshProUGUI>().text = firstGuessPuzzle;
                buttons[firstGuessIndex].transform.Find("backImage").gameObject.SetActive(false);

            }
            else if (!secondGuess && (secondGuessIndex = int.Parse(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name)) >= 5)
            {
                secondGuess = true;

                secondGuessPuzzle = puzzleDisplayed[secondGuessIndex];
                buttons[secondGuessIndex].GetComponentInChildren<TextMeshProUGUI>().text = secondGuessPuzzle;
                buttons[secondGuessIndex].transform.Find("backImage").gameObject.SetActive(false);

            }
            if (firstGuess && secondGuess)
            {
                StartCoroutine(checkPuzzleMatch());
            }
        }
        else//beginner
        {
            if (!firstGuess && (firstGuessIndex = int.Parse(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name)) < 5)
            {
                firstGuess = true;
                buttons[firstGuessIndex].GetComponent<Image>().color = selectedColor;

                firstGuessPuzzle = puzzleDisplayed[firstGuessIndex];

                buttons[firstGuessIndex].GetComponentInChildren<TextMeshProUGUI>().text = firstGuessPuzzle;

                buttons[firstGuessIndex].transform.Find("backImage").gameObject.SetActive(false);

            }
            else if (!secondGuess && (secondGuessIndex = int.Parse(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name)) >= 5)
            {
                secondGuess = true;
                buttons[secondGuessIndex].GetComponent<Image>().color = selectedColor;

                secondGuessPuzzle = puzzleDisplayed[secondGuessIndex];
                buttons[secondGuessIndex].GetComponentInChildren<TextMeshProUGUI>().text = secondGuessPuzzle;
                buttons[secondGuessIndex].transform.Find("backImage").gameObject.SetActive(false);

            }
            if (firstGuess && secondGuess)
            {
                StartCoroutine(checkPuzzleMatch());
            }
        }
        
    }

    private System.Collections.IEnumerator checkPuzzleMatch()
    {
        tries++;
        if (isHard == true)
        {
            yield return new WaitForSeconds(0.5f);
            //pe ce pozitie e firstGuessPuzzle in puzzleSolution? si iau index urile alea si le compar
            if (puzzleSolution.IndexOf(firstGuessPuzzle) == puzzleSolution.IndexOf(secondGuessPuzzle) % 5)
            {
                yield return new WaitForSeconds(0.2f);
                buttons[firstGuessIndex].interactable = false;
                buttons[secondGuessIndex].interactable = false;

                UnityEngine.Color originalColor = buttons[firstGuessIndex].colors.normalColor;
                ColorBlock cb1 = buttons[firstGuessIndex].colors;
                cb1.disabledColor = originalColor;
                buttons[firstGuessIndex].colors = cb1;

                ColorBlock cb2 = buttons[secondGuessIndex].colors;
                cb2.disabledColor = originalColor;
                buttons[secondGuessIndex].colors = cb2;

                CheckGameFinished();
            }
            else
            {
                buttons[firstGuessIndex].transform.Find("backImage").gameObject.SetActive(true);
                buttons[secondGuessIndex].transform.Find("backImage").gameObject.SetActive(true);
                buttons[firstGuessIndex].GetComponentInChildren<TextMeshProUGUI>().text = "";
                buttons[secondGuessIndex].GetComponentInChildren<TextMeshProUGUI>().text = "";

            }
            firstGuess = secondGuess = false;
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);  
            if (puzzleSolution.IndexOf(firstGuessPuzzle) == puzzleSolution.IndexOf(secondGuessPuzzle) % 5)
            {
                yield return new WaitForSeconds(0.2f);
                buttons[firstGuessIndex].interactable = false;
                buttons[secondGuessIndex].interactable = false;

                CheckGameFinished();
            }
            else
            {
                buttons[firstGuessIndex].GetComponent<Image>().color = UnityEngine.Color.red;
                buttons[secondGuessIndex].GetComponent<Image>().color = UnityEngine.Color.red;
                yield return new WaitForSeconds(.5f);
                buttons[firstGuessIndex].GetComponent<Image>().color = originalColor;
                buttons[secondGuessIndex].GetComponent<Image>().color = originalColor;
            }
            firstGuess = secondGuess = false;
            yield return new WaitForSeconds(0.5f);
        }
    }

    void CheckGameFinished()
    {
        stopwatch.Stop();
        print(countCorrectGuesses);
        countCorrectGuesses++;

        if (countCorrectGuesses == gameGuesses)
        {
            if (counter < words.Count)
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
            GameData.XP += 100;
            GeneralFunctions.UpdateXPUI(XPValue);
            print("game finish");
        }
    }

    void Shuffle(List<string> list)
{
    // Shuffle the first 5 elements
    int firstHalfCount = Mathf.Min(5, list.Count);  // Ensure we don't go out of bounds
    for (int i = 0; i < firstHalfCount; i++)
    {
        string temp = list[i];
        int randomIndex = Random.Range(i, firstHalfCount); // Randomize within the first 5 elements
        list[i] = list[randomIndex];
        list[randomIndex] = temp;
    }

    // Shuffle the last 5 elements
    int lastHalfStart = Mathf.Max(0, list.Count - 5);  // Start at 5 elements before the end
    for (int i = lastHalfStart; i < list.Count; i++)
    {
        string temp = list[i];
        int randomIndex = Random.Range(i, list.Count); // Randomize within the last 5 elements
        list[i] = list[randomIndex];
        list[randomIndex] = temp;
    }
}

}
