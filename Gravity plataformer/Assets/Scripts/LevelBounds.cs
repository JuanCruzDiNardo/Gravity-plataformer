using UnityEngine;

public class LevelBounds : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.ResetLevel();
        }
    }
}