using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI quantityText;
    
    public UIManager uiManager;

    public int index;
    public InventoryItem currentItem;

    public void SetSlot()
    {
        gameObject.SetActive(true);
        icon.sprite = currentItem.itemData.icon;
        quantityText.text = currentItem.quantity.ToString();
    }

    public void ClearSlot()
    {
        currentItem = null;
        icon.sprite = null;
        quantityText.text = "";
        gameObject.SetActive(false);
    }
}
