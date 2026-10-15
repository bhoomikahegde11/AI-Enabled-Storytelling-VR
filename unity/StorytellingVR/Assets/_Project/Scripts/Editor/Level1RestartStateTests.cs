using System;
using System.Collections;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class Level1RestartStateTests
{
    private string directory;
    private string previousDirectory;
    private int previousSlot;
    private GameObject host;
    private Level1GameState state;
    private bool ownsFixture;
    private static readonly FieldInfo InstanceField = typeof(Level1GameState)
        .GetField("instance", BindingFlags.Static | BindingFlags.NonPublic);

    [SetUp]
    public void SetUp()
    {
        ownsFixture = false;
        Assert.IsNull(Level1GameState.ExistingInstance);
        Assert.IsNull(OrderManager.Instance);
        previousDirectory = LocalSaveManager.EditorTestSaveDirectoryOverride;
        previousSlot = LocalSaveManager.ActiveSlot;
        directory = Path.Combine(Path.GetTempPath(), "Level1Restart_" + Guid.NewGuid().ToString("N"));
        LocalSaveManager.EditorTestSaveDirectoryOverride = directory;
        Assert.IsTrue(LocalSaveManager.SelectSlot(1));
        ownsFixture = true;

        var saves = new LocalSaveManager();
        Assert.IsTrue(saves.CreateFreshProfile("First journey", "Asha", "Female"));
        LocalProfileData profile = saves.LoadProfile();
        profile.current_scene = "SpicesInteraction";
        profile.progression_index = 6;
        profile.intro_completed = true;
        profile.global_metrics.total_varahas = 250;
        profile.global_metrics.reputation = 61;
        profile.global_metrics.completed_levels.Add("NewCoinScene");
        profile.shift_stats.total_deals_made = 2;
        profile.shift_stats.total_varahas_earned = 80;
        Assert.IsTrue(saves.SaveProfile(profile));

        host = new GameObject("Level 1 restart fixture");
        host.SetActive(false);
        state = host.AddComponent<Level1GameState>();
        InstanceField.SetValue(null, state);
        state.EnsureInitialized();
        Assert.IsTrue(state.CaptureLevel1EntrySnapshot(false));
    }

    [TearDown]
    public void TearDown()
    {
        if (!ownsFixture) return;
        if (host != null) UnityEngine.Object.DestroyImmediate(host);
        InstanceField.SetValue(null, null);
        LocalSaveManager.EditorTestSaveDirectoryOverride = previousDirectory;
        if (previousSlot != 0) LocalSaveManager.SelectSlot(previousSlot);
        if (Directory.Exists(directory)) Directory.Delete(directory, true);
    }

    [Test]
    public void SettledAttemptRestoresEntryEconomyInventoryStatsAndEarlierProgress()
    {
        LocalProfileData entry = new LocalSaveManager().LoadProfile();
        int startingPepper = entry.inventory.Find(item => item.spiceKey == "pepper").grams;
        state.SyncTradeFromBackend("Lakshmi Amma", "Hampi", "Pepper", "2 Seers (~560g)", 560, 173, null);
        TradeTermsSnapshot terms = TradeTermsSnapshot.Capture(state.ActiveTrade);
        Assert.IsTrue(state.ActiveTrade.BindAcceptedTerms(terms));
        Assert.IsTrue(state.ResolveAcceptedTrade(terms, 0.7f, 0.2f, 0).isSuccess);
        Assert.AreEqual(423, state.CurrentMoney);
        Assert.AreEqual(3, new LocalSaveManager().LoadProfile().shift_stats.total_deals_made);

        Assert.IsTrue(state.RestoreLevel1EntrySnapshot());
        LocalProfileData restored = new LocalSaveManager().LoadProfile();
        Assert.AreEqual(250, state.CurrentMoney);
        Assert.AreEqual(61, state.CurrentReputation);
        Assert.AreEqual(startingPepper, restored.inventory.Find(item => item.spiceKey == "pepper").grams);
        Assert.AreEqual(2, restored.shift_stats.total_deals_made);
        Assert.AreEqual(80, restored.shift_stats.total_varahas_earned);
        Assert.Contains("NewCoinScene", restored.global_metrics.completed_levels);
        Assert.IsTrue(restored.intro_completed);
        Assert.AreEqual("Asha", restored.character_name);
        Assert.AreEqual("Female", restored.selected_gender);
        Assert.AreEqual(1, LocalSaveManager.ActiveSlot);
        Assert.IsNull(state.ActiveTrade);
    }

    [Test]
    public void UnfinishedAgreementIsClearedWithoutAwardingIt()
    {
        state.SyncTradeFromBackend("Lakshmi Amma", "Hampi", "Pepper", "2 Seers (~560g)", 560, 173, null);
        TradeTermsSnapshot terms = TradeTermsSnapshot.Capture(state.ActiveTrade);
        Assert.IsTrue(state.ActiveTrade.BindAcceptedTerms(terms));
        Assert.IsTrue(state.RestoreLevel1EntrySnapshot());
        Assert.IsNull(state.ActiveTrade);
        Assert.IsFalse(state.ResolveAcceptedTrade(terms, 0.7f, 0.2f, 0).isSuccess);
        Assert.AreEqual(250, state.CurrentMoney);
        Assert.AreEqual(2, new LocalSaveManager().LoadProfile().shift_stats.total_deals_made);
    }

    [Test]
    public void RestartAfterWalkAwayRestoresRespectWithoutInventingADeal()
    {
        state.SyncTradeFromBackend("Lakshmi Amma", "Hampi", "Pepper", "2 Seers (~560g)", 560, 173, null);
        Assert.IsFalse(state.ResolveTradeFromBackend("WALK_AWAY", 0, 560, 0f, 0.8f, 0).isSuccess);
        Assert.Less(state.CurrentReputation, 61);
        Assert.IsTrue(state.RestoreLevel1EntrySnapshot());
        Assert.AreEqual(61, state.CurrentReputation);
        Assert.AreEqual(250, state.CurrentMoney);
        Assert.AreEqual(2, new LocalSaveManager().LoadProfile().shift_stats.total_deals_made);
    }

    [Test]
    public void RestartResetsSavedTimerAndCanBeRepeated()
    {
        LocalProfileData progress = new LocalSaveManager().LoadProfile();
        progress.hasSavedMarketDayTimer = true;
        progress.remainingMarketDaySeconds = 123f;
        Assert.IsTrue(new LocalSaveManager().SaveProfile(progress));
        state.ReloadProfileFromDisk();
        Assert.AreEqual(123f, state.MarketDayRemainingSeconds);
        for (int attempt = 0; attempt < 2; attempt++)
        {
            Assert.IsTrue(state.RestoreLevel1EntrySnapshot());
            Assert.AreEqual(state.MarketDayDurationSeconds, state.MarketDayRemainingSeconds);
            Assert.IsFalse(state.MarketDayStarted);
            Assert.IsFalse(state.MarketDayEnded);
            Assert.AreEqual(state.MarketDayDurationSeconds, new LocalSaveManager().LoadProfile().remainingMarketDaySeconds);
        }
    }

    [Test]
    public void FreshAttemptHasOneFifteenMinuteClockAndTutorialSavesDoNotStartIt()
    {
        Assert.AreEqual(900f, state.MarketDayDurationSeconds);
        Assert.AreEqual(900f, state.MarketDayRemainingSeconds);
        Assert.IsFalse(state.MarketDayStarted);
        state.SaveProfileToDisk();
        Assert.IsFalse(new LocalSaveManager().LoadProfile().hasSavedMarketDayTimer);
    }

    [TestCase(180f)]
    [TestCase(1200f)]
    public void CustomInspectorDurationIsNotClampedToOldDebugMinimum(float seconds)
    {
        TimerField("marketDayDurationSeconds").SetValue(state, seconds);
        TimerInvoke("OnValidate");
        Assert.AreEqual(seconds, state.MarketDayDurationSeconds);
        Assert.AreEqual(seconds, state.MarketDayRemainingSeconds);
        Assert.IsFalse(state.MarketDayStarted);
        Assert.IsTrue(state.RestoreLevel1EntrySnapshot());
        Assert.AreEqual(seconds, state.MarketDayRemainingSeconds);
    }

    [Test]
    public void SceneConfigurationOverridesTutorialDefaultButPreservesContinueAndActiveClock()
    {
        GameObject configurationHost = new GameObject("scene timer configuration");
        configurationHost.SetActive(false);
        try
        {
            var configuration = configurationHost.AddComponent<Level1GameState>();
            TimerField("marketDayDurationSeconds").SetValue(configuration, 1200f);
            var saves = new LocalSaveManager();
            LocalProfileData continued = saves.LoadProfile();
            continued.hasSavedMarketDayTimer = true;
            continued.remainingMarketDaySeconds = 1050f;
            Assert.IsTrue(saves.SaveProfile(continued));
            state.ReloadProfileFromDisk();
            TimerInvoke("ApplySceneTimerConfiguration", configuration);
            Assert.AreEqual(1200f, state.MarketDayDurationSeconds);
            Assert.AreEqual(1050f, state.MarketDayRemainingSeconds);
            TimerInvoke("OnValidate");
            Assert.AreEqual(1050f, state.MarketDayRemainingSeconds);

            SetRunningClock(650f);
            TimerField("marketDayDurationSeconds").SetValue(configuration, 180f);
            TimerInvoke("ApplySceneTimerConfiguration", configuration);
            Assert.AreEqual(1200f, state.MarketDayDurationSeconds);
            Assert.AreEqual(650f, state.MarketDayRemainingSeconds);
        }
        finally { UnityEngine.Object.DestroyImmediate(configurationHost); }
    }

    [Test]
    public void CustomersSettlementsAndWalkAwayShareTheSameRemainingTime()
    {
        SetRunningClock(650f);
        float startedAt = (float)TimerField("marketDayStartedAt").GetValue(state);
        for (int customer = 0; customer < 2; customer++)
        {
            state.PrepareForNewCustomer();
            state.StartMarketDay(); // Repeated starts are idempotent.
            state.SyncTradeFromBackend("Lakshmi Amma", "Hampi", "Pepper", "2 Seers (~560g)", 560, 173, null);
            if (customer == 0)
            {
                TradeTermsSnapshot terms = TradeTermsSnapshot.Capture(state.ActiveTrade);
                Assert.IsTrue(state.ActiveTrade.BindAcceptedTerms(terms));
                Assert.IsTrue(state.ResolveAcceptedTrade(terms, 0.7f, 0.2f, 0).isSuccess);
            }
            else Assert.IsFalse(state.ResolveTradeFromBackend("WALK_AWAY", 0, 560, 0f, 0.8f, 0).isSuccess);
            Assert.AreEqual(650f, state.MarketDayRemainingSeconds);
            Assert.AreEqual(startedAt, TimerField("marketDayStartedAt").GetValue(state));
        }
        Assert.AreEqual(650f, new LocalSaveManager().LoadProfile().remainingMarketDaySeconds);
    }

    [Test]
    public void TimerExpiresAndCompletionCanBeClaimedOnlyOnce()
    {
        GameObject marketplaceHost = new GameObject("market completion fixture");
        marketplaceHost.SetActive(false);
        try
        {
            var marketplace = marketplaceHost.AddComponent<MarketplaceManager>();
            typeof(MarketplaceManager).GetField("marketDayState", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(marketplace, state);
            MethodInfo claim = typeof(MarketplaceManager).GetMethod("TryClaimMarketDayCompletion", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsFalse((bool)claim.Invoke(marketplace, null));
            int endings = 0;
            state.MarketDayEndedEvent += () => endings++;
            SetRunningClock(0f);
            IEnumerator timer = (IEnumerator)TimerInvoke("MarketDayTimerRoutine");
            Assert.IsFalse(timer.MoveNext());
            state.EndMarketDay();
            state.StartMarketDay();
            Assert.AreEqual(1, endings);
            Assert.AreEqual(0f, state.MarketDayRemainingSeconds);
            Assert.IsTrue(state.MarketDayEnded);
            Assert.IsTrue((bool)claim.Invoke(marketplace, null));
            Assert.IsFalse((bool)claim.Invoke(marketplace, null));
            Assert.AreEqual(250, state.CurrentMoney);
            Assert.AreEqual(61, state.CurrentReputation);
        }
        finally { UnityEngine.Object.DestroyImmediate(marketplaceHost); }
    }

    private static FieldInfo TimerField(string name) => typeof(Level1GameState)
        .GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);

    private object TimerInvoke(string method, params object[] arguments) => typeof(Level1GameState)
        .GetMethod(method, BindingFlags.NonPublic | BindingFlags.Instance).Invoke(state, arguments);

    private void SetRunningClock(float remaining)
    {
        TimerField("<MarketDayStarted>k__BackingField").SetValue(state, true);
        TimerField("marketDayRemainingSeconds").SetValue(state, remaining);
        TimerField("marketDayStartedAt").SetValue(state, Time.time - (state.MarketDayDurationSeconds - remaining));
    }

    [Test]
    public void ReloadForContinueKeepsCommittedProgressUntilRestartIsRequested()
    {
        LocalProfileData progress = new LocalSaveManager().LoadProfile();
        progress.global_metrics.total_varahas = 999;
        progress.global_metrics.reputation = 31;
        progress.current_scene = GameManager.DefaultGameplaySceneName;
        Assert.IsTrue(new LocalSaveManager().SaveProfile(progress));
        state.ReloadProfileFromDisk();
        Assert.AreEqual(999, state.CurrentMoney);
        Assert.AreEqual(31, state.CurrentReputation);
        Assert.IsTrue(state.CaptureLevel1EntrySnapshot(false));
        Assert.IsTrue(state.RestoreLevel1EntrySnapshot());
        Assert.AreEqual(250, state.CurrentMoney);
        Assert.AreEqual(61, state.CurrentReputation);
    }

    [Test]
    public void RestoreChangesOnlyTheSelectedSlot()
    {
        Assert.IsTrue(LocalSaveManager.SelectSlot(2));
        var secondSaves = new LocalSaveManager();
        Assert.IsTrue(secondSaves.CreateFreshProfile("Other journey"));
        LocalProfileData second = secondSaves.LoadProfile();
        second.global_metrics.total_varahas = 777;
        Assert.IsTrue(secondSaves.SaveProfile(second));
        Assert.IsTrue(LocalSaveManager.SelectSlot(1));
        Assert.IsTrue(state.RestoreLevel1EntrySnapshot());
        Assert.IsTrue(LocalSaveManager.TryLoadSlot(2, out LocalProfileData untouched));
        Assert.AreEqual(777, untouched.global_metrics.total_varahas);
        Assert.IsNull(untouched.level1_entry);
    }

    [Test]
    public void NewEntryReplacesAnOldAttemptBaselineWithoutChangingIdentity()
    {
        LocalProfileData nextEntry = new LocalSaveManager().LoadProfile();
        nextEntry.global_metrics.total_varahas = 390;
        nextEntry.global_metrics.reputation = 72;
        nextEntry.hasSavedMarketDayTimer = true;
        nextEntry.remainingMarketDaySeconds = 20f;
        Assert.IsTrue(new LocalSaveManager().SaveProfile(nextEntry));
        Assert.IsTrue(state.CaptureLevel1EntrySnapshot(true));
        Assert.AreEqual(state.MarketDayDurationSeconds, state.MarketDayRemainingSeconds);
        Assert.IsFalse(new LocalSaveManager().LoadProfile().hasSavedMarketDayTimer);

        LocalProfileData progress = new LocalSaveManager().LoadProfile();
        progress.global_metrics.total_varahas = 900;
        Assert.IsTrue(new LocalSaveManager().SaveProfile(progress));
        Assert.IsTrue(state.RestoreLevel1EntrySnapshot());
        Assert.AreEqual(390, state.CurrentMoney);
        Assert.AreEqual(72, state.CurrentReputation);
        Assert.AreEqual("Asha", new LocalSaveManager().LoadProfile().character_name);
    }

    [Test]
    public void SlotSwitchRefusesStaleStateBeforeWritingEitherJourney()
    {
        string firstBefore = File.ReadAllText(LocalSaveManager.GetSlotSavePath(1));
        Assert.IsTrue(LocalSaveManager.SelectSlot(2));
        var second = new LocalSaveManager();
        Assert.IsTrue(second.CreateFreshProfile("Second journey"));
        string secondBefore = File.ReadAllText(second.SavePath);

        Assert.IsFalse(state.CaptureLevel1EntrySnapshot(false));
        Assert.IsFalse(state.RestoreLevel1EntrySnapshot());
        Assert.AreEqual(firstBefore, File.ReadAllText(LocalSaveManager.GetSlotSavePath(1)));
        Assert.AreEqual(secondBefore, File.ReadAllText(second.SavePath));
        Assert.AreEqual(2, LocalSaveManager.ActiveSlot);
    }

    [Test]
    public void MissingBaselineRefusesRestartWithoutChangingSave()
    {
        var saves = new LocalSaveManager();
        LocalProfileData progress = saves.LoadProfile();
        progress.level1_entry = null;
        progress.global_metrics.total_varahas = 999;
        Assert.IsTrue(saves.SaveProfile(progress));
        string before = File.ReadAllText(saves.SavePath);
        LogAssert.Expect(LogType.Error, "[MARKET DAY] Level 1 entry snapshot is unavailable; restart was cancelled without changing the save.");
        Assert.IsFalse(state.RestoreLevel1EntrySnapshot());
        Assert.AreEqual(before, File.ReadAllText(saves.SavePath));
        Assert.AreEqual(999, saves.LoadProfile().global_metrics.total_varahas);
    }

    [Test]
    public void ExplicitButEmptySnapshotCannotResetEconomyOrAwardAnUnfinishedDeal()
    {
        var saves = new LocalSaveManager();
        LocalProfileData progress = saves.LoadProfile();
        progress.level1_entry = new Level1EntrySnapshotData();
        Assert.IsTrue(saves.SaveProfile(progress));
        state.SyncTradeFromBackend("Lakshmi Amma", "Hampi", "Pepper", "2 Seers (~560g)", 560, 173, null);
        LocalTradeState unfinished = state.ActiveTrade;
        string before = File.ReadAllText(saves.SavePath);

        LogAssert.Expect(LogType.Error, "[MARKET DAY] Level 1 entry snapshot is unavailable; restart was cancelled without changing the save.");
        Assert.IsFalse(state.RestoreLevel1EntrySnapshot());
        Assert.AreSame(unfinished, state.ActiveTrade);
        Assert.AreEqual(250, state.CurrentMoney);
        Assert.AreEqual(61, state.CurrentReputation);
        Assert.AreEqual(before, File.ReadAllText(saves.SavePath));
    }
}
