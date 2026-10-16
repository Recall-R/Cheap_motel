using UnityEngine;

public class CH_RainFollow : MonoBehaviour
{
    private Transform player;

    [SerializeField]
    private Vector3 positionOffset = new Vector3(0f, 30f, 0f);
    void Start()
    {
        player = CH_Manager.Instance.GetFPSController().transform;
    }


    void LateUpdate()
    {
        this.transform.position = player.position + positionOffset;
        this.transform.rotation = Quaternion.identity;
    }
}
