using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Binds serialized UI on the protocol step row prefab (avoids GameObject.Find at runtime).
/// </summary>
public sealed class StepProtocolRowView : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI stepId;
    [SerializeField] RawImage screenshot;
    [SerializeField] TextMeshProUGUI notes;
    [SerializeField] Toggle checkBox;

    /// <summary>Updates TMP and RawImage from protocol step data.</summary>
    public void Apply(int stepIndex, Step step, Texture2D nullTex)
    {
        stepId.text = $"Step {stepIndex}";
        screenshot.texture = step.ScreenshotTex == null ? nullTex : step.ScreenshotTex;
        notes.text = step.Notes;
        checkBox.isOn = step.IsDone;
    }
}
