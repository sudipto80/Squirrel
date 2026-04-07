namespace Squirrel.Data_Visualization.ChartJS;

public class ChartJsStackedBarChart
{
    private string innerDataTemplate = @" {
                        label: '_DATASET_LABEL_',
                        data: [_DATASET_],
                        backgroundColor: '_rgbColor_',
                        borderWidth: 1
                    }";
    
    private string template = @"<!DOCTYPE html>
<html>
<head>
    <title>Stacked Bar Chart</title>
    <script src=""https://cdn.jsdelivr.net/npm/chart.js@4.4.0/dist/chart.umd.min.js""></script>
    <style>
        body {
            margin: 20px;
            display: flex;
            justify-content: center;
            align-items: center;
            min-height: 100vh;
        }
        .chart-container {
            width: 600px;
            height: 400px;
            max-width: 90%;
            max-height: 90vh;
        }
    </style>
</head>
<body>
    <div class=""chart-container"">
        <canvas id=""stackedBarChart""></canvas>
    </div>
    <script>
        const ctx = document.getElementById('stackedBarChart').getContext('2d');
        new Chart(ctx, {
            type: 'bar',
            data: {
                labels: [_LABELS_],
                datasets: [_DATASETS_]
            },
            options: {
                responsive: true,
                maintainAspectRatio: true,
                scales: {
                    x: {
                        stacked: true
                    },
                    y: {
                        stacked: true,
                        beginAtZero: true
                    }
                },
                plugins: {
                    legend: {
                        position: '_LEGEND_POSITION_'
                    },
                    title: {
                        display: true,
                        text: '_CHART_TITLE_'
                    }
                }
            }
        });
    </script>
</body>
</html>";
}