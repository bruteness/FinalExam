using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : MonoBehaviour
{
    [Header("Spin")]
    [SerializeField] private float _spinSpeed;

    [Header("Bounce")]
    [SerializeField] private float _bounceSpeed;
    [SerializeField] private float _bounceDistance;

    [Header("BatteryStat")]
    [SerializeField] private float _amountToGivePlayer;

    private Vector3 _startingPosition;
    private bool _goingUp;

    void Start(){
        _startingPosition = transform.position;
    }

    // Update is called once per frame
    void Update() {
        Bounce();
        Spin();
    }

    private void Spin(){
        transform.Rotate(0, 1 * Time.deltaTime * _spinSpeed, 0, Space.World);
    }

    private void Bounce() {
        if (_goingUp) {
            if (transform.position.y <= _startingPosition.y + _bounceDistance)
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(_startingPosition.x, _startingPosition.y + _bounceDistance + .1f, _startingPosition.z), Time.deltaTime * _bounceSpeed);
            else
                _goingUp = false;
        } else {
            if (transform.position.y >= _startingPosition.y - _bounceDistance)
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(_startingPosition.x, _startingPosition.y - _bounceDistance - .1f, _startingPosition.z), Time.deltaTime * _bounceSpeed);
            else
                _goingUp = true;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player"){
            PlayerController.instance.AddBattery(_amountToGivePlayer);
            Destroy(gameObject);
        }
    }
}
