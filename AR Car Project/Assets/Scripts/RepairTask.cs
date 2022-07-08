using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//top node (level 0)
public class Protocol
{
    public string ProtocolName { get; set; }
    public string Technician { get; set; }
    public string Location { get; set; }
    public string Problem { get; set; }
    public string Conclusion { get; set; }
    public List<RepairTask> RepairTasks { get; set; }

    public Protocol(string protocolName, List<RepairTask> repairTasks)
    {
        ProtocolName = protocolName;
        RepairTasks = repairTasks;
    }
    
    public void GenerateHistory(List<RepairTask> repairTasks, TextMeshProUGUI history) //InputField username, InputField location, InputField problem, InputField conclusion,
    {        
        string addLines = ProtocolName + "\n";     
        addLines += $"Technician: {Technician}\nLocation: {Location}\nProblem: {Problem}\nConclusion: {Conclusion}\n";
        addLines += "==========================\n";
        foreach (RepairTask repairTask in repairTasks)
        {
            addLines += $"Repair Task: {repairTask.TaskName}\n";
            List<Step> steps = repairTask.Steps;
            foreach (Step step in steps)
            {
                string isDone = step.IsDone ? "Complete" : "Not complete";
                addLines += $"Step {steps.IndexOf(step)}: {isDone}, Notes: {step.Notes}\n";
            }
            addLines += "-------------------------------------\n";
        }
        //add protocol to product lifecycle document to store product history
        System.IO.File.AppendAllText(Application.persistentDataPath + "/lifecycle.txt", addLines);
        //display history
        history.text = System.IO.File.ReadAllText(Application.persistentDataPath + "/lifecycle.txt");        
    }    
}

//level 1 node
public class RepairTask
{
    public string TaskName { get; set; }
    public List<Step> Steps { get; set; }

    public RepairTask(string taskName, List<Step> steps)
    {
        TaskName = taskName;
        Steps = steps;
    }        
}

//level 2 node
public class Step
{
    public string StepText { get; set; }
    public bool IsDone { get; set; }
    public string Notes { get; set; }
    public Texture ScreenshotTex { get; set; }
    public string AnimatorName { get; set; }
    public string AnimationName { get; set; }
    //video

    public Step(string stepText, string animator = null, string animationName = null, bool isDone = false,  string notes = "", Texture screenshotTex = null)
    {
        StepText = stepText;
        IsDone = isDone;
        AnimatorName = animator;
        AnimationName = animationName;
        Notes = notes;
        ScreenshotTex = screenshotTex;
    }
}

