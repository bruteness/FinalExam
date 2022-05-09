using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] _car;
    [SerializeField] private float _waitToSpawnTimer;
    private float _minSpawnTimer = 0;
    private float _maxSpawnTimer = 1000;
    private bool _canSpawn = true;

    // Update is called once per frame
    void Update()
    {
        if(_canSpawn && Random.Range(_minSpawnTimer, _maxSpawnTimer) >= _maxSpawnTimer - .2){
            Instantiate(_car[Random.Range(0, _car.Length)], transform.position, transform.rotation);
            StartCoroutine(WaitToSpawnCar());
        }
    }

    private IEnumerator WaitToSpawnCar(){
        _canSpawn = false;
        yield return new WaitForSeconds(_waitToSpawnTimer);
        _canSpawn = true;
    }
}
