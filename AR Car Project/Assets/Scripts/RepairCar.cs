using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RepairCar : MonoBehaviour
{
    //repair task buttons
    public Button repairSuspensionBtn, repairBrakesBtn, repairSteeringBtn, showProtBtn;
    //instruction panel items
    public Button nextStepBtn, prevStepBtn, screenshotBtn, closeBtn;
    public Text stepTitle, stepText;
    public Toggle checkBox;
    public InputField notes;
    public GameObject ssTaken; //screenshot taken text
    //controller items
    public Button fwdBtn, bckBtn, midBtn;
    //save protocol popup items
    public Button saveBtn;
    //display protocol items
    public TextMeshProUGUI protocolName;
    public InputField technician, location, problem, conclusion;
    public GameObject repTaskPrefab, stepPrefab;
    public Transform prefabParent;
    public Texture2D nullTex;
    public Button showHistory, closeProtBtn;
    public TextMeshProUGUI history;

    private Protocol protocol;
    private RepairTask currRepairTask;
    private int currStepIdx;
    private Animator animator;

    void Start()
    {
        repairBrakesBtn.onClick.AddListener(() =>
        {
            currRepairTask = new RepairTask("Repair Brakes", new List<Step>() 
            { 
                new Step("Check if engine is off", "Wheelend", "BrakesS1"), 
                new Step("Unscrew wheel nuts and pull out wheels", "Wheelend", "BrakesS2"), 
                new Step("Remove disc brakes", "Wheelend", "BrakesS3"), 
                new Step("Separate brake calipers and change pads") 
            });
            InitInfo(); //initialize display instruction text/info           
        });

        repairSuspensionBtn.onClick.AddListener(() =>
        {
            currRepairTask = new RepairTask("Repair Suspension", new List<Step>() 
            {
                new Step("Check if engine is off", "Body", "HoodOpen"),
                new Step("Open the hood", "Car", "EngineLift"), 
                new Step("Lift the engine out", "Car", "SpringLift"), 
                new Step("Remove the springs and insert new")
            });
            InitInfo();
        });

        repairSteeringBtn.onClick.AddListener(() =>
        {
            currRepairTask = new RepairTask("Repair Steering", new List<Step>() 
            {
                new Step("Check if engine is off", "Body", "HoodOpen"),
                new Step("Open the hood", "Car", "EngineLift"),
                new Step("Lift the engine out", "Car", "SteeringLift"),
                new Step("Replace the rack with new one")
            });
            InitInfo();
        });

        //instruction panel
        nextStepBtn.onClick.AddListener(() => ShowNextStep(currRepairTask.Steps));
        prevStepBtn.onClick.AddListener(() => ShowPrevStep(currRepairTask.Steps));
        screenshotBtn.onClick.AddListener(() => StartCoroutine(TakeScreenshot()));
        closeBtn.onClick.AddListener(() =>
        {
            //bring back car original state or fully assembled
            StartCoroutine(ResetCar(currRepairTask.Steps));
            StoreStepInfo(currRepairTask.Steps[currStepIdx]);                    
        });

        //initialize protocol
        protocol = new Protocol($"P {DateTime.Now.ToString()}", new List<RepairTask>());

        //save or add repair task to protocol
        saveBtn.onClick.AddListener(() => protocol.RepairTasks.Add(currRepairTask));      

        //protocol popup
        showProtBtn.onClick.AddListener(GenerateProtocol);
        closeProtBtn.onClick.AddListener(StoreProtocolInfo);
        showHistory.onClick.AddListener(() =>
        {
            //store infos from inputfields
            StoreProtocolInfo();
            protocol.GenerateHistory(protocol.RepairTasks, history);
        });
    }

    void InitInfo()
    {
        //initialize repair step variables 
        currStepIdx = 0;
        prevStepBtn.interactable = false;
        nextStepBtn.interactable = true;
        //initialize text and checkbox
        stepTitle.text = "Step 0";
        stepText.text = currRepairTask.Steps[0].StepText;
        checkBox.isOn = currRepairTask.Steps[0].IsDone;
        notes.text = currRepairTask.Steps[0].Notes;
        //disable all other buttons
        List<Button> btns = new List<Button>() { repairSteeringBtn, repairSuspensionBtn, repairBrakesBtn , fwdBtn, bckBtn, midBtn };
        foreach(Button btn in btns)
        {
            btn.enabled = false;
        }     
    }

    void ShowNextStep(List<Step> steps)
    {
        prevStepBtn.interactable = true;
        
        Step currStep = steps[currStepIdx]; //current step
        StoreStepInfo(currStep);

        if (currStepIdx < steps.Count - 1)
        {
            //show animation            
            animator = GameObject.Find(currStep.AnimatorName).GetComponent<Animator>();
            animator.Play(currStep.AnimationName);
            //go to next step
            currStepIdx++;
            currStep = steps[currStepIdx]; //update current step
        }
        else
        {
            //disable next button if last element reached
            nextStepBtn.interactable = false;
        }
        UpdateStepInfo(currStep);       
    }

    void ShowPrevStep(List<Step> steps)
    {
        nextStepBtn.interactable = true;        

        Step currStep = steps[currStepIdx]; //current step
        StoreStepInfo(currStep);

        if (currStepIdx > 0)
        {   
            //go back a step
            currStepIdx--;
            currStep = steps[currStepIdx]; //update current step            
            //show animation             
            animator = GameObject.Find(currStep.AnimatorName).GetComponent<Animator>();
            animator.Play(currStep.AnimationName + "rev"); //reverse step
        }
        else
        {
            prevStepBtn.interactable = false;
        }
        UpdateStepInfo(currStep);
    }
    
    void StoreStepInfo(Step step)
    {
        //store infos from checkbox and notes for currStep
        step.IsDone = checkBox.isOn;
        step.Notes = notes.text;

        //store texture in step
        Texture2D screenshotTex = new Texture2D(73, 73);
        if (File.Exists(Application.persistentDataPath + "/temp.png"))
        //if(File.Exists(Application.dataPath + "/temp.png"))
        {
            var imageData = File.ReadAllBytes(Application.persistentDataPath + "/temp.png");
            //var imageData = File.ReadAllBytes(Application.dataPath + "/temp.png");
            screenshotTex.LoadImage(imageData);
            step.ScreenshotTex = screenshotTex;
            File.Delete(Application.persistentDataPath + "/temp.png");
            //File.Delete(Application.dataPath + "/temp.png");
        }        
    }

    void UpdateStepInfo(Step step)
    {
        stepText.text = step.StepText;
        stepTitle.text = "Step " + currStepIdx.ToString();
        checkBox.isOn = step.IsDone;
        notes.text = step.Notes;
    }

    IEnumerator ResetCar(List<Step> steps)
    {
        int i = currStepIdx - 1;
        while (i >= 0)
        {
            Step currStep = steps[i];
            animator = GameObject.Find(currStep.AnimatorName).GetComponent<Animator>();
            animator.Play(currStep.AnimationName + "rev"); //reverse step
            //wait till animation is over
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
            i--;
        }        
    }

    void GenerateProtocol()
    {
        protocolName.text = protocol.ProtocolName;

        float moveRepTitle = 0f; //space between repair task title
        float movePanel = 0f; //space between step panels

        for (int repTaskIdx = 0; repTaskIdx < protocol.RepairTasks.Count; repTaskIdx++)
        {
            RepairTask repTask = protocol.RepairTasks[repTaskIdx];

            //load repair task title
            GameObject repTaskTitle = Instantiate(repTaskPrefab);
            repTaskTitle.transform.SetParent(prefabParent, false);
            repTaskTitle.transform.Translate(0, moveRepTitle, 0);
            repTaskTitle.name = $"R{repTaskIdx}";
            Text repTaskText = GameObject.Find($"{repTaskTitle.name}/TitleText").GetComponent<Text>();           
            repTaskText.text = repTask.TaskName;            

            //create and load steps
            for (int stepIdx = 0; stepIdx < repTask.Steps.Count; stepIdx++)
            {
                Step step = repTask.Steps[stepIdx];

                GameObject stepPanel = Instantiate(stepPrefab);
                stepPanel.transform.SetParent(prefabParent, false);
                stepPanel.transform.Translate(0, movePanel, 0); //create panels one below another
                stepPanel.name = $"S{stepIdx}R{repTaskIdx}";
                //load step id
                TextMeshProUGUI stepId = GameObject.Find($"{stepPanel.name}/StepId").GetComponent<TextMeshProUGUI>();
                stepId.text = $"Step {stepIdx}";
                //load screenshot
                RawImage screenshot = GameObject.Find($"{stepPanel.name}/Screenshot").GetComponent<RawImage>();
                screenshot.texture = step.ScreenshotTex == null ? nullTex : step.ScreenshotTex;  
                //load notes
                TextMeshProUGUI protocolNotes = GameObject.Find($"{stepPanel.name}/Notes").GetComponent<TextMeshProUGUI>();
                protocolNotes.text = step.Notes;
                //load isStepComplete 
                Toggle isDone = GameObject.Find($"{stepPanel.name}/Checkbox").GetComponent<Toggle>();
                isDone.isOn = step.IsDone;

                movePanel -= 400f;
            }
            movePanel -= 60f;
            moveRepTitle = movePanel;
        }
    }

    void StoreProtocolInfo()
    {
        protocol.Technician = technician.text;
        protocol.Location = location.text;
        protocol.Problem = problem.text;
        protocol.Conclusion = conclusion.text;
    }

    IEnumerator TakeScreenshot()
    {
        ScreenCapture.CaptureScreenshot("temp.png");
        //ScreenCapture.CaptureScreenshot(Application.dataPath + "/temp.png"); 
        ssTaken.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        ssTaken.SetActive(false);
    }

}
