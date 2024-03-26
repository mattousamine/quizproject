
let currentQuestion = 0;
let currentLevel = 'easy';
const questionNumElement = document.getElementById('questionNum');
const questionTextElement = document.getElementById('questionText');
const optionsContainer = document.getElementById('options-container');
const prevButton = document.getElementById('prev-btn');
const nextButton = document.getElementById('next-btn');
const levelSelector = document.getElementById('level-selector');
const resultDiv = document.getElementById('result');

let timerInterval;
let answerSelected = false;

var startDate = new Date('2024-03-20T17:10:00'); 
var endDate = new Date('2024-03-20T17:15:00'); 

function startTimer() {
    timerInterval = setInterval(updateTimer, 1000);
    updateTimer(); 
}

function updateTimer() {
    const currentDate = new Date();
    let timeDifferenceSeconds;

    let timerLabel = ''; 

    if (currentDate < startDate) {
        
        timeDifferenceSeconds = Math.floor((startDate.getTime() - currentDate.getTime()) / 1000);
        timerLabel = 'Starts in';

        const buttons = optionsContainer.querySelectorAll('div');
        buttons.forEach(div => {
            div.style.pointerEvents = 'none';
        });

    } else {
        timeDifferenceSeconds = Math.floor((endDate.getTime() - currentDate.getTime()) / 1000);
        if (currentDate <= endDate) {
            timerLabel = 'Time left';

            const buttons = optionsContainer.querySelectorAll('div');
            buttons.forEach(div => {
                div.style.pointerEvents = '';
            });
        } else {
            timerLabel = 'Timer';
        }
    }

    if (currentDate >= endDate && currentDate > startDate) {
        clearInterval(timerInterval);
        calculateAndDisplayScore();
        return;
    }

    const displayDays = Math.floor(timeDifferenceSeconds / (60 * 60 * 24));
    const displayHours = Math.floor((timeDifferenceSeconds % (60 * 60 * 24)) / (60 * 60));
    const displayMinutes = Math.floor((timeDifferenceSeconds % (60 * 60)) / 60);
    const displaySeconds = timeDifferenceSeconds % 60;

    let display = '';

    if (displayDays > 0) {
        display += `${displayDays}d `;
    }
    if (displayHours > 0 || displayDays > 0) {
        display += `${displayHours}h `;
    }
    display += `${displayMinutes.toString().padStart(2, '0')}:${displaySeconds.toString().padStart(2, '0')}`;

    const timerDisplay = document.getElementById('timer-display');
    const timerTitle = document.getElementById('Timer-Title');
    if (timerDisplay) {
        if (timeDifferenceSeconds <= 59) {
            timerDisplay.style.color = 'red';
        } else {
            timerDisplay.style.color = '';
        }

        timerDisplay.innerText = display;
        timerTitle.innerText = timerLabel; 
    }
}






function resetTimer() {
    startTimer(); 
}

function shuffleArray(array) {
    for (let i = array.length - 1; i > 0; i--) {
        const j = Math.floor(Math.random() * (i + 1));
        [array[i], array[j]] = [array[j], array[i]];
    }
    return array;
}

function showQuestion() {
    const currentQuizData = quizData[currentLevel][currentQuestion];
    questionNumElement.innerText = currentQuizData.questionNum;
    questionTextElement.innerText = currentQuizData.questionText;

    optionsContainer.innerHTML = '';

    const shuffledOptions = shuffleArray(currentQuizData.options);
    nextButton.disabled = true;
    for (const option of currentQuizData.options) {
        const optionDiv = document.createElement('div');
        optionDiv.className = 'answer-item';

        const span = document.createElement('span');
        span.innerText = option;

        const radioInput = document.createElement('input');
        radioInput.type = 'radio';
        radioInput.name = 'answer';
        optionDiv.appendChild(radioInput);
        optionDiv.appendChild(span);

        optionDiv.addEventListener('click', () => checkAnswer(option));

        optionsContainer.appendChild(optionDiv);
    }

   

    if (currentQuestion === quizData[currentLevel].length - 1) {
        nextButton.innerText = 'Terminer';
    } else {
        nextButton.innerText = 'Suivant';
    }

    resultDiv.innerHTML = '';

    // Affichage dynamique du contexte de la question
    const questionContext = document.getElementById('questionContext');
    questionContext.innerText = `Question ${currentQuestion + 1}/${quizData[currentLevel].length}`;

    // Generation dynamique des numeros de question
    const questionNumbersList = document.getElementById('questionNumbersList');
    questionNumbersList.innerHTML = '';
    for (let i = 0; i < quizData[currentLevel].length; i++) {
        const listItem = document.createElement('li');
        const questionNumberLink = document.createElement('a');
        questionNumberLink.href = '#';
        questionNumberLink.innerText = i + 1;

        // Verifier si la question a ete repondu
        if (i < currentQuestion) {
            const currentQuizData = quizData[currentLevel][i];
            if (currentQuizData.answeredCorrectly) {
                questionNumberLink.classList.add('done', 'correct');
            } else {
                questionNumberLink.classList.add('done', 'incorrect');
            }
        } else if (i === currentQuestion) {
            questionNumberLink.classList.add('active');
        }

        // Ajouter un gestionnaire d'evenements pour afficher la question correspondante lorsqu'un lien est clique
        questionNumberLink.onclick = function () {
            showQuestion(i);
        };

        listItem.appendChild(questionNumberLink);
        questionNumbersList.appendChild(listItem);
    }

    // Desactiver le selecteur de niveau si la question actuelle n'est pas la première
    if (currentQuestion !== 0) {
        if (levelSelector != null) {
            levelSelector.disabled = true;
        }
    } else {
        if (levelSelector != null) {
            levelSelector.disabled = false;
        }
        
    }
    answerSelected = false;
}

