using UnityEngine;

public class LevelSelectorController : MonoBehaviour
{
    public GameObject StreakNumber;
    public GameObject XPValue;
    private void Start()
    {
        GeneralFunctions.UpdateStreakUI(StreakNumber);
        GeneralFunctions.UpdateXPUI(XPValue);
    }
    
}
