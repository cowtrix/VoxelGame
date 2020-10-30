#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

public class Texture2DArrayGeneratorEditor : ScriptableWizard
{
    #region Filed

    public string fileName = "tex2darray.asset";

    public TextureFormat format = TextureFormat.ARGB32;

    public Texture2D[] textures;

    #endregion Field

    #region Method

    [MenuItem("Custom/Texture2DArrayGenerator")]
    static void Init()
    {
        DisplayWizard<Texture2DArrayGeneratorEditor>("Create Texture Array", "Create");
    }

    void OnWizardCreate()
    {
        string path = AssetCreationHelper.CreateAssetInCurrentDirectory
                          (Texture2DArrayGenerator.Generate(this.textures, this.format), this.fileName);
    }

    #endregion Method
}

#endif // UNITY_EDITOR