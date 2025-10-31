using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    private bool objectiveCompleted;
    // Start is called before the first frame update
    void Start()
    {
        objectiveCompleted = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setObjectiveCompleted(bool status) {
        objectiveCompleted = status;
    }

    public bool getObjectiveCompleted() {
        return objectiveCompleted;
    }
}
