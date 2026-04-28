using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// Scales an optional child icon on pointer hover / press for tactile feedback.
/// </summary>
public sealed class PressedBtn : MonoBehaviour, IPointerDownHandler, IPointerClickHandler, IPointerEnterHandler,
    IPointerExitHandler
{
    const float PressedScale = 1.2f;

    [FormerlySerializedAs("btn")]
    [SerializeField] Button button;
    [FormerlySerializedAs("myIcon")]
    [SerializeField] Transform icon;

    void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();

        if (icon == null && transform.childCount > 0)
            icon = transform.GetChild(0);
    }

    public void OnClick()
    {
        if (icon != null)
            icon.localScale = Vector3.one;
    }

    public void OnPressed()
    {
        if (icon != null)
            icon.localScale = Vector3.one * PressedScale;
    }

    public void OnPointerDown(PointerEventData eventData) => OnPressed();

    public void OnPointerClick(PointerEventData eventData) { }

    public void OnPointerEnter(PointerEventData eventData) => OnPressed();

    public void OnPointerExit(PointerEventData eventData) => OnClick();
}
