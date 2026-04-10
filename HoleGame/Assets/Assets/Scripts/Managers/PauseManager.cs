using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
  public static PauseManager Instance { get; private set; }

  private bool isPaused = false;
  public bool IsPaused => isPaused;
  [Header("UI Елементи")]
  [SerializeField] private GameObject pauseMenu; // Панель меню паузи
  private void Awake()
  {
    if (Instance != null && Instance != this)
    {
      Destroy(gameObject);
    }
    else
    {
      Instance = this;
      DontDestroyOnLoad(gameObject);
    }
  }


  private void Update()
  {
    // Гаряча клавіша для паузи (Esc або P)
    if (Input.GetKeyDown(KeyCode.P))
    {
      TogglePause();
    }
  }


  public void TogglePause()
  {
    isPaused = !isPaused;

    if (isPaused)
    {
      Time.timeScale = 0f;          // Зупиняємо час
      pauseMenu.SetActive(true);     // Показуємо меню
      Debug.Log("Game Paused");
    }
    else
    {
      Time.timeScale = 1f;          // Повертаємо час
      pauseMenu.SetActive(false);    // Ховаємо меню
      Debug.Log("Game Resumed");
    }
  }

  // Окремі методи, якщо захочеш прив'язати їх до конкретних кнопок (Resume/Pause)
  public void ResumeGame()
  {
    if (isPaused) TogglePause();
  }
}
