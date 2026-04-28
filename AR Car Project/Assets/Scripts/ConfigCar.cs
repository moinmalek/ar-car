using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Cycles car assembly variants; variant roots are wired in the Inspector (issue #1 — no GameObject.Find).
/// </summary>
public sealed class ConfigCar : MonoBehaviour
{
    [Serializable]
    sealed class VariantRoot
    {
        [SerializeField] string variantName;
        [SerializeField] GameObject root;

        public string VariantName => variantName;
        public GameObject Root => root;
    }

    [SerializeField] Text propertyText;
    [SerializeField] Text variantText;
    [SerializeField] List<string> variants = new List<string>();
    [SerializeField] Button nextBtn;
    [SerializeField] Button prevBtn;
    [SerializeField] Material bodyMat;
    [SerializeField] List<VariantRoot> variantRoots = new List<VariantRoot>();

    Dictionary<string, GameObject> _variantByName;

    void Awake()
    {
        _variantByName = new Dictionary<string, GameObject>(StringComparer.Ordinal);
        for (int i = 0; i < variantRoots.Count; i++)
        {
            VariantRoot vr = variantRoots[i];
            if (vr.Root != null && !string.IsNullOrEmpty(vr.VariantName))
                _variantByName[vr.VariantName] = vr.Root;
        }

        if (propertyText == null || propertyText.text == "Color")
            return;

        for (int i = 0; i < variants.Count; i++)
        {
            string v = variants[i];
            if (string.IsNullOrEmpty(v) || _variantByName.ContainsKey(v))
                continue;

            GameObject found = GameObject.Find(v);
            if (found != null)
            {
                _variantByName[v] = found;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                Debug.LogWarning(
                    $"ConfigCar on '{name}': variant '{v}' was resolved via GameObject.Find. " +
                    "Assign variant roots in the Inspector to avoid runtime hierarchy scans.");
#endif
            }
        }
    }

    void Start()
    {
        propertyText.text = transform.name;
        variantText.text = "Choose " + propertyText.text.ToLower();

        bool isPropertyColor = propertyText.text == "Color";

        nextBtn.onClick.AddListener(() =>
        {
            string newVariant = NextBtnPressed(variants, variantText);
            ChangeVariant(newVariant, isPropertyColor);
        });

        prevBtn.onClick.AddListener(() =>
        {
            string newVariant = PrevBtnPressed(variants, variantText);
            ChangeVariant(newVariant, isPropertyColor);
        });
    }

    static string NextBtnPressed(List<string> variantsList, Text variantLabel)
    {
        int currVarIdx = variantsList.IndexOf(variantLabel.text);
        if (currVarIdx >= variantsList.Count - 1)
            currVarIdx = 0;
        else
            currVarIdx++;

        variantLabel.text = variantsList[currVarIdx];
        return variantLabel.text;
    }

    static string PrevBtnPressed(List<string> variantsList, Text variantLabel)
    {
        int currVarIdx = variantsList.IndexOf(variantLabel.text);
        if (currVarIdx <= 0)
            currVarIdx = variantsList.Count - 1;
        else
            currVarIdx--;

        variantLabel.text = variantsList[currVarIdx];
        return variantLabel.text;
    }

    void ChangeVariant(string newVariant, bool isColor)
    {
        if (!isColor)
        {
            foreach (string variant in variants)
            {
                if (!_variantByName.TryGetValue(variant, out GameObject variantModel) || variantModel == null)
                {
                    Debug.LogWarning($"ConfigCar: assign variant root for '{variant}' in the Inspector.");
                    continue;
                }

                bool show = variant == newVariant;
                variantModel.transform.localScale = show ? Vector3.one : Vector3.zero;
            }
        }
        else
        {
            string currColorText = newVariant.ToLowerInvariant();
            Color currColor = currColorText switch
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

            bodyMat.SetColor("_Color", currColor);
        }
    }
}
