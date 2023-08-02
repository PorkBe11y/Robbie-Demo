using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class GameManager : MonoBehaviour
{
   static GameManager instace;
   private scenefader fader;
   private List<Orb> orbs;
   private door lockedDoor;

   private float gameTime;
   private bool gameIsOver;
   private bool gameIsRestart;

   //public int orbNum;
   public int deathNum;

   private void Start()
   {
      deathNum = 0;
      gameTime = 0;
   }

   private void Awake()
   {
      if (instace != null)
      {
         Destroy(gameObject);
         return;
      }
      instace = this;

      orbs = new List<Orb>();
      
      DontDestroyOnLoad(gameObject);
   }

   public void Update()
   {
      if (gameIsRestart)
      {
         return;
      }
      if (gameIsOver)
      {
         return;
      }
      
      // orbNum = instace.orbs.Count;
      gameTime += Time.deltaTime;
     //uimanager
     UIManager.UpdateTimeUI(gameTime);
   }
   public static void RegisterSceneFader(scenefader obj)
   {
      instace.fader = obj;
   }

   public static void RegisterDoor(door door)
   {
      instace.lockedDoor = door;
   }
   public static void RegisterOrb(Orb orb)
   {
      if (!instace.orbs.Contains(orb))
      {
         instace.orbs.Add(orb);
      }
      UIManager.UpdateOrbUI(instace.orbs.Count);
   }

   public static void PlayerGrabbedOrb(Orb orb)
   {
      if (!instace.orbs.Contains(orb))
      {
         return;
      }
      instace.orbs.Remove(orb);

      if (instace.orbs.Count == 0)
      {
         instace.lockedDoor.Open();
      }
      UIManager.UpdateOrbUI(instace.orbs.Count);
   }
   
   public static void Playerwon()
   {
      instace.gameIsOver = true;
      //ui gameover
      //UIManager.DisplayGameOver();
      AudioManager.PPlayerWonAudio();
      instace.Invoke("GameOverRestart",2f);
   }
   
   public static bool GameOver()
   {
      return instace.gameIsOver;
   }
   public static void PlayerDied()
   {
      instace.fader.FadeOut();
      instace.deathNum++;
      UIManager.UpdateDeathUI(instace.deathNum);
      instace.Invoke("RestartScene",1.5f);
   }

   public static void Restart()
   {
      instace.gameIsOver = false;
      instace.fader.FadeOut();
      instace.Invoke("RestartScene",1.5f);
   }
   void RestartScene()
   {
      instace.orbs.Clear();
      SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
   }

   void GameOverRestart()
   {
      instace.orbs.Clear();
      float time = 0;
      SceneManager.LoadScene("end");
   }
}

