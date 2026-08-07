using UnityEngine;

[RequireComponent(typeof(Animator))]
public class FootIK : MonoBehaviour
{
    [Header("IK Settings")]
    [Range(0, 1)] public float ikWeight = 1.0f;
    public LayerMask groundLayers = ~0; // seteaza in Inspector doar layer-ul de teren/scari

    [Header("Raycast")]
    public float raycastDistance = 1.0f;   // cat de jos verificam sub picior
    public float footOffset = 0.05f;       // ridica putin talpa deasupra suprafetei (evita clipping)
    public float heightSmoothSpeed = 10f;  // cat de repede se adapteaza inaltimea (evita teleport brusc)

    private Animator anim;

    // tinem ultima inaltime aplicata pentru fiecare picior, ca sa interpolam lin
    private float lastLeftY, lastRightY;
    private bool initialized = false;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (anim == null) return;

        ProcessFoot(AvatarIKGoal.LeftFoot, ref lastLeftY);
        ProcessFoot(AvatarIKGoal.RightFoot, ref lastRightY);

        if (!initialized) initialized = true;
    }

    void ProcessFoot(AvatarIKGoal foot, ref float lastY)
    {
        // pozitia curenta a piciorului din animatie (inainte de IK)
        Vector3 footPos = anim.GetIKPosition(foot);

        Ray ray = new Ray(footPos + Vector3.up * raycastDistance * 0.5f, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance, groundLayers))
        {
            float targetY = hit.point.y + footOffset;

            // prima data cand ruleaza, nu avem valoare anterioara -> setam direct
            if (!initialized) lastY = targetY;

            // interpolam lin ca sa nu "sara" piciorul brusc cand apare treapta
            lastY = Mathf.Lerp(lastY, targetY, Time.deltaTime * heightSmoothSpeed);

            Vector3 newPos = footPos;
            newPos.y = lastY;

            anim.SetIKPositionWeight(foot, ikWeight);
            anim.SetIKPosition(foot, newPos);

            // optional: aliniaza si rotatia talpii cu panta terenului
            anim.SetIKRotationWeight(foot, ikWeight);
            Quaternion footRot = anim.GetIKRotation(foot);
            Quaternion targetRot = Quaternion.FromToRotation(Vector3.up, hit.normal) * footRot;
            anim.SetIKRotation(foot, Quaternion.Slerp(footRot, targetRot, Time.deltaTime * heightSmoothSpeed));
        }
        else
        {
            // nimic sub picior -> lasam animatia originala sa decida (fara IK)
            anim.SetIKPositionWeight(foot, 0f);
            anim.SetIKRotationWeight(foot, 0f);
        }
    }
}