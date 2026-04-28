using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// Enables <see cref="imageTarget"/> when a reference image is tracked and disables it when tracking is lost.
/// Pose is synced each frame so content stays aligned with the detected image.
/// </summary>
public class ImageTargetEnabler : MonoBehaviour
{
    [SerializeField] ARTrackedImageManager trackedImageManager;
    [SerializeField] GameObject imageTarget;

    void Awake()
    {
        if (trackedImageManager == null)
            trackedImageManager = GetComponent<ARTrackedImageManager>();
    }

    void OnEnable()
    {
        if (trackedImageManager != null)
            trackedImageManager.trackablesChanged.AddListener(OnTrackablesChanged);
    }

    void OnDisable()
    {
        if (trackedImageManager != null)
            trackedImageManager.trackablesChanged.RemoveListener(OnTrackablesChanged);
    }

    void OnTrackablesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> args)
    {
        // AR Foundation 6+: removed is KeyValuePair<TrackableId, ARTrackedImage>, not ARTrackedImage.
        if (args.removed.Count > 0 && imageTarget != null)
            imageTarget.SetActive(false);

        var added = args.added;
        for (int i = 0; i < added.Count; i++)
            ApplyTrackedImage(added[i]);

        var updated = args.updated;
        for (int i = 0; i < updated.Count; i++)
            ApplyTrackedImage(updated[i]);
    }

    void ApplyTrackedImage(ARTrackedImage img)
    {
        if (imageTarget == null)
            return;

        if (img.trackingState == TrackingState.None)
        {
            imageTarget.SetActive(false);
            return;
        }

        imageTarget.SetActive(true);
        imageTarget.transform.SetPositionAndRotation(img.transform.position, img.transform.rotation);
    }
}
