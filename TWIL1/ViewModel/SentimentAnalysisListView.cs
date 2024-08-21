namespace TWIL1.ViewModel
{
    public class SentimentAnalysisListView
    {
        public IEnumerable<SentimentResult> Results { get; set; }
        public IEnumerable<object> PieChartData { get; set; }
        public IEnumerable<object> HistogramData { get; set; }
        public string error { get; set; } = string.Empty;
    }
}
