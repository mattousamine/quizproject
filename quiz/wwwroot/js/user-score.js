document.addEventListener('DOMContentLoaded', function () {
    fetch('/Quiz/GetUserQuizScores')
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {


            // Parse the data to populate the chart
            var quizNames = data.map(q => q.quizName); // Ensure the property names are in lowercase
            var quizScores = data.map(q => q.score);   // Ensure the property names are in lowercase



            var ctx = document.getElementById("myBarChart");
            var myBarChart = new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: quizNames,
                    datasets: [{
                        label: "Score",
                        backgroundColor: "rgba(2,117,216,1)",
                        borderColor: "rgba(2,117,216,1)",
                        data: quizScores,
                    }],
                },
                options: {
                    scales: {
                        xAxes: [{
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
                                max: 100, // Assuming scores are out of 100
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
        })
        .catch(error => {
            console.error('Error fetching user quiz scores:', error);
        });
});
