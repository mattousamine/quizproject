// Set new default font family and font color to mimic Bootstrap's default styling
Chart.defaults.global.defaultFontFamily = '-apple-system,system-ui,BlinkMacSystemFont,"Segoe UI",Roboto,"Helvetica Neue",Arial,sans-serif';
Chart.defaults.global.defaultFontColor = '#292b2c';

// Function to update the pie chart with fetched data
function updatePieChart(data) {
    var ctx = document.getElementById("myPieChart");
    var myPieChart = new Chart(ctx, {
        type: 'pie',
        data: {
            labels: ["Category", "Quiz", "Question", "Option"],
            datasets: [{
                data: [data.categoryCount, data.quizCount, data.questionCount, data.optionCount],
                backgroundColor: ['#007bff', '#dc3545', '#28a745', '#ffc107'],
            }],
        },
    });
}

// Fetch stats data and update the pie chart
fetch('/Admin/GetStats')
    .then(response => response.json())
    .then(data => updatePieChart(data))
    .catch(error => console.error('Unable to get stats.', error));

