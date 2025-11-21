using System.Collections.Generic;
using UnityEngine;

public class TutorialDialog : MonoBehaviour
{
    public List<GameObject> windows;

    public int currentWindow = 0;

    private void OnEnable()
    {
        foreach (Transform child in transform)
        {
            windows.Add(child.gameObject);
            child.gameObject.SetActive(false);
        }

        windows[currentWindow].SetActive(true);
    }

    public void SetActiveWindow(int index)
    {
        if (index < 0 || index >= windows.Count)
        {
            Debug.LogError("Invalid window index");
            return;
        }

        windows[currentWindow].SetActive(false);
        currentWindow = index;
        windows[index].SetActive(true);
    }

    public void Finish()
    {
        gameObject.SetActive(false);
    }
}