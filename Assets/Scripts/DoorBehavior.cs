using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorBehavior : MonoBehaviour
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

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            if (objectiveManager.getObjectiveCompleted()) {
                EnterNextLevel();
            } else {
                Debug.Log("Objective not completed yet!");
            }
        }
    }

    void EnterNextLevel() {
        // Logic to transition to the next level
        Debug.Log("Transitioning to the next level...");
        SceneManager.LoadScene("level2");
    }
}
