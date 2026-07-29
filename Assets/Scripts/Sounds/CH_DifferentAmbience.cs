using UnityEngine;

public class CH_DifferentAmbience : MonoBehaviour
{
    [Header("Ambience Clips")]
    [SerializeField] private AudioClip outsideClip;
    [SerializeField] private AudioClip insideClip;

    [Header("Transition")]
    [SerializeField, Range(0.5f, 10f)] private float transitionDuration = 2f;
    [SerializeField, Range(0.5f, 15f)] private float doorwayRadius = 3f;
    [SerializeField, Range(0.1f, 1f)] private float outsideAtDoorway = 0.6f;
    [SerializeField, Range(0.1f, 1f)] private float insideAtDoorway = 0.7f;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool startOutside = true;

    private AudioSource outsideSource;
    private AudioSource insideSource;
    private Transform playerTransform;
    private bool isInside;

    private void Awake()
    {
        outsideSource = CreateAudioSource("OutsideAmbience");
        insideSource = CreateAudioSource("InsideAmbience");
    }

    private void Start()
    {
        ConfigureSource(outsideSource, outsideClip);
        ConfigureSource(insideSource, insideClip);

        outsideSource.volume = startOutside ? 1f : 0f;
        insideSource.volume = startOutside ? 0f : 1f;

        if (outsideSource.clip != null)
        {
            outsideSource.Play();
        }

        if (insideSource.clip != null)
        {
            insideSource.Play();
        }
    }

    private void Update()
    {
        if (playerTransform == null)
        {
            playerTransform = FindPlayer();
            if (playerTransform == null)
            {
                return;
            }
        }

        float distanceToDoorway = Vector3.Distance(playerTransform.position, transform.position);
        float doorwayBlend = Mathf.Clamp01(1f - (distanceToDoorway / doorwayRadius));

        float outsideTarget;
        float insideTarget;

        if (isInside)
        {
            outsideTarget = Mathf.Lerp(0.15f, outsideAtDoorway, doorwayBlend);
            insideTarget = Mathf.Lerp(0.95f, insideAtDoorway, doorwayBlend);
        }
        else
        {
            outsideTarget = 1f;
            insideTarget = 0.15f;
        }

        outsideSource.volume = Mathf.Lerp(outsideSource.volume, outsideTarget, Time.deltaTime * 2f);
        insideSource.volume = Mathf.Lerp(insideSource.volume, insideTarget, Time.deltaTime * 2f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag))
        {
            return;
        }

        playerTransform = other.transform;
        isInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag))
        {
            return;
        }

        playerTransform = other.transform;
        isInside = false;
    }

    private AudioSource CreateAudioSource(string sourceName)
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.loop = true;
        source.spatialBlend = 0f;
        source.rolloffMode = AudioRolloffMode.Linear;
        source.minDistance = 0.5f;
        source.maxDistance = 20f;
        source.volume = 0f;
        source.name = sourceName;
        return source;
    }

    private void ConfigureSource(AudioSource source, AudioClip clip)
    {
        source.clip = clip;
        source.loop = true;
        source.playOnAwake = false;
        source.volume = 0f;
    }

    private Transform FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        return player != null ? player.transform : null;
    }
}
