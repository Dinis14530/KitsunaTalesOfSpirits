using System;
using UnityEngine;

public class MapManager : MonoBehaviour
{

    public static MapManager Instance { get; private set; }

    [SerializeField] private GameObject miniMap;
    [SerializeField] private GameObject largeMap;

    public bool isLargeMapOpen;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        closeLargeMap();
    }

    private void Update()
    {
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
        miniMap.SetActive(true);
        largeMap.SetActive(false);
        isLargeMapOpen = false;
        Time.timeScale = 1;
    }
}
