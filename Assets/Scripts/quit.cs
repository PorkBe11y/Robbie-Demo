using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class quit : MonoBehaviour
{
  
    public void Restart()
    {
        SceneManager.LoadScene("SampleScene");
        GameManager.Restart();
    }

    public void Close()
    {
        Application.Quit();
    }
}
