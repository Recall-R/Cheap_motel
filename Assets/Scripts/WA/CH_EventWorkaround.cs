using UnityEngine;

public class CH_EventWorkaround : MonoBehaviour
{
    
    public void WA_TrySpawnClient()
    {
        StartCoroutine(TrySpawnClientNextFrame());
    }

    private System.Collections.IEnumerator TrySpawnClientNextFrame()
    {
        yield return null;

        if (CH_NightDirectr.Instance != null && CH_NightDirectr.Instance.isActiveAndEnabled)
        {
            CH_NightDirectr.Instance.isSpawnableAllowed();
        }
    }
    public void WA_TryDespawnBus()
    {
        if(CH_BusManager.Instance != null && CH_BusManager.Instance.isActiveAndEnabled)
        {
            CH_BusManager.Instance.DestroyBus(this.gameObject);
        }
    }
}
