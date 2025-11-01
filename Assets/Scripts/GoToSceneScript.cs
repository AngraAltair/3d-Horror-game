using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoBackCode : MonoBehaviour
{
    public GameObject ReturnObject;
    public string ThisScene;

    public void Return()
    {
        SceneManager.LoadScene(ThisScene);
    }

}

