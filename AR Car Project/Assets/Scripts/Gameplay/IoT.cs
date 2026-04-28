using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// Blynk bridge: brake lights and door sensor.
/// </summary>
public sealed class IoT : MonoBehaviour
{
    [SerializeField] Button bckBtn;
    [SerializeField] Button fwdBtn;
    [SerializeField] Button midBtn;
    [SerializeField] Button okBtn;
    [SerializeField] Button closeStepBtn;
    [SerializeField] Animator animator;

    [Tooltip("Blynk device auth token — assign locally; never commit real credentials. Optional: env BLYNK_TOKEN or Assets/Resources/BlynkToken.txt (gitignored).")]
    [SerializeField] string blynkToken;

    [SerializeField] string getUrlBase = "https://blynk.cloud/external/api/get?token=";
    [SerializeField] string updateUrlBase = "https://blynk.cloud/external/api/update?token=";

    const int DoorSensorPin = 14;
    int _sensorVal = 100;
    Coroutine _pollCoroutine;
    bool _pollRequested;
    string _effectiveToken;

    void Start()
    {
        _effectiveToken = ResolveBlynkToken(blynkToken);

        // Door open/close animations work without Blynk; wire these regardless.
        midBtn.onClick.AddListener(CloseDoor);
        closeStepBtn.onClick.AddListener(StopPollingDoorSensor);

        if (string.IsNullOrEmpty(_effectiveToken))
        {
            Debug.LogWarning(
                "IoT: No Blynk token (Inspector, env BLYNK_TOKEN, or Resources/BlynkToken.txt). Brake lights and door sensor polling are disabled.");
            return;
        }

        string tokenParam = _effectiveToken;
        bckBtn.onClick.AddListener(() => StartCoroutine(UpdateValue($"{updateUrlBase}{tokenParam}&v1=1")));
        fwdBtn.onClick.AddListener(() => StartCoroutine(UpdateValue($"{updateUrlBase}{tokenParam}&v1=0")));
        okBtn.onClick.AddListener(StartPollingDoorSensor);
    }

    static string ResolveBlynkToken(string serializedToken)
    {
        return TryFirstNonEmpty(
            serializedToken,
            Environment.GetEnvironmentVariable("BLYNK_TOKEN"),
            LoadResourcesToken());
    }

    static string LoadResourcesToken()
    {
        var ta = Resources.Load<TextAsset>("BlynkToken");
        return ta != null ? ta.text : null;
    }

    static string TryFirstNonEmpty(params string[] candidates)
    {
        if (candidates == null)
            return null;
        foreach (var c in candidates)
        {
            if (string.IsNullOrEmpty(c))
                continue;
            var t = c.Trim();
            if (t.Length > 0)
                return t;
        }

        return null;
    }

    void OnDisable()
    {
        StopPollingDoorSensor();
    }

    void StartPollingDoorSensor()
    {
        if (string.IsNullOrEmpty(_effectiveToken))
            return;

        _pollRequested = true;
        if (_pollCoroutine != null)
            return;

        string uri = $"{getUrlBase}{_effectiveToken}&v{DoorSensorPin}";
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

                // Physical sensor threshold drives virtual door (original behaviour).
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
