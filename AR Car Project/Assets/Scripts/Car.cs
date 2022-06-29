using System.Collections.Generic;
using UnityEngine;

public class Car //starts only when MainPage is activated => derives void Start() {info from last page} & creates a car
{
    public Steering steering { get; set; } //single steering

    public GameObject Get3DModel(string currVariant)
    {
        return null;
    }
   
}

public class Steering
{
    public string currVariant; 
}