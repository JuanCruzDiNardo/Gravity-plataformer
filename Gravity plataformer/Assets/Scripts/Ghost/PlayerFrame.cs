using UnityEngine;

[System.Serializable]
public struct PlayerFrame
{
    public Vector3 position;
    public Quaternion rotation;
    public GravityController.GravityDirection gravity;

    public PlayerFrame(Vector3 pos, Quaternion rot, GravityController.GravityDirection grav)
    {
        position = pos;
        rotation = rot;
        gravity = grav;
    }
}