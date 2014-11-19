using UnityEditor;
using UnityEngine;

public class Force2DSound : AssetPostprocessor
{

    public void OnPreprocessAudio()
    {
        AudioImporter ai = assetImporter as AudioImporter;
        ai.threeD = false;
    }
}