function checkAnswer(selectedOption) {
    if (answerSelected) {
        return;
    }
    answerSelected = true;
    const currentQuizData = quizData[currentLevel][currentQuestion];
    const correctAnswer = currentQuizData.correctAnswer;

    currentQuizData.answeredCorrectly = (selectedOption === correctAnswer); // Mise à jour de answeredCorrectly

    const buttons = optionsContainer.querySelectorAll('div');
    buttons.forEach(div => {
        div.disabled = true;
        if (div.innerText === correctAnswer) {
            div.style.backgroundColor = '#4CAF50';
        } else {
            div.style.backgroundColor = '#f44336';
        }
    });

    const fullSentence = `${correctAnswer}.`;
    resultDiv.innerText = fullSentence;

    if (selectedOption === correctAnswer) {
        console.log('Bonne reponse!');
    } else {
        console.log('Mauvaise reponse!');
    }
    nextButton.disabled = false;
}

function nextQuestion() {
    answerSelected = false;
    if (currentQuestion < quizData[currentLevel].length - 1) {
        currentQuestion++;
        showQuestion();
    } else {
        calculateAndDisplayScore(); 
        clearInterval(timerInterval);
        const optionDivs = optionsContainer.querySelectorAll('div');
        optionDivs.forEach(div => {
            div.disabled = true;
        });
        answerSelected = false;
        nextButton.disabled = true;
    }
}


function prevQuestion() {
    /*
    currentQuestion--;
    if (currentQuestion < 0) {

        currentQuestion = quizData[currentLevel].length - 1;
    }
    showQuestion();*/
   

}
window.onclick = function (event) {
    var modal = document.getElementById('scoreModal');
    if (event.target == modal) {
        closeModal();
    }
}
function drawGauge(scoreValue) {
    // Remove any existing svg to ensure the gauge is redrawn correctly
    d3.select("#gaugeContainer").selectAll("svg").remove();

    // Dynamically get the width and height based on the container's current size
    var gaugeContainer = document.getElementById('gaugeContainer');
    var width = gaugeContainer.offsetWidth; // Use the width of the container
    var height = gaugeContainer.offsetHeight; // Use the height of the container
    var radius = Math.min(width, height) / 2;
    var arc = d3.arc()
        .innerRadius(radius - 20) // Adjust the inner radius based on the new dimensions
        .outerRadius(radius)
        .startAngle(0);

    var tau = 2 * Math.PI;
    var svg = d3.select("#gaugeContainer").append("svg")
        .attr("width", width)
        .attr("height", height)
        .append("g")
        .attr("transform", "translate(" + width / 2 + "," + height / 2 + ")");

    // Background arc
    var background = svg.append("path")
        .datum({ endAngle: tau })
        .style("fill", "#add8e6")
        .attr("d", arc);

    // Foreground arc for the score
    var score = scoreValue / 100; // Convert scoreValue to a fraction of 100
    var foreground = svg.append("path")
        .datum({ endAngle: score * tau })
        .style("fill", "#0000ff") // Blue color for the score arc
        .attr("d", arc);

    // Animation for the foreground arc
    foreground.transition()
        .duration(1500) // Animation duration in milliseconds
        .attrTween("d", function (d) {
            var interpolate = d3.interpolate(d.endAngle, score * tau);
            return function (t) {
                d.endAngle = interpolate(t);
                return arc(d);
            };
        });

    // Add and animate score text
    var scoreText = svg.append("text")
        .attr("text-anchor", "middle")
        .attr("dy", "0.35em")
        .attr("class", "scoreText")
        .text("0");

    var scoreInterpolate = d3.interpolateRound(0, scoreValue);
    d3.transition().duration(1500).tween("text", function () {
        return function (t) {
            scoreText.text(scoreInterpolate(t));
        };
    });
}

