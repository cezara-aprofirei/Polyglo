using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToMainMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void GoToLevelSelectorGamesScene()
    {
        SceneManager.LoadScene("LevelSelectorGames");
    }

    public void GoToWelcomeScreenScene()
    {
        SceneManager.LoadScene("WelcomeScreen");
    }

    public void GoToLessonSelectorScreen()
    {
        SceneManager.LoadScene("LessonSelector");
    }


   
}
