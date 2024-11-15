using UnityEngine;
using TMPro;


public class KillCounter : MonoBehaviour, ITask
{
    [SerializeField] private TMP_Text killCounterText;
    [SerializeField] private string Tag;  
    private int killedEnemies = 0;
    private int activatedInteractables = 0;
    private int totalEnemies;
    private int totalInteractables;

    public string TaskName()
    {
        return "Kills and Objectives";
    }

 private bool taskStarted = false;

public void StartTask()
{
    Debug.Log("StartTask called for KillCounter.");

    if (!taskStarted)
    {
        killedEnemies = 0;
        activatedInteractables = 0; // Сброс прогресса объектов
        totalEnemies = 0;

        foreach (var human in FindObjectsOfType<Human>())
        {
            if (human.CompareTag(Tag))
            {
                totalEnemies++;
            }
        }

        totalInteractables = 0;
        foreach (var interactable in FindObjectsOfType<InteractableObject>())
        {
            if (interactable.CompareTag(Tag))
            {
                totalInteractables++;
            }
        }

        taskStarted = true;
    }

    UpdateUI();
}

    public bool IsTaskCompleted()
    {
        return killedEnemies >= totalEnemies && activatedInteractables >= totalInteractables;
    }

    public void EnemyKilled()
    {
        if (!taskStarted) return; // Игнорируем вызов, если задание уже завершено
        killedEnemies++;
        Debug.Log($"Enemy killed. Progress: {killedEnemies}/{totalEnemies}");
        UpdateUI();
    }

    public void InteractableActivated()
    {
        activatedInteractables++;
        UpdateUI();
    }

    private float GetKillCompletionPercentage()
    {
        return totalEnemies > 0 ? (killedEnemies / (float)totalEnemies) * 100 : 0;
    }

    private float GetInteractableCompletionPercentage()
    {
        return totalInteractables > 0 ? (activatedInteractables / (float)totalInteractables) * 100 : 0;
    }

    private void UpdateUI()
    {
        Debug.Log($"Total Enemies after UpdateUi {totalEnemies}");
        float killPercentage = GetKillCompletionPercentage();
        float interactablePercentage = GetInteractableCompletionPercentage();

        killCounterText.text = 
                               $"Enemies killed: {killedEnemies}/{totalEnemies} ({killPercentage:F1}%)\n" +
                               $"Objects activated: {activatedInteractables}/{totalInteractables} ({interactablePercentage:F1}%)";
    }

    public override string ToString()
    {
        float killPercentage = GetKillCompletionPercentage();
        float interactablePercentage = GetInteractableCompletionPercentage();
        
        return 
               $"Enemies killed: {killedEnemies}/{totalEnemies} ({killPercentage:F1}%)\n" +
               $"Objects activated: {activatedInteractables}/{totalInteractables} ({interactablePercentage:F1}%)";
    }


    public bool ObjectivesDone()
    {
        return killedEnemies == totalEnemies && activatedInteractables == totalInteractables && totalEnemies != 0;
    }

    public void StopTask()
{
    Debug.Log("Stopping KillCounter task.");
    taskStarted = false;

    // Обнулите прогресс
    killedEnemies = 0;
    activatedInteractables = 0;

    // Очистите UI, если нужно
    
}

    
}