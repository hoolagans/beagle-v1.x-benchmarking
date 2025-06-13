using System.Collections.Generic;
using Newtonsoft.Json;
using Supermodel.Presentation.WebMonk.Bootstrap4.D3.Models.Base;
using WebMonk.RazorSharp.HtmlTags;
using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace Supermodel.Presentation.WebMonk.Bootstrap4.D3.Models;

public static partial class D3
{
    public class BarChartMvcModel : BrightChartsD3MvcModelBase
    {
        #region Embedded Types
        public class Datum(string name, double value)
        {
            #region Properties
            [JsonProperty("name")] public string Name { get; } = name;
            [JsonProperty("value")] public double Value { get; } = value;

            #endregion
        }
        #endregion
            
        #region Overrides
        public override IGenerateHtml GenerateD3Script(string containerId)
        {
            var script = $@"
                    $(function() {{
                        {containerId}_Bar();
                    }});
                    function {containerId}_Bar()
                    {{                            
                        const data = { JsonConvert.SerializeObject(Data) };

                        let chart = britecharts.bar();
                        chart
                            {SetColorWidthHeightIsAnimated()}
                            .enableLabels(true)
                            {SetLabelsNumberFormat()}
                            .isHorizontal({IsHorizontal.ToString().ToLower()})
                        d3.select(""#{containerId}"").datum(data).call(chart);
                    }};";

            return new Script {new Txt(script)};
        }
        public override bool ContainsData()
        {
            return Data.Count > 0;
        }
        #endregion

        #region Methods
        protected virtual string SetLabelsNumberFormat()
        {
            if (string.IsNullOrEmpty(LabelsNumberFormat)) return "";

            return $".labelsNumberFormat('{LabelsNumberFormat}')";
        }
        #endregion

        #region Properties
        public List<Datum> Data { get; } = new();
        public bool IsHorizontal { get; set; }
        public string LabelsNumberFormat { get; set; } = ""; //https://github.com/d3/d3-format/blob/master/README.md
        #endregion
    }
}