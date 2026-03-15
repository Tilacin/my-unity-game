using KinematicCharacterController.Examples;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    [SerializeField] private float _restartDelay = 3f;
    [SerializeField] private PlayerCharacter _player;
    [SerializeField] private ExampleCharacterCamera _gameCamera;
    [SerializeField] private float _mouseSensetivity = 2f;
    private MultiplayerManager _multiplayerManager;
    private bool _hold = false;
    private bool _hideCursor;
    private void Start()
    {
        _multiplayerManager = MultiplayerManager.Instance;
        _hideCursor = true;
        Cursor.lockState = CursorLockMode.Locked;

        // Находим камеру, если не назначена
        if (_gameCamera == null)
            _gameCamera = FindObjectOfType<ExampleCharacterCamera>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _hideCursor = !_hideCursor;
            Cursor.lockState = _hideCursor ? CursorLockMode.Locked : CursorLockMode.None;

            // Блокируем/разблокируем камеру у игрока
            if (_player != null)
                _player.LockCamera(!_hideCursor);
        }

        if (_hold) return;

        // Движение (WASD)
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // Прыжок
        bool space = Input.GetKeyDown(KeyCode.Space);
        if (space) _player.Jump();

        // Передаем только движение в PlayerCharacter (поворот камеры теперь в PlayerCharacter.Update)
        _player.SetInput(h, v, 0);
        SendMove();
    }
    /*
    private void SendShoot(ref ShootInfo shootInfo)
    {
        shootInfo.key = _multiplayerManager.GetSessionID();
        string json = JsonUtility.ToJson(shootInfo);
        _multiplayerManager.SendMessage("shoot", json);
    }
    */
    private void SendMove()
    {
        _player.GetMoveInfo(out Vector3 position, out Vector3 velocity, out float rotateX, out float rotateY);
        Dictionary<string, object> data = new Dictionary<string, object>()
        {
            {"pX", position.x},
            {"pY", position.y},
            {"pZ", position.z},
            {"vX", velocity.x},
            {"vY", velocity.y},
            {"vZ", velocity.z},
            {"rX", rotateX},
            {"rY", rotateY}
        };
        _multiplayerManager.SendMessage("move", data);
    }

    public void Restart(int spawnIndex)
    {
        _multiplayerManager._spawnPoints.GetPoint(spawnIndex, out Vector3 position, out Vector3 rotation);
        StartCoroutine(Hold());
        _player.transform.position = position;
        rotation.x = 0;
        rotation.z = 0;
        _player.transform.eulerAngles = rotation;
        _player.SetInput(0, 0, 0);

        Dictionary<string, object> data = new Dictionary<string, object>()
        {
            {"pX", position.x},
            {"pY", position.y},
            {"pZ", position.z},
            {"vX", 0},
            {"vY", 0},
            {"vZ", 0},
            {"rX", 0},
            {"rY", rotation.y}
        };
        _multiplayerManager.SendMessage("move", data);
    }

    private IEnumerator Hold()
    {
        _hold = true;
        yield return new WaitForSecondsRealtime(_restartDelay);
        _hold = false;
    }
}

// Структуры пока оставим, вдруг пригодятся для ударов
[System.Serializable]
public struct ShootInfo
{
    public string key;
    public float pX;
    public float pY;
    public float pZ;
    public float dX;
    public float dY;
    public float dZ;
}

[Serializable]
public struct RestartInfo
{
    public float x;
    public float z;
}

