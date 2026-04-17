using System;
using UnityEngine;

public class MapManager : MonoBehaviour
{

    public static MapManager Instance { get; private set; }

    [SerializeField] private GameObject miniMap;
    [SerializeField] private GameObject largeMap;
    [SerializeField] private bool mapPurchased;

    public bool isLargeMapOpen;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        closeLargeMap();
        ApplyMapAvailability();
    }

    private void Update()
    {
        if (!mapPurchased)
            return;

        if (Input.GetKeyDown(KeyCode.J))
        {
            if (!isLargeMapOpen)
            {
                openLargeMap();
            }
            else
            {
                closeLargeMap();
            }
        }
    }

    private void openLargeMap()
    {
        miniMap.SetActive(false);
        largeMap.SetActive(true);
        isLargeMapOpen = true;
        Time.timeScale = 0;
    }
    private void closeLargeMap()
    {
        miniMap.SetActive(mapPurchased);
        largeMap.SetActive(false);
        isLargeMapOpen = false;
        Time.timeScale = 1;
    }

    public bool IsMapPurchased()
    {
        return mapPurchased;
    }

    public void SetMapPurchased(bool purchased)
    {
        mapPurchased = purchased;

        if (!mapPurchased)
            closeLargeMap();

        ApplyMapAvailability();
    }

    private void ApplyMapAvailability()
    {
        if (miniMap != null)
            miniMap.SetActive(mapPurchased && !isLargeMapOpen);

        if (largeMap != null)
            largeMap.SetActive(mapPurchased && isLargeMapOpen);
    }
}
