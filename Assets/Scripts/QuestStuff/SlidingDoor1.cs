using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;  // Подключаем TMP для работы с текстом

public class SlidingDoor1 : MonoBehaviour
{
    [SerializeField] private Transform leftWall; // Левая часть двери
    [SerializeField] private Transform rightWall; // Правая часть двери
    [SerializeField] private Vector3 openOffset;  // Смещение для открывания
    [SerializeField] private float openSpeed = 2f; // Скорость открытия
    [SerializeField] private TMP_Text notificationText; // Текстовое поле для уведомления

    private bool isOpen = false;
    private KeyPressTask KeyPressTask;

    private void Start()
    {
        // Находим KillCounter в сцене
        KeyPressTask = FindObjectOfType<KeyPressTask>();
    }

    private void Update()
    {
        // Проверка, выполнено ли условие для открытия двери
        if (!isOpen && KeyPressTask.IsTaskCompleted())
        {
            isOpen = true;
            StartCoroutine(OpenDoors());
            Debug.Log("Открываю дверку");
        }
    }

    private IEnumerator OpenDoors()
    {
        Vector3 leftTargetPos = leftWall.position - openOffset;
        Vector3 rightTargetPos = rightWall.position + openOffset;

        // Перемещаем стены, пока не достигнем целевого положения
        while (Vector3.Distance(leftWall.position, leftTargetPos) > 0.01f && Vector3.Distance(rightWall.position, rightTargetPos) > 0.01f)
        {
            leftWall.position = Vector3.MoveTowards(leftWall.position, leftTargetPos, openSpeed * Time.deltaTime);
            rightWall.position = Vector3.MoveTowards(rightWall.position, rightTargetPos, openSpeed * Time.deltaTime);
            yield return null;
        }

        // Показываем сообщение, что дверь открылась
        ShowDoorOpenedMessage();
    }

    private void ShowDoorOpenedMessage()
    {
        if (notificationText != null)
        {
            notificationText.text = "Где-то открылась дверь!"; // Сообщение
            notificationText.gameObject.SetActive(true); // Показываем текст

            // Ожидаем 3 секунды, затем скрываем сообщение
            StartCoroutine(HideMessageAfterDelay(3f));
        }
    }

    private IEnumerator HideMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Задержка перед скрытием
        notificationText.gameObject.SetActive(false); // Скрываем текст
    }
}
