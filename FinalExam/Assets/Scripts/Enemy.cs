using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _health;
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private BoxCollider _collider;
    [SerializeField] private float _slowedSpeed;

    private Animator _animator;
    private NavMeshAgent _navMeshAgent;
    private float _startingSpeed;
    private bool _isSlowed;
    
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _collider = GetComponent<BoxCollider>();
        _startingSpeed = _navMeshAgent.speed;
    }

    // Update is called once per frame
    void Update()
    {
        ControlEnemyDestination();
        DecreaseEnemyStats();
    }

    private void ControlEnemyDestination()
    {
        if (_health > 0)
            _navMeshAgent.SetDestination(FindObjectOfType<PlayerController>().transform.position); // Set enemy target
        else{
            _navMeshAgent.ResetPath();
            _animator.SetTrigger("Died");
            StartCoroutine(WaitToDestroy());
        }
    }

    private void DecreaseEnemyStats()
    {
        if (_isSlowed){
            _navMeshAgent.speed = _slowedSpeed;
            _health -= PlayerController._instance.LightDamage * Time.deltaTime;
            _health = _health < 0 ? 0 : _health;
            _healthSlider.value = _health / 100;
            _healthSlider.gameObject.SetActive(true);
        }
        else{
            _navMeshAgent.speed = _startingSpeed;
            _healthSlider.gameObject.SetActive(false);
        }
    }

    public void HitWithFlashlight(bool slowed){
        _isSlowed = slowed;
    }

    private IEnumerator WaitToDestroy(){
        Destroy(_collider);
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

}
