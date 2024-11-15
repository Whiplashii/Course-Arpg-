using UnityEngine;

public class DropController : MonoBehaviour
{
    [SerializeField] private Human target;
    [SerializeField] private ItemSO itemToDrop;
    private TaskManager taskManager;

    private void Start()
    {
        taskManager = FindObjectOfType<TaskManager>();  // Получаем ссылку на TaskManager
        target.OnDied += Target_OnDied;
    }

    private void Target_OnDied(Human obj)
    {
        ITask currentTask = taskManager.GetCurrentActiveTask();  // Получаем текущее активное задание

        if (currentTask is KillCounter killCounter)  // Проверяем, если это задание KillCounter
        {
            killCounter.EnemyKilled();  // Вызываем EnemyKilled из правильного задания
        }
        else if(currentTask is ThirdTask thirdTask)
        {
            thirdTask.EnemyKilled();
        }

        if (itemToDrop != null)
        {
            DropManager.Instance.DropItem(itemToDrop, target.transform.position);
            Debug.Log("Drop triggered");
        }
        else
        {
            Debug.Log("ItemSO для дропа не назначен.");
        }
    }
}
