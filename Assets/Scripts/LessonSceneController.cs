using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LessonSceneController : MonoBehaviour
{
    public GameObject Title;
    public GameObject Subtitle;
    public GameObject Theory;
    public GameObject ExtraButton;
    public GameObject ExtraButtonTitle;
    public GameObject ExtraButtonText;
    public GameObject GoToPractice;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Lesson currentLesson = DatabaseController.GetLessonData(GameData.lessonLevelSelected);
        
        Title.GetComponent<TextMeshProUGUI>().text = currentLesson.Title;
        Subtitle.GetComponent<TextMeshProUGUI>().text = currentLesson.Subtitle;
        Theory.GetComponent<TextMeshProUGUI>().text = currentLesson.Theory;
        ExtraButton.GetComponent<TextMeshProUGUI>().text = currentLesson.ExtraButtonTitle;
        ExtraButtonTitle.GetComponent<TextMeshProUGUI>().text = currentLesson.ExtraButtonTitle;
        ExtraButtonText.GetComponent<TextMeshProUGUI>().text = currentLesson.ExtraButtonText;
        
    }

    public void OnGoToPracticeButtonPressed()
    {
        int ongoingLesson = GameData.lessonLevelSelected;
        //i need to call the scenes for the games that need to be completed for this ongoing lesson
        GameData.gameScenes = new Queue<string>();
        if (ongoingLesson == 1)//eventual aici facem un query si vedem lectia aia ce jocuri are asociate si facem queue in functie de asta
        {
            GameData.gameScenes.Enqueue("Hangman");
            GameData.gameScenes.Enqueue("GrammarPolice");
            GameData.gameScenes.Enqueue("WordOrder");
            GameData.gameScenes.Enqueue("MemoryGame");
            GameData.gameScenes.Enqueue("PronouncePro");
            GameData.gameScenes.Enqueue("Taboo");
            
        }
        GeneralFunctions.LoadNextGameScene();
    }
    

}
