using NUnit.Framework;
using Game.Runtime.ValueObject;

public class GameSettingsValueTests
{
    [Test]
    public void GameSettingsValue_DefaultValues_Reasonable()
    {
        var settings = new GameSettingsValue();

        Assert.AreEqual(0.8f, settings.MasterVolume);
        Assert.AreEqual(0.7f, settings.MusicVolume);
        Assert.AreEqual(1.0f, settings.SfxVolume);
        Assert.IsFalse(settings.MuteAll);
        Assert.AreEqual(2, settings.QualityLevel);
        Assert.IsTrue(settings.Fullscreen);
        Assert.IsTrue(settings.VSync);
        Assert.AreEqual("zh-CN", settings.Language);
        Assert.IsTrue(settings.CameraShake);
        Assert.IsTrue(settings.ShowDamageNumbers);
        Assert.AreEqual(3f, settings.AutoCollectRange);
        Assert.IsFalse(settings.InvertY);
        Assert.AreEqual(1f, settings.SensitivityX);
        Assert.AreEqual(1f, settings.SensitivityY);
    }

    [Test]
    public void GameSettingsValue_VolumeClamping_0to1()
    {
        var settings = new GameSettingsValue();

        settings.MasterVolume = 2f;
        Assert.AreEqual(1f, settings.MasterVolume);

        settings.MasterVolume = -1f;
        Assert.AreEqual(0f, settings.MasterVolume);

        settings.MusicVolume = 999f;
        Assert.AreEqual(1f, settings.MusicVolume);

        settings.SfxVolume = 0.5f;
        Assert.AreEqual(0.5f, settings.SfxVolume);
    }

    [Test]
    public void GameSettingsValue_SensitivityClamping()
    {
        var settings = new GameSettingsValue();

        settings.SensitivityX = 10f;
        Assert.AreEqual(5f, settings.SensitivityX);

        settings.SensitivityY = 0f;
        Assert.AreEqual(0.1f, settings.SensitivityY);
    }

    [Test]
    public void GameSettingsValue_QualityLevel_Clamped()
    {
        var settings = new GameSettingsValue();
        settings.QualityLevel = 10;
        Assert.AreEqual(6, settings.QualityLevel); // Max 6 quality levels

        settings.QualityLevel = -1;
        Assert.AreEqual(0, settings.QualityLevel);
    }

    [Test]
    public void GameSettingsValue_ResetToDefault_RestoresDefaults()
    {
        var settings = new GameSettingsValue();
        settings.MasterVolume = 0.3f;
        settings.Fullscreen = false;
        settings.Language = "en-US";

        settings.ResetToDefault();

        Assert.AreEqual(0.8f, settings.MasterVolume);
        Assert.IsTrue(settings.Fullscreen);
        Assert.AreEqual("zh-CN", settings.Language);
    }

    [Test]
    public void GameSettingsValue_AutoCollectRange_Clamped()
    {
        var settings = new GameSettingsValue();
        settings.AutoCollectRange = -1f;
        Assert.AreEqual(0f, settings.AutoCollectRange);

        settings.AutoCollectRange = 20f;
        Assert.AreEqual(10f, settings.AutoCollectRange);
    }

    [Test]
    public void GameSettingsValue_Language_EmptyFallback()
    {
        var settings = new GameSettingsValue();
        settings.Language = "";
        Assert.AreEqual("zh-CN", settings.Language);

        settings.Language = null;
        Assert.AreEqual("zh-CN", settings.Language);
    }
}
