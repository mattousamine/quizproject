
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
let timerSeconds = 50;

function startTimer() {
    timerInterval = setInterval(updateTimer, 1000);
}

function updateTimer() {
    const minutes = Math.floor(timerSeconds / 60);
    const seconds = timerSeconds % 60;
    const display = `${minutes < 10 ? '0' : ''}${minutes}:${seconds < 10 ? '0' : ''}${seconds}`;
    const timerDisplay = document.getElementById('timer-display');

    if (timerSeconds <= 30) {
        timerDisplay.style.color = 'red';
    } else {
        timerDisplay.style.color = '';
    }

    timerDisplay.innerText = display;

    if (timerSeconds <= 0) {
        clearInterval(timerInterval);
        calculateAndDisplayScore();
    } else {
        timerSeconds--;
    }
}

startTimer();

document.addEventListener('keydown', function (event) {
    if (event.code === 'Space' && timerSeconds <= 0) {
        resetTimer();
    }
});

function resetTimer() {
    timerSeconds = 40;
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
        levelSelector.disabled = true;
    } else {
        levelSelector.disabled = false;
    }
}

function checkAnswer(selectedOption) {
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
    if (currentQuestion < quizData[currentLevel].length - 1) {
        currentQuestion++;
        showQuestion();
    } else {
        calculateAndDisplayScore(); 
        clearInterval(timerInterval);
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

    var tau = 2 * Math.PI;
    var width = 200, height = 200, radius = Math.min(width, height) / 2;
    var arc = d3.arc()
        .innerRadius(radius - 20)
        .outerRadius(radius)
        .startAngle(0);

    var svg = d3.select("#gaugeContainer").append("svg")
        .attr("width", width)
        .attr("height", height)
        .append("g")
        .attr("transform", "translate(" + width / 2 + "," + height / 2 + ")");

    var background = svg.append("path")
        .datum({ endAngle: tau })
        .style("fill", "#add8e6")
        .attr("d", arc);

    var score = scoreValue / 100; // Convert scoreValue to a fraction of 100

    // Create the foreground arc but don't set the end angle yet
    var foreground = svg.append("path")
        .datum({ endAngle: score * tau })
        .style("fill", "#0000ff") // Blue color for the score arc
        .attr("d", arc);

    // Animate the foreground arc
    foreground.transition()
        .duration(1500) // animation duration in milliseconds
        .attrTween("d", function (d) {
            var interpolate = d3.interpolate(d.endAngle, score * tau);
            return function (t) {
                d.endAngle = interpolate(t);
                return arc(d);
            };
        });

    // Optional: Add text to display the score
    // You might want to animate this as well
    var scoreText = svg.append("text")
        .attr("text-anchor", "middle")
        .attr("dy", "0.35em")
        .attr("class", "scoreText")
        .text("0"); // Start text at 0

    // Animate score text update
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

    let quizId = sessionStorage.getItem('quizId') ? parseInt(sessionStorage.getItem('quizId'), 10) : 1;

    // Call the SaveQuizSession action via AJAX
    saveQuizSession(quizId, scoreValue);
}
function saveQuizSession(quizId, scorePercentage) {
    $.ajax({
        url: '/Quiz/SaveQuizSession', 
        type: 'GET',
        data: {
            quizId: quizId,
            score: scorePercentage
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
    fetchQuizDataAndShowQuestion();
    
});



function fetchQuizDataAndShowQuestion() {
    const quizId = sessionStorage.getItem('quizId') ? parseInt(sessionStorage.getItem('quizId'), 10) : 1;
    const level = 2; 

    $.ajax({
        url: '/Quiz/GetQuestions',
        type: 'GET',
        data: { quizId: quizId, level: level },
        dataType: 'json',
        success: function (response) {
            quizData = response; // Load the fetched data into quizData
            showQuestion(); // Call showQuestion to display the first question
            $('#questionNumbersList').off('click', 'a').on('click', 'a', function(event) {
                event.preventDefault();
                event.stopPropagation();
                console.log('Click on link prevented.');
            });
        },
        error: function (xhr, status, error) {
            console.error('Error fetching quiz data:', status, error);
        }
    });
}