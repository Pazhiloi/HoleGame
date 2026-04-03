using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestManager : MonoBehaviour
{
  public static TestManager Instance { get; private set; }

  private void Awake()
  {
    // Реалізація паттерна Сінглтон
    if (Instance != null && Instance != this)
    {
      Destroy(gameObject); // Видаляємо дублікат, якщо він з'явився
    }
    else
    {
      Instance = this;
      DontDestroyOnLoad(gameObject); // Розкоментуйте, якщо об'єкт має жити при переході між сценами
    }
  }

  void Update()
  {
    // Перевірка натискання клавіші R
    if (Input.GetKeyDown(KeyCode.R))
    {
      RestartLevel();
    }
  }

  public void RestartLevel()
  {
    // Отримуємо назву поточної активної сцени
    string currentSceneName = SceneManager.GetActiveScene().name;

    // Завантажуємо її заново
    SceneManager.LoadScene(currentSceneName);

    Debug.Log("Сцена перезавантажена: " + currentSceneName);
  }
}
