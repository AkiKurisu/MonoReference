# MonoReference


***Read this document in Chinese: [中文文档](./README.md)***

Use Reference method to serialize MonoBehaviour, no longer relying on Unity ResourceManager when loading resources. Instead, another Proxy script adds it after loading

## Scenes to be used

Used with [HybridCLR](https://github.com/focus-creative-games/hybridclr) to add a new MonoBehaviour to hot update resources, the Assembly does not need to be added to ``ScriptAssembly.json`` in advance, so that the Mod function can be realized

Please refer to https://hybridclr.doc.code-philosophy.com/docs/basic/monobehaviour. The author’s comments are as follows

```C#
private void AddHotFixAssembliesToScriptingAssembliesJson(string path)
{
     Debug.Log($"[PatchScriptingAssemblyList]. path:{path}");
     /*
         * All dll names are recorded in the ScriptingAssemblies.json file. This list is automatically loaded when the game starts.
         * DLLs not in this list cannot have their types found during resource deserialization
         * Therefore, the items removed in OnFilterAssemblies need to be added back
         */
     string[] jsonFiles = Directory.GetFiles(path, SettingsUtil.ScriptingAssembliesJsonFile, SearchOption.AllDirectories);

     if (jsonFiles.Length == 0)
     {
         //Debug.LogError($"can not find file {SettingsUtil.ScriptingAssembliesJsonFile}");
         return;
     }

     foreach (string file in jsonFiles)
     {
         var patcher = new ScriptingAssembliesJsonPatcher();
         patcher.Load(file);
         patcher.AddScriptingAssemblies(SettingsUtil.HotUpdateAssemblyFilesIncludePreserved);
         patcher.Save(file);
     }
}
```
If you do not put it in json, ``MissingReference`` will appear when the hot update resource is deserialized.

## Ordinary C# class Reference?

Just use Activator.CreateInstance to instantiate it directly in HybridCLR