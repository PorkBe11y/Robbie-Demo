using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scenefader : MonoBehaviour
{
    Animator anim;
    private int faderID;

    private void Start()
    {
        anim = GetComponent<Animator>();
        faderID = Animator.StringToHash("Fade");
        GameManager.RegisterSceneFader(this);
    }

    public void FadeOut()
    {
        anim.SetTrigger(faderID);
    }
}
