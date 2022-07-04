using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
public class RepairCar : MonoBehaviour
{
    public Button nextStepBtn, prevStepBtn;
    public Button repairSuspensionBtn, repairBrakesBtn, repairSteeringBtn, showOffBtn;
    public Text stepTitle, stepText;
    public Toggle checkBox;
    public InputField notes;
    public Button screenshotBtn;
    public RawImage image;
    public Animator animator;
    private int currStepIdx = 0;

    void Start()
    {
        //RepairTask currRepairTask = new RepairTask();

        RepairTask repairSuspension = new RepairTask("Repair Suspension");
        repairSuspension.Steps = new List<Step>()
        { new Step("Step1 Susp"), new Step("Step2 Susp"), new Step("Step3 Susp") };
        //repairSuspensionBtn.onClick.AddListener(() => { currRepairTask = repairSuspension; });

        RepairTask repairBrakes = new RepairTask("Repair Brakes");
        repairBrakes.Steps = new List<Step>()
        {new Step("Check if engine is off") , new Step("Unscrew wheel nuts and pull out wheels"), new Step("Remove disc brakes"), new Step("Separate brake calipers and change pads")};
        //repairBrakesBtn.onClick.AddListener(() => { currRepairTask = repairBrakes; });

        RepairTask repairSteering = new RepairTask("Repair Steering");
        repairSteering.Steps = new List<Step>()
        { new Step("Step1 Steer"), new Step("Step2 Steer"), new Step("Step3 Steer") };
        //repairSteeringBtn.onClick.AddListener(() => { currRepairTask = repairSteering; });

        //initialize repair step text
        prevStepBtn.interactable = false;
        stepTitle.text = "Step 0";
        stepText.text = repairBrakes.Steps[currStepIdx].StepText;
        checkBox.isOn = repairBrakes.Steps[currStepIdx].IsDone;

        nextStepBtn.onClick.AddListener(() => { ShowNextStep(repairBrakes.Steps); });
        prevStepBtn.onClick.AddListener(() => { ShowPrevStep(repairBrakes.Steps); });
        
        screenshotBtn.onClick.AddListener(() => TakeScreenshot(0));
        
        //temporarily showoffBtn is to generate protocol & show Screenshot
        showOffBtn.onClick.AddListener(() => LoadScreenshot(0));
    }

    void ShowNextStep(List<Step> steps)
    {
        prevStepBtn.interactable = true;
        
        Step currStep = steps[currStepIdx];
        //store infos from checkbox and notes for currStep
        currStep.IsDone = checkBox.isOn;
        currStep.Notes = notes.text;

        if (currStepIdx < steps.Count - 1)
        {
            currStepIdx++;
            //show animation
            animator.SetTrigger($"S{currStepIdx}Trigg"); //Triggers e.g. S1Trigg for Step1
        }
        else
        {
            //disable next button if last element reached
            nextStepBtn.interactable = false;
        }        
        //update text, title
        stepText.text = steps[currStepIdx].StepText;
        stepTitle.text = "Step " + currStepIdx.ToString();
        //load info from checkbox and notes for newStep
        checkBox.isOn = steps[currStepIdx].IsDone;
        notes.text = steps[currStepIdx].Notes;

    }

    void ShowPrevStep(List<Step> steps)
    {
        nextStepBtn.interactable = true;        

        Step currStep = steps[currStepIdx];
        //store info from checkbox for currStep
        currStep.IsDone = checkBox.isOn;
        currStep.Notes = notes.text;
        if (currStepIdx > 0)
        {
            //show animation
            animator.SetTrigger($"S{currStepIdx}revTrigg");
            currStepIdx--;
        }
        else
        {
            prevStepBtn.interactable = false;
        }
        stepText.text = steps[currStepIdx].StepText;
        stepTitle.text = "Step " + currStepIdx.ToString();
        //load info from checkbox for newStep
        checkBox.isOn = steps[currStepIdx].IsDone;
        notes.text = steps[currStepIdx].Notes;
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

    //reset all toggles to false and removes all data
    void ResetBtn()
    {

    }
}
