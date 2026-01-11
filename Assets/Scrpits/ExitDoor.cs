using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    [SerializeField] GameObject activeExitDoor;

    public void OpenDoor()
    {
        activeExitDoor.SetActive(true);
        Destroy(gameObject);
    }
    
}
