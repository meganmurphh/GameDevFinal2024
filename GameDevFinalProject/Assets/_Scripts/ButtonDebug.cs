using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonDiagnostics : MonoBehaviour
{
    public Button spawnBirdButton;
    public GameObject eventSystem;

    void Start()
    {
        CheckButton();
        CheckEventSystem();
    }

    void CheckButton()
    {
        if (spawnBirdButton == null)
        {
            Debug.LogError("SpawnBirdButton is not assigned in the inspector.");
            return;
        }

        Debug.Log("SpawnBirdButton is assigned.");

        // Check if Button component exists
        if (spawnBirdButton.GetComponent<Button>() == null)
        {
            Debug.LogError("SpawnBirdButton does not have a Button component.");
            return;
        }

        Debug.Log("SpawnBirdButton has a Button component.");

        // Check if onClick event has listeners
        if (spawnBirdButton.onClick.GetPersistentEventCount() == 0)
        {
            Debug.LogError("SpawnBirdButton does not have any onClick listeners.");
        }
        else
        {
            Debug.Log("SpawnBirdButton has onClick listeners.");
            for (int i = 0; i < spawnBirdButton.onClick.GetPersistentEventCount(); i++)
            {
                Debug.Log($"Listener {i}: Method = {spawnBirdButton.onClick.GetPersistentMethodName(i)}, Target = {spawnBirdButton.onClick.GetPersistentTarget(i)}");
            }
        }

        // Check if the button is interactable
        if (!spawnBirdButton.interactable)
        {
            Debug.LogError("SpawnBirdButton is not interactable.");
        }
        else
        {
            Debug.Log("SpawnBirdButton is interactable.");
        }
    }

    void CheckEventSystem()
    {
        if (eventSystem == null)
        {
            Debug.LogError("EventSystem is not assigned in the inspector.");
            return;
        }

        // Check if EventSystem component exists
        if (eventSystem.GetComponent<EventSystem>() == null)
        {
            Debug.LogError("EventSystem GameObject does not have an EventSystem component.");
        }
        else
        {
            Debug.Log("EventSystem is present in the scene.");
        }
    }
}
