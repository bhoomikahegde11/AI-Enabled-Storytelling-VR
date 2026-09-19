using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace StorytellingVR.Editor
{
    /// <summary>
    /// This script patches a known issue in Meta Voice SDK Conduit where duplicate 
    /// "Assembly-CSharp" assemblies during build cause an ArgumentException in 
    /// AssemblyWalker, breaking the Android build pipeline.
    /// It automatically modifies the AssemblyWalker.cs file in the PackageCache.
    /// </summary>
    [InitializeOnLoad]
    public class VoiceSDKConduitFix
    {
        static VoiceSDKConduitFix()
        {
            ApplyPatch();
        }

        private static void ApplyPatch()
        {
            // Find the Voice SDK package folder
            var packageDir = Directory.GetDirectories("Library/PackageCache", "com.meta.xr.sdk.voice@*").FirstOrDefault();
            if (packageDir == null) return;

            string path = Path.Combine(packageDir, "Lib/Wit.ai/Scripts/Editor/Conduit/AssemblyWalker.cs");
            if (!File.Exists(path)) return;

            string content = File.ReadAllText(path);

            // The problematic line that throws ArgumentException if duplicate assemblies exist
            string targetLine = "_assemblies.Add(conduitAssembly.FullName.Split(',').First(), conduitAssembly);";
            
            // Safe replacement that uses indexer or checks ContainsKey
            string replacement = "var key = conduitAssembly.FullName.Split(',').First();\n                if (!_assemblies.ContainsKey(key))\n                {\n                    _assemblies.Add(key, conduitAssembly);\n                }";

            // If the target line is found, we patch it
            if (content.Contains(targetLine))
            {
                content = content.Replace(targetLine, replacement);
                
                // Remove readonly attribute if present
                var fileInfo = new FileInfo(path);
                if (fileInfo.IsReadOnly)
                {
                    fileInfo.IsReadOnly = false;
                }

                File.WriteAllText(path, content);
                Debug.Log($"[VoiceSDKConduitFix] Successfully patched {path} to prevent duplicate Assembly-CSharp build error.");
                
                // Request script reload to compile the patched PackageCache file
                EditorUtility.RequestScriptReload();
            }
        }
    }
}
