using UnityEngine;
using UnityEngine.AI;
public class CH_SubEvent_Watcher_Hide : MonoBehaviour
{
    private GameObject Watchernpc;
    public void PrepareSubEvent()
    {
        this.GetComponent<Collider>().enabled = true;
        Watchernpc = CH_EventManager.Instance.GetGameObjectKillerFromRoom("watcher");
        Watchernpc.transform.position = this.transform.position;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CH_EventManager.Instance.WatcherEvent();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            this.GetComponent<Collider>().enabled = false;
        }
    }
}
