namespace Squirrel.Data_Visualization.ChartJS;

public class CharJSLineChartDataset
{
    // label: 'Revenue',
    // data: [12, 19, 3, 5, 2, 3],
    // fill: false,
    // borderColor: 'rgb(75, 192, 192)',
    // tension: 0.1
    //     
    //     
    
    private string Label { get; set; }
    private List<int> Data { get; set; }
    private bool Fill { get; set; }
    private string BorderColor { get; set; }
    private double Tension { get; set; }

    public override string ToString()
    {
        return $"{{ label: '{Label}', data: [{string.Join(", ", Data)}], fill: {Fill.ToString().ToLower()}, borderColor: '{BorderColor}', tension: {Tension} }}";
    }
}