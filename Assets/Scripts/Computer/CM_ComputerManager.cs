using UnityEngine;

public class CM_ComputerManager : MonoBehaviour
{
    //Here you can add any additional functionality for managing the computer system, such as handling multiple computer icons, managing their states, or integrating with other systems in your game.

    void Update()
    {
        // if(!CH_HorrorManager.Instance.IsAllLightsOn && CH_RenderingMonitor.Instance != null && CH_RenderingMonitor.Instance.IsInMonitorMode)
        // {
        //     CH_RenderingMonitor.Instance.OnExitMonitor();
        //     Debug.Log("CM_ComputerManager: Am ieșit din modul de monitorizare deoarece curentul este oprit");
        // }

        if (!CH_HorrorManager.Instance.IsAllLightsOn)
        {
            CH_RenderingMonitor.Instance.IsMonitorUIActive = false;
            if(CH_RenderingMonitor.Instance.IsInMonitorMode)
            {
                CH_RenderingMonitor.Instance.OnExitMonitor();
                Debug.Log("CM_ComputerManager: Am ieșit din modul de monitorizare deoarece curentul este oprit");
            }
        }
    }
    
}
