using System;
using Microsoft.SPOT;

namespace Pachube.EmbeddableGraphGenerator
{
    public class EmbeddableHTML
    {
        /// <summary>
        /// Generates the pachube embeddable graph. 
        /// </summary>
        /// <seealso cref="http://pachube.github.com/pachube_graph_library/"/>
        /// <param name="alContent">The Embeddable HTML content.</param>
        public static void GenerateHTML(System.Collections.ArrayList alContent)
        {
            string update = GraphFunctionality.AutoUpdate.ToString().ToLower();
            string rolling = GraphFunctionality.Rolling.ToString().ToLower();
            try
            {
                alContent.Clear();
                foreach (string ID in PachubeData.TOTAL_DATASTREAM_ID)
                {
                    string EmbeddableHTML =
                        "<div id=\"graph\" class=\"pachube-graph\" pachube-resource=\"feeds/" + AlarmByZones.Alarm.UserData.Pachube.feedId + "/datastreams/" + ID +
                        "\" pachube-key=\"" +
                        AlarmByZones.Alarm.UserData.Pachube.apiKey + "\" pachube-options=\"timespan:" + 
                        Pachube.EmbeddableGraphGenerator.GraphFunctionality.DEFAULT_TIMESPAN[0] +
                        ";update:"+update+";rolling:"+rolling+";background-color:#FFFFFF;line-color:#FF0066;grid-color:#EFEFEF;" +
                        "border-color:#9D9D9D;text-color:#555555;\" style=\"width:" + 
                        Pachube.EmbeddableGraphGenerator.GraphAppearance.WIDTH + 
                        ";height:" + Pachube.EmbeddableGraphGenerator.GraphAppearance.HEIGHT +
                        ";background:#F0F0F0;\">Graph: Feed " + AlarmByZones.Alarm.UserData.Pachube.feedId + 
                        ", Datastream 1</div><script type=\"text/javascript\" " +
                        "src=\"http://beta.apps.pachube.com/embeddable_graphs/lib/PachubeLoader.js\"></script>";

                    alContent.Add(EmbeddableHTML);
                    alContent.Add("<br/>");
                }
                alContent.Add("<br/>");
                alContent.Add("Reference material from: <a href=\"http://pachube.github.com/pachube_graph_library/ \">Pachube graphic library.</a>" + "");
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
            }
        }
    }
}
