using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRecorder : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GravityController gravityController;
    [SerializeField] private GameObject ghostPrefab;

    [Header("Grabación")]
    [SerializeField] private float maxRecordTime = 30f;
    [SerializeField] private float recordRate = 0.02f; //50 FPS

    [Header("UI")]
    [SerializeField] private GameObject recordIndicator;
    [SerializeField] private UnityEngine.UI.Image recordFill;

    private List<PlayerFrame> frames = new List<PlayerFrame>();

    private bool recording = false;
    private float recordTimer = 0f;
    private float recordIntervalTimer = 0f;

    private PlayerInputAction action;

    public static PlayerRecorder Instance;

    private void Awake()
    {
        action = new PlayerInputAction();
    }

    private void OnEnable() => action.Enable();
    private void OnDisable() => action.Disable();

    private void Update()
    {
        if (action.Player.RecordAction.WasPressedThisFrame())
        {
            if (!recording)
                StartRecording();
            else
                StopRecording();
        }

        if (recording)
            Record();
    }

    private void StartRecording()
    {
        frames.Clear();
        recordTimer = 0f;
        recordIntervalTimer = 0f;
        recording = true;


        recordIndicator.SetActive(true);
        recordFill.fillAmount = 1f;
        //Debug.Log("Recording started");
    }

    private void StopRecording()
    {
        recording = false;

        recordIndicator.SetActive(false);

        if (frames.Count > 0)
        {
            SpawnGhost();
        }

        Debug.Log("Recording stopped");
    }

    private void Record()
    {
        recordFill.fillAmount = 1f - (recordTimer / maxRecordTime);

        recordTimer += Time.deltaTime;
        recordIntervalTimer += Time.deltaTime;

        if (recordIntervalTimer >= recordRate)
        {
            recordIntervalTimer = 0f;

            frames.Add(new PlayerFrame(
                transform.position,
                transform.rotation,
                gravityController.GetGravity()
            ));
        }

        if (recordTimer >= maxRecordTime)
        {
            StopRecording();
        }
    }

    private void SpawnGhost()
    {
        GameObject ghost = Instantiate(ghostPrefab, frames[0].position, frames[0].rotation);

        GhostPlayer ghostPlayer = ghost.GetComponent<GhostPlayer>();
        ghostPlayer.Initialize(frames);
    }
}