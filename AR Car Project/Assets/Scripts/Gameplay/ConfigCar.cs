using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// Selects car assembly variants by showing/hiding inspector-assigned roots (no hierarchy searches).
/// </summary>
public sealed class ConfigCar : MonoBehaviour
{
    [Serializable]
    sealed class VariantRoot
    {
        [FormerlySerializedAs("variantName")]
        [SerializeField] string _variantName;
        [FormerlySerializedAs("root")]
        [SerializeField] GameObject _root;

        public string VariantName => _variantName;
        public GameObject Root => _root;
    }

    [FormerlySerializedAs("propertyText")]
    [SerializeField] Text _propertyText;
    [FormerlySerializedAs("variantText")]
    [SerializeField] Text _variantText;
    [FormerlySerializedAs("variants")]
    [SerializeField] private List<string> _variants = new List<string>();
    [FormerlySerializedAs("nextBtn")]
    [SerializeField] Button _nextBtn;
    [FormerlySerializedAs("prevBtn")]
    [SerializeField] Button _prevBtn;
    [FormerlySerializedAs("bodyMat")]
    [SerializeField] Material _bodyMat;
    [FormerlySerializedAs("variantRoots")]
    [SerializeField] List<VariantRoot> _variantRoots = new List<VariantRoot>();

    Dictionary<string, GameObject> _variantByName;

    void Awake()
    {
        _variantByName = new Dictionary<string, GameObject>(StringComparer.Ordinal);
        foreach (VariantRoot vr in _variantRoots)
        {
            if (vr.Root != null && !string.IsNullOrEmpty(vr.VariantName))
                _variantByName[vr.VariantName] = vr.Root;
        }
    }

    void Start()
    {
        _propertyText.text = transform.name;
        _variantText.text = "Choose " + _propertyText.text.ToLower();

        bool isPropertyColor = _propertyText.text == "Color";

        _nextBtn.onClick.AddListener(() =>
        {
            string newVariant = NextVariant(_variants, _variantText);
            ApplyVariant(newVariant, isPropertyColor);
        });

        _prevBtn.onClick.AddListener(() =>
        {
            string newVariant = PrevVariant(_variants, _variantText);
            ApplyVariant(newVariant, isPropertyColor);
        });
    }

    static string NextVariant(List<string> variantsList, Text variantLabel)
    {
        int idx = variantsList.IndexOf(variantLabel.text);
        idx = idx >= variantsList.Count - 1 ? 0 : idx + 1;
        variantLabel.text = variantsList[idx];
        return variantLabel.text;
    }

    static string PrevVariant(List<string> variantsList, Text variantLabel)
    {
        int idx = variantsList.IndexOf(variantLabel.text);
        idx = idx <= 0 ? variantsList.Count - 1 : idx - 1;
        variantLabel.text = variantsList[idx];
        return variantLabel.text;
    }

    void ApplyVariant(string newVariant, bool isColor)
    {
        if (!isColor)
        {
            foreach (string variant in _variants)
            {
                // Fail fast in editor/dev so missing wiring is obvious before ship builds.
                if (!_variantByName.TryGetValue(variant, out GameObject variantModel) || variantModel == null)
                {
                    Debug.LogError(
                        $"ConfigCar on '{name}': assign variant root for '{variant}' in the Inspector (no runtime Find).");
                    continue;
                }

                bool show = variant == newVariant;
                variantModel.transform.localScale = show ? Vector3.one : Vector3.zero;
            }
        }
        else
        {
            string colorKey = newVariant.ToLowerInvariant();
            Color color = colorKey switch
            {
                "blue" => new Color32(45, 190, 240, 70),
                "rose gold" => new Color32(240, 155, 125, 70),
                "goblin" => new Color32(45, 170, 75, 70),
                "olive" => new Color32(180, 190, 50, 70),
                "silver" => new Color32(140, 170, 230, 70),
                "rosa" => new Color32(230, 140, 165, 70),
                "golden" => new Color32(230, 210, 140, 70),
                "green" => new Color32(150, 230, 140, 70),
                "cyan" => new Color32(140, 230, 220, 70),
                "sky" => new Color32(140, 200, 230, 70),
                "violet" => new Color32(170, 140, 230, 70),
                "white" => new Color32(255, 255, 255, 70),
                _ => new Color32(45, 190, 240, 70)
            };

            _bodyMat.SetColor("_Color", color);
        }
    }
}
