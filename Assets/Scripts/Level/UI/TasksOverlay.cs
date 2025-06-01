using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TasksOverlay : MonoBehaviour
{
    [Header("Gameobject References")]
    public GameObject text;

    [Header("Text Lines")]
    public TMP_Text headerLine;
    public TMP_Text task1Line;
    public TMP_Text task2Line;

    [Header("Slide")]
    public float slideOffsetX;
    public float slideOffsetChangeSpeedX;

    [Header("Misc")]
    public float valueCuttingTreshold;

    // Slide
    private bool isVisible;
    private Vector3 defaultPosition;
    private float currentOffsetX;

    // Inputs
    private InputAction showTasksOverlayAction;

    void Start()
    {
        defaultPosition = text.transform.localPosition;
        currentOffsetX = slideOffsetX;

        text.transform.localPosition = defaultPosition + Vector3.right * slideOffsetX;

        showTasksOverlayAction = InputSystem.actions.FindAction("ShowTasksOverlay");
    }

    void Update()
    {
        if (showTasksOverlayAction.WasPressedThisFrame())
        {
            isVisible = !isVisible;
        }

        if (isVisible && currentOffsetX != 0f)
        {
            currentOffsetX = Mathf.Lerp(currentOffsetX, 0f, Time.deltaTime * slideOffsetChangeSpeedX);

            if (currentOffsetX > 0f - valueCuttingTreshold)
            {
                currentOffsetX = 0f;
            }

            text.transform.localPosition = defaultPosition + Vector3.right * currentOffsetX;
        }

        if (!isVisible && currentOffsetX != slideOffsetX)
        {
            currentOffsetX = Mathf.Lerp(currentOffsetX, slideOffsetX, Time.deltaTime * slideOffsetChangeSpeedX);

            if (currentOffsetX < slideOffsetX + valueCuttingTreshold)
            {
                currentOffsetX = slideOffsetX;
            }

            text.transform.localPosition = defaultPosition + Vector3.right * currentOffsetX;
        }
    }

    public void UpdateTask1(int moneyCollected)
    {
        task1Line.text = " 1. Собрать деньги (" + moneyCollected + "$ из 569$)";

        if (moneyCollected == 569)
        {
            task1Line.text = " 1. Собрать деньги (" + moneyCollected + "$ из 569$) - Выполнено";
            task1Line.alpha = 0.25f;
        }
    }

    public void UpdateTask2()
    {
        task2Line.alpha = 0.25f;
    }
}
