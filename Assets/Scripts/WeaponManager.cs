using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private GameObject[] weapons;
    [SerializeField] private Transform[] bulletSpawnPoints;

    private int currentWeaponIndex = 0;
    private PlayerGun playerGun;

    private void Start()
    {
        if (weapons.Length != bulletSpawnPoints.Length)
        {
            Debug.LogError(" оличество оружий и точек спавна пуль не совпадает!");
        }

        playerGun = GetComponent<PlayerGun>();
        if (playerGun == null)
        {
           
            playerGun = GetComponentInChildren<PlayerGun>(true);
        }

        if (playerGun == null)
        {
            Debug.LogError("PlayerGun не найден на игроке или его дет€х!");
            return;
        }

   
        SwitchWeapon(0);
    }

    private void Update()
    {
        HandleWeaponInput();
    }

    private void HandleWeaponInput()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            NextWeapon();
        }
     
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            PreviousWeapon();
        }

    
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchWeapon(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchWeapon(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchWeapon(2);
        }
    }

    public void SwitchWeapon(int newIndex)
    {
      
        if (newIndex < 0 || newIndex >= weapons.Length) return;

     
        if (newIndex == currentWeaponIndex) return;

     
        weapons[currentWeaponIndex].SetActive(false);

     
        currentWeaponIndex = newIndex;
        weapons[currentWeaponIndex].SetActive(true);

    
        UpdateBulletSpawnPoint();


        SendWeaponSwitchToServer(newIndex);

        Debug.Log($"ѕереключено на оружие {newIndex}");
    }

    private void NextWeapon()
    {
        int nextIndex = currentWeaponIndex + 1;
        if (nextIndex >= weapons.Length) nextIndex = 0;
        SwitchWeapon(nextIndex);
    }

    private void PreviousWeapon()
    {
        int prevIndex = currentWeaponIndex - 1;
        if (prevIndex < 0) prevIndex = weapons.Length - 1;
        SwitchWeapon(prevIndex);
    }

    private void UpdateBulletSpawnPoint()
    {
        if (playerGun != null && bulletSpawnPoints[currentWeaponIndex] != null)
        {
     
            playerGun.UpdateBulletPoint(bulletSpawnPoints[currentWeaponIndex]);
        }
    }

    private void SendWeaponSwitchToServer(int weaponIndex)
    {
        if (MultiplayerManager.Instance != null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>()
        {
            { "weaponIndex", weaponIndex }
        };
            MultiplayerManager.Instance.SendMessage("weaponSwitch", data);
        }
    }
    public Transform GetCurrentBulletSpawnPoint()
    {
        return bulletSpawnPoints[currentWeaponIndex];
    }

    // ћетод дл€ получени€ индекса текущего оружи€
    public int GetCurrentWeaponIndex()
    {
        return currentWeaponIndex;
    }
}