using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Numerics;

public class RadialMenuController : MonoBehaviour
{
    [Header("Button Settings")]
    public GameObject buttonPrefab;
    public string[] labels;
    public float radius = 100f;

    void Start()
    {
        GenerateMenu();

    }

    public void GenerateMenu()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        GenerateButtons();
    }

    void GenerateButtons()
    {
        int count = labels.Length;
        float angleStep = 360f / count;

        for (int i = 0; i < count; i++)
        {

            GameObject newButton = Instantiate(buttonPrefab, transform);
            RectTransform rectTransform = newButton.GetComponent<RectTransform>();


            float angle = angleStep * i * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            rectTransform.anchoredPosition = new UnityEngine.Vector2(x, y);


            TMP_Text text = newButton.GetComponentInChildren<TMP_Text>();
            if (text != null)
                text.text = labels[i];


            int index = i;
            newButton.GetComponent<Button>().onClick.AddListener(() => OnButtonClick(index));
        }
    }

    void OnButtonClick(int index)
    {
        CloseMenu();

    }

    public void OpenMenu()
    {
        gameObject.SetActive(true);

    }

    public void CloseMenu()
    {
        gameObject.SetActive(false);
    }
}


