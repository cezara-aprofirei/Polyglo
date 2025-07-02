using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using TMPro;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEngine.UI;

public class WhisperClient : MonoBehaviour
{
    public GameObject StreakNumber;
    public GameObject XPValue;
    public AudioRecorder recorder;
    public TMP_Text resultText;
    public TMP_Text textToPronounce;
    private string targetSentence;
    private int nrOfWords = 0;

    public Button micButton; // Reference to the Button component
    public Sprite micIcon;   // Mic icon sprite
    public Sprite stopIcon;
    private Image buttonImage;

    private bool isRecording = false;//arata microfonul
    private List<string> words = new List<string>();
    private int counter;
    private float accuracy;

    public GameObject WinScreen;
    public GameObject LoseScreen;

    private int tries;

    private void Start()
    {
        if (GameData.freeplayMode == false)
        {//in a lesson
            //extract the words we need to play
            words = DatabaseController.GetWordsForGameAndLesson(GameData.lessonLevelSelected, 5);
        }
        else
        {
            //in freeplaymode
        }
        GeneralFunctions.UpdateStreakUI(StreakNumber);
        GeneralFunctions.UpdateXPUI(XPValue);
        buttonImage = micButton.GetComponent<Image>();
        counter = 0;
        Initialise();
    }

    private string GenerateWord()
    {
        return words[counter++].ToString();
    }

    public void OnMicButtonClicked()
    {
        if (isRecording)//arata patratul
        {
            StopAndTranscribe();
        }
        else//arata microfonul
        {
            StartRecording();
        }
    }

    void Initialise()
    {
        tries = 0;
        resultText.text = "";
        targetSentence = GenerateWord();
        textToPronounce.text = targetSentence;
        nrOfWords = targetSentence.Split(" ").Length;
        buttonImage.sprite = micIcon;
    }
    public void StartRecording()
    {
        buttonImage.sprite = stopIcon;
        isRecording = true;
        recorder.StartRecording();
        
    }
    public void StopAndTranscribe()
    {
        buttonImage.sprite = micIcon;
        isRecording = false;
        StartCoroutine(SendAudio());
    }

    IEnumerator SendAudio()
    {
        print("number of words" + nrOfWords);
        recorder.StopRecording();
        string path = recorder.GetAudioPath();

        byte[] audioData = File.ReadAllBytes(path);
        WWWForm form = new WWWForm();
        form.AddBinaryData("audio", audioData, "recorded.wav", "audio/wav");

        UnityWebRequest www = UnityWebRequest.Post("http://localhost:5000/transcribe", form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            tries++;
            string json = www.downloadHandler.text;
            string transcription = JsonUtility.FromJson<TranscriptionResponse>(json).transcription;
            if (nrOfWords == 1)
            {
                ShowFeedback1(transcription);
                //TODO : feedback for the pronounciation window pop-up, and also make an evaluation like let them go to the next one if
                //it's over 80% accuracy or sth like that
                if (accuracy - 80 >= float.Epsilon)//accept it
                {
                    if (counter < words.Count)
                    {
                        Invoke("Initialise", 1f);
                    }
                    else
                    {
                        yield return new WaitForSeconds(2f);
                        WinScreen.SetActive(true);
                    }
                    GameData.XP += 100;
                    GeneralFunctions.UpdateXPUI(XPValue);
                }
            }
            else
            {
                ShowFeedback2(transcription);
                if (accuracy - 80 >= float.Epsilon)
                {
                    if (counter < words.Count)
                    {
                        Invoke("Initialise", 1f);
                    }
                    else
                    {
                        yield return new WaitForSeconds(2f);
                        WinScreen.SetActive(true);
                    }
                    GameData.XP -= 100;
                    GeneralFunctions.UpdateXPUI(XPValue);
                }  
            }
        }
        else
        {
            Debug.LogError("Whisper error: " + www.error);
        }
    }

    void ShowFeedback1(string actual)
    {
        float similarity = ComputeSimilarity(targetSentence.ToLower().Trim(), actual.ToLower().Trim());
        accuracy = similarity * 100;
        resultText.text = $"You said: \"{actual}\"\nAccuracy: {accuracy:0}%";
    }

