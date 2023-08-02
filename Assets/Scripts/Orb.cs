using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb : MonoBehaviour
{
   int player;
   public GameObject explosionVFXPrefab;

    void Start()
    {
        player = LayerMask.NameToLayer("player");
        GameManager.RegisterOrb(this);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == player)
        {
            Instantiate(explosionVFXPrefab, transform.position, transform.rotation);
            gameObject.SetActive(false);
            AudioManager.PlayOrbAudio();
            GameManager.PlayerGrabbedOrb(this);
        }
    }

}
