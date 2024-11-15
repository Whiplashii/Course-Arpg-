using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Метод для кнопки "Играть"
    public void PlayGame()
    {
        // Загрузка основной сцены (замените "MainScene" на имя вашей основной сцены)
        SceneManager.LoadScene("SampleScene");
    }

    // Метод для кнопки "Выход"
    public void ExitGame()
    {
        // Завершение работы приложения
        Debug.Log("Game is exiting...");
        Application.Quit();
    }
}

