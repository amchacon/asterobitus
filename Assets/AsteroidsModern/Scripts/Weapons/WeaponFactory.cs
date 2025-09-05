using AsteroidsModern.Core;
using AsteroidsModern.Enums;
using AsteroidsModern.Interfaces;
using AsteroidsModern.Managers;

namespace AsteroidsModern.Weapons
{
    public class WeaponFactory
    {
        private readonly IProjectileFactory _projectileFactory;
        private readonly GameSettings _settings;

        public WeaponFactory(IProjectileFactory projectileFactory)
        {
            _projectileFactory = projectileFactory ?? throw new System.ArgumentNullException(nameof(projectileFactory));
            _settings = ConfigLoader.Instance.GameSettings;
        }

        public WeaponBase CreateWeapon(WeaponType weaponType)
        {
            return weaponType switch
            {
                WeaponType.Standard => new StandardWeapon(_projectileFactory, _settings),
                WeaponType.DoubleShot => new DoubleShotWeapon(_projectileFactory, _settings),
                _ => new StandardWeapon(_projectileFactory, _settings)
            };
        }
    }
}