using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using System.Runtime.Serialization.Formatters.Binary;


public class Saving : MonoBehaviour
{
    public GameObject to_save;
    public static GameObject SavedGameObject;
    public static string path = "./Assets/SaveData/";

    // Saved Agent
    public string save_name;
    public string load_name;
    public Dictionary<string, string> agents;
    public List<string> names = new List<string>();

    public void Start()
    {
        // Set the agent list
        agents = new Dictionary<string, string>();
        string name;
        foreach (string p in Directory.GetDirectories(path))
        {
            name = p.Split('/')[p.Split('/').Length - 1];
            agents.Add(name, p);
            names.Add(name);
        }
    }

    /// <summary>
    /// Save a genome to a file
    /// </summary>
    /// <param name="g"></param>
    public static void SaveGenome(Genome g, string save_path)
    {
        g.SaveGenome(save_path);
    }

    /// <summary>
    /// Loads saved genome from path
    /// </summary>
    /// <param name="load_path"></param>
    /// <returns></returns>
    public static Genome LoadGenome(string load_path)
    {
        return Genome.LoadFrom(load_path);
    }

    /// <summary>
    /// Saves an agents genes object
    /// </summary>
    /// <param name="g"></param>
    /// <param name="save_path"></param>
    public static void SaveGenes(Genes g, string save_path)
    {
        g.SaveGenes(save_path);
    }

    /// <summary>
    /// Loads saved genes from path
    /// </summary>
    /// <param name="load_path"></param>
    /// <returns></returns>
    public static Genes LoadGenes(string load_path)
    {
        return Genes.LoadFrom(load_path);
    }

    public static void SaveAgent(BaseAgent agent, string save_path)
    {
        // Create dir for the agent
        Directory.CreateDirectory(save_path);

        agent.genes.SaveGenes(save_path + "/genes.txt");
        ((EvolutionaryNEATLearner)(agent.brain)).genome.SaveGenome(save_path + "/genome.txt");
    }

    /// <summary>
    /// Loads an agent in entirety. Transforms 'agent' into the saved agent found at load path and resets it to age 0
    /// </summary>
    /// <param name="agent"></param>
    /// <param name="load_path"></param>
    public static void LoadAgent(BaseAgent agent, string load_path)
    {
        Genes genes = LoadGenes(load_path + "/genes.txt");
        Genome genome = LoadGenome(load_path + "/genome.txt");
        agent.Setup((int)ID.Wobbit, genes);
        agent.manager.anc_manager.UpdatePopulation(agent);
        ((EvolutionaryNEATLearner)(agent.brain)).SetModel(genome);
    }

    /// <summary>
    /// Replaces an agents brain with the one saved at the specified path
    /// </summary>
    /// <param name="agent"></param>
    /// <param name="load_path"></param>
    public static void LoadAgentBrain(BaseAgent agent, string load_path)
    {
        Genome genome = LoadGenome(load_path + "/genome.txt");
        ((EvolutionaryNEATLearner)(agent.brain)).SetModel(genome);
    }

    public static BaseAgent LoadAgent(string load_path)
    {
        return null;
    }

    public static bool Save(GameObject obj)
    {
        PlayerPrefs.Save();
        return true;
    }


    public static object Load()
    {
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);


            string data = formatter.Deserialize(stream) as string;
            SavedGameObject = (GameObject)AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));

            stream.Close();
        }
        else
        {
            Debug.LogWarning("Save file not found.");
            return null;
        }
        return null;
    }
}


