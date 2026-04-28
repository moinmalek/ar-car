using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;

/// <summary>
/// Marks domain objects that belong to the repair protocol graph (issue #8).
/// </summary>
public interface IProtocolEntity { }

/// <summary>
/// Root protocol aggregate (level 0).
/// </summary>
[Serializable]
public sealed class Protocol : IProtocolEntity
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
        RepairTasks = repairTasks ?? new List<RepairTask>();
    }

    public void GenerateHistory(List<RepairTask> repairTasks, TextMeshProUGUI history)
    {
        if (history == null)
            return;

        var path = ProtocolPersistence.LifecycleFilePath;
        var sb = new System.Text.StringBuilder(512);
        sb.Append(ProtocolName).Append('\n');
        sb.Append("Technician: ").Append(Technician).Append("\nLocation: ").Append(Location)
            .Append("\nProblem: ").Append(Problem).Append("\nConclusion: ").Append(Conclusion).Append('\n');
        sb.Append("==========================\n");

        for (int r = 0; r < repairTasks.Count; r++)
        {
            RepairTask repairTask = repairTasks[r];
            sb.Append("Repair Task: ").Append(repairTask.TaskName).Append('\n');
            List<Step> steps = repairTask.Steps;
            for (int i = 0; i < steps.Count; i++)
            {
                Step step = steps[i];
                string isDone = step.IsDone ? "Complete" : "Not complete";
                sb.Append("Step ").Append(i).Append(": ").Append(isDone).Append(", Notes: ").Append(step.Notes).Append('\n');
            }
            sb.Append("-------------------------------------\n");
        }

        try
        {
            File.AppendAllText(path, sb.ToString());
            history.text = File.ReadAllText(path);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Could not write lifecycle history: {e.Message}");
        }
    }
}

/// <summary>
/// Single repair task with ordered steps (level 1).
/// </summary>
[Serializable]
public sealed class RepairTask : IProtocolEntity
{
    public string TaskName { get; set; }
    public List<Step> Steps { get; set; }

    public RepairTask(string taskName, List<Step> steps)
    {
        TaskName = taskName;
        Steps = steps ?? new List<Step>();
    }
}

/// <summary>
/// One instruction step with optional AR animation binding (level 2).
/// </summary>
[Serializable]
public sealed class Step : IProtocolEntity
{
    public string StepText { get; set; }
    public bool IsDone { get; set; }
    public string Notes { get; set; }
    /// <summary>Runtime screenshot; not written by JsonUtility — persisted via <see cref="ProtocolPersistence"/>.</summary>
    public Texture2D ScreenshotTex { get; set; }
    public string AnimatorName { get; set; }
    public string AnimationName { get; set; }

    public Step(string stepText, string animator = null, string animationName = null, bool isDone = false, string notes = "", Texture2D screenshotTex = null)
    {
        StepText = stepText;
        IsDone = isDone;
        AnimatorName = animator;
        AnimationName = animationName;
        Notes = notes ?? "";
        ScreenshotTex = screenshotTex;
    }
}

/// <summary>
/// JSON file persistence for <see cref="Protocol"/> and step screenshot files (issue #5).
/// </summary>
public static class ProtocolPersistence
{
    public static string LifecycleFilePath => Path.Combine(Application.persistentDataPath, "lifecycle.txt");
    static string ProtocolFilePath => Path.Combine(Application.persistentDataPath, "protocol_save.json");
    static string ScreenshotsDir => Path.Combine(Application.persistentDataPath, "protocol_screenshots");

    [Serializable]
    sealed class FileDto
    {
        public string protocolName;
        public string technician;
        public string location;
        public string problem;
        public string conclusion;
        public RepairTaskDto[] repairTasks;
    }

    [Serializable]
    sealed class RepairTaskDto
    {
        public string taskName;
        public StepDto[] steps;
    }

