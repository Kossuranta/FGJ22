using System.Collections.Generic;
using UnityEngine;

public class CloudManager : MonoBehaviour
{
    [SerializeField]
    CloudConfigure cloudConfigure = null;

    List<(Transform cloud, CloudData data, float speed)> clouds = null;

    void Awake()
    {
        if (cloudConfigure == null)
        {
            Debug.LogError("cloudConfigure is null!", this);
            enabled = false;
        }
        
        if (cloudConfigure.Clouds.Length == 0)
        {
            Debug.LogError("CloudConfigure has no clouds added!", this);
            enabled = false;
        }
    }

    void Start()
    {
        InstantiateClouds();
    }

    void InstantiateClouds()
    {
        clouds = new List<(Transform cloud, CloudData data, float speed)>(cloudConfigure.Count);
        for (int i = 0; i < cloudConfigure.Count; i++)
        {
            int rng = Random.Range(0, cloudConfigure.Clouds.Length - 1);
            CloudData data = cloudConfigure.Clouds[rng];

            Transform cloud = Instantiate(data.Prefab, transform, false);
            Vector2 pos = Vector2.zero;
            pos.x = Random.Range(-30f, 30f);
            pos.y = Random.Range(data.MinHeight, data.MaxHeight);
            cloud.transform.localPosition = pos;

            float speed = Random.Range(cloudConfigure.MinSpeed, cloudConfigure.MaxSpeed);
            clouds.Add((cloud, data, speed));
        }
    }

    void Update()
    {
        foreach ((Transform cloud, CloudData data, float speed) in clouds)
        {
            Vector2 pos = cloud.localPosition;
            pos.x += Time.deltaTime * speed;

            if (pos.x > cloudConfigure.MaxPosX)
            {
                pos.x = -cloudConfigure.MaxPosX;
                pos.y = Random.Range(data.MinHeight, data.MaxHeight);
            }

            cloud.localPosition = pos;
        }
    }
}
