using UnityEngine;

public class RadialMenuManager : MonoBehaviour
{
    public RadialMenuController radialMenu;

    private void Awake()
    {
        radialMenu.gameObject.SetActive(false);
    }

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

