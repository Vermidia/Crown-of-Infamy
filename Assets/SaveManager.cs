using System.Collections.Generic;
using System.IO;
using UnityEditor.AnimatedValues;
using UnityEditor.Rendering;
using UnityEngine;

public static class SaveManager
{
    public static string baseName = "save";

    public static string ending = ".txt";

    private static char token = '=';

    public static string folder = "save";

    private static Dictionary<string, string> dataDict;

    private static string directory;

    static SaveManager()
    {
        directory = Directory.GetParent(Application.dataPath).FullName;
        directory = Path.Combine(directory, folder);
        if(!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
        dataDict = new();
    }

    public static void SaveString(string key, string value)
    {
        dataDict[key] = value;
    }

    public static string LoadString(string key)
    {
        if(!dataDict.ContainsKey(key))
            return "";
        return dataDict[key];
    }

    public static void SaveInt(string key, int value)
    {
        dataDict[key] = value.ToString();
    }

    public static int LoadInt(string key)
    {
        if(!dataDict.ContainsKey(key))
            return 0;

        if(int.TryParse(dataDict[key], out var result))
            return result;

        return 0;
    }

    public static void SaveDouble(string key, double value)
    {
        dataDict[key] = value.ToString();
    }

    public static double LoadDouble(string key)
    {
        if(!dataDict.ContainsKey(key))
            return 0;

        if(double.TryParse(dataDict[key], out var result))
            return result;

        return 0;
    }

    public static void SaveToFile(int slot)
    {
        var name = baseName + slot + ending;
        var filePath = Path.Combine(directory, name);

        using(StreamWriter writer = new StreamWriter(filePath))
        {
            foreach(var pair in dataDict)
            {
                writer.WriteLine($"{pair.Key}{token}{pair.Value}");
            }
        }
    }

    public static void LoadFromFile(int slot)
    {
        var name = baseName + slot + ending;
        var filePath = Path.Combine(directory, name);
        try
        {
            if(File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                foreach(string line in lines)
                {
                    string[] parts = line.Split(token);
                    if(parts.Length == 2)
                        dataDict[parts[0]] = parts[1];
                }
            }
        }
        catch
        {
            Debug.LogError("No such save exists!");
        }
    }

    public static void DeleteFile(int slot)
    {
        var name = baseName + slot + ending;
        var filePath = Path.Combine(directory, name);
        try
        {
            if(File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        catch
        {
            Debug.LogError("No such save exists!");
        }
    }

    public static bool CheckIfSlotExists(int slot)
    {
        var name = baseName + slot + ending;
        var filePath = Path.Combine(directory, name);

        return File.Exists(filePath);
    }
/// <summary>
/// Flushes the dataDict of data - important because we are storing a list!!
/// </summary>
    public static void Flush()
    {
        dataDict = new();
    }
}