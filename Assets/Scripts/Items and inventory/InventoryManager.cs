using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

    
public class InventoryManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [HideInInspector] public bool isStorageOpened;

    [SerializeField] private Human player;

    [SerializeField] GameObject[] Slots = new GameObject[12];
    [SerializeField] Transform handParent;
    [SerializeField] GameObject itemPrefab;
    [SerializeField] Camera cam;
    [SerializeField] GameObject[] itemPrefabs;

    [SerializeField] private GameObject helmetSlot;
    [SerializeField] private GameObject armorSlot;
    [SerializeField] private GameObject weaponSlot;

        
        // Текст для имени предмета
     // Текст для описания
    [SerializeField] private TMP_Text StatsText;

    GameObject draggedObject;
    GameObject lastItemSlot;

    bool isInventoryOpened;
    public bool IsInventoryOpened => isInventoryOpened;
    int selectedHotbarSlot = 0;

    void Start()
    {
        if (player == null)
        {
        player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Human>();
        ShowInfo();
        }
        
        
        //Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
{
    // Перетаскивание предмета
    if (draggedObject != null)
    {
        draggedObject.transform.position = Input.mousePosition;
    }
    
    // Открытие/закрытие инвентаря
    if (Input.GetKeyDown(KeyCode.Tab))
    {
        if (isInventoryOpened)
        {
            Cursor.lockState = CursorLockMode.None;
            isInventoryOpened = false;
            isStorageOpened = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            isInventoryOpened = true;
        }
    }
    ShowInfo();
    // Обновление информации о предмете при наведении
    
}




    public void OnPointerDown(PointerEventData eventData)
{
    if (eventData.button == PointerEventData.InputButton.Left)
    {
        GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;
        InventorySlot slot = clickedObject.GetComponent<InventorySlot>();

        if (slot != null)
        {
            // Проверяем, если это слот экипировки, не начинаем перетаскивание
            if (slot.CompareTag("EquipSlot") && slot.heldItem != null)
            {
                
                return;
            }

            if (slot.heldItem != null)
            {
                
                // Начинаем перетаскивание, если предмет в обычном слоте инвентаря
                draggedObject = slot.heldItem;
                slot.heldItem = null;
                lastItemSlot = clickedObject;

                
            }
            else
            {
                Debug.Log("Нажали на пустой слот: " + slot.name);
            }
        }
        else
        {
            Debug.Log("Нажали не на слот, а на: " + clickedObject.name);
        }
    }
}


    public void OnPointerUp(PointerEventData eventData)
    {
        if (draggedObject != null && eventData.pointerCurrentRaycast.gameObject != null && eventData.button == PointerEventData.InputButton.Left)
        {
            GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;
            InventorySlot targetSlot = clickedObject.GetComponent<InventorySlot>();

            if (targetSlot != null)
            {
                if (targetSlot.CompareTag("EquipSlot"))
                {
                    // Обрабатываем перемещение в слот экипировки
                    HandleEquipSlot(targetSlot, draggedObject);
                }
                else
                {
                    // Обрабатываем перемещение в обычный слот инвентаря
                    HandleInventorySlot(targetSlot);
                }
            }
            else
            {
                // Если не на слот, возвращаем предмет обратно
                ReturnItemToPreviousSlot();
            }

            draggedObject = null; // Обнуляем переменную, чтобы завершить перетаскивание
        }
    }



    private void HandleEquipSlot(InventorySlot targetSlot, GameObject draggedItem)
{
    InventoryItem draggedInventoryItem = draggedItem.GetComponent<InventoryItem>();
    ItemSO itemSO = draggedInventoryItem.itemScriptableObject;

    // Проверка, что предмет имеет правильный тип
    if (itemSO.itemType == ItemSO.ItemType.Default)
    {
        
        ReturnItemToPreviousSlot();
        return;
    }

    // Проверяем совместимость предмета с типом слота
    if (IsItemCompatibleWithSlot(itemSO, targetSlot))
    {
        // Если слот пуст, просто экипируем предмет
        if (targetSlot.heldItem == null)
        {
            EquipItemToSlot(targetSlot, draggedItem);
            Debug.Log(itemSO.itemType + " экипирован.");
        }
        else
        {
            // Если слот занят другим предметом, меняем их местами
            SwapItemsBetweenSlots(targetSlot, draggedItem);
            Debug.Log("Слот был занят, предметы заменены.");
        }
    }
    else
    {
        // Если предмет не подходит для этого слота, ищем свободный слот в инвентаре
        Debug.Log($"Невозможно экипировать предмет с типом {itemSO.itemType} в данный слот. Перемещаем его в инвентарь.");

        // Ищем свободный слот в инвентаре и кладем туда предмет
        MoveItemToInventory(draggedItem);
    }
}






// Метод для перемещения предмета в инвентарь
private void MoveItemToInventory(GameObject draggedItem)
{
    // Ищем свободный слот в инвентаре
    foreach (var slot in Slots)
    {
        InventorySlot inventorySlot = slot.GetComponent<InventorySlot>();
        if (inventorySlot.heldItem == null)
        {
            // Находим свободный слот и помещаем туда предмет
            inventorySlot.SetHeldItem(draggedItem);
            draggedItem.transform.SetParent(inventorySlot.transform);
            draggedItem.transform.localPosition = Vector3.zero;
            Debug.Log($"Предмет {draggedItem.name} перемещен в свободный слот инвентаря.");
            return;
        }
    }

    // Если не нашли свободный слот, можно добавить логику обработки (например, уничтожить предмет или показать сообщение)
    Debug.Log("Нет свободных слотов в инвентаре для перемещения предмета.");
}

// Метод для проверки совместимости предмета с типом слота
private bool IsItemCompatibleWithSlot(ItemSO itemSO, InventorySlot targetSlot)
{
    // Получаем имя слота
    string slotName = targetSlot.gameObject.name.ToLower(); 

    // Проверяем, что имя слота соответствует типу предмета
    switch (itemSO.itemType)
    {
        case ItemSO.ItemType.Helmet:
            return slotName.Contains("helmet"); // Если слот называется "helmetSlot" или что-то похожее
        case ItemSO.ItemType.Armor:
            return slotName.Contains("armor"); // Если слот называется "armorSlot"
        case ItemSO.ItemType.Weapon:
            return slotName.Contains("weapon"); // Если слот называется "weaponSlot"
        default:
            return false; // По умолчанию — не совместим
    }
}


public void ShowInfo()
{
    // Получаем объект игрока
    Human playerStats = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Human>();

    if (playerStats != null)
    {
        // Формируем текстовое отображение статов игрока
        StatsText.text = $"мувспид: {playerStats.moveSpeed}\n" +
                         $"манареген: {playerStats.manaRegenSpeed}\n" +
                         $"урон: {playerStats.baseDamage+playerStats.additionalDamageFromItems}\n" +
                         $"Здоровье: {(int)playerStats.health}\n" +
                         $"энергия: {(int)playerStats.mana}";
    }
    else
    {
        Debug.LogError("Игрок с тегом 'Player' не найден или компонент Human отсутствует.");
        StatsText.text = "Статы персонажа не найдены.";
    }
}
    


    private void HandleInventorySlot(InventorySlot targetSlot)
{
    // Проверка, что предмет перемещается из слота экипировки
    if (lastItemSlot.CompareTag("EquipSlot"))
    {
        InventoryItem draggedItem = draggedObject.GetComponent<InventoryItem>();
        ItemSO itemSO = draggedItem.itemScriptableObject;

        player.RemoveItemStats(itemSO);

        // Если мы пытаемся переместить предмет из экипировки в инвентарь, проверяем совместимость
        if (targetSlot.heldItem != null)
        {
            InventoryItem targetItem = targetSlot.heldItem.GetComponent<InventoryItem>();
            // Если типы предметов не совпадают, отменяем операцию
            if (targetItem.itemScriptableObject.itemType != itemSO.itemType)
            {
                Debug.Log("Невозможно переместить предмет из экипировки в слот инвентаря с несовместимым типом.");
                ReturnItemToPreviousSlot(); // Возвращаем предмет в исходный слот
                return;
            }
        }

        // Выполняем обычное перемещение предмета
        targetSlot.SetHeldItem(draggedObject);
        draggedObject.transform.SetParent(targetSlot.transform);
        draggedObject.transform.localPosition = Vector3.zero;
    }
    else
    {
        // Если предмет перемещается в слот инвентаря (не из экипировки)
        if (targetSlot.heldItem == null)
        {
            targetSlot.SetHeldItem(draggedObject);
            draggedObject.transform.SetParent(targetSlot.transform);
            draggedObject.transform.localPosition = Vector3.zero;
        }
        else
        {
            InventoryItem targetItem = targetSlot.heldItem.GetComponent<InventoryItem>();
            InventoryItem draggedItem = draggedObject.GetComponent<InventoryItem>();

            if (targetItem.itemScriptableObject == draggedItem.itemScriptableObject)
            {
                // Суммируем количество предметов
                int total = targetItem.stackCurrent + draggedItem.stackCurrent;

                if (total <= targetItem.stackMax)
                {
                    targetItem.stackCurrent = total;
                    Destroy(draggedObject);
                }
                else
                {
                    SwapItemsBetweenSlots(targetSlot, draggedObject);
                }
            }
            else
            {
                SwapItemsBetweenSlots(targetSlot, draggedObject);
            }
        }
    }
}


    private void SwapItemsBetweenSlots(InventorySlot targetSlot, GameObject newItem)
{
    GameObject existingItem = targetSlot.heldItem;

    // Если слот был занят другим предметом, нужно очистить его
    if (existingItem != null)
    {
        InventoryItem existingItemComponent = existingItem.GetComponent<InventoryItem>();
        if (existingItemComponent != null)
        {
            // Применяем логику для очистки и возврата характеристик от старого предмета, если это необходимо
            player.RemoveItemStats(existingItemComponent.itemScriptableObject);
        }
    }

    // Обновляем целевой слот и помещаем новый предмет
    targetSlot.SetHeldItem(newItem);
    newItem.transform.SetParent(targetSlot.transform);
    newItem.transform.localPosition = Vector3.zero;

    // Если исходный слот был занят, перемещаем обратно старый предмет
    if (existingItem != null)
    {
        InventorySlot lastSlot = lastItemSlot.GetComponent<InventorySlot>();
        lastSlot.SetHeldItem(existingItem);
        existingItem.transform.SetParent(lastSlot.transform);
        existingItem.transform.localPosition = Vector3.zero;
    }

    // Применяем статистику для нового предмета, если он был экипирован
    if (newItem != null)
    {
        InventoryItem newItemComponent = newItem.GetComponent<InventoryItem>();
        if (newItemComponent != null)
        {
            player.ApplyItemStats(newItemComponent.itemScriptableObject);
        }
    }

    Debug.Log("Предметы обменялись местами.");
}

    private void EquipItemToSlot(InventorySlot slot, GameObject item)
    {
        InventoryItem inventoryItem = item.GetComponent<InventoryItem>();
        ItemSO itemSO = inventoryItem.itemScriptableObject;

        // Применяем бонусы от предмета к игроку
        player.ApplyItemStats(itemSO);

        slot.SetHeldItem(item);
        item.transform.SetParent(slot.transform);
        item.transform.localPosition = Vector3.zero;
        Debug.Log("Предмет " + item.name + " экипирован в слот " + slot.name);
    }

    private void ReturnItemToPreviousSlot()
    {
        if (lastItemSlot != null)
        {
            lastItemSlot.GetComponent<InventorySlot>().SetHeldItem(draggedObject);
            draggedObject.transform.SetParent(lastItemSlot.transform);
            draggedObject.transform.localPosition = Vector3.zero;
        }
    }

    public void ItemPicked(DropLabel dropLabel)
    {
        ItemSO itemSO = dropLabel.DropInfo.item;
        GameObject emptySlot = null;

        foreach (var slot in Slots)
        {
            InventorySlot inventorySlot = slot.GetComponent<InventorySlot>();
            if (inventorySlot.heldItem == null)
            {
                emptySlot = slot;
                break;
            }
        }

        if (emptySlot != null)
        {
            GameObject newItem = Instantiate(itemPrefab);
            newItem.GetComponent<InventoryItem>().itemScriptableObject = itemSO;
            newItem.transform.SetParent(emptySlot.transform);
            newItem.GetComponent<InventoryItem>().stackCurrent = 1;

            emptySlot.GetComponent<InventorySlot>().SetHeldItem(newItem);
            Destroy(dropLabel.gameObject);
        }
    }
}