    [Serializable]
    sealed class StepDto
    {
        public string stepText;
        public bool isDone;
        public string notes;
        public string animatorName;
        public string animationName;
        public string screenshotFile;
    }

    public static void Save(Protocol protocol)
    {
        if (protocol == null)
            return;

        Directory.CreateDirectory(ScreenshotsDir);

        var dto = new FileDto
        {
            protocolName = protocol.ProtocolName,
            technician = protocol.Technician,
            location = protocol.Location,
            problem = protocol.Problem,
            conclusion = protocol.Conclusion,
            repairTasks = new RepairTaskDto[protocol.RepairTasks.Count]
        };

        for (int r = 0; r < protocol.RepairTasks.Count; r++)
        {
            RepairTask rt = protocol.RepairTasks[r];
            var taskDto = new RepairTaskDto
            {
                taskName = rt.TaskName,
                steps = new StepDto[rt.Steps.Count]
            };

            for (int s = 0; s < rt.Steps.Count; s++)
            {
                Step step = rt.Steps[s];
                string shotFile = null;
                if (step.ScreenshotTex != null)
                {
                    shotFile = $"r{r}_s{s}.png";
                    string full = Path.Combine(ScreenshotsDir, shotFile);
                    try
                    {
                        byte[] png = step.ScreenshotTex.EncodeToPNG();
                        File.WriteAllBytes(full, png);
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning($"Failed to save screenshot {full}: {e.Message}");
                        shotFile = null;
                    }
                }

                taskDto.steps[s] = new StepDto
                {
                    stepText = step.StepText,
                    isDone = step.IsDone,
                    notes = step.Notes ?? "",
                    animatorName = step.AnimatorName,
                    animationName = step.AnimationName,
                    screenshotFile = shotFile
                };
            }

            dto.repairTasks[r] = taskDto;
        }

        try
        {
            File.WriteAllText(ProtocolFilePath, JsonUtility.ToJson(dto, true));
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Protocol save failed: {e.Message}");
        }
    }

    public static Protocol LoadOrCreate()
    {
        if (!File.Exists(ProtocolFilePath))
            return new Protocol($"P {DateTime.Now}", new List<RepairTask>());

        try
        {
            string json = File.ReadAllText(ProtocolFilePath);
            var dto = JsonUtility.FromJson<FileDto>(json);
            if (dto == null || dto.repairTasks == null)
                return new Protocol($"P {DateTime.Now}", new List<RepairTask>());

            var tasks = new List<RepairTask>(dto.repairTasks.Length);
            for (int r = 0; r < dto.repairTasks.Length; r++)
            {
                RepairTaskDto td = dto.repairTasks[r];
                var steps = new List<Step>(td.steps?.Length ?? 0);
                if (td.steps != null)
                {
                    for (int s = 0; s < td.steps.Length; s++)
                    {
                        StepDto sd = td.steps[s];
                        var step = new Step(sd.stepText, sd.animatorName, sd.animationName, sd.isDone, sd.notes ?? "");
                        if (!string.IsNullOrEmpty(sd.screenshotFile))
                        {
                            string full = Path.Combine(ScreenshotsDir, sd.screenshotFile);
                            if (File.Exists(full))
                            {
                                byte[] data = File.ReadAllBytes(full);
                                var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                                tex.name = sd.screenshotFile;
                                if (tex.LoadImage(data))
                                    step.ScreenshotTex = tex;
                                else
                                    UnityEngine.Object.Destroy(tex);
                            }
                        }
                        steps.Add(step);
                    }
                }
                tasks.Add(new RepairTask(td.taskName, steps));
            }

            var p = new Protocol(dto.protocolName ?? $"P {DateTime.Now}", tasks)
            {
                Technician = dto.technician,
                Location = dto.location,
                Problem = dto.problem,
                Conclusion = dto.conclusion
            };
            return p;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Protocol load failed, using new protocol: {e.Message}");
            return new Protocol($"P {DateTime.Now}", new List<RepairTask>());
        }
    }
}
