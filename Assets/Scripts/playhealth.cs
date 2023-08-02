using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class playhealth : MonoBehaviour
{
    public GameObject deathVFXPrefab;
    public GameObject deathSmokeVFXPrefab;
    int trapsLayer;
    
    
    void Start()
    {
        trapsLayer = LayerMask.NameToLayer("traps");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == trapsLayer)
        {

            Instantiate(deathSmokeVFXPrefab, transform.position, transform.rotation);
            
            Instantiate(deathVFXPrefab, transform.position, Quaternion.Euler(0,0,Random.Range(-45,90)));
           
            gameObject.SetActive(false);  
            
            AudioManager.PlayDeathAudio();

            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            
            GameManager.PlayerDied();
        }
    }

}
