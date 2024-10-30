
using System.Reflection;
using TMPro;
using UnityEngine;
public class InventoryUI : MonoBehaviour
{
    public ItemSlot[] slots;

    public GameObject inventoryWindow;
    public Transform slotPanel;

    [Header("Quickslots")]
    public GameObject QuickslotButton1;
    public GameObject QuickslotButton2;
    public GameObject QuickslotButton3;

    [Header("Select Item")]
    public TextMeshProUGUI selectedItemName;
    public TextMeshProUGUI selectedDescription;
    public TextMeshProUGUI selectedStatsName;
    public TextMeshProUGUI selectedStatsValue;
    public GameObject useButton;
    public GameObject equipButton;
    public GameObject unequipButton;
    public GameObject discardButton;

    private PlayerController controller;
    private PlayerCondition condition;
    private Transform dropPosition;

    private ItemData selectedItem;
    private int selectedItemIndex = 0;

    private int curEquipIndex = 0;
    void Start()
    {
        controller = CharacterManager.Instance.Player.GetComponent<PlayerController>();
        condition = CharacterManager.Instance.Player.GetComponent<PlayerCondition>();
        dropPosition = CharacterManager.Instance.Player.dropPosition;
        controller.inventory += Toggle;
        CharacterManager.Instance.Player.addItem += AddItem;

        inventoryWindow.SetActive(false);
        slots = new ItemSlot[slotPanel.childCount];

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = slotPanel.GetChild(i).GetComponent<ItemSlot>();
            slots[i].index = i;
            slots[i].inventory = this;
        }

        ClearSelectedItemWindow();
    }

    private void Toggle()
    {
        inventoryWindow.SetActive(!inventoryWindow.activeInHierarchy);
    }

    private void AddItem()
    {
        ItemData data = CharacterManager.Instance.Player.itemData;

        if (data.canStack)
        {
            ItemSlot slot = GetItemStack(data);
            if (slot != null)
            {
                slot.quantity++;
                UpdateUI();
                CharacterManager.Instance.Player.itemData = null;
                return;
            }
        }

        ItemSlot emptySlot = GetEmptySlot();

        if (emptySlot != null)
        {
            emptySlot.data = data;
            emptySlot.quantity = 1;
            UpdateUI();
            CharacterManager.Instance.Player.itemData = null;
            return;
        }

        ThrowItem(data);
        CharacterManager.Instance.Player.itemData = null;

    }

    private void ThrowItem(ItemData data)
    {
        Instantiate(data.dropPrefab, dropPosition.position, Quaternion.Euler(Vector3.one * Random.value * 360));
    }

    private void RemoveSelectedItem()
    {
        slots[selectedItemIndex].quantity--;
        if(slots[selectedItemIndex].quantity <= 0 )
        {
            selectedItem = null;
            slots[selectedItemIndex].data = null;
            selectedItemIndex = -1;
            ClearSelectedItemWindow();
        }
        UpdateUI();
    }

    public void UpdateUI()
    {
        for(int i=0; i<slots.Length; i++)
        {
            if (slots[i].data != null)
            {
                slots[i].Set();
            }
            else slots[i].Clear();
        }
    }

    private void ClearSelectedItemWindow()
    {
        selectedItemName.text = string.Empty;
        selectedDescription.text = string.Empty;
        selectedStatsName.text = string.Empty;
        selectedStatsValue.text = string.Empty;

        useButton.SetActive(false);
        equipButton.SetActive(false);
        unequipButton.SetActive(false);
        discardButton.SetActive(false);

        QuickslotButton1.SetActive(false);
        QuickslotButton2.SetActive(false);
        QuickslotButton3.SetActive(false);
    }

    public void OnUseButton()
    {
        if (selectedItem.type == ItemType.Consumable)
        {
            for (int i = 0; i < selectedItem.consumables.Length; i++)
            {
                switch (selectedItem.consumables[i].type)
                {
                    case ConsumableType.Health:
                        condition.Heal(selectedItem.consumables[i].value);
                        break;
                    case ConsumableType.Stamina:
                        condition.StaminaHeal(selectedItem.consumables[i].value);
                        break;
                    case ConsumableType.Speed:
                        condition.SpeedUp(selectedItem.consumables[i].value);
                        break;
                }
            }
            RemoveSelectedItem();
        }
    }

    public void OnEquipButton()
    {
        Equip();
        SelectItem(selectedItemIndex);
    }

    public void OnUnequipButton()
    {
        UnEquip(selectedItemIndex);
    }

    public void OnDropButton()
    {
        if (slots[selectedItemIndex].equipped == true) UnEquip(selectedItemIndex);
        ThrowItem(selectedItem);
        RemoveSelectedItem();
    }

    private void Equip()
    {
        if (slots[curEquipIndex].equipped)
        {
            UnEquip(curEquipIndex);
        }

        slots[selectedItemIndex].equipped = true;
        curEquipIndex = selectedItemIndex;
        CharacterManager.Instance.Player.equip.EquipNew(selectedItem);
        UpdateUI();
    }

    private void UnEquip(int index)
    {
        slots[index].equipped = false;
        CharacterManager.Instance.Player.equip.UnEquip();
        UpdateUI();

        if (selectedItemIndex == index)
        {
            SelectItem(selectedItemIndex);
        }
    }

    public void SelectItem(int index)
    {
        if (slots[index].data == null) return;

        selectedItem = slots[index].data;
        selectedItemIndex = index;

        selectedItemName.text = selectedItem.itemName;
        selectedDescription.text = selectedItem.description;
        selectedStatsName.text = string.Empty;
        selectedStatsValue.text = string.Empty;

        for (int i = 0; i < selectedItem.consumables.Length; i++)
        {
            selectedStatsName.text += selectedItem.consumables[i].type.ToString() + "\n";
            selectedStatsValue.text += selectedItem.consumables[i].value.ToString() + "\n";
        }

        useButton.SetActive(selectedItem.type == ItemType.Consumable);
        equipButton.SetActive(selectedItem.type == ItemType.Equipable && !slots[index].equipped);
        unequipButton.SetActive(selectedItem.type == ItemType.Equipable && slots[index].equipped);
        discardButton.SetActive(true);
    }

    private ItemSlot GetEmptySlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].data == null)
            {
                return slots[i];
            }
        }
        return null;
    }

    private ItemSlot GetItemStack(ItemData data)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].data == data && slots[i].quantity < data.maxStack)
            {
                return slots[i];
            }
        }
        return null;
    }

}