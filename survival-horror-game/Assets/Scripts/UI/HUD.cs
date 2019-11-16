using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        _dd = FindObjectOfType<DontDestroyOnLoad>();

        if (_dd == null)
        {
#if UNITY_EDITOR
            Debug.LogError("Error: DontDestroyOnLoad hasn't been found in the scene.");
            UnityEditor.EditorApplication.isPlaying = false;
#else
            // Display an error message and exit the app
#endif
        }
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
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0.5f);

            image.rectTransform.sizeDelta = new Vector2(size, size);
            go.transform.SetParent(keyZone.transform);
        }
    }

    public void DisplayItems(List<Utility> items)
    {
        foreach (Transform child in itemZone.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Utility item in items)
        {
            float size = itemZone.GetComponent<RectTransform>().sizeDelta.x;

            GameObject go = new GameObject();
            Image image = go.AddComponent<Image>();
            image.sprite = item.sprite;
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0.5f);

            image.rectTransform.sizeDelta = new Vector2(size, size);
            go.transform.SetParent(itemZone.transform);

            if (item is Teddy)
            {
                GameObject go2 = new GameObject();
                Text text = go2.AddComponent<Text>();
                text.text = _dd.GetKey(Controls.HugTeddy).Item1.ToString();
                text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                text.alignment = TextAnchor.MiddleLeft;

                text.rectTransform.sizeDelta = new Vector2(size, size);
                text.rectTransform.position = new Vector2(size, 0);
                go2.transform.SetParent(go.transform);
            }
        }
    }

    public void ChangeBatteryNumber(int batteryNumber)
    {
        batteryNumberZone.text = batteryNumber.ToString();
    }

    public GameObject keyZone;
    public GameObject itemZone;
    public StressBar stressBar;
    public BatteryBar batteryBar;
    public ActionBar actionBar;
    public Text batteryNumberZone;
    public Helper helper;
    public DialogManager dialog;

    // Player settings
    private DontDestroyOnLoad _dd;
}
