using UnityEngine;

public interface IInteractable
{
    public string GetInfo();
    public void OnInteract();
}
public class ItemObject : MonoBehaviour, IInteractable
{
    public ItemData data;

    public string GetInfo()
    {
        string info = $"{data.itemName}\n{data.description}";
        return info;
    }

    public void OnInteract()
    {
        CharacterManager.Instance.Player.itemData = data;
        CharacterManager.Instance.Player.addItem?.Invoke();
        Destroy(gameObject);
     }
}