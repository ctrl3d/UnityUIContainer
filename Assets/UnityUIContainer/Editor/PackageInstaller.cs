using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.PackageManager;
using UnityEngine;

namespace work.ctrl3d.UnityUIContainer
{
    [InitializeOnLoad]
    public class PackageInstaller
    {
        private const string UniTaskName = "com.cysharp.unitask";
        private const string UniTaskGitUrl = "https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask";
        
        private const string LitMotionName = "com.annulusgames.lit-motion";
        private const string LitMotionGitUrl = "https://github.com/annulusgames/LitMotion.git?path=src/LitMotion/Assets/LitMotion";

        private const string AlchemyName = "com.annulusgames.alchemy";
        private const string AlchemyGitUrl = "https://github.com/annulusgames/Alchemy.git?path=/Alchemy/Assets/Alchemy";
        
        private const string UnityExtensionsName = "work.ctrl3d.unity-extensions";
        private const string UnityExtensionsGitUrl = "https://github.com/ctrl3d/UnityExtensions.git?path=Assets/UnityExtensions";
        
        static PackageInstaller()
        {
            var isUniTaskInstalled = CheckPackageInstalled(UniTaskName);
            if (!isUniTaskInstalled) AddGitPackage(UniTaskName, UniTaskGitUrl);
            
            var isLitMotionInstalled = CheckPackageInstalled(LitMotionName);
            if (!isLitMotionInstalled) AddGitPackage(LitMotionName, LitMotionGitUrl);
            
            var isUnityExtensionsInstalled = CheckPackageInstalled(UnityExtensionsName);
            if (!isUnityExtensionsInstalled) AddGitPackage(UnityExtensionsName, UnityExtensionsGitUrl);
            
            var isAlchemyInstalled = CheckPackageInstalled(AlchemyName);
            if (!isAlchemyInstalled) AddGitPackage(AlchemyName, AlchemyGitUrl);
            
            if(HasScriptingDefineSymbol("UNITASK_SUPPORT")) return;
            AddScriptingDefineSymbol("UNITASK_SUPPORT");
            
            if(HasScriptingDefineSymbol("LITMOTION_SUPPORT_UNITASK")) return;
            AddScriptingDefineSymbol("LITMOTION_SUPPORT_UNITASK");
        }
        
        private static void AddGitPackage(string packageName, string gitUrl)
        {
            var path = Path.Combine(Application.dataPath, "../Packages/manifest.json");
            var jsonString = File.ReadAllText(path);

            var indexOfLastBracket = jsonString.IndexOf("}", StringComparison.Ordinal);
            var dependenciesSubstring = jsonString[..indexOfLastBracket];
            var endOfLastPackage = dependenciesSubstring.LastIndexOf("\"", StringComparison.Ordinal);

            jsonString = jsonString.Insert(endOfLastPackage + 1, $", \n \"{packageName}\": \"{gitUrl}\"");

            File.WriteAllText(path, jsonString);
            Client.Resolve();
        }

        private static bool CheckPackageInstalled(string packageName)
        {
            var path = Path.Combine(Application.dataPath, "../Packages/manifest.json");
            var jsonString = File.ReadAllText(path);
            return jsonString.Contains(packageName);
        }
        
        private static void AddScriptingDefineSymbol(string symbol)
        {
            var buildTarget = EditorUserBuildSettings.activeBuildTarget;
            var targetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
            var namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(targetGroup);
            
            var symbols = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);
            if (!symbols.Contains(symbol))
            {
                symbols += $";{symbol}";
            }
            
            PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, symbols);
        }
        
        private static bool HasScriptingDefineSymbol(string symbol)
        {
            var buildTarget = EditorUserBuildSettings.activeBuildTarget;
            var targetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
            var namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(targetGroup);
            
            var symbols = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);
            var symbolArray = symbols.Split(';');
            return symbolArray.Any(existingSymbol => existingSymbol == symbol);
        }
    }
}