using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class BoardUtility
{
    public static Board LoadFromJson(string path)
    {
        var boardData = File.ReadAllText(path);
        return JsonUtility.FromJson<Board>(boardData);
    }
}
