using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoryController : MonoBehaviour
{
    public GameObject textContainer; 
    public GameObject blankPrefab; 
    private string passage = "The [gap] is shining and the [gap] is blue.";
    private List<Button> gapButtons = new List<Button>();
    void Start()
    {
        GenerateTextWithGaps();
        //GameObject newButton = Instantiate(blankPrefab, textContainer.transform);
        
        

    }

    void GenerateTextWithGaps()
    {
        
        string[] words = passage.Split(' '); // Split text into words

        foreach (string word in words)
        {
            if (word == "[gap]") // Replace marker with a blank
            {
                GameObject blank = Instantiate(blankPrefab, textContainer.transform);
                Button blankButton = blank.GetComponent<Button>();
                blankButton.GetComponentInChildren<TextMeshProUGUI>().text = "___"; // Placeholder text
                gapButtons.Add(blankButton);
            }
            else // Add regular words as TMP Text
            {
                GameObject textObj = new GameObject("Word", typeof(TextMeshProUGUI));
                TextMeshProUGUI tmp = textObj.GetComponent<TextMeshProUGUI>();
                tmp.text = word; // Add space after words
                tmp.fontSize = 36; // Adjust font size if needed
                tmp.transform.SetParent(textContainer.transform, false);
            }
        }
    }
}
