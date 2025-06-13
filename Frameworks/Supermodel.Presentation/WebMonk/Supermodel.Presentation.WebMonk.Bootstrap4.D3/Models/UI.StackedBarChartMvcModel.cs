using System.Collections.Generic;
using Newtonsoft.Json;
using Supermodel.Presentation.WebMonk.Bootstrap4.D3.Models.Base;
using WebMonk.RazorSharp.HtmlTags;
using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace Supermodel.Presentation.WebMonk.Bootstrap4.D3.Models;

public static partial class D3
{
    public class StackedBarChartMvcModel : BrightChartsD3MvcModelBase
    {
        #region Embedded Types
        public class Datum(string name, string stack, double value)
        {
            #region Properties
            [JsonProperty("name")] public string Name { get; } = name;
            [JsonProperty("stack")] public string Stack { get; } = stack;
            [JsonProperty("value")] public double Value { get; } = value;

            #endregion
        }
        #endregion
            
        #region Overrides
        public override IGenerateHtml GenerateD3Script(string containerId)
        {
            var script = $@"
                    $(function() {{
                        {containerId}_StackedBar();
                    }});
                    function {containerId}_StackedBar()
                    {{                            
                        const data = { JsonConvert.SerializeObject(Data) };

                        let chart = britecharts.stackedBar();
                        chart
                            {SetColorWidthHeightIsAnimated()}
                            .grid(""{Grid.ToString().ToLower()}"")
                            .isHorizontal({IsHorizontal.ToString().ToLower()})
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
                    const legendData = data.map((val, index) => ( {{ id:val.id, name:val.stack }}) );
                    const uniqueLegendData = legendData.filter((x, index, self) => index === self.findIndex((t) => (t.name === x.name)));
                    let legend = britecharts.legend();
                    {SetHorizontalLegend()}
                    d3.select(""#{containerId}"").datum(uniqueLegendData).call(legend);";
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
        public bool IsHorizontal { get; set; }
        public GridEnum Grid { get; set; } = GridEnum.Horizontal;
        public bool ShowLegend { get; set; } = true;
        public bool IsHorizontalLegend { get; set; }
        #endregion
    }
}