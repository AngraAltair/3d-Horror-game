using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioHeartbeat : MonoBehaviour
{
    public GameObject player;
    public GameObject enemy;
    public AudioSource heartbeatAudio;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (enemy.GetComponent<EnemyPatrol>().getAggroStatus())
        {
            float distance = Vector3.Distance(player.transform.position, enemy.transform.position);
            heartbeatAudio.volume = Mathf.Clamp(1 / distance, 0.1f, 1f);
            if (!heartbeatAudio.isPlaying)
            {
                heartbeatAudio.Play();
            }
        }
        else
        {
            heartbeatAudio.Stop();
        }
    }
}
