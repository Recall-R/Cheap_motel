using UnityEngine;
using System.Collections;

public class CH_Manager : MonoBehaviour {

    public static CH_Manager Instance { get; private set; }

    [SerializeField] private GameObject fpsController;

    [SerializeField] private GameObject generalCanvas;

    [SerializeField] private float killCamTurnSpeed = 620f; // grade / secundă
    [SerializeField] private GameObject exitPointForNpcs;
    private float rotationSpeed = 5f; // Adjust this value to control the speed of rotation

    private void Awake() {
        if (Instance != null && Instance != this) {
            Debug.LogWarning("CH_Manager: Există deja o instanță activă. Se distruge duplicatul.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public GameObject GetExitPointForNpcs()
    {
        if (exitPointForNpcs == null)
        {
            Debug.LogWarning("CH_Manager: exitPointForNpcs nu este setat în inspector.");
            return null;
        }

        return exitPointForNpcs;
    }

    public bool SetFPSControllerActive(bool isActive) {
        if (fpsController == null) {
            Debug.LogWarning("CH_Manager: fpsController nu este setat în inspector.");
            return false;
        }

        fpsController.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = isActive;
        return true;
    }
    
    public GameObject GetFPSController() {
        if (fpsController == null) {
            Debug.LogWarning("CH_Manager: fpsController nu este setat în inspector.");
            return null;
        }

        return fpsController;
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

    public void SetGeneralCanvasActive(bool isActive) {
        if (generalCanvas == null) {
            Debug.LogWarning("CH_Manager: generalCanvas nu este setat în inspector.");
            return;
        }

        generalCanvas.SetActive(isActive);
    }

    public void PrintDebugMessage(string message) {
        Debug.LogWarning($"CH_Manager: {message}");
    }

    public IEnumerator KillCamRoutine(GameObject killer, float maxTime = 1f, float angleThreshold = 3f)
    {
        SetCursorLockState(false);
        SetFPSControllerActive(false);
        SetGeneralCanvasActive(false);

        Transform cam = Camera.main.transform;
        float t = 0f;

        while (t < maxTime) // maxTime = plasă de siguranță, să nu rămână blocat
        {
            Vector3 dir = (killer.transform.position + killer.transform.up * 1.1f) - cam.position;
            if (dir.sqrMagnitude < 0.0001f) break;

            Quaternion desired = Quaternion.LookRotation(dir);
            cam.rotation = Quaternion.RotateTowards(cam.rotation, desired, killCamTurnSpeed * Time.deltaTime);

            if (Quaternion.Angle(cam.rotation, desired) < angleThreshold) break;

            t += Time.deltaTime;
            yield return null;
        }
    }
}
