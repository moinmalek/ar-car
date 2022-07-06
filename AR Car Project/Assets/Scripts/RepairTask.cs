using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//top node (level 0)
public class Protocol
{
    public string ProtocolName { get; set; }
    public List<RepairTask> RepairTasks { get; set; }

    public Protocol(string protocolName, List<RepairTask> repairTasks)
    {
        ProtocolName = protocolName;
        RepairTasks = repairTasks;
    }
    
    public void GenerateProtocol(List<RepairTask> repairTasks)
    {
        string addLines = ProtocolName + "\n";
        foreach (RepairTask repairTask in repairTasks)
        {
            addLines += repairTask.TaskName + "\n";
            List<Step> steps = repairTask.Steps;
            foreach (Step step in steps)
            {
                addLines += $"{steps.IndexOf(step)} {step.IsDone} {step.Notes}\n";
            }
        }
        Debug.Log(addLines);

        //add protocol to product lifecycle document which stores product history
        System.IO.File.AppendAllText("lifecycle.txt", addLines);
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
    public RawImage Screenshot { get; set; }
    public string AnimatorName { get; set; }
    public string AnimationName { get; set; }
    //video

    public Step(string stepText, string animator = null, string animationName = null, bool isDone = false,  string notes = "", RawImage screenshot = null)
    {
        StepText = stepText;
        IsDone = isDone;
        AnimatorName = animator;
        AnimationName = animationName;
        Notes = notes;
        Screenshot = screenshot;
    }
}

