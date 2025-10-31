using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorUnlock : MonoBehaviour
{
    public ObjectiveManager objectiveManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter() {
        objectiveManager.setObjectiveCompleted(true);
        Destroy(gameObject);
        Debug.Log("Door Unlocked");
    }
}
