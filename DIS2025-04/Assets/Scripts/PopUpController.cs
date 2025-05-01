using UnityEngine;

public class PopUpController: MonoBehaviour
{
    public GameObject cheatSheetPanel;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            cheatSheetPanel.SetActive(!cheatSheetPanel.activeSelf);
        }

        //another way to activate the cheat sheet
        /*
        if (detectedGesture == "Help")
        {
            cheatSheetPanel.SetActive(true);
        }
        */

    }
}
