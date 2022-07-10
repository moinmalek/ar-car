using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class IoT : MonoBehaviour
{
    public Button bckBtn, fwdBtn, midBtn;
    public Button okBtn, closeStepBtn;
    public Animator animator;

    //device token
    private string token = "CFy5FiOPmded4t69LfepCaRi83FWaY0Y";
    private string getUrl = "https://blynk.cloud/external/api/get?token=";
    private string updateUrl = "https://blynk.cloud/external/api/update?token=";
    private int sensorVal = 100;

    void Start()
    {
        //turn on brake lights or v1 = 1
        bckBtn.onClick.AddListener(delegate { StartCoroutine(UpdateValue($"{updateUrl}{token}&v1=1")); });
        //turn off brake lights or v1 = 0
        fwdBtn.onClick.AddListener(delegate { StartCoroutine(UpdateValue($"{updateUrl}{token}&v1=0")); });
        midBtn.onClick.AddListener(CloseDoor);
        //read sensor values
        okBtn.onClick.AddListener(delegate { StartCoroutine(GetValue($"{getUrl}{token}&v14")); });
        closeStepBtn.onClick.AddListener(delegate { StartCoroutine(GetValue($"{getUrl}{token}&v14")); });
    }

    IEnumerator UpdateValue(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();            
        }
    }

    public IEnumerator GetValue(string uri)
    {
        while(true)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                yield return webRequest.SendWebRequest();
                sensorVal = Int32.Parse(webRequest.downloadHandler.text);
                Debug.Log(sensorVal);
            }
            OpenDoor();
            yield return new WaitForSeconds(0.5f);
        }                      
    }

    void OpenDoor()
    {
        if (sensorVal < 50)
        {
            animator.Play("DoorLOpen");
        }                      
    }

    void CloseDoor()
    {
        animator.Play("DoorLOpenrev");
    }
}
