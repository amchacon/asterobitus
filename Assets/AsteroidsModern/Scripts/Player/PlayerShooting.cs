using AsteroidsModern.Core;
using AsteroidsModern.Enums;
using AsteroidsModern.Interfaces;
using AsteroidsModern.Weapons;
using UnityEngine;

namespace AsteroidsModern.Player
{
    public class PlayerShooting : MonoBehaviour
    {
        [SerializeField] private Transform firePoint;
        [SerializeField] private WeaponType currentWeaponType = WeaponType.Standard;
        [SerializeField] private ParticleSystem muzzleFlash; 
            
        private WeaponBase _currentWeapon;
        private WeaponFactory _weaponFactory;
        private IProjectileFactory _projectileFactory;

        private bool CanShoot => _currentWeapon?.CanFire ?? false;
        
        private void Start()
        {
            InitializeDependencies();
        }

        private void Update()
        {
            UpdateWeaponCooldown();
            HandleInput(); 
        }

        private void InitializeDependencies()
        {
            _projectileFactory = GetComponent<IProjectileFactory>();
            _weaponFactory = new WeaponFactory(_projectileFactory);
            ChangeWeapon(currentWeaponType);
        }
        
        private void UpdateWeaponCooldown()
        {
            _currentWeapon.UpdateCooldown(Time.deltaTime);
        }

        private void HandleInput()
        {
            if (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0))
            {
                Shoot(transform.up);
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                ToggleWeapon();
            }
        }

        private void Shoot(Vector2 direction)
        {
            if (!CanShoot || _currentWeapon == null) return;
            
            _currentWeapon.Fire(firePoint.position, direction);
            muzzleFlash.Play();
        }

        private void ChangeWeapon(WeaponType weaponType)
        {
            currentWeaponType = weaponType;
            _currentWeapon = _weaponFactory.CreateWeapon(weaponType);
            GameEvents.TriggerWeaponChanged(_currentWeapon);
        }

        private void ToggleWeapon()
        {
            WeaponType nextWeapon = currentWeaponType == WeaponType.Standard ? 
                WeaponType.DoubleShot : WeaponType.Standard;
            ChangeWeapon(nextWeapon);
        }
    }
}