using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameGenerator
{
    public string genus { get; }
    public string species { get; }

    public static string GenerateFullName()
    {
        // Generate a random name in the form: Genus Species
        string[] lines = System.IO.File.ReadAllLines(@"Assets/Scripts/Agent/names.txt");
        string gline = lines[Random.Range(0, lines.Length)].Split(',')[Random.Range(0, 2)];
        string sline = lines[Random.Range(0, lines.Length)].Split(',')[Random.Range(0, 2)]; ;

        return gline + " " + sline;
    }
}
