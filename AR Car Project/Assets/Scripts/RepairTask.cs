using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RepairTask
{
    public string TaskName { get; set; }
    public List<Step> Steps { get; set; }

    public RepairTask(string taskName)
    {
        TaskName = taskName;
    }

    public RepairTask()
    {

    }

    //method to generate protocol
    //generate protocol showed only when closed button clicked. Close button clicked => invoke nextBtn to update saved infos
    public void GenerateProtocol()
    {
        Debug.Log(TaskName);
        foreach (Step step in Steps)
        {            
            Debug.Log($"{Steps.IndexOf(step)} {step.IsDone} {step.Notes}");
        }
    }
}
public class Step
{
    public string StepText { get; set; }
    public bool IsDone { get; set; }
    public string Notes { get; set; }
    public RawImage Screenshot { get; set; }
    //video
    //animation

    public Step(string stepText, bool isDone = false, string notes = "", RawImage screenshot = null)
    {
        StepText = stepText;
        IsDone = isDone;
        Notes = notes;
        Screenshot = screenshot;
    }
}

