using NUnit.Framework;
using Game.Runtime.ValueObject;

public class WeaponDataValueTests
{
    [Test]
    public void WeaponDataValue_DefaultConstructor_ValidState()
    {
        var weapon = new WeaponDataValue();

        Assert.AreEqual("", weapon.WeaponId);
        Assert.AreEqual(WeaponCategory.MainCannon, weapon.WeaponCategory);
        Assert.AreEqual(WeaponType.MainCannon, weapon.WeaponType);
        Assert.AreEqual(DamageType.PHYSICAL, weapon.DamageType);
        Assert.AreEqual(WeaponRarity.COMMON, weapon.Rarity);
        Assert.AreEqual(10f, weapon.Damage);
        Assert.AreEqual(1f, weapon.AttackSpeed);
        Assert.AreEqual(5f, weapon.Range);
        Assert.AreEqual(1, weapon.Level);
    }

    [Test]
    public void WeaponDataValue_ParameterizedConstructor_PropertiesSet()
    {
        var weapon = new WeaponDataValue("test_gun", "测试枪", WeaponCategory.MachineGun, WeaponType.Gatling,
            15f, 3f, 8f, DamageType.PHYSICAL, WeaponRarity.EPIC);

        Assert.AreEqual("test_gun", weapon.WeaponId);
        Assert.AreEqual("测试枪", weapon.WeaponName);
        Assert.AreEqual(WeaponCategory.MachineGun, weapon.WeaponCategory);
        Assert.AreEqual(WeaponType.Gatling, weapon.WeaponType);
        Assert.AreEqual(DamageType.PHYSICAL, weapon.DamageType);
        Assert.AreEqual(WeaponRarity.EPIC, weapon.Rarity);
        Assert.AreEqual(15f, weapon.Damage);
        Assert.AreEqual(3f, weapon.AttackSpeed);
        Assert.AreEqual(8f, weapon.Range);
        Assert.AreEqual(1, weapon.Level);
    }

    [Test]
    public void WeaponDataValue_CanAttack_CooldownLogic()
    {
        var weapon = new WeaponDataValue("test", "test", WeaponCategory.MainCannon, WeaponType.Cannon,
            10f, 2f, 10f); // attackSpeed = 2 → cooldown = 0.5s

        // Should be able to attack initially
        Assert.IsTrue(weapon.CanAttack());

        // Execute attack
        weapon.ExecuteAttack();
        Assert.IsFalse(weapon.CanAttack()); // Just attacked, can't immediately
    }

    [Test]
    public void WeaponDataValue_Damage_Clamped_NonNegative()
    {
        var weapon = new WeaponDataValue();
        weapon.Damage = -10f;
        Assert.AreEqual(0f, weapon.Damage); // Mathf.Max(0, value)
    }

    [Test]
    public void WeaponDataValue_AttackSpeed_Clamped_Minimum()
    {
        var weapon = new WeaponDataValue();
        weapon.AttackSpeed = 0f;
        Assert.AreEqual(0.1f, weapon.AttackSpeed); // Mathf.Max(0.1f, value)
    }

    [Test]
    public void WeaponDataValue_Upgrade_IncreasesDamageAndSpeed()
    {
        var weapon = new WeaponDataValue("test", "test", WeaponCategory.MainCannon, WeaponType.Cannon,
            100f, 1f, 10f);
        weapon.UpgradeDamagePerLevel = 0.2f; // 20% per level

        bool result = weapon.Upgrade();

        Assert.IsTrue(result);
        Assert.AreEqual(2, weapon.Level);
        Assert.AreEqual(120f, weapon.Damage, 0.01f); // 100 * 1.2
        Assert.AreEqual(1.05f, weapon.AttackSpeed, 0.01f); // 1 * 1.05
    }

    [Test]
    public void WeaponDataValue_Upgrade_MaxLevelCaps()
    {
        var weapon = new WeaponDataValue("test", "test", WeaponCategory.MainCannon, WeaponType.Cannon,
            10f, 1f, 10f);
        weapon.MaxLevel = 1; // Already at max

        bool result = weapon.Upgrade();

        Assert.IsFalse(result);
        Assert.AreEqual(1, weapon.Level);
    }

    [Test]
    public void WeaponDataValue_ProjectileProperties_Default()
    {
        var weapon = new WeaponDataValue();

        Assert.AreEqual(10f, weapon.ProjectileSpeed);
        Assert.AreEqual(1, weapon.ProjectileCount);
        Assert.AreEqual(0f, weapon.Knockback);
    }

    [Test]
    public void WeaponDataValue_ProjectileCount_MinimumOne()
    {
        var weapon = new WeaponDataValue();
        weapon.ProjectileCount = 0;
        Assert.AreEqual(1, weapon.ProjectileCount);
    }
}
