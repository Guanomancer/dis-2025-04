using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScenes : MonoBehaviour
{
    public void GoToDeleteScene(){
        SceneManager.LoadScene("Delete");
    }

    public void GoToRotateScene()
    {
        SceneManager.LoadScene("Rotate");
    }

    public void GoToEditScene()
    {
        SceneManager.LoadScene("Edit");
    }

    public void GoToScaleScene()
    {
        SceneManager.LoadScene("Scale");
    }

    public void GoToMoveScene()
    {
        SceneManager.LoadScene("Move");
    }

    public void GoToActionMenu()
    {
        SceneManager.LoadScene("ActionMenu");
    }

    public void RestartScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    
    public void QuitApp()
    {
        Application.Quit();
        
    }

    public void GoToDemoScene()
    {
        SceneManager.LoadScene("Demo");
    }


}
