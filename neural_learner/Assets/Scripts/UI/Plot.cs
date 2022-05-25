using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[System.Serializable]
public struct PlotPoint{
    public RectTransform t;
    public Vector2 target_location;
    }

[System.Serializable]
public struct ScatterPlot{
    public List<float> values;
    public List<PlotPoint> points;
    public Color color;
}

public class Plot : MonoBehaviour
{
    /// <summary>
    /// Our list of values to plot
    /// </summary>
    [SerializeField]
    private List<ScatterPlot> plots;

    /// <summary>
    /// The size of our scatter points
    /// </summary>
    public Vector2 point_size;

    /// <summary>
    /// The backdrop of our scatter plot. Also determines the boundaries of the plot.
    /// </summary>
    public Image background;

    /// <summary>
    /// Prefab used as a scatter plot point
    /// </summary>
    public GameObject plot_point_prefab;

    public bool test;

    /// <summary>
    /// Min value for our plot
    /// </summary>
    float min = float.MaxValue;

    /// <summary>
    /// Max value for our plot
    /// </summary>
    float max = float.MinValue;

    /// <summary>
    /// Add a plot to our plotter
    /// </summary>
    /// <param name="plot"></param>
    public void AddPlot(List<float> plot, Color c){
        ScatterPlot s = new ScatterPlot();
        s.values = plot;
        s.points = new List<PlotPoint>();
        s.color = c;
        plots.Add(s);

        //Find min and max
        foreach(float v in plot){
            if(v < min){
                min = v;
            }
            if(v > max){
                max = v;
            }
        }
    }

    void OnEnable(){
        StartCoroutine(Setup());
    }

    void OnDisable()
    {
        StopAllCoroutines();

        // Check if we have incomplete creation of points
        if(plots[0].values.Count != background.transform.childCount){
            foreach(Transform t in background.transform){
                Destroy(t.gameObject);
            }
            plots.Clear();
        }

        print(plots[0].values.Count);
        print( background.transform.childCount);

        // Create our plots
        foreach(ScatterPlot sp in plots){
            // Create our points
            float step_size = background.rectTransform.rect.width / ((float)sp.values.Count - 1);
            foreach(PlotPoint pp in sp.points){
                pp.t.localPosition = new Vector2(-background.rectTransform.rect.width/2, - background.rectTransform.rect.height/2f);
            }
        }
    }

    IEnumerator Setup(){

        if(test && plots.Count == 0){
            for(int i = 0; i < 5; i++){
                List<float> l = new List<float>();
                for(int x = 0; x < 100; x++){
                    l.Add((Mathf.PerlinNoise(i/2f, x/10f)));
                }
                AddPlot(l, UnityEngine.Random.ColorHSV());
            }
        }

        if(background.transform.childCount == 0){
            // Create our plots
            foreach(ScatterPlot sp in plots){
                // Create our points
                int x = 0;
                float step_size = background.rectTransform.rect.width / ((float)sp.values.Count - 1);
                foreach(float f in sp.values){
                    PlotPoint p = new PlotPoint();
                    p.t = Instantiate(plot_point_prefab, transform).GetComponent<RectTransform>();
                    p.t.localScale = point_size;
                    p.t.GetComponent<Image>().color = sp.color;
                    p.t.parent = background.transform;
                    p.t.localPosition = new Vector2(-background.rectTransform.rect.width/2, - background.rectTransform.rect.height/2f);
                    p.target_location = new Vector2(step_size * x - background.rectTransform.rect.width/2, background.rectTransform.rect.height * f/max - background.rectTransform.rect.height/2);
                    sp.points.Add(p);
                    x++;

                    if(x % 10 == 0){
                        yield return null;
                    }
                }
                yield return null;
            }
        }else{
            int plot_num = 0;
            foreach(ScatterPlot sp in plots){
                // Create our points
                int x = 0;
                float step_size = background.rectTransform.rect.width / ((float)sp.values.Count - 1);
                foreach(float f in sp.values){
                    PlotPoint p = new PlotPoint();
                    p.t = background.transform.GetChild(x + plot_num * (sp.values.Count-1)).GetComponent<RectTransform>();
                    p.t.localScale = point_size;
                    // p.t.GetComponent<Image>().color = sp.color;
                    // p.t.parent = background.transform;
                    p.t.localPosition = new Vector2(-background.rectTransform.rect.width/2, - background.rectTransform.rect.height/2f);
                    p.target_location = new Vector2(step_size * x - background.rectTransform.rect.width/2, background.rectTransform.rect.height * f/max - background.rectTransform.rect.height/2);
                    // sp.points.Add(p);
                    x++;

                    if(x % 10 == 0){
                        yield return null;
                    }
                }
                yield return null;
                plot_num++;
            }
        }

        // For each of our plots...
        bool finished = false;
        while (!finished){
            finished = true;
            foreach(ScatterPlot sp in plots){
                // Move each point to their desired position
                int x = 0;
                foreach(PlotPoint pp in sp.points){
                    float amount = Time.deltaTime/Mathf.Max(1, (5 * (sp.points.Count - x)/(float)(sp.points.Count)));
                    pp.t.localPosition = Vector2.Lerp(pp.t.localPosition, pp.target_location, amount);
                    if(!(((Vector2)pp.t.localPosition - pp.target_location).magnitude < 0.01f)){
                        finished = false;
                    }
                    x++;
                }
            }
            yield return null;
        }
    }
}
