using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigCar : MonoBehaviour
{
    public Text propertyText;
    public Text variantText;
    public List<string> variants;    
    public Button nextBtn;
    public Button prevBtn;
    public Material bodyMat;

    void Start()
    {   
        //display property text as objects name
        propertyText.text = transform.name;
        //initialize variant text e.g. "Choose steering"
        variantText.text = "Choose " + propertyText.text.ToLower();

        bool isPropertyColor = propertyText.text == "Color";

        nextBtn.onClick.AddListener(() => 
        { string newVariant = NextBtnPressed(variants, variantText);
          ChangeVariant(variants, newVariant, isPropertyColor);
        });

        prevBtn.onClick.AddListener(() => 
        { string newVariant = PrevBtnPressed(variants, variantText);
            ChangeVariant(variants, newVariant, isPropertyColor);
        });
    }
    
    //display next variant text
    string NextBtnPressed(List<string> variants, Text variantText)
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

        return variantText.text;
    }

    //display previous variant text
    string PrevBtnPressed(List<string> variants, Text variantText)
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

        return variantText.text;        
    }

    //change variant to current variant displayed
    void ChangeVariant(List<string> variants, string newVariant, bool isColor)
    {
        if(!isColor)
        {
            foreach (string variant in variants)
            {
                GameObject variantModel = GameObject.Find(variant);
                if (variant == newVariant)
                {
                    //display new variant selected
                    variantModel.transform.localScale = new Vector3(1, 1, 1);
                }
                else
                {
                    //hide all other variants
                    variantModel.transform.localScale = new Vector3(0, 0, 0);
                }
            }
        }
        else
        {              
            string currColorText = newVariant.ToLower();
            Color currColor; 

            switch(currColorText)
            {
                case "blue":
                    currColor = new Color32(45, 190, 240, 70);
                    break;
                case "rose gold":
                    currColor = new Color32(240, 155, 125, 70);
                    break;
                case "goblin":
                    currColor = new Color32(45, 170, 75, 70);
                    break;
                case "olive":
                    currColor = new Color32(180, 190, 50, 70);
                    break;
                case "silver":
                    currColor = new Color32(140, 170, 230, 70);
                    break;
                case "rosa":
                    currColor = new Color32(230, 140, 165, 70);
                    break;
                case "golden":
                    currColor = new Color32(230, 210, 140, 70);
                    break;
                case "green":
                    currColor = new Color32(150, 230, 140, 70);
                    break;
                case "cyan":
                    currColor = new Color32(140, 230, 220, 70);
                    break;
                case "sky":
                    currColor = new Color32(140, 200, 230, 70);
                    break;
                case "violet":
                    currColor = new Color32(170, 140, 230, 70);
                    break;
                case "white":
                    currColor = new Color32(255, 255, 255, 70);
                    break;
                default:
                    currColor = new Color32(45, 190, 240, 70);
                    break;
            }              

            //change material color            
            bodyMat.SetColor("_Color", currColor);
        }        
    }
}
