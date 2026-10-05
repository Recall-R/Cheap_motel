
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;
using CH_AICharacter;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Collections;
public class CH_EventManager : MonoBehaviour
{
    public static CH_EventManager Instance { get; private set; }

    [SerializeField] private List<int> roomsThatHaveKiller = new List<int>();
    [SerializeField] private PlayableDirector deathTimeline;
    [SerializeField] private string killerAnimationTrackName;

    [Header("Watcher")]
    [SerializeField] private float catchDistance = 2f;   // de la ce distanță se declanșează
    [SerializeField] private float killDistance = 1.2f;  // distanța NPC-jucător la începutul animației
    [SerializeField] private float trackRotationOffsetY = -30f; // offset rotație pe track

    [SerializeField] private GameObject deathScreenCanvas;
    private bool caught;


    [SerializeField] private bool isPlayerInInterior_WATCHER;

    //getter setter isPlayerInInterior_WATCHER
    public bool IsPlayerInInterior_WATCHER
    {
        get { return isPlayerInInterior_WATCHER; }
        set { isPlayerInInterior_WATCHER = value; }
    }
    [SerializeField] private Animator cameraAnimator;
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

    void WatcherEvent()
    {
        if (caught) return;

        Transform player = CH_Manager.Instance.GetFPSController().transform;

        for (int i = 0; i < roomsThatHaveKiller.Count; i++)
        {
            int roomIndex = roomsThatHaveKiller[i];
            GameObject npcCharacter = CH_RoomManager.Instance.getNpcCharacterInRoom(roomIndex);
            if (npcCharacter == null) continue;

            var suspicion = npcCharacter.GetComponent<CH_NPCSuspicionState>();
            if (suspicion.SuspicionProfile.DangerType != "watcher") continue;

            var agent = npcCharacter.GetComponent<NavMeshAgent>();

        
            if (Vector3.Distance(player.position, npcCharacter.transform.position) < catchDistance)
            {
                caught = true;
                StartCoroutine(KillSequence(npcCharacter));

            }
            else
            {
                agent.isStopped = false;
                agent.SetDestination(player.position);
            }
            break;
        }
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

    public void StartEvent()
    {
        WatcherEvent();
    }

    public void ResetWatcher()
    {
        caught = false;
    }

    public void TurnOnDeathScreen()
    {
        if (deathScreenCanvas != null)
        {
            deathScreenCanvas.SetActive(true);
        }
    }

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