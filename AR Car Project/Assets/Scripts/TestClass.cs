public class Task
{
    public string name;
    public SubTask subtask;
}
public class SubTask
{
    public string name;
    public string Surname { get; set; }

    public SubTask(string name, string surname)
    {
        this.name = name;
        Surname = surname;
    }
}
