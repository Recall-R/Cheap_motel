
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.AI;
using CH_AICharacter;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Collections;
using UnityEngine.Events;

[System.Serializable]
public class EventClass
{
    public string eventName;
    public UnityEvent eventAction;

    [Range(0f, 1f)]
    public float weight = 1f;

    public float cooldown = 30f;

    private float lastTriggeredTime = -999f;

    public ConditionAbstract[] condition;
    public float duration = 2f;
    public bool isForWatcherFollow = false;

    public bool subEventActive = false;

    public UnityEvent subEventAction;

    public bool isAvailable()
    {
        bool coolDownOk = Time.time - lastTriggeredTime >= cooldown;
        bool conditionOk = condition == null || condition.All(c => c.Evaluate());

        return coolDownOk && conditionOk;
    }

    public void MarkTriggered()
    {
        lastTriggeredTime = Time.time;
    }
}



public class CH_EventManager : MonoBehaviour
{
    public static CH_EventManager Instance { get; private set; }

    [Header("Event Settings")]
    [SerializeField] private List<EventClass> events = new List<EventClass>();
    [SerializeField] private bool isBusy = false;
    public bool IsBusy
    {
        get { return isBusy; }
        set { isBusy = value; }
    }
    [Header("Wacher killer Settings")]
    [SerializeField] private List<int> roomsThatHaveKiller = new List<int>();
    [SerializeField] private PlayableDirector deathTimeline;
    [SerializeField] private string killerAnimationTrackName;

    [Header("Watcher")]
    [SerializeField] private float catchDistance = 2f;   // de la ce distanță se declanșează
    [SerializeField] private float killDistance = 1.2f;  // distanța NPC-jucător la începutul animației
    [SerializeField] private float trackRotationOffsetY = -30f; // offset rotație pe track

    [SerializeField] private GameObject deathScreenCanvas;
    private bool caught;

    [Header("Additions")]
    [SerializeField] private GameObject exitPointForNpcs;
    
    [SerializeField] private bool isPlayerInInterior_WATCHER;

    //getter setter isPlayerInInterior_WATCHER
    public bool IsPlayerInInterior_WATCHER
    {
        get { return isPlayerInInterior_WATCHER; }
        set { isPlayerInInterior_WATCHER = value; }
    }
    [SerializeField] private Animator cameraAnimator;


