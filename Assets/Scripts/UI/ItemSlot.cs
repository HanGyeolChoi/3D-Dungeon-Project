using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public ItemData data;

    public Button button;
    public Image itemIcon;
    public TextMeshProUGUI quantityText;
    public TextMeshProUGUI equippedText;

    public int index;
    public bool equipped;
    public int quantity;

    public InventoryUI inventory;
    public void Set()
    {
        itemIcon.gameObject.SetActive(true);
        itemIcon.sprite = data.icon;
        quantityText.text = quantity > 1 ? quantity.ToString() : string.Empty;

        equippedText.gameObject.SetActive(equipped);
    }

    public void Clear()
    {
        data = null;
        itemIcon.gameObject.SetActive(false);
        quantityText.text = string.Empty;
    }

    public void OnClickButton()
    {
        inventory.SelectItem(index);
    }
}
