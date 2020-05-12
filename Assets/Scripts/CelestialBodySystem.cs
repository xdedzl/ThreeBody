using UnityEngine;

/// <summary>
/// 天体系统
/// </summary>
public class CelestialBodySystem : MonoBehaviour
{
    //public static float G = 0.0000000000667f;

    public float G = 0.0000000000667f;
    private CelestialBody[] bodies;

    [Header("Debug")]
    public bool precomputation = true;
    public float stepTime = 1;
    [Range(2, 100000)]
    public int stepCount = 1000;

    void Awake()
    {
        bodies = GameObject.FindObjectsOfType<CelestialBody>();
        Time.fixedDeltaTime = stepTime;
    }


    void FixedUpdate()
    {
        float deltaTime = Time.fixedDeltaTime;

        // 更新天体速度
        foreach (var body in bodies)
        {
            UpdateVelocity(body, bodies, deltaTime);
        }

        foreach (var body in bodies)
        {
            UpdatePosition(body, deltaTime);
        }
    }

    /// <summary>
    /// 更新速度
    /// </summary>
    private void UpdateVelocity(CelestialBody body, CelestialBody[] bodies, float deltaTime)
    {
        foreach (var otherBody in bodies)
        {
            if (body != otherBody)
            {
                Vector3 dir = (otherBody.transform.position - body.transform.position).normalized;
                float sqrDis = (body.transform.position - otherBody.transform.position).sqrMagnitude;
                Vector3 acceleration = (G * otherBody.mass / sqrDis) * dir;

                body.velocity += acceleration * deltaTime;
            }
        }
    }

    /// <summary>
    /// 更新位置
    /// </summary>
    private void UpdatePosition(CelestialBody body, float deltaTime)
    {
        //body.transform.Translate(body.velocity * deltaTime);
        body.transform.position += body.velocity * deltaTime;
    }

    private void OnDrawGizmos()
    {
        if (precomputation)
        {
            var bodies = GameObject.FindObjectsOfType<CelestialBody>();

            VirtualBody[] virtualBodies = new VirtualBody[bodies.Length];
            for (int i = 0; i < bodies.Length; i++)
            {
                virtualBodies[i] = new VirtualBody(bodies[i]);
            }

            Vector3[][] bodiesPath = new Vector3[virtualBodies.Length][];
            for (int i = 0; i < bodiesPath.Length; i++)
            {
                bodiesPath[i] = new Vector3[stepCount + 1];
                bodiesPath[i][0] = virtualBodies[i].position;
            }

            for (int step = 1; step <= stepCount; step++)
            {
                for (int index = 0; index < virtualBodies.Length; index++)
                {
                    VirtualBody virtualBody = virtualBodies[index];
                    PreCalculate(virtualBody, virtualBodies, stepTime);
                }

                for (int index = 0; index < virtualBodies.Length; index++)
                {
                    VirtualBody virtualBody = virtualBodies[index];
                    virtualBody.position += virtualBody.velocity * stepTime;

                    bodiesPath[index][step] = virtualBody.position;
                }

            }

            for (int i = 0; i < bodiesPath.Length; i++)
            {
                var path = bodiesPath[i];

                if (path.Length > 2)
                {
                    Gizmos.color = bodies[i].color;
                    for (int j = 0; j < path.Length - 1; j++)
                    {
                        Gizmos.DrawLine(path[j], path[j + 1]);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 预计算
    /// </summary>
    private void PreCalculate(VirtualBody body, VirtualBody[] bodies, float deltaTime)
    {
        foreach (var otherBody in bodies)
        {
            if (body != otherBody)
            {
                Vector3 dir = (otherBody.position - body.position).normalized;
                float sqrDis = (body.position - otherBody.position).sqrMagnitude;
                Vector3 acceleration = (G * otherBody.mass / sqrDis) * dir;

                body.velocity += acceleration * deltaTime;
            }
        }
    }
}