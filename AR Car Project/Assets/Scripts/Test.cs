using System.Collections.Generic;
using UnityEngine;


public class Test : MonoBehaviour
{
    void Start()
    {
        Task task1 = new Task();
        task1.name = "myTask";

        SubTask subtask1 = new SubTask("myName", "mySurname");
        //subtask1.name = "mySubTask";

        task1.subtask = subtask1;

        Debug.Log(task1.subtask.Surname);
    }

}
