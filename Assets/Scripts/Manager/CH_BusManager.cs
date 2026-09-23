using UnityEngine;

public class CH_BusManager : MonoBehaviour
{
    public static CH_BusManager Instance { get; private set; }

    [SerializeField] private GameObject busPrefab;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("CH_BusManager: Există deja o instanță activă. Se distruge duplicatul.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SpawnBus(Vector3 position, Quaternion rotation)
    {
        if (busPrefab == null)
        {
            Debug.LogWarning("CH_BusManager: busPrefab nu este setat în inspector.");
            return;
        }

        Quaternion prefabRotation = busPrefab.transform.rotation;
        GameObject busInstance = Instantiate(busPrefab, position, prefabRotation);
        busInstance.transform.SetPositionAndRotation(position, prefabRotation);
    }

    public void DestroyBus(GameObject busInstance)
    {
        if (busInstance == null)
        {
            Debug.LogWarning("CH_BusManager: busInstance este null.");
            return;
        }

        Destroy(busInstance);
    }

    
}
