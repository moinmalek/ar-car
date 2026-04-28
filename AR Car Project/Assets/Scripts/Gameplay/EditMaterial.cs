using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Swaps body material between opaque (main view) and transparent fade (configurator).
/// </summary>
public sealed class EditMaterial : MonoBehaviour
{
    [SerializeField] Button openMainPageBtn;
    [SerializeField] Button openConfigPageBtn;

    [SerializeField] Material material;

    void Start()
    {
        ChangeToFade();
        openMainPageBtn.onClick.AddListener(ChangeToOpaque);
        openConfigPageBtn.onClick.AddListener(ChangeToFade);
    }

    /// <summary>Opaque standard shader setup for main navigation.</summary>
    public void ChangeToOpaque()
    {
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        material.SetInt("_ZWrite", 1);
        material.DisableKeyword("_ALPHATEST_ON");
        material.DisableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = -1;
        material.SetFloat("_Mode", 0);

        material.SetFloat("_Metallic", 0.7f);
        material.SetFloat("_Glossiness", 0.5f);
    }

    /// <summary>Alpha-blended setup for overlay/configurator visibility.</summary>
    public void ChangeToFade()
    {
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 3000;
        material.SetFloat("_Mode", 2);

        material.SetFloat("_Metallic", 0.2f);
        material.SetFloat("_Glossiness", 0.4f);
    }
}
