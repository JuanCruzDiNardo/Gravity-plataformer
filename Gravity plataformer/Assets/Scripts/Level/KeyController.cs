using System;
using UnityEngine;

public class KeyController : MonoBehaviour
{
    private ExitController exitPlatform;
    private int keyIndex;
    private bool activated = false;

    public static event Action onKeyPickup;

    public void Initialize(ExitController platform, int index)
    {
        exitPlatform = platform;
        keyIndex = index;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (activated)
            return;

        if (other.CompareTag("Player"))
        {
            activated = true;
            onKeyPickup?.Invoke();
            exitPlatform.ActivateKey(keyIndex);
            gameObject.SetActive(false);
        }
    }
}