using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DriveCar : MonoBehaviour
{
    public Button FwdBtn, BckBtn, RightBtn, LeftBtn;
    public GameObject frontWheelL;//, frontWheelR, rearWheelL, rearWheelR;
    //public GameObject steeringWheel;
    //public GameObject brakeLights;

    public int velocity = 10;

    void Start()
    {
        FwdBtn.onClick.AddListener(GoFwd);
        BckBtn.onClick.AddListener(GoBck);
    }
    
    void GoFwd()
    {
        //frontWheelL.transform.Rotate(new Vector3(0,1,0), -velocity * Time.deltaTime);
        frontWheelL.GetComponent<Rigidbody>().AddTorque(velocity, 0, 0);
    }

    void GoBck()
    {
        //frontWheelL.GetComponent<Rigidbody>().AddTorque(-velocity, 0, 0);
    }


}
