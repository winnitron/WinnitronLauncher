// C# example.
using System.Diagnostics;
using UnityEditor.Callbacks;
using UnityEditor;
using System.IO;
using UnityEngine;

public class PostBuildProcessor
{
    [PostProcessBuildAttribute(1)]

    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        pathToBuiltProject = pathToBuiltProject.Replace("/WINNITRON.exe", "");

        // Copy a file from the project folder to the build folder, alongside the built game.
        FileUtil.CopyFileOrDirectory("Assets/Options", pathToBuiltProject + "/WINNITRON_data/Options");

        //Delete META files
        var dir = new DirectoryInfo(pathToBuiltProject);

        foreach (var file in dir.GetFiles("*.meta"))
        {
            UnityEngine.Debug.Log("POST: deleting file " + file.Name);
            file.Delete();
        }
    }
}
