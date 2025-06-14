using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TasksOverlay : MonoBehaviour
{
    [Header("Gameobject References")]
    public GameObject tasksOverlay;
    public GameObject houseDoor;
    public GameObject gate;
    public GameObject enemy;
    public GameObject car;

    [Header("Text Lines")]
    public TMP_Text headerLine;
    public TMP_Text task1Line;
    public TMP_Text task2Line;
    public TMP_Text task3Line;

    [Header("Slide")]
    public float slideOffsetX;
    public float slideOffsetChangeSpeedX;

    [Header("Misc")]
    public float valueCuttingTreshold;

    // Slide
    [HideInInspector] public bool isVisible;
    private Vector3 defaultPosition;
    private float currentOffsetX;

    // Inputs
    private InputAction showTasksOverlayAction;

    void Start()
    {
        defaultPosition = tasksOverlay.transform.localPosition;
        currentOffsetX = slideOffsetX;

        tasksOverlay.transform.localPosition = defaultPosition + Vector3.right * slideOffsetX;

        showTasksOverlayAction = InputSystem.actions.FindAction("ShowTasksOverlay");

        car.SetActive(false);
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

            tasksOverlay.transform.localPosition = defaultPosition + Vector3.right * currentOffsetX;
        }

        if (!isVisible && currentOffsetX != slideOffsetX)
        {
            currentOffsetX = Mathf.Lerp(currentOffsetX, slideOffsetX, Time.deltaTime * slideOffsetChangeSpeedX);

            if (currentOffsetX < slideOffsetX + valueCuttingTreshold)
            {
                currentOffsetX = slideOffsetX;
            }

            tasksOverlay.transform.localPosition = defaultPosition + Vector3.right * currentOffsetX;
        }
    }

    public void UpdateTask1()
    {
        enemy.SetActive(true);

        task1Line.alpha = 0.25f;
    }

    public void UpdateTask2(int moneyCollected)
    {
        task2Line.text = " 2. Собрать деньги (" + moneyCollected + "$ из 569$)";

        if (moneyCollected == 569)
        {
            houseDoor.GetComponent<DoubleDoorsSpecial>().isInteractable = true;
            gate.GetComponent<DoubleDoorsSpecial>().isInteractable = true;

            task2Line.alpha = 0.25f;
        }
    }

    public void UpdateTask3()
    {
        enemy.SetActive(false);

        task3Line.alpha = 0.25f;

        car.SetActive(true);

        Invoke(nameof(UpdateTask4), 0.5f);
    }

    public void UpdateTask4()
    {
        task1Line.alpha = 1f;
        task1Line.text = "4. Беги по дороге и не оборачивайся";

        task2Line.alpha = 0f;
        task3Line.alpha = 0f;
    }

    public void UpdateTask5()
    {
        task1Line.alpha = 0.25f;
    }
}
