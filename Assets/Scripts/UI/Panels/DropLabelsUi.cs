using System.Collections.Generic;
using UnityEngine;

public class DropLabelsUi : MonoBehaviour
{
    [SerializeField]
    private DropLabel dropLabelPrefab;

    [SerializeField]
    private Transform labelsParent;

    private Dictionary<int, DropLabel> labels = new Dictionary<int, DropLabel>();

    private void Start()
    {
        DropManager.Instance.OnDropAdded += Instance_OnDropAdded;
        DropManager.Instance.OnDropRemoved += Instance_OnDropRemoved;    
    }

    private void OnDestroy()
    {
        if (DropManager.Instance != null)
        {
            DropManager.Instance.OnDropAdded -= Instance_OnDropAdded;
            DropManager.Instance.OnDropRemoved -= Instance_OnDropRemoved;
        }
    }

    private void LateUpdate()
{
    foreach (var item in labels)
    {
        // Измените здесь, чтобы использовать dropInfo для получения позиции
        item.Value.transform.position = Camera.main.WorldToScreenPoint(item.Value.DropInfo.position);
    }
}

    private void Instance_OnDropRemoved(int dropId)
{
    if (labels.TryGetValue(dropId, out DropLabel label))
    {
        labels.Remove(dropId);
        Destroy(label.gameObject);
    }
}

    private void Instance_OnDropAdded(ItemDropInfo dropInfo)
{
    DropLabel labelInst = Instantiate(dropLabelPrefab, labelsParent);
    labelInst.Fill(dropInfo); // Используйте dropInfo вместо item
    labels.Add(dropInfo.id, labelInst);
}
}
