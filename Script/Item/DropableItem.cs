using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropableItem : MonoBehaviour
{
    public int itemID;
    float disappearTime = 30;
    public Item item { get; set; }

    // Update is called once per frame
    private void Start()
    {
        DisappearC();
    }
    IEnumerator DisappearC()
    {
        yield return new WaitForSeconds(disappearTime);
        if(gameObject.activeSelf)
            gameObject.SetActive(false);
    }

    public void Set(int itemID)
    {
        this.itemID = itemID;
        item = FindObjectOfType<DatabaseManager>().ReturnItem(itemID);
        GetComponent<SpriteRenderer>().sprite = item.itemIcon;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (Inventory.instance.IsInventoryFull(itemID))
            {
                AudioManager.instance.Play("Beep");
                InformationPanel.instance.EnableDownInfoPanel("인벤토리가 꽉 찼습니다.");
                return;
            }
            Inventory.instance.GetAnItem(itemID);
            StopCoroutine(DisappearC());
            Destroy(gameObject);
        }
    }
}
