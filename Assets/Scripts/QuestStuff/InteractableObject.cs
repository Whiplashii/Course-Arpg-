using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public static int ActivatedCount { get; private set; } = 0; // Counter for activated objects

    private bool playerInRange = false;
    private bool isActivated = false;
    private KillCounter killCounter;
    private TaskManager taskManager;  // Ссылка на TaskManager для получения активного задания

    private void Start()
    {
        taskManager = FindObjectOfType<TaskManager>();  // Получаем ссылку на TaskManager
        killCounter = FindObjectOfType<KillCounter>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !isActivated)
        {
            isActivated = true;

            // Получаем текущее активное задание
            ITask currentTask = taskManager.GetCurrentActiveTask();

            // Проверяем, если текущее задание — это KillCounter
            if (currentTask is KillCounter currentKillCounter)
            {
                currentKillCounter.InteractableActivated();  // Вызываем InteractableActivated только для активного задания
            }

            ActivateObject();
        }
    }

    private void ActivateObject()
    {
        ActivatedCount++;
        Debug.Log("Object Activated! Total Activations: " + ActivatedCount);
        // Add more functionality here, like animations or effects

        // Optional: Destroy the object after activation
        Destroy(gameObject);
    }
}
