using UnityEngine;
using System.IO;

public class QuitManager : MonoBehaviour
{
    public void QuitGame()
    {
        //UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
}
