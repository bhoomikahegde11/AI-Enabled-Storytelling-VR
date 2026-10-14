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

    [Test]
    public void AutomaticChoiceUsesFirstEmptySlotIncludingGaps()
    {
        Assert.AreEqual(1, LocalSaveManager.FirstEmptySlot());
        Assert.IsTrue(LocalSaveManager.TryCreateJourneyInSlot(1, "First", "Asha", "Female", false));
        Assert.IsTrue(LocalSaveManager.TryCreateJourneyInSlot(3, "Third", "Ravi", "Male", false));
        Assert.AreEqual(2, LocalSaveManager.FirstEmptySlot());
        Assert.IsTrue(LocalSaveManager.TryCreateJourneyInSlot(LocalSaveManager.FirstEmptySlot(),
            "Second", "Mira", "Female", false));
        Assert.AreEqual(0, LocalSaveManager.FirstEmptySlot());
    }

    [Test]
    public void JourneyNamesAreTrimmedOrdinalCaseInsensitiveAndCheckedAtCreation()
    {
        Assert.IsTrue(LocalSaveManager.TryCreateJourneyInSlot(1, " Hampi Adventure ", "Asha", "Female", false));
        Assert.IsTrue(LocalSaveManager.TryLoadSlot(1, out LocalProfileData first));
        Assert.AreEqual("Hampi Adventure", first.save_name);
        Assert.AreEqual(1, LocalSaveManager.FindJourneyNameConflict("hampi adventure"));
        Assert.IsFalse(LocalSaveManager.TryCreateJourneyInSlot(2, "  HAMPI ADVENTURE  ", "Ravi", "Male", false));
        Assert.IsFalse(LocalSaveManager.TryCreateJourneyInSlot(2, "   ", "Ravi", "Male", false));
        Assert.IsFalse(LocalSaveManager.SlotHasAnyData(2));
        Assert.AreEqual(1, LocalSaveManager.ActiveSlot);
        Assert.IsTrue(LocalSaveManager.TryCreateJourneyInSlot(1, " hampi adventure ", "Mira", "Female", true),
            "A name may be reused only when its own slot is explicitly overwritten.");
    }

    [Test]
    public void FullSlotsRequireConfirmedSpecificOverwriteAndKeepOtherSlotsUntouched()
    {
        for (int slot = 1; slot <= LocalSaveManager.SlotCount; slot++)
            Assert.IsTrue(LocalSaveManager.TryCreateJourneyInSlot(slot, "Journey " + slot, "Merchant", "Male", false));
        Assert.AreEqual(0, LocalSaveManager.FirstEmptySlot());

        LocalSaveManager.SelectSlot(2);
        var secondSaves = new LocalSaveManager();
        LocalProfileData progress = secondSaves.LoadProfile();
        progress.global_metrics.total_varahas = 900;
        progress.level1_entry = new Level1EntrySnapshotData { varahas = 250, reputation = 61 };
        Assert.IsTrue(secondSaves.SaveProfile(progress));
        string firstBefore = File.ReadAllText(LocalSaveManager.GetSlotSavePath(1));
        string thirdBefore = File.ReadAllText(LocalSaveManager.GetSlotSavePath(3));

        Assert.IsFalse(LocalSaveManager.TryCreateJourneyInSlot(2, "Replacement", "Mira", "Female", false),
            "Cancel/no confirmation must not write the occupied slot.");
        Assert.IsFalse(LocalSaveManager.TryCreateJourneyInSlot(2, " JOURNEY 1 ", "Mira", "Female", true),
            "Even a confirmed overwrite cannot duplicate another occupied slot's name.");
        Assert.AreEqual(900, secondSaves.LoadProfile().global_metrics.total_varahas);
        Assert.IsTrue(LocalSaveManager.TryCreateJourneyInSlot(2, "Replacement", "Mira", "Female", true));
        LocalProfileData replaced = secondSaves.LoadProfile();
        Assert.AreEqual("Replacement", replaced.save_name);
        Assert.AreEqual(PlayerState.DefaultVarahas, replaced.global_metrics.total_varahas);
        Assert.IsNull(replaced.level1_entry, "A new journey must not inherit its old Level 1 restart baseline.");
        Assert.AreEqual(firstBefore, File.ReadAllText(LocalSaveManager.GetSlotSavePath(1)));
        Assert.AreEqual(thirdBefore, File.ReadAllText(LocalSaveManager.GetSlotSavePath(3)));
    }

    [Test]
    public void CorruptOrInterruptedSlotIsNeverAnAutomaticVacancy()
    {
        string path = LocalSaveManager.GetSlotSavePath(1);
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        File.WriteAllText(path, "{}");
        Assert.AreEqual(2, LocalSaveManager.FirstEmptySlot());
        Assert.IsFalse(LocalSaveManager.TryCreateJourneyInSlot(1, "New", "Asha", "Female", false));
        Assert.IsFalse(LocalSaveManager.TryCreateJourneyInSlot(1, "New", "Asha", "Female", true));
        Assert.AreEqual("{}", File.ReadAllText(path));
        File.WriteAllText(LocalSaveManager.GetSlotSavePath(3) + ".tmp", "interrupted");
        Assert.AreEqual(2, LocalSaveManager.FirstEmptySlot());
    }

    [Test]
    public void OlderUnnamedSaveRemainsLoadableWithoutAnInventedName()
    {
        Assert.IsTrue(LocalSaveManager.TryCreateJourneyInSlot(1, "Old", "Asha", "Female", false));
        LocalSaveManager.SelectSlot(1);
        var saves = new LocalSaveManager();
        LocalProfileData legacy = saves.LoadProfile();
        legacy.save_name = string.Empty;
        legacy.current_scene = GameManager.DefaultGameplaySceneName;
        Assert.IsTrue(saves.SaveProfile(legacy));
        Assert.AreEqual(0, LocalSaveManager.FindJourneyNameConflict("Journey"));
        Assert.IsTrue(LocalSaveManager.TryLoadSlot(1, out LocalProfileData loaded));
        Assert.AreEqual(string.Empty, loaded.save_name);
        Assert.AreEqual(GameManager.DefaultGameplaySceneName, loaded.current_scene);
        Assert.IsTrue(LocalSaveManager.TryCreateJourneyInSlot(2, "Journey", "Mira", "Female", false));
    }

    [Test]
    public void ContinueProfileAndLevel1RestartSnapshotStayInTheirOriginalSlot()
    {
        Assert.IsTrue(LocalSaveManager.TryCreateJourneyInSlot(1, "Market", "Asha", "Female", false));
        var saves = new LocalSaveManager();
        LocalProfileData progress = saves.LoadProfile();
        progress.current_scene = GameManager.DefaultGameplaySceneName;
        progress.progression_index = 7;
        progress.global_metrics.total_varahas = 481;
        progress.level1_entry = new Level1EntrySnapshotData { varahas = 250, reputation = 61 };
        Assert.IsTrue(saves.SaveProfile(progress));

        Assert.IsTrue(LocalSaveManager.TryCreateJourneyInSlot(2, "Fresh", "Mira", "Female", false));
        Assert.IsTrue(LocalSaveManager.TryLoadSlot(1, out LocalProfileData continued));
        Assert.AreEqual(GameManager.DefaultGameplaySceneName, continued.current_scene);
        Assert.AreEqual(7, continued.progression_index);
        Assert.AreEqual(481, continued.global_metrics.total_varahas);
        Assert.AreEqual(250, continued.level1_entry.varahas);
        Assert.IsTrue(LocalSaveManager.TryLoadSlot(2, out LocalProfileData fresh));
        Assert.IsNull(fresh.level1_entry);
        Assert.AreEqual(PlayerState.DefaultVarahas, fresh.global_metrics.total_varahas);
    }

    [TestCase(0)]
    [TestCase(1)]
    public void NullEntrySnapshotSurvivesRepeatedSaveLoadCycles(int slot)
    {
        if (slot != 0) Assert.IsTrue(LocalSaveManager.SelectSlot(slot));
        var saves = new LocalSaveManager();
        Assert.IsTrue(saves.CreateFreshProfile("No baseline"));
        for (int cycle = 0; cycle < 3; cycle++)
        {
            LocalProfileData loaded = saves.LoadProfile();
            Assert.IsNull(loaded.level1_entry);
            Assert.AreEqual(1, loaded.level1_entry_format);
            Assert.IsFalse(loaded.level1_entry_present);
            Assert.IsTrue(saves.SaveProfile(loaded));
        }
    }

    [TestCase(false)]
    [TestCase(true)]
    public void LegacySnapshotReadDistinguishesEmptyInlineArtifactFromRealCapture(bool captured)
    {
        LocalSaveManager.SelectSlot(1);
        var saves = new LocalSaveManager();
        LocalProfileData legacy = saves.LoadProfile();
        legacy.level1_entry_format = 0;
        legacy.level1_entry_present = false;
        legacy.level1_entry = new Level1EntrySnapshotData();
        if (captured)
        {
            // Zero economy is valid when backed by a genuine inventory snapshot.
            legacy.level1_entry.inventory.Add(new InventoryEntry { spiceKey = "pepper", grams = 0 });
            legacy.level1_entry.completedLevels.Add("NewCoinScene");
            legacy.level1_entry.shiftStats.total_deals_made = 2;
        }
        string json = JsonUtility.ToJson(legacy, true);
        File.WriteAllText(saves.SavePath, json); // Only this fixture's temporary save.

        Assert.IsTrue(saves.TryLoadExistingProfile(out LocalProfileData loaded));
        if (captured)
        {
            Assert.IsNotNull(loaded.level1_entry);
            Assert.AreEqual(0, loaded.level1_entry.varahas);
            Assert.AreEqual(0, loaded.level1_entry.reputation);
            Assert.AreEqual(2, loaded.level1_entry.shiftStats.total_deals_made);
            Assert.Contains("NewCoinScene", loaded.level1_entry.completedLevels);
        }
        else Assert.IsNull(loaded.level1_entry);
        Assert.AreEqual(json, File.ReadAllText(saves.SavePath), "Reading legacy data must not migrate or rewrite it.");
    }
}
