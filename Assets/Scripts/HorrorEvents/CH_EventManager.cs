using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;
using CH_AICharacter;
public class CH_EventManager : MonoBehaviour
{
    public static CH_EventManager Instance { get; private set; }

    [SerializeField] private List<int> roomsThatHaveKiller = new List<int>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("CH_EventManager: Există deja o instanță activă. Se distruge duplicatul.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void isAKillerInScene()
    {
        for (int i = 0; i < CH_RoomManager.Instance.GetRoomCount(); i++)
        {
            if (CH_RoomManager.Instance.isInRoomKiller(i))
            {
                if (!roomsThatHaveKiller.Contains(i))
                {
                    roomsThatHaveKiller.Add(i);
                }
            }
            else
            {
                if (roomsThatHaveKiller.Contains(i))
                {
                    roomsThatHaveKiller.Remove(i);
                }
            }
        }
    }

    void WatcherEvent()
    {
        GameObject tmp_npcCharacter = null;
        for (int i = 0; i < roomsThatHaveKiller.Count; i++)
        {
            int roomIndex = roomsThatHaveKiller[i];
            GameObject npcCharacter = CH_RoomManager.Instance.getNpcCharacterInRoom(roomIndex);
            tmp_npcCharacter = npcCharacter;
            if (npcCharacter != null)
            {
                if(npcCharacter.GetComponent<CH_NPCSuspicionState>().SuspicionProfile.DangerType == "watcher") {
                    npcCharacter.GetComponent<NavMeshAgent>().SetDestination(CH_Manager.Instance.GetFPSController().transform.position);
                    break;
                }
            }
        }

        if(Vector3.Distance(CH_Manager.Instance.GetFPSController().transform.position, tmp_npcCharacter.transform.position) < 2f) {
            // Implement the logic for when the player is caught by the watcher
            Debug.Log("Player caught by watcher!");

        }
    }


    public void StartEvent()
    {
        
        WatcherEvent();
    }

}
