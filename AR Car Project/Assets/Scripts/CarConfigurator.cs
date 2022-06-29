using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarConfigurator : MonoBehaviour
{
    void Start()
    {
        Car car = new Car();
        car.steering = new Steering();
        car.steering.currVariant = "servu"; 
    }

    //method which returns class car from selected input params
    //input params = what user selects
    //manually name the Text exactly same as variant name
    //return it to show parts and animation

    //method which takes in class car from above method & returns animation/hide-unhide of objects
    //drive display etc of car object

   

}

/*public class Car
{
    public string Color { get; set; }
    public string Engine { get; set; }
    public string Suspension { get; set; }
    
    //public string Steering { get; set; }
    public string Spoiler { get; set; }
    public string Cooling { get; set; }
    public string Turbo { get; set; }

    //constructor with all properties
    public Car(string color, string engine, string suspension, string steering, string spoiler, string cooling, string turbo)
    {
        color = Color;
        engine = Engine;
        suspension = Suspension;
        //steering = Steering;
        spoiler = Spoiler;
        cooling = Cooling;
        turbo = Turbo;
    }

    //default constructor
    public Car()
    {

    }

    Steering steering1 = new Steering();
    

    //create classes for Engine, Suspension etc?
    public class Steering
    {
        public string propertyName { get; set; }
        Dictionary<List<string>, GameObject> variants;
        GameObject nextBtn;
        GameObject prevBtn;

    }
}*/



