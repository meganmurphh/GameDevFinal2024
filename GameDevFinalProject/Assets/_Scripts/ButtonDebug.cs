using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseButtonDebug : MonoBehaviour
{
    public Canvas canvas;
    public Button pauseButton;

    void Start()
    {
        if (canvas == null || pauseButton == null)
        {
            Debug.LogError("Canvas or PauseButton is not assigned.");
            return;
        }

        // Check if Graphic Raycaster is attached to Canvas
        if (canvas.GetComponent<GraphicRaycaster>() == null)
        {
            Debug.LogError("Graphic Raycaster component is missing on Canvas.");
        }

        // Check if EventSystem is present in the scene
        if (FindObjectOfType<EventSystem>() == null)
        {
            Debug.LogError("EventSystem is missing in the scene.");
        }

        // Check if Button has Raycast Target enabled
        if (!pauseButton.GetComponent<Image>().raycastTarget)
        {
            Debug.LogError("Raycast Target is not enabled on the PauseButton.");
        }

        // Check if Canvas is interactable
        CanvasGroup canvasGroup = canvas.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            if (!canvasGroup.interactable || !canvasGroup.blocksRaycasts)
            {
                Debug.LogError("CanvasGroup is not interactable or not blocking raycasts.");
            }
        }

        // Add a listener to the button to log clicks
        pauseButton.onClick.AddListener(() => Debug.Log("Pause button clicked."));
    }
}
