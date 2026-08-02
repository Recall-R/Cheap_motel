using System.Collections;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;


public class CH_RenderingMonitor : MonoBehaviour
{
    public static CH_RenderingMonitor Instance { get; private set; }
    
    [Header("Camera References")]
    [Tooltip("Camera principală care se mișcă spre monitor.")]
    [SerializeField] private GameObject mainCamera;

    [Tooltip("Transformul / poziția monitorului către care se deplasează camera.")]
    [SerializeField] private GameObject renderingCamera;

    [Tooltip("FirstPersonController folosit pentru a opri mișcarea când sunt la monitor.")]
    [SerializeField] private FirstPersonController fpsController;

    [Header("Transition Settings")]
    [SerializeField] private float transitionDuration = 1f;
    [SerializeField] private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    [SerializeField] private bool originalSaved;
    private Coroutine transitionCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("CH_RenderingMonitor: Există deja o instanță activă. Se distruge duplicatul.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        if (mainCamera == null)
            Debug.LogWarning("CH_RenderingMonitor: mainCamera nu este setată în inspector.");
        if (renderingCamera == null)
            Debug.LogWarning("CH_RenderingMonitor: renderingCamera nu este setată în inspector.");
        if (fpsController == null)
            Debug.LogWarning("CH_RenderingMonitor: fpsController nu este setat în inspector.");
    }

    public void OnEnterMonitor()
    {
        if (mainCamera == null || renderingCamera == null || fpsController == null)
        {
            Debug.LogWarning("CH_RenderingMonitor: Nu se poate intra în monitor deoarece referințele nu sunt setate.");
            return;
        }
        CH_RoomManager.Instance.setNameInputFieldReadOnly(false);
        originalPosition = mainCamera.transform.position;
        originalRotation = mainCamera.transform.rotation;
        originalSaved = true;

        if (transitionCoroutine != null)
            StopCoroutine(transitionCoroutine);

        transitionCoroutine = StartCoroutine(TransitionCamera(mainCamera.transform, renderingCamera.transform, true));
        CH_Manager.Instance.SetGeneralCanvasActive(false);
    }

    public void OnExitMonitor()
    {
        if (!originalSaved || mainCamera == null || fpsController == null)
        {
            Debug.LogWarning("CH_RenderingMonitor: Nu se poate ieși din monitor deoarece referințele nu sunt setate sau poziția inițială nu este salvată.");
            return;
        }

        if (transitionCoroutine != null)
            StopCoroutine(transitionCoroutine);

        mainCamera.transform.position = originalPosition;
        mainCamera.transform.rotation = originalRotation;
        fpsController.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Debug.Log("CH_RenderingMonitor: Am ieșit din modul de monitorizare.");
        CH_Manager.Instance.SetGeneralCanvasActive(true);
        CH_RoomManager.Instance.setNameInputFieldReadOnly(true);
    }

    private IEnumerator TransitionCamera(Transform cameraTransform, Transform targetTransform, bool enterMonitor)
    {
        Vector3 startPosition = cameraTransform.position;
        Quaternion startRotation = cameraTransform.rotation;
        Vector3 endPosition = enterMonitor ? targetTransform.position : originalPosition;
        Quaternion endRotation = enterMonitor ? targetTransform.rotation : originalRotation;

        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = transitionCurve.Evaluate(Mathf.Clamp01(elapsed / transitionDuration));
            cameraTransform.position = Vector3.Lerp(startPosition, endPosition, t);
            cameraTransform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
            yield return null;
        }

        cameraTransform.position = endPosition;
        cameraTransform.rotation = endRotation;

        if (enterMonitor)
        {
            fpsController.enabled = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Debug.Log("CH_RenderingMonitor: Am intrat în modul de monitorizare.");
        }

        transitionCoroutine = null;
    }
}
