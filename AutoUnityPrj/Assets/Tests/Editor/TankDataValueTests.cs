using NUnit.Framework;
using Game.Runtime.ValueObject;

public class TankDataValueTests
{
    [Test]
    public void TankDataValue_DefaultValues_MatchSpec()
    {
        var tank = new TankDataValue();

        Assert.AreEqual(100, tank.MaxHealth);
        Assert.AreEqual(0.5f, tank.HealthRegen);
        Assert.AreEqual(0f, tank.Lifesteal);
        Assert.AreEqual(5f, tank.AttackSpeed);
        Assert.AreEqual(5f, tank.CritRate);
        Assert.AreEqual(0, tank.Armor);
        Assert.AreEqual(0f, tank.Dodge);
        Assert.AreEqual(3f, tank.MoveSpeed);
        Assert.AreEqual(5f, tank.Range);
        Assert.AreEqual(0.85f, tank.AimAccuracy);
        Assert.AreEqual(0f, tank.Luck);
        Assert.AreEqual(1f, tank.Harvest);
    }

    [Test]
    public void TankDataValue_Clamping_HpNeverNegative()
    {
        var tank = new TankDataValue();
        tank.MaxHealth = -50;
        Assert.AreEqual(0, tank.MaxHealth); // Mathf.Max(0, value)
    }

    [Test]
    public void TankDataValue_Clamping_CritRateBounds()
    {
        var tank = new TankDataValue();
        tank.CritRate = 150f;
        Assert.AreEqual(100f, tank.CritRate); // Clamp(0,100)

        tank.CritRate = -10f;
        Assert.AreEqual(0f, tank.CritRate);
    }

    [Test]
    public void TankDataValue_Clamping_MoveSpeedMin()
    {
        var tank = new TankDataValue();
        tank.MoveSpeed = 0f;
        Assert.AreEqual(0.1f, tank.MoveSpeed); // Mathf.Max(0.1f, value)
    }

    [Test]
    public void TankDataValue_Clamping_AimAccuracy()
    {
        var tank = new TankDataValue();
        tank.AimAccuracy = 2f;
        Assert.AreEqual(1f, tank.AimAccuracy);

        tank.AimAccuracy = -1f;
        Assert.AreEqual(0f, tank.AimAccuracy);
    }

    [Test]
    public void TankDataValue_LoadFromSave_RestoresCorrectly()
    {
        var tank = new TankDataValue();
        tank.MaxHealth = 150;
        tank.MoveSpeed = 4f;
        tank.Armor = 10;

        var saveData = tank.ExportToSave();
        var restored = new TankDataValue();
        restored.LoadFromSave(saveData);

        Assert.AreEqual(150, restored.MaxHealth);
        Assert.AreEqual(4f, restored.MoveSpeed);
        Assert.AreEqual(10, restored.Armor);
    }

    [Test]
    public void TankDataValue_ExportToSave_RoundTrip_DataPreserved()
    {
        var tank = new TankDataValue
        {
            MaxHealth = 200,
            HealthRegen = 1.5f,
            Lifesteal = 10f,
            PercentDamage = 20f,
            RangedDamage = 15f,
            MeleeDamage = 5f,
            ElementDamage = 8f,
            Engineering = 12f,
            AttackSpeed = 3f,
            CritRate = 25f,
            Range = 8f,
            AimAccuracy = 0.9f,
            Armor = 5,
            Dodge = 15f,
            MoveSpeed = 4f,
            Luck = 10f,
            Harvest = 2f,
            CurrentHealth = 180
        };

        var saveData = tank.ExportToSave();
        var restored = new TankDataValue();
        restored.LoadFromSave(saveData);

        Assert.AreEqual(tank.MaxHealth, restored.MaxHealth);
        Assert.AreEqual(tank.HealthRegen, restored.HealthRegen);
        Assert.AreEqual(tank.Lifesteal, restored.Lifesteal);
        Assert.AreEqual(tank.PercentDamage, restored.PercentDamage);
        Assert.AreEqual(tank.RangedDamage, restored.RangedDamage);
        Assert.AreEqual(tank.MeleeDamage, restored.MeleeDamage);
        Assert.AreEqual(tank.ElementDamage, restored.ElementDamage);
        Assert.AreEqual(tank.Engineering, restored.Engineering);
        Assert.AreEqual(tank.AttackSpeed, restored.AttackSpeed);
        Assert.AreEqual(tank.CritRate, restored.CritRate);
        Assert.AreEqual(tank.Range, restored.Range);
        Assert.AreEqual(tank.AimAccuracy, restored.AimAccuracy);
        Assert.AreEqual(tank.Armor, restored.Armor);
        Assert.AreEqual(tank.Dodge, restored.Dodge);
        Assert.AreEqual(tank.MoveSpeed, restored.MoveSpeed);
        Assert.AreEqual(tank.Luck, restored.Luck);
        Assert.AreEqual(tank.Harvest, restored.Harvest);
        Assert.AreEqual(tank.CurrentHealth, restored.CurrentHealth);
    }
}
