using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// Linear fill animation on an <see cref="Image"/> (horizontal fill amount).
/// </summary>
public sealed class SimpleProgress1 : MonoBehaviour
{
    [FormerlySerializedAs("b")]
    [SerializeField] bool animate = true;
    [SerializeField] Image image;
    [SerializeField] float speed = 1f;

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

        if (_time > 1f)
            _time = 0f;
    }
}