    void ShowFeedback2(string actual)
    {
        actual = actual.Trim();
        List<string> targetWords = targetSentence.ToLower().Split(' ').ToList<string>();
        List<string> actualWords = actual.ToLower().Split(' ').ToList<string>();

        foreach (string word in targetWords)
        {
            print(word);
        }

        foreach (string word in actualWords)
        {

            print(word);
        }

        List<string> mispronouncedWords = new List<string>();
        print("actual word for 0 " +actualWords[0]);
        for (int i = 0; i < targetWords.Count; i++)
        {
            if (i >= actualWords.Count || Soundex(targetWords[i]) != Soundex(actualWords[i]))
            {
                //print("target word soundex " + Soundex(targetWords[i]));
                //print("actual word soundex " + Soundex(actualWords[i]));
                mispronouncedWords.Add(targetWords[i]);
            }
        }
        accuracy = ComputeSimilarity(targetSentence.ToLower().Trim(), actual.ToLower().Trim()) * 100;
        string feedback = "You said: \"" + actual + "\"\nAccuracy: " +  accuracy.ToString("0") + "%";

        if (mispronouncedWords.Count > 0)
        {
            feedback += "\nMispronounced words: " + string.Join(", ", mispronouncedWords);
            feedback += "\nTry pronouncing these words more clearly!";
        }
        else
        {
            feedback += "\nGreat job! No mispronounced words.";
        }

        resultText.text = feedback;
    }

    float ComputeSimilarity(string a, string b)
    {
        print("Target sentence : " + a);
        print("Your sentence : " + b);
        int distance = LevenshteinDistance(a, b);
        return 1.0f - (float)distance / Mathf.Max(a.Length, b.Length);
    }

    int LevenshteinDistance(string s, string t)
    {
        var dp = new int[s.Length + 1, t.Length + 1];
        for (int i = 0; i <= s.Length; i++) dp[i, 0] = i;
        for (int j = 0; j <= t.Length; j++) dp[0, j] = j;

        for (int i = 1; i <= s.Length; i++)
        {
            for (int j = 1; j <= t.Length; j++)
            {
                int cost = (s[i - 1] == t[j - 1]) ? 0 : 1;
                dp[i, j] = Mathf.Min(
                    dp[i - 1, j] + 1,
                    dp[i, j - 1] + 1,
                    dp[i - 1, j - 1] + cost
                );
            }
        }
        return dp[s.Length, t.Length];
    }

    [System.Serializable]
    class TranscriptionResponse
    {
        public string transcription;
    }

    public static string Soundex(string word)
    {
        print("in soundex " + word);
        if (string.IsNullOrEmpty(word)) return string.Empty;

        word = word.ToUpper();

        char[] soundexMapping = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        StringBuilder soundexCode = new StringBuilder(word[0].ToString());

        Dictionary<char, char> letterToDigit = new Dictionary<char, char>()
    {
        {'A', '0'}, {'B', '1'}, {'C', '2'}, {'D', '3'}, {'E', '0'}, {'F', '1'},
        {'G', '2'}, {'H', '0'}, {'I', '0'}, {'J', '1'}, {'K', '2'}, {'L', '4'},
        {'M', '5'}, {'N', '5'}, {'O', '0'}, {'P', '1'}, {'Q', '2'}, {'R', '6'},
        {'S', '2'}, {'T', '3'}, {'U', '0'}, {'V', '1'}, {'W', '0'}, {'X', '2'},
        {'Y', '0'}, {'Z', '2'}
    };

        char prevCode = '0';

        for (int i = 1; i < word.Length; i++)
        {
            if (letterToDigit.TryGetValue(word[i], out char code))
            {
                if (code != prevCode)
                {
                    soundexCode.Append(code);
                    prevCode = code;
                }
            }
        }

        // Pad with zeros if necessary (Soundex is a 4-character code)
        return soundexCode.ToString().PadRight(4, '0').Substring(0, 4);
    }

}
