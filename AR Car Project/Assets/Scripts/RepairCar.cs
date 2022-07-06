using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using System.Collections;

public class RepairCar : MonoBehaviour
{   
    //repair task buttons
    public Button repairSuspensionBtn, repairBrakesBtn, repairSteeringBtn, showOffBtn;
    //instruction panel items
    public Button nextStepBtn, prevStepBtn;
    public Text stepTitle, stepText;
    public Toggle checkBox;
    public InputField notes;
    public Button screenshotBtn;
    public RawImage image;
    //save protocol popup items
    public Button saveBtn, cancelBtn, closePopupBtn;


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

        nextStepBtn.onClick.AddListener(() => ShowNextStep(currRepairTask.Steps));
        prevStepBtn.onClick.AddListener(() => ShowPrevStep(currRepairTask.Steps));

        //initialize protocol
        protocol = new Protocol($"Protocol {DateTime.Now.ToString()}", new List<RepairTask>());

        //add repair task to protocol
        saveBtn.onClick.AddListener(() => protocol.RepairTasks.Add(currRepairTask));

        //bring back car original state or fully assembled
        cancelBtn.onClick.AddListener(() => StartCoroutine(ResetCar(currRepairTask.Steps)));
        closePopupBtn.onClick.AddListener(() => StartCoroutine(ResetCar(currRepairTask.Steps)));

        showOffBtn.onClick.AddListener(() => protocol.GenerateProtocol(protocol.RepairTasks));

        screenshotBtn.onClick.AddListener(() => TakeScreenshot(0));
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
        //disable repair task buttons else all variable infos are reloaded and stored infos lost
        repairSteeringBtn.enabled = false;
        repairSuspensionBtn.enabled = false;
        repairBrakesBtn.enabled = false;
    }

    void ShowNextStep(List<Step> steps)
    {
        prevStepBtn.interactable = true;
        
        Step currStep = steps[currStepIdx]; //current step
        //store infos from checkbox and notes for currStep
        currStep.IsDone = checkBox.isOn;
        currStep.Notes = notes.text;

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
        //update text, title
        stepText.text = currStep.StepText;
        stepTitle.text = "Step " + currStepIdx.ToString();
        //load info from checkbox and notes for newStep
        checkBox.isOn = currStep.IsDone;
        notes.text = currStep.Notes;

    }

    void ShowPrevStep(List<Step> steps)
    {
        nextStepBtn.interactable = true;        

        Step currStep = steps[currStepIdx]; //current step
        //store info from checkbox for currStep
        currStep.IsDone = checkBox.isOn;
        currStep.Notes = notes.text;
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
        stepText.text = currStep.StepText;
        stepTitle.text = "Step " + currStepIdx.ToString();
        //load info from checkbox for newStep
        checkBox.isOn = currStep.IsDone;
        notes.text = currStep.Notes;
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

    void TakeScreenshot(int stepIdx)
    {
        //change screenshot folder and name
        ScreenCapture.CaptureScreenshot($"{stepIdx.ToString()}.png");
    }

    void LoadScreenshot(int stepIdx)
    {
        Texture2D myTexture = new Texture2D(73, 73);        
        //if direct save in Application.persistentDataPath => no problem, if /.../... => problem
        var imageData = File.ReadAllBytes(Application.persistentDataPath + $"/{stepIdx.ToString()}.png");
        myTexture.LoadImage(imageData);
        image.texture = myTexture;        
    }

}
