using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarConfigurator : MonoBehaviour
{
    //define different properties
    List<string> colors = new List<string>() { "Blue", "Red", "Green", "Yellow" };
    List<string> engines = new List<string>() { "Engine1", "Engine2" };
    List<string> steerings = new List<string>() { "Ackerman", "Anti-Ackerman" };
    List<string> suspensions = new List<string>() { "Semi-dependent", "Independent" };
    List<string> spoilers = new List<string>() { "Without", "Rear-Wing" };
    List<string> coolings = new List<string>() { "Radiator1", "Radiator2" };
    List<string> turbos = new List<string>() { "Single", "Double" };   
    
    void Start()
    {
        //all variants together 
        List<List<string>> variants = new List<List<string>>() { colors, engines, steerings, suspensions, spoilers, coolings, turbos };
        
        foreach (List<string> variant in variants)
        {
            foreach(string property in variant)
            { 
                Debug.Log(property); 
            }
        }
    }

    void Update()
    {
        
    }

    //method which returns class car from selected input params
    //input params = what user selects
    //manually name the Text exactly same as variant name
    //return it to show parts and animation
    public Car ReturnCar()
    {
        Car car = new Car();
        return car;
    }

    //method which takes in class car from above method & returns animation/hide-unhide of objects
    //drive display etc of car object

}

public class Car
{
    public string Color { get; set; }
    public string Engine { get; set; }
    public string Suspension { get; set; }
    public string Steering { get; set; }
    public string Spoiler { get; set; }
    public string Cooling { get; set; }
    public string Turbo { get; set; }

    //constructor with all properties
    public Car(string color, string engine, string suspension, string steering, string spoiler, string cooling, string turbo)
    {
        color = Color;
        engine = Engine;
        suspension = Suspension;
        steering = Steering;
        spoiler = Spoiler;
        cooling = Cooling;
        turbo = Turbo;
    }

    //default constructor
    public Car()
    {

    }
}


