using UnityEngine;


public class CH_Manager : MonoBehaviour {

    public static CH_Manager Instance { get; private set; }


    [SerializeField] private GameObject fpsController;



    private void Awake() {
        if (Instance != null && Instance != this) {
            Debug.LogWarning("CH_Manager: Există deja o instanță activă. Se distruge duplicatul.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    public bool SetFPSControllerActive(bool isActive) {
        if (fpsController == null) {
            Debug.LogWarning("CH_Manager: fpsController nu este setat în inspector.");
            return false;
        }

        fpsController.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = isActive;
        return true;
    }

    public bool isFPSControllerActive() {
        if (fpsController == null) {
            Debug.LogWarning("CH_Manager: fpsController nu este setat în inspector.");
            return false;
        }

        return fpsController.activeSelf;
    }

    public bool SetCursorLockState(bool isLocked) {
        Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isLocked;
        return true;
    }

}