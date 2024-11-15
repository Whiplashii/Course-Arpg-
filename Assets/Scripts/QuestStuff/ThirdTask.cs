using UnityEngine;
using TMPro;

public class ThirdTask : MonoBehaviour, ITask
{
    [SerializeField] private TMP_Text killCounterText;
    [SerializeField] private string Tag;  
    private int killedEnemies = 0;
    private int totalEnemies;

    public string TaskName()
    {
        return "Kills";
    }

    private bool taskStarted = false;

    public void StartTask()
    {
        Debug.Log("StartTask called for third.");

        if (!taskStarted)
        {
            totalEnemies = 0;
            foreach (var human in FindObjectsOfType<Human>())
            {
                if (human.CompareTag(Tag))
                {
                    totalEnemies++;
                }
            }
            Debug.Log($"Total Enemies at start: {totalEnemies}");

            taskStarted = true;
        }

        UpdateUI();
    }

    public bool IsTaskCompleted()
    {
        return killedEnemies >= totalEnemies;
    }

    public void EnemyKilled()
    {
        killedEnemies++;
        Debug.Log($"Enemy killed. Progress: {killedEnemies}/{totalEnemies}");
        UpdateUI();
    }

    private float GetKillCompletionPercentage()
    {
        return totalEnemies > 0 ? (killedEnemies / (float)totalEnemies) * 100 : 0;
    }

    private void UpdateUI()
    {
        Debug.Log($"Total Enemies after UpdateUi {totalEnemies}");
        float killPercentage = GetKillCompletionPercentage();

        killCounterText.text = 
                               $"Enemies killed: {killedEnemies}/{totalEnemies} ({killPercentage:F1}%)";
    }

    public override string ToString()
    {
        float killPercentage = GetKillCompletionPercentage();

        return 
               $"Enemies killed: {killedEnemies}/{totalEnemies} ({killPercentage:F1}%)";
    }

    public bool ObjectivesDone()
    {
        return killedEnemies == totalEnemies && totalEnemies != 0;
    }
    public void StopTask()
{
    Debug.Log("Stopping 3task.");
    taskStarted = false;

    // Обнулите прогресс
    killedEnemies = 0;

    
}
}
