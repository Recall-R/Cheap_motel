using UnityEngine;
using System;
using TMPro;
public class CH_NightDirectr: MonoBehaviour
{
    public static CH_NightDirectr Instance { get; private set; }
    
    [SerializeField] private int startHour = 22;   // ora de start (22:00)
    [SerializeField] private int endHour = 5;       // ora de final (05:00, dupa miezul noptii)
 
    [Header("Viteza timpului")]
    [Tooltip("Cate secunde reale dureaza 1 minut de joc. Valoare mai mica = timpul trece mai repede.")]
    [SerializeField] private float realSecondsPerGameMinute = 1f;
    [SerializeField] private TMP_Text clockText; // referinta la textul UI pentru ceas

    // minute scurse de la inceputul turei (0 = startHour:00)
    private float currentTotalMinutes;
    private int totalShiftMinutes;
    private bool isRunning;
 
    public int CurrentHour { get; private set; }
    public int CurrentMinute { get; private set; }
 
    /// <summary>Se declanseaza de fiecare data cand se schimba minutul afisat.</summary>
    public event Action<int, int> OnTimeChanged;
 
    /// <summary>Se declanseaza o singura data, cand se ajunge la ora de final.</summary>
    public event Action OnShiftEnded;
 
 
    public void PauseShift() => isRunning = false;
    public void ResumeShift() => isRunning = true;
 
    private void Awake()
    {
        // calculam durata totala a turei in minute, gestionand trecerea peste miezul noptii
        int startTotal = startHour * 60;
        int endTotal = endHour * 60;
 
        if (endTotal <= startTotal)
            endTotal += 24 * 60; // ex: 22:00 -> 05:00 devine 22:00 -> 29:00 (adica 7 ore)
 
        totalShiftMinutes = endTotal - startTotal;
        ResumeShift(); // pornim tura imediat la startul scenei
    }
 
    //TIME MANAGEMENT
    /// <summary>Porneste tura de la inceput (currentTotalMinutes = 0).</summary>
    public void StartShift()
    {
        currentTotalMinutes = 0f;
        isRunning = true;
        UpdateClockValues();
    }
    private void Update()
    {
        if (!isRunning) return;
 
        float minutesPerSecond = 1f / realSecondsPerGameMinute;
        float previousMinutes = currentTotalMinutes;
        currentTotalMinutes += Time.deltaTime * minutesPerSecond;
 
        // am ajuns la finalul turei
        if (currentTotalMinutes >= totalShiftMinutes)
        {
            currentTotalMinutes = totalShiftMinutes;
            isRunning = false;
            UpdateClockValues();
            OnShiftEnded?.Invoke();
            return;
        }
 
        // declansam evenimentul doar cand se schimba minutul intreg afisat, nu la fiecare frame
        if (Mathf.FloorToInt(previousMinutes) != Mathf.FloorToInt(currentTotalMinutes))
        {
            UpdateClockValues();
        }
    }
 
    private void UpdateClockValues()
    {
        int absoluteMinutes = (startHour * 60) + Mathf.FloorToInt(currentTotalMinutes);
        absoluteMinutes %= (24 * 60); // wrap la 24h, ca ora afisata sa fie corecta (ex: 29:00 -> 05:00)
 
        CurrentHour = absoluteMinutes / 60;
        CurrentMinute = absoluteMinutes % 60;
 
        if (clockText != null)
            clockText.text = GetFormattedTime();
 
        OnTimeChanged?.Invoke(CurrentHour, CurrentMinute);
    }
 
    /// <summary>Ora curenta formatata, ex: "23:47".</summary>
    public string GetFormattedTime() => $"{CurrentHour:00}:{CurrentMinute:00}";
 
    /// <summary>Progresul turei, de la 0 (inceput) la 1 (final).</summary>
    public float GetShiftProgress01() =>
        totalShiftMinutes <= 0 ? 0f : currentTotalMinutes / totalShiftMinutes;



    void isSpawnableAllowed()
    {
        if(!CH_RoomManager.Instance.isAtleastOneRoomFree())
        {
            Debug.Log("No free rooms available. Cannot spawn AI character.");
            return;
        }
        if(CH_AICharacterQueueManager.Instance == null || !CH_AICharacterQueueManager.Instance.IsQueueEmpty())
        {
            Debug.Log("Some AI characters are already in the queue. Cannot spawn new character.");
            return;
        }

        CH_AIManager aiManager = CH_AIManager.Instance;
        aiManager.InvokeAICharacter();
    }

}