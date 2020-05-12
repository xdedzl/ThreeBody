using UnityEngine;

/// <summary>
/// 虚拟天体
/// </summary>
public class VirtualBody
{
    public Vector3 position;
    public Vector3 velocity;
    public float mass;

    public VirtualBody(CelestialBody celestialBody)
    {
        this.position = celestialBody.transform.position;
        this.velocity = celestialBody.velocity;
        this.mass = celestialBody.mass;
    }
}