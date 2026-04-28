using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// Blynk bridge: brake lights and door sensor. Token must be set in the Inspector or config asset — never committed (issue: security).
/// </summary>
public sealed class IoT : MonoBehaviour
{
    [SerializeField] Button bckBtn;
    [SerializeField] Button fwdBtn;
    [SerializeField] Button midBtn;
    [SerializeField] Button okBtn;
    [SerializeField] Button closeStepBtn;
    [SerializeField] Animator animator;

    [Tooltip("Blynk device auth token. Prefer a ScriptableObject / remote config in production; do not commit real tokens.")]
    [SerializeField] string blynkToken;

    [SerializeField] string getUrlBase = "https://blynk.cloud/external/api/get?token=";
    [SerializeField] string updateUrlBase = "https://blynk.cloud/external/api/update?token=";

    const int DoorSensorPin = 14;
    int _sensorVal = 100;
    Coroutine _pollCoroutine;
    bool _pollRequested;

    void Start()
    {
        if (string.IsNullOrEmpty(blynkToken))
        {
            Debug.LogError("IoT: Assign blynkToken in the Inspector (use a non-committed config). IoT controls are disabled.");
            return;
        }

        string tokenParam = blynkToken;
        bckBtn.onClick.AddListener(() => StartCoroutine(UpdateValue($"{updateUrlBase}{tokenParam}&v1=1")));
        fwdBtn.onClick.AddListener(() => StartCoroutine(UpdateValue($"{updateUrlBase}{tokenParam}&v1=0")));
        midBtn.onClick.AddListener(CloseDoor);
        okBtn.onClick.AddListener(StartPollingDoorSensor);
        closeStepBtn.onClick.AddListener(StopPollingDoorSensor);
    }

    void OnDisable()
    {
        StopPollingDoorSensor();
    }

    void StartPollingDoorSensor()
    {
        if (string.IsNullOrEmpty(blynkToken))
            return;

        _pollRequested = true;
        if (_pollCoroutine != null)
            return;

        string uri = $"{getUrlBase}{blynkToken}&v{DoorSensorPin}";
        _pollCoroutine = StartCoroutine(PollSensorLoop(uri));
    }

    void StopPollingDoorSensor()
    {
        _pollRequested = false;
        if (_pollCoroutine != null)
        {
            StopCoroutine(_pollCoroutine);
            _pollCoroutine = null;
        }
    }

    IEnumerator PollSensorLoop(string uri)
    {
        while (_pollRequested)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                yield return webRequest.SendWebRequest();

#if UNITY_2020_2_OR_NEWER
                if (webRequest.result != UnityWebRequest.Result.Success)
#else
                if (webRequest.isNetworkError || webRequest.isHttpError)
#endif
                {
                    Debug.LogWarning($"IoT GET failed: {webRequest.error}");
                    yield return new WaitForSeconds(0.5f);
                    continue;
                }

                string body = webRequest.downloadHandler.text;
                if (int.TryParse(body.Trim(), out int v))
                    _sensorVal = v;

                OpenDoor();
            }

            yield return new WaitForSeconds(0.5f);
        }

        _pollCoroutine = null;
    }

    static IEnumerator UpdateValue(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();
#if UNITY_2020_2_OR_NEWER
            if (webRequest.result != UnityWebRequest.Result.Success)
#else
            if (webRequest.isNetworkError || webRequest.isHttpError)
#endif
                Debug.LogWarning($"IoT update failed: {webRequest.error}");
        }
    }

    void OpenDoor()
    {
        if (_sensorVal < 50)
            animator.Play("DoorLOpen");
    }

    void CloseDoor()
    {
        animator.Play("DoorLOpenrev");
    }
}
