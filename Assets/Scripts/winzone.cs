using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class winzone : MonoBehaviour
{
    private int playerLayer;
    void Start()
    {
        playerLayer = LayerMask.NameToLayer("player");
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == playerLayer)
        {
           Debug.Log("你牛逼");
           
           GameManager.Playerwon();
        }
    }


}
