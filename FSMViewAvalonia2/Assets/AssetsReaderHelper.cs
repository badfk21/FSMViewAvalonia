using FSMViewAvalonia2.Context;

namespace FSMViewAvalonia2.Assets;
internal static class AssetsReaderHelper
{
    public static AssetTypeTemplateField GetField(this AssetTypeTemplateField field, string name)
    {
        return field.Children.First(x => x.Name == name);
    }
    public static AssetTypeTemplateField RemoveFieldsAfter(this AssetTypeTemplateField field,
        string name, bool includeLatest = false)
    {
        AssetTypeTemplateField l = includeLatest ? field.GetField(name) : null;
        field.Children = field.Children.TakeWhile(x => x.Name != name).ToList();
        if (l != null)
        {
            field.Children.Add(l);
        }

        return field;
    }
    public static AssetTypeTemplateField GetTypeTemplateFieldFromAsset(
        this AssetsFileInstance assetsFileInstance,
        GameContext ctx,
        AssetFileInfo info,
        string assemblyName, string nameSpace, string typeName,
        List<AssetTypeTemplateField> parent = null)
    {
        AssetTypeTemplateField result = new()
        {
            Children = (parent?.ToList()) ?? []
        };
        AssetsFile file = assetsFileInstance.file;
        if (file.Metadata.TypeTreeEnabled)
        {
            result.FromTypeTree(file.Metadata.FindTypeTreeTypeByID(info.GetTypeId(file), file.GetScriptIndex(info)));
        }
        else
        {
            result = ctx.assemblyProvider.mono.GetTemplateField(result, assemblyName, nameSpace,
                                                               typeName, new(file.Metadata.UnityVersion));
        }

        return result;
    }
}
