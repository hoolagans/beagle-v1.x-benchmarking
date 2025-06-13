using System.Collections.Generic;
using Newtonsoft.Json;
using Supermodel.Presentation.WebMonk.Bootstrap4.D3.Models.Base;
using WebMonk.RazorSharp.HtmlTags;
using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace Supermodel.Presentation.WebMonk.Bootstrap4.D3.Models;

public static partial class D3
{
    public class DonutChartMvcModel : BrightChartsD3MvcModelBase
    {
        #region Embedded Types
        public class Datum(long id, string name, double quantity)
        {
            #region Properties
            [JsonProperty("id")] public long Id { get; } = id;
            [JsonProperty("name")] public string Name { get; } = name;
            [JsonProperty("quantity")] public double Quantity { get; } = quantity;

            #endregion
        }
        #endregion
            
        #region Overrides
        public override IGenerateHtml GenerateD3Script(string containerId)
        {
            var script = $@"
                    $(function() {{
                        {containerId}_Donut();
                    }});
                    function {containerId}_Donut()
                    {{                            
                        const data = { JsonConvert.SerializeObject(Data) };

                        let chart = britecharts.donut();
                        chart
                            {SetColorWidthHeightIsAnimated()};
                        d3.select(""#{containerId}"").datum(data).call(chart);
                        
                        {ShowLegendIfApplicable(containerId)}
                    }};";

            return new Script {new Txt(script)};
        }
        public override bool ContainsData()
        {
            return Data.Count > 0;
        }
        #endregion

        #region Methods
        protected virtual string ShowLegendIfApplicable(string containerId)
        {
            if (!ShowLegend) return "";
            return $@"                            
                    let legend = britecharts.legend();
                    {SetHorizontalLegend()}

                    d3.select(""#{containerId}"").datum(data).call(legend);
                    chart
                        .on('customMouseOver', function(data) {{
                            legend.highlight(data.data.id);
                        }})
                        .on('customMouseOut', function() {{
                            legend.clearHighlight();
                        }});";
        }
        protected virtual string SetHorizontalLegend()
        {
            if (!IsHorizontalLegend) return "";
            return $@"
                    legend
                        .isHorizontal(true)
                        .markerSize(8)
                        .height(40);";
        }
        #endregion

        #region Properties
        public List<Datum> Data { get; } = new();
        public bool ShowLegend { get; set; } = true;
        public bool IsHorizontalLegend { get; set; }
        #endregion
    }
}