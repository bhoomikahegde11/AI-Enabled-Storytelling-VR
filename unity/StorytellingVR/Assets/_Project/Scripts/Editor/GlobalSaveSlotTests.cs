using System;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class GlobalSaveSlotTests
{
    private string directory;
    private string previousOverride;
    private int previousSlot;

    [SetUp]
    public void SetUp()
    {
        previousOverride = LocalSaveManager.EditorTestSaveDirectoryOverride;
        previousSlot = LocalSaveManager.ActiveSlot;
        directory = Path.Combine(Path.GetTempPath(), "anubhava-slots-" + Guid.NewGuid().ToString("N"));
        LocalSaveManager.EditorTestSaveDirectoryOverride = directory;
        LocalSaveManager.ClearActiveSlot();
    }

    [TearDown]
    public void TearDown()
    {
        LocalSaveManager.ClearActiveSlot();
        LocalSaveManager.EditorTestSaveDirectoryOverride = previousOverride;
        if (previousSlot != 0) LocalSaveManager.SelectSlot(previousSlot);
        if (Directory.Exists(directory)) Directory.Delete(directory, true);
    }

    [Test]
    public void ThreeSlotsHaveSeparateFilesAndStableActiveContext()
    {
        Assert.IsFalse(LocalSaveManager.SelectSlot(4));
        Assert.AreEqual(0, LocalSaveManager.ActiveSlot);
        for (int slot = 1; slot <= LocalSaveManager.SlotCount; slot++)
        {
            Assert.IsTrue(LocalSaveManager.SelectSlot(slot));
            var saves = new LocalSaveManager();
            Assert.AreEqual(LocalSaveManager.GetSlotSavePath(slot), saves.SavePath);
            Assert.IsTrue(saves.CreateFreshProfile("Journey " + slot, "Merchant " + slot));
            LocalProfileData profile = saves.LoadProfile();
            profile.global_metrics.total_varahas = 100 + slot;
            saves.SaveProfile(profile);
        }

        for (int slot = 1; slot <= LocalSaveManager.SlotCount; slot++)
        {
            Assert.IsTrue(LocalSaveManager.TryLoadSlot(slot, out LocalProfileData profile));
            Assert.AreEqual(100 + slot, profile.global_metrics.total_varahas);
            Assert.AreEqual("Journey " + slot, profile.save_name);
        }
        LocalSaveManager.ClearActiveSlot();
        Assert.IsFalse(File.Exists(LocalSaveManager.GetActiveSavePath()), "Slot saves must not create the legacy profile.");
    }

    [Test]
    public void ExplicitOverwriteResetsOnlyChosenSlot()
    {
        LocalSaveManager.SelectSlot(1);
        var first = new LocalSaveManager();
        Assert.IsTrue(first.CreateFreshProfile("Old", "Old Merchant", "Female"));
        LocalProfileData progress = first.LoadProfile();
        progress.global_metrics.total_varahas = 777;
        progress.global_metrics.reputation = 12;
        progress.shift_stats.total_deals_made = 5;
        progress.inventory[0].grams = 9;
        progress.current_scene = GameManager.DefaultGameplaySceneName;
        progress.progression_index = 7;
        first.SaveProfile(progress);

        LocalSaveManager.SelectSlot(2);
        var second = new LocalSaveManager();
        Assert.IsTrue(second.CreateFreshProfile("Other", "Second Merchant", "Male"));
        LocalProfileData other = second.LoadProfile();
        other.global_metrics.total_varahas = 444;
        second.SaveProfile(other);

        LocalSaveManager.SelectSlot(1);
        Assert.IsTrue(new LocalSaveManager().CreateFreshProfile("Fresh", "New Merchant", "Male"));
        Assert.IsTrue(LocalSaveManager.TryLoadSlot(1, out LocalProfileData fresh));
        Assert.AreEqual(PlayerState.DefaultVarahas, fresh.global_metrics.total_varahas);
        Assert.AreEqual(PlayerState.DefaultReputation, fresh.global_metrics.reputation);
        Assert.AreEqual(0, fresh.shift_stats.total_deals_made);
        Assert.AreEqual(LocalSaveManager.DefaultProgressionIndex, fresh.progression_index);
        Assert.IsFalse(fresh.hasSavedMarketDayTimer);
        Assert.AreEqual("New Merchant", fresh.character_name);
        Assert.IsTrue(LocalSaveManager.TryLoadSlot(2, out LocalProfileData retained));
        Assert.AreEqual(444, retained.global_metrics.total_varahas);
        Assert.AreEqual("Second Merchant", retained.character_name);
    }

    [Test]
    public void DeleteRemovesOnlySelectedSlotAndLeavesLegacyProfileUntouched()
    {
        Assert.IsTrue(new LocalSaveManager().CreateFreshProfile("Legacy"));
        string legacyPath = LocalSaveManager.GetActiveSavePath();
        string legacyJson = File.ReadAllText(legacyPath);
        for (int slot = 1; slot <= LocalSaveManager.SlotCount; slot++)
        {
            LocalSaveManager.SelectSlot(slot);
            Assert.IsTrue(new LocalSaveManager().CreateFreshProfile("Journey " + slot));
        }

        LocalSaveManager.SelectSlot(2);
        Assert.IsTrue(new LocalSaveManager().CreateFreshProfile("Replaced journey"));
        Assert.IsTrue(File.Exists(LocalSaveManager.GetSlotSavePath(2) + ".bak"));

        Assert.IsTrue(LocalSaveManager.DeleteSlot(2));
        Assert.IsFalse(LocalSaveManager.SlotHasAnyData(2));
        Assert.IsTrue(LocalSaveManager.TryLoadSlot(1, out _));
        Assert.IsTrue(LocalSaveManager.TryLoadSlot(3, out _));
        Assert.AreEqual(legacyJson, File.ReadAllText(legacyPath));
    }

    [Test]
    public void CorruptSlotCannotBeContinuedOrSilentlyReplaced()
    {
        LocalSaveManager.SelectSlot(1);
        string path = LocalSaveManager.GetActiveSavePath();
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        File.WriteAllText(path, "{}");
        Assert.IsTrue(LocalSaveManager.SlotHasAnyData(1));
        Assert.IsFalse(LocalSaveManager.TryLoadSlot(1, out _));
        LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex("Profile is invalid; refusing to overwrite"));
        LocalProfileData defaults = new LocalSaveManager().LoadProfile();
        LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex("Refusing to overwrite an invalid profile"));
        new LocalSaveManager().SaveProfile(defaults);
        Assert.AreEqual("{}", File.ReadAllText(path));
        Assert.IsTrue(new LocalSaveManager().CreateFreshProfile("Confirmed overwrite"));
        Assert.IsTrue(LocalSaveManager.TryLoadSlot(1, out LocalProfileData fresh));
        Assert.AreEqual("Confirmed overwrite", fresh.save_name);
    }
}
