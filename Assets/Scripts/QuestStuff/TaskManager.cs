using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public interface ITask
{
    string TaskName();
    void StartTask();
    bool IsTaskCompleted();
    void StopTask();
}
public class TaskManager : MonoBehaviour
{
    [SerializeField] private TMP_Text taskText;
    [SerializeField] private MonoBehaviour[] taskScripts; // Массив заданий типа MonoBehaviour
    private ITask[] tasks; // Реальный массив интерфейсов для работы
    private int currentTaskIndex = 0;
    private ITask currentActiveTask;

    private void Start()
    {
        tasks = new ITask[taskScripts.Length];
        for (int i = 0; i < taskScripts.Length; i++)
        {
            tasks[i] = taskScripts[i] as ITask;
            taskScripts[i].enabled = false; // Отключаем все скрипты заданий по умолчанию
        }

        if (tasks.Length > 0)
        {
            StartCurrentTask();
        }
        else
        {
            Debug.LogWarning("No tasks assigned in TaskManager.");
        }
    }

    private void Update()
    {
        if (currentTaskIndex < tasks.Length && tasks[currentTaskIndex].IsTaskCompleted())
        {
            Debug.Log($"Task {tasks[currentTaskIndex].TaskName()} completed. Moving to the next task.");
            NextTask();
        }
    }

    private void StartCurrentTask()
    {
        if (currentTaskIndex < tasks.Length)
        {
            // Включаем скрипт текущего задания
            taskScripts[currentTaskIndex].enabled = true;
            currentActiveTask = tasks[currentTaskIndex];  // Обновляем активную задачу
            tasks[currentTaskIndex].StartTask();
            UpdateTaskText();
        }
    }

    private void NextTask()
    {
        taskScripts[currentTaskIndex].enabled = false;
        currentTaskIndex++;

        if (currentTaskIndex < tasks.Length)
        {
            StartCurrentTask();
        }
        else
        {
            taskText.text = "All tasks completed!";
        }
    }


    private void UpdateTaskText()
    {
        taskText.text = $"Task: {tasks[currentTaskIndex].TaskName()}:";
    }
    
    public ITask GetCurrentActiveTask()
    {
        return currentActiveTask;  // Получаем текущее активное задание
    }
    
}
