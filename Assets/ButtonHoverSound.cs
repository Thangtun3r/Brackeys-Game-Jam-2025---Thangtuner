
using UnityEngine;
using UnityEngine.EventSystems;
using FMODUnity;

public class ButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    private string hoverSoundEventPath = "event:/UIButton"; // FMOD event for hover
    private string clickSoundEventPath = "event:/UIButtonClick"; // FMOD event for click

    public void OnPointerEnter(PointerEventData eventData)
    {
        RuntimeManager.PlayOneShot(hoverSoundEventPath);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        RuntimeManager.PlayOneShot(clickSoundEventPath);
    }
}
