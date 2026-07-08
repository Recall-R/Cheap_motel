using UnityEngine;
using CH_AICharacter;
using System.Collections.Generic;
public class CH_AIManager : MonoBehaviour
{
    [SerializeField]
    private CH_AICharacterClass _aiCharacter = new CH_AICharacterClass();

    [SerializeField]
    private GameObject CH_SpawnPoint;

    private void Start()
    {
        if (CH_SpawnPoint == null)
        {
            Debug.LogWarning("Spawn point is not assigned in inspector.");
            return;
        }

        _aiCharacter.InvokeCharacter(CH_SpawnPoint.transform.position); 
    }

}
