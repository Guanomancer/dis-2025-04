using UnityEngine;

public class UrlButton : MonoBehaviour
{
    [SerializeField] private string _url;

    public void OpenUrl()
    {
        if (string.IsNullOrEmpty(_url))
        {
            Debug.LogWarning("URL is not set.", this);
            return;
        }

        Application.OpenURL(_url);
    }

    public void OpenUrl(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            Debug.LogWarning("URL is not set.", this);
            return;
        }
        Application.OpenURL(url);
    }
}
