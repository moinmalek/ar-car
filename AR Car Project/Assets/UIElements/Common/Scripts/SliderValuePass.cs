using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Updates a <see cref="Text"/> label from a normalized slider value (0–1), typically wired from UI events.
/// </summary>
public sealed class SliderValuePass : MonoBehaviour
{
    [SerializeField] Text progress;

    void Awake()
    {
        if (progress == null)
            progress = GetComponent<Text>();
    }

    public void UpdateProgress(float normalizedValue)
    {
        if (progress == null)
            return;
        progress.text = Mathf.Round(normalizedValue * 100f) + "%";
    }
}
