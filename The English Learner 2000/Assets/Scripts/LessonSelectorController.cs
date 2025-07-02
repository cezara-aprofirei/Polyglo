using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LessonSelectorController : MonoBehaviour
{
    public GameObject StreakNumber;
    public GameObject XPValue;
    public GameObject lessonButton;
    public GameObject ScrollMenuButtonsGrid;
    void Start()
    {
        GeneralFunctions.UpdateStreakUI(StreakNumber);
        GeneralFunctions.UpdateXPUI(XPValue);
        DrawEnrolledLanguageButtons();
    }

    public void DrawEnrolledLanguageButtons()
    {
        foreach (Transform child in ScrollMenuButtonsGrid.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 1; i <= GameData.numberOfLessons; i++) //replace with the amount of lessons in the database
        {
            int lessonNumber = i;
            GameObject newLessonButton = Instantiate(lessonButton, ScrollMenuButtonsGrid.transform);
            newLessonButton.GetComponentInChildren<TextMeshProUGUI>().text = lessonNumber.ToString();
            newLessonButton.GetComponent<Button>().onClick.AddListener(() => OnLevelSelected(lessonNumber));
        }
    }

    public void OnLevelSelected(int lessonNumber)
    {
        //go to the screen with the according lesson number and put that on the screen
        GameData.lessonLevelSelected = lessonNumber;
        SceneManager.LoadScene("LessonScene");
    }


}
