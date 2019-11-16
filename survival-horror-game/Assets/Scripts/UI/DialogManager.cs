using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Person
{
    MC,
    DAD,
    MOM
};

public enum Expression
{
    AFRAID,
    ANGRY,
    DEAD,
    DISAPPOINTED,
    HAPPY,
    LAUGHING,
    NORMAL,
    SAD,
    SLEEPING,
    SURPRISED
};

[System.Serializable]
public class Dialog
{
    public Dialog(string text, Expression expression = Expression.NORMAL, Person speaker = Person.MC)
    {
        this.text = text;
        this.speaker = speaker;
        this.expression = expression;
    }

    public string text;
    public Person speaker = Person.MC;
    public Expression expression = Expression.NORMAL;
}

public class DialogManager : MonoBehaviour
{
    private void Start()
    {
        _dialogs = new List<Dialog>();
        StopDisplayingDialog();
    }

    public bool FinishSpeaking()
    {
        return _dialogs.Count == 0;
    }

    public void AddDialog(string text, Expression expression = Expression.NORMAL, Person speaker = Person.MC)
    {
        AddDialog(new Dialog(text, expression, speaker));
    }

    public void AddDialog(Dialog dialog)
    {
        _dialogs.Add(dialog);
        if (_dialogs.Count == 1)
        {
            DisplayNextDialog();
            background.enabled = true;
            expressionZone.enabled = true;
        }
    }

    public void NextDialog()
    {
        if (_dialogs.Count > 0)
        {
            _dialogs.RemoveAt(0);

            if (_dialogs.Count > 0)
            {
                DisplayNextDialog();
            }
            else
            {
                StopDisplayingDialog();
            }
        }
    }

    private void DisplayNextDialog()
    {
        textZone.text = _dialogs[0].text;
        expressionZone.sprite = GetExpressionSprite(_dialogs[0]);
    }

    private void StopDisplayingDialog()
    {
        textZone.text = "";
        background.enabled = false;
        expressionZone.enabled = false;
    }

    private Sprite GetExpressionSprite(Dialog dialog)
    {
        string path = "Sprites/Expressions/";

        switch (dialog.speaker)
        {
            case Person.MC:
                path += "Mc/";
                break;
            case Person.DAD:
                path += "Dad/";
                break;
            case Person.MOM:
                path += "Mom/";
                break;
        }

        Sprite basic = Resources.Load<Sprite>(path + "Normal");

        switch (dialog.expression)
        {
            case Expression.AFRAID:
                path += "Afraid";
                break;
            case Expression.ANGRY:
                path += "Angry";
                break;
            case Expression.DEAD:
                path += "Dead";
                break;
            case Expression.DISAPPOINTED:
                path += "Disappointed";
                break;
            case Expression.HAPPY:
                path += "Happy";
                break;
            case Expression.LAUGHING:
                path += "Laughing";
                break;
            case Expression.NORMAL:
                path += "Normal";
                break;
            case Expression.SAD:
                path += "Sad";
                break;
            case Expression.SLEEPING:
                path += "Sleeping";
                break;
            case Expression.SURPRISED:
                path += "Surprised";
                break;
        }

        Sprite toReturn = Resources.Load<Sprite>(path);

        if (toReturn == null)
            return basic;

        return toReturn;
    }

    private List<Dialog> _dialogs;

    public Text textZone;
    public Image expressionZone;
    public Image background;
}
