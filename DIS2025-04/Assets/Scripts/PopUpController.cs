using UnityEngine;

public class PopUpController : MonoBehaviour
{
    public GameObject cheatSheetPanel;
    public GameObject deleteHand;
    public GameObject rotateHand;
    public GameObject scaleHand;
    public GameObject selectHand;
    public GameObject moveHand;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            bool newState = !cheatSheetPanel.activeSelf;
            cheatSheetPanel.SetActive(newState);
            //turning hands off when pop up is off
            deleteHand.SetActive(newState);
            rotateHand.SetActive(newState);
            scaleHand.SetActive(newState);
            selectHand.SetActive(newState);
            moveHand.SetActive(newState);
        }

        // another way to activate the cheat sheet
        /*
        if (detectedGesture == "Help")
        {
            cheatSheetPanel.SetActive(true);
            debugDots.SetActive(true);
        }
        */
    }
}

