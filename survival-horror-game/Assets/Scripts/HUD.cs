using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void DisplayKeys(List<Key> keys)
    {
        foreach (Transform child in keyZone.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Key key in keys)
        {
            float size = keyZone.GetComponent<RectTransform>().sizeDelta.y;

            GameObject go = new GameObject();
            Image image = go.AddComponent<Image>();
            image.sprite = key.sprite;

            image.rectTransform.sizeDelta = new Vector2(size, size);
            go.transform.SetParent(keyZone.transform);
        }
    }

    public void DisplayItems(List<Item> items, Teddy teddy)
    {
        foreach (Transform child in itemZone.transform)
        {
            Destroy(child.gameObject);
        }

        if (teddy != null)
        {
            items.Insert(0, teddy);
        }

        foreach (Item item in items)
        {
            float size = itemZone.GetComponent<RectTransform>().sizeDelta.x;

            GameObject go = new GameObject();
            Image image = go.AddComponent<Image>();
            image.sprite = item.sprite;

            image.rectTransform.sizeDelta = new Vector2(size, size);
            go.transform.SetParent(itemZone.transform);
        }
    }

    public void ChangeBatteryNumber(int batteryNumber)
    {
        batteryNumberZone.text = "x " + batteryNumber;
    }

    public GameObject keyZone;
    public GameObject itemZone;
    public StressBar stressBar;
    public Text batteryNumberZone;
    public Helper helper;
}
