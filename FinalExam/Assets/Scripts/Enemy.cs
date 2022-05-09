using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour {
    [SerializeField] private float _health;
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private BoxCollider _collider;
    [SerializeField] private float _slowedSpeed;

    private GameObject _playerLightDamageHitbox;
    private AudioSource _audioSource;
    private Animator _animator;
    private NavMeshAgent _navMeshAgent;
    private float _startingSpeed;
    private float _timeToGrowl = 3;
    private float _resetTimer;
    private bool _isSlowed;
    private bool _isDead = false;

    // Start is called before the first frame update
    void Start() {
        _audioSource = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _collider = GetComponent<BoxCollider>();
        _startingSpeed = _navMeshAgent.speed;
        _playerLightDamageHitbox = GameObject.Find("PlayerDamageHitbox");
        if(_playerLightDamageHitbox == null){
            _playerLightDamageHitbox = FindInActiveObjectByName("PlayerDamageHitbox");
        }

        _resetTimer = _timeToGrowl;
    }

    // Update is called once per frame
    void Update() {
        ControlEnemyDestination();
        DecreaseEnemyStats();
        Growl();

        CheckForPlayerLight();
    }

    private void CheckForPlayerLight() {
        if (!_playerLightDamageHitbox.activeInHierarchy) {
            HitWithFlashlight(false);
        }
    }

    private GameObject FindInActiveObjectByName(string name) {
        Transform[] objs = Resources.FindObjectsOfTypeAll<Transform>() as Transform[];
        for (int i = 0; i < objs.Length; i++) {
            if (objs[i].hideFlags == HideFlags.None) {
                if (objs[i].name == name) {
                    return objs[i].gameObject;
                }
            }
        }
        return null;
    }

    private void Growl() {
        if (_timeToGrowl <= 0 && !_audioSource.isPlaying) {
            if (_isDead || GameManager.instance.didGameWin || PlayerController.instance.IsDead) return;
            AudioSource.PlayClipAtPoint(_audioSource.clip, transform.position);
            _timeToGrowl = _resetTimer;
        } else {
            _timeToGrowl -= Time.deltaTime;
            _timeToGrowl += Random.Range(0, 1);
        }
    }

    private void ControlEnemyDestination() {
        if (_health > 0 && !PlayerController.instance.IsDead && !GameManager.instance.didGameWin)
            _navMeshAgent.SetDestination(FindObjectOfType<PlayerController>().transform.position); // Set enemy target
        else {
            _navMeshAgent.ResetPath();
            _animator.SetTrigger("Died");
            StartCoroutine(WaitToDestroy());
        }
    }

    private void DecreaseEnemyStats() {
        if (_isSlowed && !PlayerController.instance.IsDead) {
            _navMeshAgent.speed = _slowedSpeed;
            _health -= PlayerController.instance.LightDamage * Time.deltaTime;
            _health = _health < 0 ? 0 : _health;
            _healthSlider.value = _health / 100;
            _healthSlider.gameObject.SetActive(true);
        } else {
            _navMeshAgent.speed = _startingSpeed;
            _healthSlider.gameObject.SetActive(false);
        }
    }

    public void HitWithFlashlight(bool slowed) {
        _isSlowed = slowed;
    }

    private IEnumerator WaitToDestroy() {
        _isDead = true;
        Destroy(_collider);
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            PlayerController.instance.IsDead = true;
        }
    }
}
