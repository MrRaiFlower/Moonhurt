using UnityEngine;
using UnityEngine.InputSystem;

public class CoinsManager : MonoBehaviour
{
    [Header("Gameobject References")]
    public GameObject[] coinsPiles;

    [Header("UI")]
    public GameObject tasksUIOverlay;

    [Header("Coins Piles Spawner")]
    public int coinsPilesNumber;
    public int moneyIncrementRandomRange;

    // Coins Piles Spawner
    private GameObject currentCoinsPile;
    private int spawnedCoinsPilesNumber;

    // Coins Piles Manager
    private int coinsPilesCollected;
    private int moneyCollected;
    private int BaseMoneyIncrement;

    void Start()
    {
        BaseMoneyIncrement = (int)(569f / coinsPilesNumber);

        while (spawnedCoinsPilesNumber < coinsPilesNumber)
        {
            currentCoinsPile = coinsPiles[Random.Range(0, coinsPiles.Length)];

            if (!currentCoinsPile.activeSelf)
            {
                currentCoinsPile.SetActive(true);
                currentCoinsPile.GetComponent<CoinsPile>().SetModel();
                currentCoinsPile.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

                spawnedCoinsPilesNumber++;
            }
        }
    }

    void Update()
    {
        
    }

    public void Collect()
    {
        coinsPilesCollected += 1;

        if (coinsPilesCollected != coinsPilesNumber)
        {
            moneyCollected += BaseMoneyIncrement + Random.Range(-1 * moneyIncrementRandomRange, moneyIncrementRandomRange);
        }
        else
        {
            moneyCollected = 569;
        }

        tasksUIOverlay.GetComponent<TasksOverlay>().UpdateTask1(moneyCollected);
    }
}
