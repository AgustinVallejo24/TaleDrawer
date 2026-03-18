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

    public void Load()
    {
        _currentSaveData = SaveSystem.Load();
        _currentLevelData = _currentSaveData.levelsData[_gameManager.currentLevelIndex];

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

        _character.transform.position = _checkPoints[_currentLevelData.currentCheckPoint].position;
    }
    public void Save(Checkpoint checkPoint)
    {

        for (int i = 0; i < _puzzles.Length; i++)
        {
            if (_puzzles[i].completed)
            {
                _currentLevelData.puzzlesCompleted[i] = true;
            }
        }

        for (int i = 0; i < _enemies.Length; i++)
        {
            if (!_enemies[i].gameObject.activeSelf)
            {
                _currentLevelData.killedEnemies[i] = true;
            }
        }
        _currentLevelData.currentCheckPoint = Array.IndexOf(_checkPoints, checkPoint.transform);

        _currentSaveData.levelsData[_gameManager.currentLevelIndex] = _currentLevelData;

        SaveSystem.Save(_currentSaveData);
    }

}
