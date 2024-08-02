using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRoadManager : MonoBehaviour
{
    public GameObject _roadPrefab;
    public float _roadLength;
    public float _roadCounter;
    void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            SpawnRoad();
        }
    }

    public void SpawnRoad()
    {
        GameObject road = Instantiate(_roadPrefab);
        road.transform.position = new Vector3(0, 0, _roadLength * _roadCounter);
        _roadCounter++;
    }

}
