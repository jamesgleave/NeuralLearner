using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneBarChart : MonoBehaviour
{
    // Settings
    public int num_bars = 10;
    public float width = 10;
    public float separation = 1;
    public float length_scaler = 1;
    public float min_length = 25f;
    public float max_length = 250f;
    public float value_padding = 1;
    public float min_value_padding = 1;
    public float title_padding = 1;
    public bool update = false;

    // The bar prefab
    public GameObject bar;

    // What we use to keep track of everything
    List<Bar> bars = new List<Bar>();
    List<Text> bars_labels = new List<Text>();
    List<Text> bars_values = new List<Text>();
    public List<GameObject> all_objects;

    // Things to help me
    private Bar largest_bar;

    public void Normalize(List<Text> vals)
    {

    }


    public void Setup(string[] names, List<float> values)
    {
        // Get the number of bars
        num_bars = names.Length;

        // Turn on the system!
        update = true;

        // Create each bar
        for (int i = 0; i < num_bars; i++)
        {
            if (num_bars != all_objects.Count)
            {
                // Instantiate new bar object
                var instance = Instantiate(bar, transform);

                // Get the bar object
                Bar b = instance.GetComponent<Bar>();

                // Set the position of the bar
                var value = values[i];
                b.bar.position = transform.position + Vector3.up * -separation * i;

                // Add the bar to the list
                bars.Add(b);

                // Update the game object list
                all_objects.Add(b.gameObject);

                // Pad the values so we get an equal size across all attributes
                string formatted = value.ToString().PadRight(6, '0');
                b.value.text = formatted.Substring(0, 6);
                b.title.text = names[i];

                // For now create a random value
                bars_values.Add(b.value);
                bars_labels.Add(b.title);

                // Set the bar size
                float bar_size = Mathf.Min(max_length, length_scaler * float.Parse(bars_values[i].text) + min_length);
                b.SetBarSize(bar_size, width);

                // Set up the position of the title and value text
                b.title.rectTransform.position = b.bar.position + Vector3.up * width / title_padding;
                b.value.rectTransform.localPosition = b.bar.localPosition + new Vector3(bar_size / 2 + value_padding, 0, 0);

                // Update and track the largest bar
                if (largest_bar == null || largest_bar.GetBarLen() < b.GetBarLen())
                {
                    largest_bar = b;
                }
            }
            else
            {
                Bar b = bars[i];

                // Set the position of the bar
                var value = values[i];
                b.bar.position = transform.position + Vector3.up * -separation * i;
                // Set the bar size
                float bar_size = Mathf.Min(max_length, length_scaler * float.Parse(bars_values[i].text) + min_length);
                b.SetBarSize(bar_size, width);

                // Set up the position of the title and value text
                float barsize = Mathf.Max(50, b.GetBarLen());
                b.title.rectTransform.position = b.bar.position + Vector3.up * width / title_padding;
                b.value.rectTransform.localPosition = b.bar.localPosition + new Vector3(barsize / 2 + value_padding, 0, 0);

                // Pad the values so we get an equal size across all attributes
                string formatted = value.ToString().PadRight(6, '0');
                b.value.text = formatted.Substring(0, 6);
                b.title.text = names[i];

                // Update and track the largest bar
                if (largest_bar == null || largest_bar.GetBarLen() < b.GetBarLen())
                {
                    largest_bar = b;
                }
            }
        }
    }

    public void Update()
    {
        if (update)
        {
            for (int i = 0; i < num_bars; i++)
            {
                // Update positions
                RectTransform b = bars[i].bar;
                b.position = transform.position + Vector3.up * -separation * i;

                // Set up the position of the title and value text
                float barsize = Mathf.Max(min_value_padding, b.rect.width);
                bars_labels[i].rectTransform.position = b.position + Vector3.up * width / title_padding;
                bars_values[i].rectTransform.localPosition = b.localPosition + new Vector3(barsize / 2 + value_padding, 0, 0);

                float bar_size = Mathf.Min(max_length, length_scaler * float.Parse(bars_values[i].text) + min_length);
                bars[i].SetBarSize(bar_size, width);

                // Update and track the largest bar
                if (largest_bar == null || largest_bar.GetBarLen() < bars[i].GetBarLen())
                {
                    largest_bar = bars[i];
                }
            }
        }
    }
}
