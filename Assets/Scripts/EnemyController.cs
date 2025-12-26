using Colyseus.Schema;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private EnemyCharacter _cherecter;
    [SerializeField] private EnemyGun _gun;
    private List<float> _receiveTimeInterval = new List<float> { 0,0,0,0,0};
    private string _sessionId;
    [SerializeField] private GameObject[] enemyWeapons; // 3 оружия врага
    private int currentWeaponIndex = 0;
   
    private float AverageInterval
    {
        get
        {
            int receiveTimeIntervalCount = _receiveTimeInterval.Count;
            float sum = 0;
            for (int i = 0; i < receiveTimeIntervalCount; i++)
            {
                sum += _receiveTimeInterval[i];
            }
            return sum/receiveTimeIntervalCount;
        }
    }
    private float _LastReceiveTime = 0f;
    private Player _player;

    public void Init(string key, Player player)
    {
        _sessionId = key;
        _cherecter.Init(key);

      _player = player;
        _cherecter.SetSpeed(player.speed);
        _cherecter.SetMaxHP(player.maxHP);
        player.OnChange += OnChange;

        SetWeapon(player.weaponIndex);
    }

    public void Shoot(in ShootInfo info)
    {
       Vector3 position = new Vector3(info.pX, info.pY, info.pZ);
       Vector3 velocity =new Vector3(info.dX, info.dY, info.dZ);
        _gun.Shoot(position, velocity);
    }
    public void Destroy()
    {
        _player.OnChange -= OnChange;
       Destroy(gameObject); 
    }
    private void SaveReceiveTime()
    {
        float interval = Time.time - _LastReceiveTime;
        _LastReceiveTime = Time.time;

        _receiveTimeInterval.Add(interval);
        _receiveTimeInterval.RemoveAt(0);
    }
    internal void OnChange(List<DataChange> changes)
    {
        SaveReceiveTime();

        Vector3 position = transform.position;
        Vector3 velocity = _cherecter.velocity;

        foreach (var dataChange in changes)
        {
          switch (dataChange.Field)
            {
                case "loss":
                    MultiplayerManager.Instance._lossCounter.SetEnemyLoss((byte)dataChange.Value);
                    break;
                case "currentHP":
                    if ((sbyte)dataChange.Value > (sbyte)dataChange.PreviousValue)
                        _cherecter.RestoreHP((sbyte)dataChange.Value);
                    break;
                case "pX":
                    position.x = (float)dataChange.Value;
                    break;
                case "pY":
                    position.y = (float)dataChange.Value;
                    break;
                case "pZ":
                    position.z = (float)dataChange.Value;
                    break;
              
                case "vX":
                    velocity.x = (float)dataChange.Value;
                    break;
                case "vY":
                    velocity.y = (float)dataChange.Value;
                    break;
                case "vZ":
                    velocity.z = (float)dataChange.Value;
                    break;
                case "rX":
                    _cherecter.SetRotateX((float)dataChange.Value);
                    break;
                case "rY":
                    _cherecter.SetRotateY((float)dataChange.Value);
                    break;
                default: Debug.LogWarning("Не обрабатывается изменение поля" + dataChange.Field);
                    break;
            }
        }
        _cherecter.SetMovement(position, velocity, AverageInterval);
    }
    public void SetWeapon(int weaponIndex)
    {
        // Проверяем валидность индекса
        if (weaponIndex < 0 || weaponIndex >= enemyWeapons.Length) return;
        if (weaponIndex == currentWeaponIndex) return;

        // Деактивируем текущее
        if (currentWeaponIndex >= 0 && currentWeaponIndex < enemyWeapons.Length)
            enemyWeapons[currentWeaponIndex].SetActive(false);

        // Активируем новое
        currentWeaponIndex = weaponIndex;
        enemyWeapons[currentWeaponIndex].SetActive(true);

        Debug.Log($"Враг {_sessionId} сменил оружие на {weaponIndex}");
    }
}
