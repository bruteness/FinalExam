using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour {
    [SerializeField] private float _speed;

    private AudioSource _audioSource;
    private bool _isStopped;

    void Start() {
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {
        if (!_isStopped)
            transform.Translate(Vector3.forward * _speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player" || other.tag == "Car") {
            _isStopped = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag == "Player" || other.tag == "Car") {
            StartCoroutine(WaitToStart());
        }
    }

    // So the car doesn't gitter when player is moving away while in from of car
    private IEnumerator WaitToStart() {
        yield return new WaitForSeconds(1);
        _isStopped = false;
    }
}
