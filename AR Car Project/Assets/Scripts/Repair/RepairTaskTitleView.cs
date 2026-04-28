using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Binds serialized UI on the repair-task title row prefab (avoids GameObject.Find at runtime).
/// </summary>
public sealed class RepairTaskTitleView : MonoBehaviour
{
    [SerializeField] Text titleText;

    /// <summary>Legacy UI Text bound in the prefab.</summary>
    public Text TitleText => titleText;
}
