using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI _noteTextUI;
    [SerializeField] private TextMeshProUGUI _gameOverResetTextUI;
    [SerializeField] private TextMeshProUGUI _gameWinResetTextUI;
    [SerializeField] private TextMeshProUGUI _notesFoundTextUI;
    [SerializeField] private NoteScript[] _notes;

    [Header("Canvas")]
    [SerializeField] private GameObject _gameOverCanvas;
    [SerializeField] private GameObject _gameWinCanvas;
    public GameObject noteCanvas;

    [Header("Game Over")]
    [SerializeField] private Animator _gameOverAnimator;
    [SerializeField] private AudioClip _gameOverScream;
    [Range(0.0f, 1.0f)]
    [SerializeField] private float _screamVolume;

    [Header("Game Win")]
    [SerializeField] private Animator _gameWinAnimator;
    [SerializeField] private AudioClip _gameWinAudio;

    [Header("Player")]
    [SerializeField] private GameObject _playerSpawnPosition;

    [Header("Enemy")]
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject[] _enemySpawnLocations;

    public int _maxNotes;
    private string _currentText;
    private int _currentIndex = 0;
    private bool _didGameOver = false;
    public bool didGameWin = false;

    public int currentNotesFound;

    // Start is called before the first frame update
    void Start() {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;
        DontDestroyOnLoad(gameObject);

        _maxNotes = GameObject.FindGameObjectsWithTag("Note").Length;

        _currentText = _notes[_currentIndex].NoteText;
        _noteTextUI.text = _currentText;
    }

    // Update is called once per frame
    void Update() {
        // Update Notes Found
        _notesFoundTextUI.text = $"{currentNotesFound} / {_maxNotes}";
        if (currentNotesFound >= _maxNotes && !noteCanvas.activeInHierarchy)
            SetGameWin();

        // Stop showing note
        if (Input.GetKeyDown(KeyCode.E) && noteCanvas.activeInHierarchy)
            CloseNote();

        // Stop time if note is open
        if (noteCanvas.activeInHierarchy)
            Time.timeScale = 0;

        // Reset Game
        if (_didGameOver && Input.GetKeyDown(KeyCode.Space))
            ResetGame();

        if (didGameWin && Input.GetKeyDown(KeyCode.Space))
            Application.Quit();
    }

    private void ResetGame() {
        _didGameOver = false;
        PlayerController.instance.controller.enabled = false;
        PlayerController.instance.transform.SetPositionAndRotation(_playerSpawnPosition.transform.position, _playerSpawnPosition.transform.rotation);
        PlayerController.instance.controller.enabled = true;
        PlayerController.instance.IsDead = false;
        _gameOverCanvas.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    private void CloseNote() {
        Time.timeScale = 1;
        noteCanvas.SetActive(false);
        if (_currentIndex + 1 < _notes.Length) {
            _currentText = _notes[++_currentIndex].NoteText;
            _noteTextUI.text = _currentText;
            GameObject closestEnemySpawn = GetClosestEnemy(_enemySpawnLocations);
            Instantiate(_enemyPrefab, closestEnemySpawn.transform.position, closestEnemySpawn.transform.rotation);
        }
    }

    private GameObject GetClosestEnemy(GameObject[] spawnPositions) {
        GameObject shortestDistObject = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = PlayerController.instance.transform.position;
        foreach (GameObject g in spawnPositions) {
            float dist = Vector3.Distance(g.transform.position, currentPos);
            if (dist < minDist) {
                shortestDistObject = g;
                minDist = dist;
            }
        }
        return shortestDistObject;
    }

    public void SetGameOver() {
        if (_didGameOver) return;
        _didGameOver = true;

        // Reset pickups
        currentNotesFound = 0;
        _currentIndex = 0;
        _currentText = _notes[0].NoteText;
        _noteTextUI.text = _currentText;

        AudioSource.PlayClipAtPoint(_gameOverScream, PlayerController.instance.transform.position, _screamVolume);
        _gameOverCanvas.SetActive(true);
        _gameOverAnimator.SetTrigger("GameOver");
        StartCoroutine(WaitToShowResetText(_gameOverResetTextUI));
    }

    public void SetGameWin() {
        if (didGameWin) return;
        didGameWin = true;
        AudioSource.PlayClipAtPoint(_gameWinAudio, PlayerController.instance.transform.position, _screamVolume);
        _gameWinCanvas.SetActive(true);
        _gameWinAnimator.SetTrigger("GameWin");
        StartCoroutine(WaitToShowResetText(_gameWinResetTextUI));
    }

    private IEnumerator WaitToShowResetText(TextMeshProUGUI resetText) {
        yield return new WaitForSeconds(5f);
        resetText.gameObject.SetActive(true);
    }

}
