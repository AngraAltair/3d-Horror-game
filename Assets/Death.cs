// using UnityEngine;
// using UnityEngine.SceneManagement;

// public class Death : MonoBehaviour
// {
//     [Header("Scene Load Options")]
//     [Tooltip("When the player touches a hazard or fall zone the specified scene will be loaded. Leave empty to only log a warning.")]
//     public string sceneToLoad = "";

//     void OnCollisionEnter(Collision collision)
//     {
//         if (collision.gameObject.CompareTag("Hazard"))
//         {
//             HandleDeath();
//         }
//     }

//     void OnTriggerEnter(Collider other)
//     {
//         Debug.Log("Entered trigger: " + other.name);
//         if (other.CompareTag("Death"))
//         {
//             HandleDeath();
//         }
//     }

//     void HandleDeath()
//     {
//         if (!string.IsNullOrEmpty(sceneToLoad))
//         {
//             SceneManager.LoadScene(sceneToLoad);
//         }
//         else
//         {
//             Debug.LogWarning("Death: sceneToLoad is empty — no scene will be loaded. Set the sceneToLoad in the Death component.");
//         }
//     }
// }