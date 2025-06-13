using System.Collections.Generic;
using Newtonsoft.Json;
using Supermodel.Presentation.Mvc.Bootstrap4.D3.Models.Base;

namespace Supermodel.Presentation.Mvc.Bootstrap4.D3.Models;

public static partial class D3
{
    public class BarChartMvcModel : BrightChartsD3MvcModelBase
    {
        #region Embedded Types
        public class Datum
        {
            #region Constructors
            public Datum(string name, double value)
            {
                Name = name;
                Value = value;
            }
            #endregion
        
            #region Properties
            [JsonProperty("name")] public string Name { get; }
            [JsonProperty("value")] public double Value { get; }
            #endregion
        }
        #endregion
            
        #region Overrides
        public override string GenerateD3Script(string containerId)
        {
            return $@"
                    <script>
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
                        }};
                    </script>";
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