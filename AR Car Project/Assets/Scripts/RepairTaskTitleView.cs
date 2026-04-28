using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Binds serialized UI on the repair-task title row prefab (avoids GameObject.Find at runtime).
/// </summary>
public sealed class RepairTaskTitleView : MonoBehaviour
{
    [SerializeField] Text titleText;

    public Text TitleText => titleText;
}
