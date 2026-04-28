using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Digital repair manual UI: step flow, screenshots, protocol generation (issues #1–#5, #9).
/// </summary>
public sealed class RepairCar : MonoBehaviour
{
    const string StepTitleFormat = "Step {0}";

    [Header("Repair actions")]
    [SerializeField] Button repairSuspensionBtn;
    [SerializeField] Button repairBrakesBtn;
    [SerializeField] Button repairSteeringBtn;
    [SerializeField] Button showProtBtn;

    [Header("Instruction panel")]
    [SerializeField] Button nextStepBtn;
    [SerializeField] Button prevStepBtn;
    [SerializeField] Button screenshotBtn;
    [SerializeField] Button closeBtn;
    [SerializeField] Text stepTitle;
    [SerializeField] Text stepText;
    [SerializeField] Toggle checkBox;
    [SerializeField] InputField notes;
    [SerializeField] GameObject ssTaken;

    [Header("Car remote")]
    [SerializeField] Button fwdBtn;
    [SerializeField] Button bckBtn;
    [SerializeField] Button midBtn;

    [Header("Protocol")]
    [SerializeField] Button saveBtn;
    [SerializeField] TextMeshProUGUI protocolName;
    [SerializeField] InputField technician;
    [SerializeField] InputField location;
    [SerializeField] InputField problem;
    [SerializeField] InputField conclusion;
    [SerializeField] GameObject repTaskPrefab;
    [SerializeField] GameObject stepPrefab;
    [SerializeField] Transform prefabParent;
    [SerializeField] Texture2D nullTex;
    [SerializeField] Button showHistory;
    [SerializeField] Button closeProtBtn;
    [SerializeField] TextMeshProUGUI history;

    [Header("AR repair animators")]
    [SerializeField] AnimatorRepairBindings animatorBindings;

    Protocol _protocol;
    RepairTask _currRepairTask;
    int _currStepIdx;

    SimpleGameObjectPool _titlePool;
    SimpleGameObjectPool _stepPool;

    static string TempScreenshotPath => Path.Combine(Application.persistentDataPath, "temp.png");

    void Awake()
    {
        if (prefabParent != null && repTaskPrefab != null && stepPrefab != null)
        {
            _titlePool = new SimpleGameObjectPool(repTaskPrefab, prefabParent);
            _stepPool = new SimpleGameObjectPool(stepPrefab, prefabParent);
        }
    }

    void Start()
    {
        _protocol = ProtocolPersistence.LoadOrCreate();
        if (technician != null) technician.text = _protocol.Technician ?? "";
        if (location != null) location.text = _protocol.Location ?? "";
        if (problem != null) problem.text = _protocol.Problem ?? "";
        if (conclusion != null) conclusion.text = _protocol.Conclusion ?? "";

        repairBrakesBtn?.onClick.AddListener(() =>
        {
            _currRepairTask = new RepairTask("Repair Brakes", new List<Step>
            {
                new Step("Check if engine is off", "Wheelend", "BrakesS1"),
                new Step("Unscrew wheel nuts and pull out wheels", "Wheelend", "BrakesS2"),
                new Step("Remove disc brakes", "Wheelend", "BrakesS3"),
                new Step("Separate brake calipers and change pads")
            });
            InitInfo();
        });

        repairSuspensionBtn?.onClick.AddListener(() =>
        {
            _currRepairTask = new RepairTask("Repair Suspension", new List<Step>
            {
                new Step("Check if engine is off", "Body", "HoodOpen"),
                new Step("Open the hood", "Car", "EngineLift"),
                new Step("Lift the engine out", "Car", "SpringLift"),
                new Step("Remove the springs and insert new")
            });
            InitInfo();
        });

        repairSteeringBtn?.onClick.AddListener(() =>
        {
            _currRepairTask = new RepairTask("Repair Steering", new List<Step>
            {
                new Step("Check if engine is off", "Body", "HoodOpen"),
                new Step("Open the hood", "Car", "EngineLift"),
                new Step("Lift the engine out", "Car", "SteeringLift"),
                new Step("Replace the rack with new one")
            });
            InitInfo();
        });

        nextStepBtn?.onClick.AddListener(() =>
        {
            if (_currRepairTask?.Steps != null)
                ShowNextStep(_currRepairTask.Steps);
        });

        prevStepBtn?.onClick.AddListener(() =>
        {
            if (_currRepairTask?.Steps != null)
                ShowPrevStep(_currRepairTask.Steps);
        });

        screenshotBtn?.onClick.AddListener(() => StartCoroutine(TakeScreenshot()));

        closeBtn?.onClick.AddListener(() =>
        {
            if (_currRepairTask?.Steps == null)
                return;

            StartCoroutine(ResetCar(_currRepairTask.Steps));
            StoreStepInfo(_currRepairTask.Steps[_currStepIdx]);
        });

        saveBtn?.onClick.AddListener(() =>
        {
            if (_currRepairTask != null)
            {
                _protocol.RepairTasks.Add(_currRepairTask);
                StoreProtocolInfo();
                ProtocolPersistence.Save(_protocol);
            }
        });

        showProtBtn?.onClick.AddListener(GenerateProtocol);
        closeProtBtn?.onClick.AddListener(() =>
        {
            StoreProtocolInfo();
            ProtocolPersistence.Save(_protocol);
        });

        showHistory?.onClick.AddListener(() =>
        {
            StoreProtocolInfo();
            ProtocolPersistence.Save(_protocol);
            _protocol.GenerateHistory(_protocol.RepairTasks, history);
        });
    }

    void InitInfo()
    {
        if (_currRepairTask?.Steps == null || _currRepairTask.Steps.Count == 0)
            return;

        _currStepIdx = 0;
        if (prevStepBtn != null)
            prevStepBtn.interactable = false;
        if (nextStepBtn != null)
            nextStepBtn.interactable = true;

        stepTitle.text = string.Format(StepTitleFormat, 0);
        stepText.text = _currRepairTask.Steps[0].StepText;
        checkBox.isOn = _currRepairTask.Steps[0].IsDone;
        notes.text = _currRepairTask.Steps[0].Notes;

        SetRepairButtonsInteractable(false);
    }

    void SetRepairButtonsInteractable(bool enabled)
    {
        if (repairSteeringBtn != null) repairSteeringBtn.interactable = enabled;
        if (repairSuspensionBtn != null) repairSuspensionBtn.interactable = enabled;
        if (repairBrakesBtn != null) repairBrakesBtn.interactable = enabled;
        if (fwdBtn != null) fwdBtn.interactable = enabled;
        if (bckBtn != null) bckBtn.interactable = enabled;
        if (midBtn != null) midBtn.interactable = enabled;
    }

    void ShowNextStep(List<Step> steps)
    {
        prevStepBtn.interactable = true;

        Step currStep = steps[_currStepIdx];
        StoreStepInfo(currStep);

        if (_currStepIdx < steps.Count - 1)
        {
            PlayStepAnimation(currStep, forward: true);
            _currStepIdx++;
            currStep = steps[_currStepIdx];
        }
        else
        {
            nextStepBtn.interactable = false;
        }

        UpdateStepInfo(currStep);
    }

    void ShowPrevStep(List<Step> steps)
    {
        nextStepBtn.interactable = true;

        Step currStep = steps[_currStepIdx];
        StoreStepInfo(currStep);

        if (_currStepIdx > 0)
        {
            _currStepIdx--;
            currStep = steps[_currStepIdx];
            PlayStepAnimation(currStep, forward: false);
        }
        else
        {
            prevStepBtn.interactable = false;
        }

        UpdateStepInfo(currStep);
    }

    void PlayStepAnimation(Step step, bool forward)
    {
        if (animatorBindings == null || string.IsNullOrEmpty(step.AnimatorName))
            return;

        if (!animatorBindings.TryGetAnimator(step.AnimatorName, out Animator anim))
        {
            Debug.LogWarning($"Repair animator not bound: '{step.AnimatorName}'");
            return;
        }

        string stateName = forward ? step.AnimationName : step.AnimationName + "rev";
        if (string.IsNullOrEmpty(step.AnimationName))
            return;

        anim.Play(stateName, 0, 0f);
    }

    void StoreStepInfo(Step step)
    {
        step.IsDone = checkBox.isOn;
        step.Notes = notes.text;

        // ScreenCapture writes asynchronously; path matches CaptureScreenshot("temp.png") → persistentDataPath/temp.png
        if (!File.Exists(TempScreenshotPath))
            return;

        byte[] imageData;
        try
        {
            imageData = File.ReadAllBytes(TempScreenshotPath);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Could not read screenshot: {e.Message}");
            return;
        }

        if (step.ScreenshotTex != null)
            Destroy(step.ScreenshotTex);

        var screenshotTex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        if (screenshotTex.LoadImage(imageData))
        {
            step.ScreenshotTex = screenshotTex;
            try { File.Delete(TempScreenshotPath); }
            catch { /* ignore */ }
        }
        else
        {
            Destroy(screenshotTex);
        }
    }

    void UpdateStepInfo(Step step)
    {
        stepText.text = step.StepText;
        stepTitle.text = string.Format(StepTitleFormat, _currStepIdx);
        checkBox.isOn = step.IsDone;
        notes.text = step.Notes;
    }

    IEnumerator ResetCar(List<Step> steps)
    {
        int i = _currStepIdx - 1;
        while (i >= 0)
        {
            Step currStep = steps[i];
            if (animatorBindings != null && animatorBindings.TryGetAnimator(currStep.AnimatorName, out Animator anim)
                && !string.IsNullOrEmpty(currStep.AnimationName))
            {
                anim.Play(currStep.AnimationName + "rev", 0, 0f);
                // Length comes from current layer state; assumes controller clips match reverse naming (+ "rev").
                yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
            }
            i--;
        }

        animatorBindings?.ResetAllToIdle();
        SetRepairButtonsInteractable(true);
    }

    void ReleaseProtocolChildren()
    {
        if (prefabParent == null)
            return;

        for (int c = prefabParent.childCount - 1; c >= 0; c--)
        {
            Transform t = prefabParent.GetChild(c);
            GameObject go = t.gameObject;
            if (_titlePool != null && go.GetComponent<RepairTaskTitleView>() != null)
                _titlePool.Release(go);
            else if (_stepPool != null && go.GetComponent<StepProtocolRowView>() != null)
                _stepPool.Release(go);
            else
                Destroy(go);
        }
    }

    void GenerateProtocol()
    {
        if (_protocol == null || prefabParent == null || repTaskPrefab == null || stepPrefab == null)
            return;

        protocolName.text = _protocol.ProtocolName;

        ReleaseProtocolChildren();

        float moveRepTitle = 0f;
        float movePanel = 0f;

        for (int repTaskIdx = 0; repTaskIdx < _protocol.RepairTasks.Count; repTaskIdx++)
        {
            RepairTask repTask = _protocol.RepairTasks[repTaskIdx];

            GameObject repTaskTitle = _titlePool != null ? _titlePool.Get() : Instantiate(repTaskPrefab, prefabParent, false);
            repTaskTitle.transform.SetParent(prefabParent, false);
            repTaskTitle.transform.localPosition = Vector3.zero;
            repTaskTitle.transform.localRotation = Quaternion.identity;
            repTaskTitle.transform.localScale = Vector3.one;
            repTaskTitle.transform.Translate(0, moveRepTitle, 0);
            repTaskTitle.name = $"R{repTaskIdx}";

            var titleView = repTaskTitle.GetComponent<RepairTaskTitleView>();
            if (titleView?.TitleText != null)
                titleView.TitleText.text = repTask.TaskName;

            for (int stepIdx = 0; stepIdx < repTask.Steps.Count; stepIdx++)
            {
                Step step = repTask.Steps[stepIdx];

                GameObject stepPanel = _stepPool != null ? _stepPool.Get() : Instantiate(stepPrefab, prefabParent, false);
                stepPanel.transform.SetParent(prefabParent, false);
                stepPanel.transform.localPosition = Vector3.zero;
                stepPanel.transform.localRotation = Quaternion.identity;
                stepPanel.transform.localScale = Vector3.one;
                stepPanel.transform.Translate(0, movePanel, 0);
                stepPanel.name = $"S{stepIdx}R{repTaskIdx}";

                var row = stepPanel.GetComponent<StepProtocolRowView>();
                if (row != null)
                    row.Apply(stepIdx, step, nullTex);

                movePanel -= 400f;
            }

            movePanel -= 60f;
            moveRepTitle = movePanel;
        }
    }

    void StoreProtocolInfo()
    {
        _protocol.Technician = technician.text;
        _protocol.Location = location.text;
        _protocol.Problem = problem.text;
        _protocol.Conclusion = conclusion.text;
    }

    IEnumerator TakeScreenshot()
    {
        ScreenCapture.CaptureScreenshot("temp.png");
        ssTaken.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        ssTaken.SetActive(false);
    }
}
