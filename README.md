# MonoReference


***Read this document in English: [English Document](./README_EN.md)***

使用Reference方式序列化MonoBehaviour，资源加载时不再依赖Unity ResourceManager，而是由另一个Proxy脚本在加载后添加

## 使用场景

配合[HybridCLR](https://github.com/focus-creative-games/hybridclr)使用，为热更新资源添加新的MonoBehaviour，Assembly无需提前添加进``ScriptAssembly.json``，从而可以实现Mod功能

可参考 https://hybridclr.doc.code-philosophy.com/docs/basic/monobehaviour ,作者的注释如下

```C#
private void AddHotFixAssembliesToScriptingAssembliesJson(string path)
{
    Debug.Log($"[PatchScriptingAssemblyList]. path:{path}");
    /*
        * ScriptingAssemblies.json 文件中记录了所有的dll名称，此列表在游戏启动时自动加载，
        * 不在此列表中的dll在资源反序列化时无法被找到其类型
        * 因此 OnFilterAssemblies 中移除的条目需要再加回来
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
如不放入json中则会在热更资源反序列时出现 ``MissingReference``

## 普通C#类Reference？

在HybridCLR中直接使用Activator.CreateInstance实例化即可