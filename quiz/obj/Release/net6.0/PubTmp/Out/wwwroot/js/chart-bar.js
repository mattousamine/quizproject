// Set new default font family and font color to mimic Bootstrap's default styling
Chart.defaults.global.defaultFontFamily = '-apple-system,system-ui,BlinkMacSystemFont,"Segoe UI",Roboto,"Helvetica Neue",Arial,sans-serif';
Chart.defaults.global.defaultFontColor = '#292b2c';

// Function to update the bar chart with fetched data
function updateBarChart(data) {
    var ctx = document.getElementById("myBarChart");
    var myLineChart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: data.labels, // Use dynamic labels from fetched data
            datasets: [{
                label: "Count",
                backgroundColor: "rgba(2,117,216,1)",
                borderColor: "rgba(2,117,216,1)",
                data: data.counts, // Use dynamic data from fetched data
            }],
        },
        options: {
            scales: {
                xAxes: [{
                    time: {
                        unit: 'category'
                    },
                    gridLines: {
                        display: false
                    },
                    ticks: {
                        maxTicksLimit: 6
                    }
                }],
                yAxes: [{
                    ticks: {
                        min: 0,
                        // You might want to dynamically adjust the max value based on your data
                        maxTicksLimit: 5
                    },
                    gridLines: {
                        display: true
                    }
                }],
            },
            legend: {
                display: false
            }
        }
    });
}

// Fetch stats data and update the bar chart
fetch('/Admin/GetBarChartStats')
    .then(response => response.json())
    .then(data => updateBarChart({
        labels: ["Category", "Quiz", "Question", "Option"], 
        counts: [data.CategoryCount, data.QuizCount, data.QuestionCount, data.OptionCount] 
    }))
    .catch(error => console.error('Unable to get bar chart stats.', error));
