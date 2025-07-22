using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject crosshair; // 十字線のUI要素

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetCrosshairVisibility(bool visible)
    {
        if (crosshair != null)
        {
            crosshair.SetActive(visible);
        }
    }
}
