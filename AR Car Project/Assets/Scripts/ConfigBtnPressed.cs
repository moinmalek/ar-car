using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigBtnPressed : MonoBehaviour
{
    public Text propertyText;
    public Text variantText;
    public List<string> variants;    
    public Button nextBtn;
    public Button prevBtn;

    void Start()
    {        
        //display property text as objects name
        propertyText.text = transform.name;
        //Initialize variant text e.g. "Choose steering"
        variantText.text = "Choose " + propertyText.text.ToLower();

        nextBtn.onClick.AddListener(() => { NextBtnPressed(variants, variantText); });
        //if next button pressed change the variant not just text
        //hide all other gameobjects from list variants and keep only currVariant
        //must apply to all panels

        prevBtn.onClick.AddListener(() => { PrevBtnPressed(variants, variantText); });
    }

    void NextBtnPressed(List<string> variants, Text variantText)
    {
        int currVarIdx = variants.IndexOf(variantText.text);

        if (currVarIdx >= variants.Count - 1)
        {
            currVarIdx = 0;
        }
        else
        {
            currVarIdx++;
        }

        variantText.text = variants[currVarIdx];
        //variantModel.SetActive(true) wont work coz object is inactive
        //Solution1: scale object to zero so deactivation not required
        // Hide button
        GameObject.Find("Ackerman").transform.localScale = new Vector3(0, 0, 0);
        // Show button
        //GameObject.Find("Ackerman").transform.localScale = new Vector3(1, 1, 1);
        //Solution2: renderer deactivate

        //update 3D Model
        foreach(string variant in variants)
        {
            GameObject variantModel = GameObject.Find(variant);
            if (variant == variantText.text)
            {
                variantModel.transform.localScale = new Vector3(1,1,1);
            }
            else
            {
                variantModel.transform.localScale = new Vector3(0,0,0);
            }
        }
    }

    void PrevBtnPressed(List<string> variants, Text variantText)
    {
        int currVarIdx = variants.IndexOf(variantText.text);

        if (currVarIdx <= 0)
        {
            currVarIdx = variants.Count - 1;
        }
        else
        {
            currVarIdx--;
        }

        variantText.GetComponent<Text>().text = variants[currVarIdx];

        foreach (string variant in variants)
        {
            GameObject variantModel = GameObject.Find(variant);
            if (variant == variantText.text)
            {
                variantModel.transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                variantModel.transform.localScale = new Vector3(0, 0, 0);
            }
        }
    }
}
