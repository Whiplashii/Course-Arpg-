using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KeyPressTask : MonoBehaviour, ITask
{
    public string TaskName()
    {
        return "Basics";
    }
    [SerializeField] private TMP_Text counterText;

    private Dictionary<KeyCode, bool> keyPresses = new Dictionary<KeyCode, bool>()
    {
        { KeyCode.W, false },
        { KeyCode.A, false },
        { KeyCode.S, false },
        { KeyCode.D, false },
        { KeyCode.Tab, false },
        { KeyCode.Space, false },
        { KeyCode.Mouse0, false },
        { KeyCode.Mouse1, false }
    };

    private bool taskCompleted = false;

    public void StartTask()
    {
        taskCompleted = false;
        ResetKeys();
        UpdateUI(); // Отображаем начальный текст
    }

    public bool IsTaskCompleted()
    {
        return taskCompleted;
    }

    private void Update()
    {
        if (taskCompleted) return;

        foreach (var key in new List<KeyCode>(keyPresses.Keys))
        {
            if (Input.GetKeyDown(key) && !keyPresses[key])
            {
                keyPresses[key] = true;
                UpdateUI(); // Обновляем текстовое поле при каждом нажатии
            }
        }

        if (!keyPresses.ContainsValue(false))
        {
            taskCompleted = true;
            Debug.Log("Key press task completed!");
        }
    }

    private void ResetKeys()
{
    List<KeyCode> keys = new List<KeyCode>(keyPresses.Keys); // Создаем список ключей, чтобы избежать изменения оригинальной коллекции
    foreach (var key in keys)
    {
        keyPresses[key] = false;
    }
}


    private void UpdateUI()
    {
        string keysLeft = "Нажмите:\n";
        foreach (var key in keyPresses)
        {
            if (!key.Value) keysLeft += $"{key.Key}\n";
        }
        counterText.text = keysLeft;
    }

    public override string ToString()
    {
        return "Press the required keys.";
    }
    public void StopTask()
{
    
}
    
}
