using UnityEngine;
using System;
public class SaveManager : MonoBehaviour
{
    SaveData _currentSaveData;
    LevelData _currentLevelData;
    [SerializeField] GameManager _gameManager;
    [SerializeField] Character _character;
    [SerializeField] Transform[] _checkPoints;
    [SerializeField] Puzzle[] _puzzles;
    [SerializeField] Enemy[] _enemies;

    public static SaveManager instance;
    void Awake()
    {
        Load();
        instance = this;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F10))
        DeleteCurrentLevelInfo();
    }
    public void Load()
    {
        _currentSaveData = SaveSystem.Load();
        _currentLevelData = _currentSaveData.levelsData[_gameManager.currentLevelIndex];
        if(_currentLevelData == null)
        {
            _currentLevelData = new LevelData();
        }
        if (_currentLevelData.saveState == SaveState.NotFound)
        {
            Save(_checkPoints[0]);
            _currentLevelData.saveState = SaveState.Saved;

        }
       

        for (int i = 0; i < _puzzles.Length; i++)
        {
            if (_currentLevelData.puzzlesCompleted[i])
            {
                _puzzles[i].AutoCompletePuzzle();

            }
        }

        for (int i = 0; i < _enemies.Length; i++)
        {
            if (_currentLevelData.killedEnemies[i])
            {
                _enemies[i].gameObject.SetActive(false);

            }
        }

        for (int i = 0; i < _checkPoints.Length; i++)
        {
            if(i <= _currentLevelData.currentCheckPoint)
            {
                _checkPoints[i].gameObject.SetActive(false);
            }
        }

        _character.transform.position = _checkPoints[_currentLevelData.currentCheckPoint].position;
    }
    public void Save(Transform checkPoint)
    {
        _currentLevelData.puzzlesCompleted = new bool[_puzzles.Length];
        for (int i = 0; i < _puzzles.Length; i++)
        {
            if (_puzzles[i].completed)
            {
                _currentLevelData.puzzlesCompleted[i] = true;
            }
        }
        _currentLevelData.killedEnemies = new bool[_enemies.Length];
        for (int i = 0; i < _enemies.Length; i++)
        {
            if (!_enemies[i].gameObject.activeSelf)
            {
                _currentLevelData.killedEnemies[i] = true;
            }
        }
        _currentLevelData.currentCheckPoint = Array.IndexOf(_checkPoints, checkPoint);

        _currentSaveData.levelsData[_gameManager.currentLevelIndex] = _currentLevelData;

        SaveSystem.Save(_currentSaveData);
    }


    public void DeleteCurrentLevelInfo()
    {
        _currentLevelData.saveState = SaveState.NotFound;
        _currentSaveData.levelsData[_gameManager.currentLevelIndex] = _currentLevelData;
        SaveSystem.Save(_currentSaveData);
    }
}
