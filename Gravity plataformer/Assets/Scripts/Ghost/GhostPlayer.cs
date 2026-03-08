using System.Collections.Generic;
using UnityEngine;

public class GhostPlayer : MonoBehaviour
{
    private List<PlayerFrame> frames;
    private int frameIndex = 0;

    [SerializeField] private float playbackRate = 0.02f;
    private float timer;

    public void Initialize(List<PlayerFrame> recordedFrames)
    {
        frames = new List<PlayerFrame>(recordedFrames);
    }

    void Update()
    {
        if (frames == null || frames.Count == 0)
            return;

        timer += Time.deltaTime;

        if (timer >= playbackRate)
        {
            timer = 0f;

            if (frameIndex >= frames.Count)
            {
                Destroy(gameObject);
                return;
            }

            PlayerFrame frame = frames[frameIndex];

            transform.position = frame.position;
            transform.rotation = frame.rotation;

            frameIndex++;
        }
    }
}