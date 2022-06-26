using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarConfigurator : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    //method which returns class car from selected input params
    //input params = what user selects
    //manually name the Text exactly same as variant name
    //return it to show parts and animation
    
    //method which takes in class car from above method & returns animation/hide-unhide of objects
    //drive display etc of car object


    class Car
    {
        public string color, engine, suspension, steering, spoiler, cooling;
        public int turbo;

        public Car()
        {

        }
    }
}
