using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class IndianNpcVoicePackageImporter
{
    private const string ScenePath = "Assets/_Project/Scenes/CompleteFlowWithFinalScenes/Level1_MainLoopUpdated.unity";
    private const string Root = "Assets/_Project/Audio/NPCVoices/Level1/IndianProduction";

    [MenuItem("Tools/Anubhava/Import Completed Indian NPC Voice Package")]
    private static void ImportFromMenu()
    {
        string directory = EditorUtility.OpenFolderPanel("Select one completed Indian NPC voice package", "", "");
        if (string.IsNullOrEmpty(directory))
            return;
        try
        {
            string result = Import(directory);
            EditorUtility.DisplayDialog("NPC voice import", result + "\nSave Level1_MainLoopUpdated to keep its registration.", "OK");
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            EditorUtility.DisplayDialog("NPC voice import stopped", e.Message, "OK");
        }
    }

    public static string Import(string directory)
    {
        var scene = SceneManager.GetActiveScene();
        if (EditorApplication.isPlayingOrWillChangePlaymode || scene.path != ScenePath)
            throw new InvalidOperationException("Open Level1_MainLoopUpdated as the active scene in Edit Mode first.");
        var provider = scene.GetRootGameObjects()
            .SelectMany(root => root.GetComponentsInChildren<AudioManager>(true))
            .Select(manager => manager.prerecordedNpcProvider)
            .OfType<PreRecordedNpcTtsProvider>().Distinct().SingleOrDefault();
        if (provider == null || provider.audioSource == null || !provider.enabled || provider.gameObject.scene != scene)
            throw new InvalidOperationException("Could not find the configured Level 1 prerecorded provider and its BuyerNPC AudioSource.");

        EditorUtility.DisplayProgressBar("NPC voice import", "Checking final reports, clip hashes and mappings", 0);
        try
        {
            var package = IndianNpcVoicePackage.Read(directory);
            return ImportValidated(package, provider);
        }
        finally { EditorUtility.ClearProgressBar(); }
    }

    private static string ImportValidated(IndianNpcVoicePackage package, PreRecordedNpcTtsProvider provider)
    {
        string character = package.Bank.characterId;
        string folder = Root + "/" + character;
        string bankPath = folder + "/VoiceBank.json";
        string dbPath = folder + "/VoiceDatabase.asset";
        string audioFolder = folder + "/Audio";
        var banks = provider.characterBanks ?? Array.Empty<PreRecordedNpcTtsProvider.CharacterVoiceBank>();
        var existing = banks.Where(x => x != null && x.characterId == character).SingleOrDefault();
        if (existing != null && (AssetDatabase.GetAssetPath(existing.mapping) != bankPath ||
                                 AssetDatabase.GetAssetPath(existing.clips) != dbPath))
            throw new InvalidOperationException("This character already uses a different voice bank. Resolve that registration in the Inspector first.");
        if (File.Exists(bankPath) && File.ReadAllText(bankPath) != package.BankJson)
            throw new IOException("A different imported mapping already exists at " + bankPath);
        var database = AssetDatabase.LoadAssetAtPath<DialogueVoiceDatabase>(dbPath);
        if (File.Exists(dbPath) && database == null)
            throw new IOException("Another asset already exists at " + dbPath);
        foreach (var clip in package.Clips)
        {
            string path = audioFolder + "/" + clip.clipId + ".wav";
            if (File.Exists(path) && IndianNpcVoicePackage.Hash(path) != clip.sha256.ToLowerInvariant())
                throw new IOException("A different recording already exists at " + path);
        }

        EnsureFolder(audioFolder);
        AssetDatabase.StartAssetEditing();
        try
        {
            for (int i = 0; i < package.Clips.Length; i++)
            {
                var clip = package.Clips[i];
                string path = audioFolder + "/" + clip.clipId + ".wav";
                EditorUtility.DisplayProgressBar("NPC voice import", "Copying " + character, (float)i / package.Clips.Length);
                if (!File.Exists(path))
                {
                    File.Copy(package.SourcePath(clip), path);
                    if (IndianNpcVoicePackage.Hash(path) != clip.sha256.ToLowerInvariant())
                    {
                        File.Delete(path);
                        throw new IOException("Package WAV changed while copying: " + clip.relativeWavPath);
                    }
                }
                if (AssetImporter.GetAtPath(path) == null)
                    AssetDatabase.ImportAsset(path);
            }
            if (!File.Exists(bankPath))
            {
                File.WriteAllText(bankPath, package.BankJson);
                AssetDatabase.ImportAsset(bankPath);
            }
        }
        finally { AssetDatabase.StopAssetEditing(); }

        var imported = new AudioClip[package.Clips.Length];
        for (int i = 0; i < imported.Length; i++)
        {
            var clip = package.Clips[i];
            string path = audioFolder + "/" + clip.clipId + ".wav";
            var importer = AssetImporter.GetAtPath(path) as AudioImporter;
            if (importer == null)
                throw new IOException("Unity did not create an audio importer for " + path);
            EditorUtility.DisplayProgressBar("NPC voice import", "Configuring " + character, (float)i / imported.Length);
            ConfigureAudio(importer);
            imported[i] = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
            if (imported[i] == null || imported[i].channels != 1 || imported[i].frequency != package.Bank.sampleRate)
                throw new IOException("Imported clip format mismatch at " + path);
        }
        var mapping = AssetDatabase.LoadAssetAtPath<TextAsset>(bankPath);
        if (mapping == null)
            throw new IOException("Unity did not import the voice mapping");
        if (database == null)
        {
            database = ScriptableObject.CreateInstance<DialogueVoiceDatabase>();
            AssetDatabase.CreateAsset(database, dbPath);
        }
        var db = new SerializedObject(database);
        var entries = db.FindProperty("entries");
        entries.arraySize = imported.Length;
        for (int i = 0; i < imported.Length; i++)
        {
            var entry = entries.GetArrayElementAtIndex(i);
            entry.FindPropertyRelative("lineId").stringValue = package.Clips[i].clipId;
            entry.FindPropertyRelative("audioClip").objectReferenceValue = imported[i];
            entry.FindPropertyRelative("description").stringValue = package.Clips[i].text;
        }
        db.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(database);
        AssetDatabase.SaveAssetIfDirty(database);

        if (existing == null)
        {
            Undo.RecordObject(provider, "Register NPC voice bank");
            provider.characterBanks = banks.Concat(new[] { new PreRecordedNpcTtsProvider.CharacterVoiceBank
            {
                characterId = character, mapping = mapping, clips = database,
                overrideAudibility = true, voiceVolume = 1f, minimumVoiceDistance = 4f
            }}).ToArray();
            EditorUtility.SetDirty(provider);
            EditorSceneManager.MarkSceneDirty(provider.gameObject.scene);
        }
        return $"Imported {package.Clips.Length} clips for {character}.";
    }

    private static void ConfigureAudio(AudioImporter importer)
    {
        bool Matches(AudioImporterSampleSettings settings) => settings.loadType == AudioClipLoadType.DecompressOnLoad &&
            settings.sampleRateSetting == AudioSampleRateSetting.PreserveSampleRate &&
            settings.compressionFormat == AudioCompressionFormat.PCM && !settings.preloadAudioData;
        if (Matches(importer.defaultSampleSettings) && importer.ContainsSampleSettingsOverride("Android") &&
            Matches(importer.GetOverrideSampleSettings("Android")) && !importer.forceToMono &&
            !importer.ambisonic && importer.loadInBackground)
            return;
        var settings = importer.defaultSampleSettings;
        settings.loadType = AudioClipLoadType.DecompressOnLoad;
        settings.sampleRateSetting = AudioSampleRateSetting.PreserveSampleRate;
        settings.compressionFormat = AudioCompressionFormat.PCM;
        settings.preloadAudioData = false;
        importer.defaultSampleSettings = settings;
        if (!importer.SetOverrideSampleSettings("Android", settings))
            throw new IOException("Unity rejected Android audio import settings for " + importer.assetPath);
        importer.forceToMono = false; // WAVs are already mono. Force-to-mono can normalize imported audio.
        importer.loadInBackground = true;
        importer.ambisonic = false;
        importer.SaveAndReimport();
    }

    private static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path))
            return;
        int slash = path.LastIndexOf('/');
        string parent = path.Substring(0, slash);
        EnsureFolder(parent);
        AssetDatabase.CreateFolder(parent, path.Substring(slash + 1));
    }
}