    private Coroutine watcherFollowCoroutine;


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
                    roomsThatHaveKiller.Add(i);
            }
            else
            {
                if (roomsThatHaveKiller.Contains(i))
                    roomsThatHaveKiller.Remove(i);
            }
        }
    }


    public GameObject GetGameObjectKillerFromRoom(string dangerType) {
        
        for (int i = 0; i < roomsThatHaveKiller.Count; i++)
        {
            int roomIndex = roomsThatHaveKiller[i];
            GameObject npcCharacter = CH_RoomManager.Instance.getNpcCharacterInRoom(roomIndex);
            if (npcCharacter != null)
            {
                var suspicion = npcCharacter.GetComponent<CH_NPCSuspicionState>();
                if (suspicion.SuspicionProfile.DangerType == dangerType)
                {
                    return npcCharacter;
                }
            }
        }
        return null;
    }

    public bool isWatcherInScene(string killer)
    {
        for (int i = 0; i < roomsThatHaveKiller.Count; i++)
        {
            int roomIndex = roomsThatHaveKiller[i];
            GameObject npcCharacter = CH_RoomManager.Instance.getNpcCharacterInRoom(roomIndex);
            if (npcCharacter != null)
            {
                var suspicion = npcCharacter.GetComponent<CH_NPCSuspicionState>();
                if (suspicion.SuspicionProfile.DangerType == killer)
                {
                    return true;
                }
            }
        }
        return false;
    }

    // public void WatcherEvent()
    // {
    //     if (caught) return;

    //     Transform player = CH_Manager.Instance.GetFPSController().transform;
    //     GameObject npcCharacter = GetGameObjectKillerFromRoom("watcher");
    //     NavMeshAgent agent = npcCharacter?.GetComponent<NavMeshAgent>();

    //     //avoid null reference exception
    //     if (npcCharacter == null) return;   
        
    //     if (Vector3.Distance(player.position, npcCharacter.transform.position) < catchDistance)
    //     {
    //         caught = true;
    //         StartCoroutine(KillSequence(npcCharacter));

    //     }
    //     else
    //     {
    //         agent.isStopped = false;
    //         agent.SetDestination(player.position);
    //     }        
    // }

    public void WatcherEvent()
    {
        if (caught) return;

        if (watcherFollowCoroutine != null)
            StopCoroutine(watcherFollowCoroutine);

        watcherFollowCoroutine = StartCoroutine(WatcherFollowRoutine());
    }

    private IEnumerator WatcherFollowRoutine()
    {
        GameObject npcCharacter = GetGameObjectKillerFromRoom("watcher");
        NavMeshAgent agent = npcCharacter?.GetComponent<NavMeshAgent>();

        if (npcCharacter == null || agent == null)
        {
            isBusy = false;
            yield break;
        }

        agent.isStopped = false;

        while (!caught && !IsPlayerInInterior_WATCHER)
        {
            Transform player = CH_Manager.Instance.GetFPSController().transform;

            if (Vector3.Distance(player.position, npcCharacter.transform.position) < catchDistance)
            {
                caught = true;
                yield return StartCoroutine(KillSequence(npcCharacter));
                break;
            }

            agent.SetDestination(player.position);
            npcCharacter.GetComponent<Animator>().SetBool("Running", true); //get animator from killer and set running to true
            agent.speed= 4.7f;
            yield return new WaitForSeconds(0.2f); // interval de update, nu la fiecare frame
        }

        if (!caught && IsPlayerInInterior_WATCHER)
        {
            agent.isStopped = true;
            agent.ResetPath();
            agent.SetDestination(exitPointForNpcs.transform.position);
            isBusy = false;
        }

        isBusy = false;
    }



    private IEnumerator KillSequence(GameObject killer)
    {
        // NPC-ul stă pe loc cât se întoarce camera
        var agent = killer.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.isStopped = true;
            agent.ResetPath();
            agent.velocity = Vector3.zero;
        }

        // 1. camera se întoarce spre NPC
        yield return CH_Manager.Instance.KillCamRoutine(killer);

        // 2. abia apoi pornește Timeline-ul
        TriggerDeathTimeline(killer);
    }
    
    public void TurnOnDeathScreen()
    {
        if (deathScreenCanvas != null)
        {
            deathScreenCanvas.SetActive(true);
        }
    }

    public void StartEvent()
    {
        if (isBusy) return; // dacă rulează deja ceva, nu verifica nimic altceva

        var available = events
            .Where(e => e.isAvailable()).ToList();
        if (available.Count == 0) return;

        // decidem random DACA declanșăm ceva acum (nu la fiecare frame automat)
        if (Random.value < 0.002f) // ajustezi probabilitatea per frame
        {
            TriggerRandomEvent(available);
        }
    }

    public void ResetWatcher()
    {
        caught = false;
    }


    private void TriggerRandomEvent(List<EventClass> available)
    {
        EventClass chosen = PickWeighted(available);
        StartCoroutine(RunEvent(chosen));
    }

     private EventClass PickWeighted(List<EventClass> list)
    {
        float totalWeight = list.Sum(e => e.weight);
        float roll = Random.value * totalWeight;
        float cumulative = 0f;

        foreach (var e in list)
        {
            cumulative += e.weight;
            if (roll <= cumulative)
                return e;
        }
        return list[list.Count - 1]; // fallback
    }

    private System.Collections.IEnumerator RunEvent(EventClass e)
    {
        isBusy = true;
        e.MarkTriggered();

        Debug.LogWarning($"Triggered: {e.eventName}");

        e.eventAction?.Invoke();

        if (e.subEventActive && e.subEventAction != null)
        {
            e.subEventAction.Invoke();
        }
        yield return new WaitForSeconds(e.duration);

        //isBusy = false;
    
    }


    //Killer DeathTImeline

    public void TriggerDeathTimeline(GameObject killer)
    {
        if (deathTimeline == null)
        {
            Debug.LogWarning("CH_EventManager: Timeline-ul nu este setat.");
            return;
        }

        if (killer == null)
        {
            Debug.LogWarning("CH_EventManager: Killer-ul nu este setat.");
            return;
        }

        Animator killerAnimator = killer.GetComponent<Animator>();
        if (killerAnimator == null)
        {
            Debug.LogWarning("CH_EventManager: Killer-ul nu are Animator.");
            return;
        }

        deathTimeline.Stop();

        Transform player = CH_Manager.Instance.GetFPSController().transform;

        // 1. Oprește complet NavMesh-ul (ResetPath ÎNAINTE de disable)
        var agent = killer.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
         //   agent.isStopped = true;
            agent.ResetPath();
            agent.velocity = Vector3.zero;
           // agent.enabled = false;
        }

        // 2. Blochează controlul jucătorului
        var cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;
        // TODO: dezactivează și scriptul de mișcare/look al FPS controller-ului
        // player.GetComponent<TipulTauDeFPSController>().enabled = false;

        // 3. Aliniază NPC-ul față de jucător
        // Vector3 dir = killer.transform.position - player.position;
        // dir.y = 0f;
        // dir = dir.sqrMagnitude > 0.0001f ? dir.normalized : -player.forward;

        // Vector3 pos = player.position + dir * killDistance;
        // pos.y = killer.transform.position.y;
        // Quaternion rot = Quaternion.LookRotation(-dir);
        // killer.transform.SetPositionAndRotation(pos, rot);
        // 3. Poziția NPC-ului: în fața jucătorului (mereu aceeași relație)
        // Vector3 playerFwd = player.forward;
        // playerFwd.y = 0f;
        // playerFwd = playerFwd.sqrMagnitude > 0.0001f ? playerFwd.normalized : Vector3.forward;
        Vector3 playerFwd = Camera.main.transform.forward;
        playerFwd.y = 0f;
        playerFwd = playerFwd.sqrMagnitude > 0.0001f ? playerFwd.normalized : Vector3.forward;

        Vector3 pos = player.position + playerFwd * killDistance;
        pos.y = killer.transform.position.y;

        // Rotația: NPC-ul privește exact spre jucător
        Vector3 toPlayer = player.position - pos;
        toPlayer.y = 0f;
        Quaternion faceRot = Quaternion.LookRotation(toPlayer.normalized);

        killer.transform.SetPositionAndRotation(pos, faceRot);
        killer.transform.LookAt(player.position); // privirea către jucător

        Vector3 playerToKiller = killer.transform.position - player.position;
        playerToKiller.y = 0f;
        if (playerToKiller.sqrMagnitude > 0.0001f)
            player.rotation = Quaternion.LookRotation(playerToKiller.normalized);

        Transform mainCamera = Camera.main.transform;
        Vector3 cameraTarget = killer.transform.position + killer.transform.up * 1.1f;
        Vector3 cameraDirection = cameraTarget - mainCamera.position;
        if (cameraDirection.sqrMagnitude > 0.0001f)
            mainCamera.rotation = Quaternion.LookRotation(cameraDirection);

        // 4. Bind + offset pe track-ul killer-ului
        bool trackBound = false;
        foreach (PlayableBinding output in deathTimeline.playableAsset.outputs)
        {
            if (output.sourceObject is AnimationTrack animTrack && output.streamName == killerAnimationTrackName)
            {
                animTrack.trackOffset = TrackOffset.ApplyTransformOffsets;
                animTrack.position = pos;
                // animTrack.rotation = rot;
                animTrack.rotation = faceRot * Quaternion.Euler(0f, trackRotationOffsetY, 0f);;

                deathTimeline.SetGenericBinding(animTrack, killerAnimator);
                trackBound = true;
                break;
            }
        }
        

        if (!trackBound)
        {
            Debug.LogWarning($"CH_EventManager: Animation Track '{killerAnimationTrackName}' nu a fost găsit.");
            return;
        }

        deathTimeline.RebuildGraph(); // obligatoriu după ce schimbi offset-urile
        deathTimeline.time = 0;
        cameraAnimator.enabled = true;
        deathTimeline.Play();
    }
}