function showModal() {
    document.getElementById('scoreModal').style.display = 'block';
}

function closeModal() {
    document.getElementById('scoreModal').style.display = 'none';
}

// Adjust calculateAndDisplayScore to use drawGauge
function calculateAndDisplayScore() {
    let score = 0;
    let totalQuestions = quizData[currentLevel].length;

    for (let question of quizData[currentLevel]) {
        if (question.answeredCorrectly) {
            score++;
        }
    }

    let scorePercentage = (score / totalQuestions) * 100;
    let scoreValue = Math.round(scorePercentage);

    document.getElementById('scoreValue').innerText = scoreValue;
    drawGauge(scoreValue); // Draw the gauge with the score value
    showModal();

    let quizId = parseInt(document.getElementById('quizContainer').getAttribute('data-quiz-id'), 10) || 1;

    // Call the SaveQuizSession action via AJAX
    saveQuizSession(quizId, scoreValue);

    // Désactiver les clics sur les options lorsque le score est terminée
    const buttons = optionsContainer.querySelectorAll('div');
    buttons.forEach(div => {
        div.style.pointerEvents = 'none'; 
    });
    nextButton.disabled = true;
    return;

}
function saveQuizSession(quizId, scorePercentage) {
    var username_ = localStorage.getItem('userSessionTmp');

    $.ajax({
        url: '/Quiz/SaveQuizSession', 
        type: 'GET',
        data: {
            quizId: quizId,
            score: scorePercentage,
            userSession: username_
        },
        success: function (response) {
            console.log('Session saved successfully:', response);
        },
        error: function (xhr, status, error) {
            console.error('Error saving session:', error);
        }
    });
}
function changeLevel() {
    if (currentQuestion === 0) {
        currentLevel = levelSelector.value;
        showQuestion();
    }
}
let quizData = {}; // Initialize quizData as an empty object

$(document).ready(function () {
    var quizId = parseInt(document.getElementById('quizContainer').getAttribute('data-quiz-id'), 10) || 1;

    $.ajax({
        url: '/Quiz/GetQuizDates',
        type: 'GET',
        data: { quizId: quizId },
        success: function (response) {
            startDate = new Date(response.beginDate);
            endDate = new Date(response.endDate); 
            const currentDate = new Date();
            if (currentDate >= startDate && currentDate <= endDate) {
                startTimer();
            } else if (currentDate < startDate) {
                startTimer();
            } else {
                document.getElementById('timer-display').innerText = "Unlimited Time";
            }
            fetchQuizDataAndShowQuestion();
        },
        error: function (xhr, status, error) {
            console.error('Error fetching quiz dates:', error);
        }
    });
    
});



function fetchQuizDataAndShowQuestion() {
    const quizId = parseInt(document.getElementById('quizContainer').getAttribute('data-quiz-id'), 10) || 1;
    
    const level = 2;

    // Show loading container
    document.getElementById('loadingContainer').style.display = 'flex';
    document.getElementById('quizContainer').style.display = 'none';

    $.ajax({
        url: '/Quiz/GetQuestions',
        type: 'GET',
        data: { quizId: quizId, level: level },
        dataType: 'json',
        success: function (response) {
            // Hide loading container and show quiz container
            document.getElementById('loadingContainer').style.display = 'none';
            document.getElementById('quizContainer').style.display = 'none'; // grid here

            quizData = response; // Load the fetched data into quizData
            showQuestion(); // Call showQuestion to display the first question
        },
        error: function (xhr, status, error) {
            // Hide loading container, show error message
            document.getElementById('loadingContainer').style.display = 'none';
            var errorMessageElement = document.getElementById('errorMessage');
            errorMessageElement.innerText = 'Quiz not found, come back later or verify the link you used.';
            var errorContainerElement = document.getElementById('errorContainer');
            errorContainerElement.style.display = 'block';
        }
    });
}

