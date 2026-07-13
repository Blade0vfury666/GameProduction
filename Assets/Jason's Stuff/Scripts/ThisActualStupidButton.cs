using UnityEngine;
using UnityEngine.EventSystems;

public class DebugButton : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Button was physically clicked!");
    }
}