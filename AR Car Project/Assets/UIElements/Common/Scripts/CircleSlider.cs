using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// Fills a radial <see cref="Image"/> over time and optionally mirrors progress to a <see cref="Text"/> label.
/// </summary>
public sealed class CircleSlider : MonoBehaviour
{
    [FormerlySerializedAs("b")]
    [SerializeField] bool animate = true;
    [SerializeField] Image image;
    [SerializeField] float speed = 0.5f;
    [SerializeField] Text progress;

    float _time;

    void Awake()
    {
        if (image == null)
            image = GetComponent<Image>();
    }

    void Update()
    {
        if (!animate || image == null)
            return;

        _time += Time.deltaTime * speed;
        image.fillAmount = _time;

        if (progress != null)
            progress.text = (int)(image.fillAmount * 100f) + "%";

        if (_time > 1f)
            _time = 0f;
    }
}
