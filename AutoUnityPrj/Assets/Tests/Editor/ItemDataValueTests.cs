using NUnit.Framework;
using Game.Runtime.ValueObject;

public class ItemDataValueTests
{
    private TankDataValue CreateTestTank()
    {
        return new TankDataValue
        {
            MaxHealth = 100,
            PercentDamage = 0f,
            AttackSpeed = 5f,
            MoveSpeed = 3f,
            CritRate = 5f,
            Armor = 0,
            Luck = 0,
            Harvest = 1f
        };
    }

    [Test]
    public void ItemDataValue_DefaultConstructor_ValidState()
    {
        var item = new ItemDataValue();

        Assert.AreEqual("", item.ItemId);
        Assert.AreEqual("", item.ItemName);
        Assert.AreEqual(ItemType.Consumable, item.ItemType);
        Assert.AreEqual(100, item.Price);
    }

    [Test]
    public void ItemDataValue_ParameterizedConstructor_PropertiesSet()
    {
        var item = new ItemDataValue("heart", "生命之心", ItemType.Passive, 50);

        Assert.AreEqual("heart", item.ItemId);
        Assert.AreEqual("生命之心", item.ItemName);
        Assert.AreEqual(ItemType.Passive, item.ItemType);
        Assert.AreEqual(50, item.Price);
    }

    [Test]
    public void ItemDataValue_Price_Clamped_NonNegative()
    {
        var item = new ItemDataValue();
        item.Price = -10;
        Assert.AreEqual(0, item.Price);
    }

    [Test]
    public void ItemDataValue_ApplyToTank_BonusesApplied()
    {
        var tank = CreateTestTank();
        var item = new ItemDataValue("test", "测试", ItemType.Passive, 100)
        {
            MaxHealthBonus = 20,
            DamageBonus = 10f,
            AttackSpeedBonus = 5f,
            ArmorBonus = 3,
            LuckBonus = 5,
            HarvestBonus = 0.1f
        };

        item.ApplyToTank(tank);

        Assert.AreEqual(120, tank.MaxHealth);
        Assert.AreEqual(10f, tank.PercentDamage);
        Assert.AreEqual(10f, tank.AttackSpeed); // 5 + 5
        Assert.AreEqual(3, tank.Armor);
        Assert.AreEqual(5f, tank.Luck);
        Assert.AreEqual(1.1f, tank.Harvest);
    }

    [Test]
    public void ItemDataValue_RemoveFromTank_BonusesRemoved()
    {
        var tank = CreateTestTank();
        var item = new ItemDataValue("test", "测试", ItemType.Passive, 100)
        {
            MaxHealthBonus = 20,
            DamageBonus = 10f
        };

        item.ApplyToTank(tank);
        item.RemoveFromTank(tank);

        Assert.AreEqual(100, tank.MaxHealth);
        Assert.AreEqual(0f, tank.PercentDamage);
    }

    [Test]
    public void ItemDataValue_Stacking_LimitsRespected()
    {
        var item = new ItemDataValue("test", "测试", ItemType.Passive, 100)
        {
            StackCount = 1,
            MaxStack = 5,
            CanStack = true
        };

        item.StackCount = 10; // Try to exceed max stack
        Assert.AreEqual(5, item.StackCount); // Clamped

        item.StackCount = 0; // Try below minimum
        Assert.AreEqual(1, item.StackCount); // Clamped
    }

    [Test]
    public void ItemDataValue_NonStackable_AlwaysCountOne()
    {
        var item = new ItemDataValue("unique", "唯一", ItemType.Passive, 200);
        item.CanStack = false;

        item.StackCount = 5;
        Assert.AreEqual(1, item.StackCount); // Non-stackable always returns 1
    }
}
