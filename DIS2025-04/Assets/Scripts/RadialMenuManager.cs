using UnityEngine;

public class RadialMenuManager : MonoBehaviour
{
    public RadialMenuController radialMenu;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (radialMenu.gameObject.activeSelf)
                radialMenu.CloseMenu();
            else
                radialMenu.OpenMenu();
        }
    }
}

