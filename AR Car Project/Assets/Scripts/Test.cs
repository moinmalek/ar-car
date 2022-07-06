using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Test : MonoBehaviour
{
    public Animator animator;
    public Button Btn, Btn1, Btn2;
    void Start()
    {
        Btn.onClick.AddListener(()=>Animate("B1"));
        Btn1.onClick.AddListener(() => Animate("B2"));
        Btn2.onClick.AddListener(() => Animate("B3"));
    }

    void Animate(string animation)
    {
        animator.Play(animation);
    }

}
