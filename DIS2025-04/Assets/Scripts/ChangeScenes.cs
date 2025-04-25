using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScenes : MonoBehaviour
{
    public void GoToDeleteScene(){
        SceneManager.LoadScene("Delete");
    }

    public void GoToActionMenu()
    {
        SceneManager.LoadScene("ActionMenu");
    }
    
    
}
