using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameSelector : MonoBehaviour
{
    public string gameName;
    public TextMeshProUGUI gameNameButtonText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameNameButtonText.text = gameName.ToString();
    }

    // Update is called once per frame
    public void OpenScene()
    {
        SceneManager.LoadScene(gameName);
    }
}
