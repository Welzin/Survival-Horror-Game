using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateOptions : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad dd = FindObjectOfType<DontDestroyOnLoad>();
        CreateButtons(dd.AllKeys());
    }

    private void CreateButtons(Dictionary<Controls, (KeyCode, KeyCode)> keys)
    {
        foreach(var key in keys)
        {
            GameObject go = (GameObject)Instantiate(Resources.Load("Prefabs/UI_Prefabs/ChangeBindTemplate"));
            go.transform.SetParent(gameObject.transform);
            // 0 ==> Name
            go.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = key.Key.ToString();
            // 1 ==> First bind > 1 ==> Button > 0 ==> Text
            go.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<UnityEngine.UI.Text>().text = key.Value.Item1.ToString();
            // 2 ==> Second bind > 1 ==> Button > 0 ==> Text
            go.transform.GetChild(2).GetChild(1).GetChild(0).GetComponent<UnityEngine.UI.Text>().text = key.Value.Item2.ToString();

            go.transform.localScale = transform.localScale;
        }
    }
}
