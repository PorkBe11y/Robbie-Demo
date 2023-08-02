using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class door : MonoBehaviour
{
    Animator anim;
    int onenID;
    void Start()
    {
        anim = GetComponent<Animator>();
        onenID = Animator.StringToHash("Open");
        //注册门
        GameManager.RegisterDoor(this);
    }

    public void Open()
    {
        anim.SetTrigger(onenID);
        //放音乐
        AudioManager.PlayDoorOpenAudio();
    }
}
