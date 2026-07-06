using UnityEngine;
using UnityEngine.UI;
public class CM_ComputerIconClass : MonoBehaviour
{

    [SerializeField] private GameObject computerDialog;
    


    //iCON BUTTON FROM DESKTOP TO OPEN COMPUTER DIALOG
    public void OpenComputerDialog()
    {
        computerDialog.SetActive(true);
    }

    //CLOSE BUTTON LIKE IN WINDOWS
    public void CloseComputerDialog()
    {
        computerDialog.SetActive(false);
    }




}
