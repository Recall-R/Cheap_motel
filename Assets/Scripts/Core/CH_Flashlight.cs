using UnityEngine;

public class CH_Flashlight : MonoBehaviour
{ 
    public static CH_Flashlight Instance;

    [SerializeField] private GameObject flashlight;

    void Awake()
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


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleFlashlight();
        }
    }

    void ToggleFlashlight()
    {
        flashlight.SetActive(!flashlight.activeSelf);
    }
}
