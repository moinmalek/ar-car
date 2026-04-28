using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// Animates a <see cref="Slider"/> to full, then swaps UI pages once complete.
/// </summary>
public sealed class SliderRunTo1 : MonoBehaviour
{
    [FormerlySerializedAs("b")]
    [SerializeField] bool animate = true;
    [SerializeField] Slider slider;
    [SerializeField] float speed = 0.5f;
    [SerializeField] GameObject loadingPage;
    [SerializeField] GameObject toBeLoadedPage;

    float _time;

    void Awake()
    {
        if (slider == null)
            slider = GetComponent<Slider>();
    }

    void Update()
    {
        if (!animate || slider == null)
            return;

        _time += Time.deltaTime * speed;
        slider.value = Mathf.Clamp01(_time);

        if (_time <= 1f)
            return;

        if (loadingPage != null)
            loadingPage.SetActive(false);
        if (toBeLoadedPage != null)
            toBeLoadedPage.SetActive(true);

        _time = 0f;
    }
}
