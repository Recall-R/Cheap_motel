using UnityEngine;


public class CH_Lights : MonoBehaviour
{

    //this is switch lighting system for a room
    static readonly int EmissiveColorId = Shader.PropertyToID("_EmissiveColor");
    static readonly int EmissiveColorLDRId = Shader.PropertyToID("_EmissiveColorLDR");
    static readonly int EmissiveIntensityId = Shader.PropertyToID("_EmissiveIntensity");
    static readonly int EmissiveIntensityUnitId = Shader.PropertyToID("_EmissiveIntensityUnit");
    static readonly int UseEmissiveIntensityId = Shader.PropertyToID("_UseEmissiveIntensity");
    static readonly int EmissiveExposureWeightId = Shader.PropertyToID("_EmissiveExposureWeight");

    Material[] bulbMaterials;
    Color[] originalEmissive;

    const float OnEV100 = 13f;

    static float EV100ToNits(float ev) => Mathf.Pow(2f, ev - 3f);

    [SerializeField] private GameObject[] lightsObject;
    [SerializeField] private GameObject[] lightsBulb; //meshes renders emission of a light bulb
    [SerializeField] private bool isOn = true;




    [SerializeField] private bool isSwitchable = true;

    void Awake()
    {
        bulbMaterials = new Material[lightsBulb.Length];
        originalEmissive = new Color[lightsBulb.Length];

        for (int i = 0; i < lightsBulb.Length; i++)
        {
            if (lightsBulb[i] == null) continue;

            var m = lightsBulb[i].GetComponent<Renderer>().materials[1];
            bulbMaterials[i] = m;
            originalEmissive[i] = m.GetColor(EmissiveColorId); // valoarea initiala din inspector

            m.SetFloat(EmissiveExposureWeightId, 0f); // Exposure Weight 0 in inspector
        }
    }

    public bool getIsSwitchable()
    {
        return isSwitchable;
    }
    public void setIsSwitchable(bool state)
    {
        isSwitchable = state;
    }

    public bool getISOn()
    {
        return isOn;
    }
    public void setIsOn(bool state)
    {
        isOn = state;
        SetLightsState(state);
        emissionLightBulb(state);
        if (state)
        {
            CH_SoundManager.instance.PlaySound("LightOn");
        }
        else
        {
            CH_SoundManager.instance.PlaySound("LightOff");
        }
    }

    //need to implement a switch for emission of a light bulb 
    public void SetLightsState(bool state)
    {
        if(isSwitchable) {
        foreach (GameObject lightObj in lightsObject)
        {
            if (lightObj != null)
            {
                lightObj.SetActive(state);
            }
        }
        }
    }
    public void emissionLightBulb(bool state)
    {
        for (int i = 0; i < bulbMaterials.Length; i++)
        {
            if (bulbMaterials[i] == null) continue;
            bulbMaterials[i].SetColor(EmissiveColorId, state ? originalEmissive[i] : Color.black);
        }
    }
}