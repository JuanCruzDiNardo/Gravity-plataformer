using UnityEngine;

public class LevelBoundsTrigger : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.ResetLevel();
        }
    }
}