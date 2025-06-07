using TMPro;
using UnityEngine;

public class FancyButton : MonoBehaviour
{
    [Header("Alpha")]
    public Color defaultColor;
    public Color higlightColor;

    void Start()
    {
        this.GetComponentInChildren<TMP_Text>().color = defaultColor;
    }

    void OnEnable()
    {
        this.GetComponentInChildren<TMP_Text>().color = defaultColor;
    }

    void Update()
    {

    }

    public void Higlight()
    {
        this.GetComponentInChildren<TMP_Text>().color = higlightColor;
    }

    public void Exit()
    {
        this.GetComponentInChildren<TMP_Text>().color = defaultColor;
    }

    public void Press()
    {
        this.GetComponentInChildren<TMP_Text>().color = defaultColor;
    }
}
