using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    [SerializeField]
    private EnemyBehaviour _enemyBehaviour;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log("player entered trigger");
            _enemyBehaviour.PlayerDetected();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("player exited trigger");
            _enemyBehaviour.PlayerLeftScareTrigger();
        }
    }
}
