using System;
using UnityEngine;

[Serializable]
public class CloudData
{
    [SerializeField]
    Transform prefab = null;

    [SerializeField]
    float minHeight = 10f;

    [SerializeField]
    float maxHeight = 20f;

    public Transform Prefab { get { return prefab; } }
    public float MinHeight { get { return minHeight; } }
    public float MaxHeight { get { return maxHeight; } }
}

[CreateAssetMenu(menuName = "Tomato/CloudConfigure")]
public class CloudConfigure : ScriptableObject
{
    [SerializeField]
    CloudData[] clouds = null;

    [SerializeField]
    float minSpeed = 1f;

    [SerializeField]
    float maxSpeed = 5f;

    [SerializeField]
    int count = 30;

    [SerializeField]
    float maxPosX = 13f;
    
    public CloudData[] Clouds { get { return clouds; } }
    public float MinSpeed { get { return minSpeed; } }
    public float MaxSpeed { get { return maxSpeed; } }
    public int Count { get { return count; } }
    public float MaxPosX { get { return maxPosX; } }
}
