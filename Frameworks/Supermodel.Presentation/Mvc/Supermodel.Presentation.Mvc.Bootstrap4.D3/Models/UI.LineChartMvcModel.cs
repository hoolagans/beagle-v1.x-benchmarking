using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using Supermodel.Presentation.Mvc.Bootstrap4.D3.Models.Base;
using Supermodel.ReflectionMapper;

namespace Supermodel.Presentation.Mvc.Bootstrap4.D3.Models;

public static partial class D3
{
    public class LineChartMvcModel : BrightChartsD3MvcModelBase
    {
        #region Embedded Types
        public enum LineCurveEnum
        {
            [Description("linear")] Linear, 
            [Description("natural")] Natural, 
            [Description("monotoneX")] MonotoneX, 
            [Description("monotoneY")] MonotoneY, 
            [Description("step")] Step, 
            [Description("stepAfter")] StepAfter, 
            [Description("stepBefore")] StepBefore, 
            [Description("cardinal")] Cardinal, 
            [Description("catmullRom")] CatmullRom
        }
        public enum XAxisTimeCombinationsEnum
        { 
            [Description("MINUTE_HOUR")] MinuteHour, 
            [Description("HOUR_DAY")] HourDay, 
            [Description("DAY_MONTH")] DayMonth, 
            [Description("MONTH_YEAR")] MonthYear,
            [Description("CUSTOM")] Custom,
        }
        public class Datum
        {
            #region Constructors
            public Datum(string topicName, string name, DateTime date, double value)
            {
                TopicName = topicName;
                Name = name;
                Date = date;
                Value = value;
            }
            #endregion
        
            #region Properties
            [JsonProperty("topicName")] public string TopicName { get; }
            [JsonProperty("name")] public string Name { get; }
            [JsonProperty("date")] public DateTime Date { get; }
            [JsonProperty("value")] public double Value { get; }
            #endregion
        }
        private class FullDatum
        {
            public FullDatum(List<Datum> data) { Data = data; }
            // ReSharper disable once MemberCanBePrivate.Local
            [JsonProperty("data")] public List<Datum> Data { get; }
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
                            const data = { JsonConvert.SerializeObject(new FullDatum(Data)) };

                            let chart = britecharts.line();
                            chart
                                {SetColorWidthHeightIsAnimated()}
                                .grid(""{Grid.ToString().ToLower()}"")
                                .lineCurve(""{LineCurve.GetDescription()}"")
                                {SetAxisTimeCombinations()};                                
                            d3.select(""#{containerId}"").datum(data).call(chart);

                            {ShowLegendIfApplicable(containerId)}
                        }};
                    </script>";
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
                    const legendData = data.data.map((val, index) => ( {{ id:val.topic, name:val.topicName }}) );
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
        protected string SetAxisTimeCombinations()
        {
            if (XAxisFormat != XAxisTimeCombinationsEnum.Custom)
            {
                if (!string.IsNullOrEmpty(XAxisCustomFormat)) throw new ArgumentException($"XAxisCustomFormat must be set to empty when XAxisFormat = {XAxisFormat}");
                return $".xAxisCustomFormat(chart.axisTimeCombinations.{XAxisFormat.GetDescription()})";
            }
            else
            {
                if (string.IsNullOrEmpty(XAxisCustomFormat)) throw new ArgumentException($"XAxisCustomFormat must be set to a value when XAxisFormat = {XAxisFormat}");
                return @$".xAxisFormat(chart.axisTimeCombinations.{XAxisFormat.GetDescription()})
                              .xAxisCustomFormat(""{XAxisCustomFormat}"")";
            }
        }
        #endregion

        #region Properties
        public List<Datum> Data { get; } = new();
        public GridEnum Grid { get; set; } = GridEnum.Horizontal;
        public XAxisTimeCombinationsEnum XAxisFormat { get; set; } = XAxisTimeCombinationsEnum.DayMonth;
        public string XAxisCustomFormat { get; set; } = ""; //https://github.com/d3/d3-time-format#locale_format
        public LineCurveEnum LineCurve { get; set; } = LineCurveEnum.Linear; //https://github.com/d3/d3-shape#curves
        public bool ShowLegend { get; set; } = true;
        public bool IsHorizontalLegend { get; set; }
        #endregion
    }
}