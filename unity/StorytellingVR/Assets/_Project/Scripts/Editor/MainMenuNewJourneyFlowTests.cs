using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class MainMenuNewJourneyFlowTests
{
    private string directory;
    private string previousDirectory;
    private int previousSlot;
    private GameObject host;
    private GameObject main;
    private GameObject slots;
    private GameObject form;
    private GameObject confirmation;
    private MainMenuSaveFlow flow;

    [SetUp]
    public void SetUp()
    {
        previousDirectory = LocalSaveManager.EditorTestSaveDirectoryOverride;
        previousSlot = LocalSaveManager.ActiveSlot;
        directory = Path.Combine(Path.GetTempPath(), "MainMenuNewJourney_" + Guid.NewGuid().ToString("N"));
        LocalSaveManager.EditorTestSaveDirectoryOverride = directory;
        host = new GameObject("New Journey flow fixture");
        host.SetActive(false); // Avoid scene UI construction and scene loading in EditMode.
        flow = host.AddComponent<MainMenuSaveFlow>();
        main = new GameObject("Main Menu");
        slots = new GameObject("Slot Choices");
        form = new GameObject("Journey Form");
        confirmation = new GameObject("Confirmation");
        Set("mainMenuRoot", main);
        Set("saveSelectPanel", slots);
        Set("newSavePanel", form);
        Set("deleteConfirmation", confirmation);
    }

    [TearDown]
    public void TearDown()
    {
        UnityEngine.Object.DestroyImmediate(host);
        UnityEngine.Object.DestroyImmediate(main);
        UnityEngine.Object.DestroyImmediate(slots);
        UnityEngine.Object.DestroyImmediate(form);
        UnityEngine.Object.DestroyImmediate(confirmation);
        LocalSaveManager.EditorTestSaveDirectoryOverride = previousDirectory;
        if (previousSlot != 0) LocalSaveManager.SelectSlot(previousSlot);
        if (Directory.Exists(directory)) Directory.Delete(directory, true);
    }

    [Test]
    public void NewJourneyOpensFormDirectlyAndBackReturnsToMainMenu()
    {
        flow.OpenSaveSelect();
        Assert.IsTrue(form.activeSelf);
        Assert.IsFalse(slots.activeSelf);
        Assert.IsFalse(main.activeSelf);
        flow.Back();
        Assert.IsTrue(main.activeSelf);
        Assert.IsFalse(form.activeSelf);
        Assert.IsFalse(LocalSaveManager.SlotHasAnyData(1));
    }

    [Test]
    public void CancelOverwriteReturnsToChooserThenFormWithoutWriting()
    {
        flow.OpenSaveSelect();
        form.SetActive(false);
        slots.SetActive(false);
        confirmation.SetActive(true);
        flow.Back();
        Assert.IsFalse(confirmation.activeSelf);
        Assert.IsTrue(slots.activeSelf);
        flow.Back();
        Assert.IsFalse(slots.activeSelf);
        Assert.IsTrue(form.activeSelf);
        for (int slot = 1; slot <= LocalSaveManager.SlotCount; slot++)
            Assert.IsFalse(LocalSaveManager.SlotHasAnyData(slot));
    }

    private void Set(string fieldName, object value)
    {
        typeof(MainMenuSaveFlow).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(flow, value);
    }
}
