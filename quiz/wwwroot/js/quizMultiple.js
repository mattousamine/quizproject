const quizData = {
    easy: [
        {
            questionNum: "Question 1",
            questionText: "La POO(Programmation Orientee  Objet) est: ",
            options: ["Un paradigme de programmation",
                "Un langage de programmation"],
            correctAnswer: "Un paradigme de programmation",
            answeredCorrectly: false
        },
        {
            questionNum: "Question 2",
            questionText: "La generalisation est permet a des sous-classe de derriver et ainsi de finir des sous-classes plus specifiques.",
            options: ["VRAI",
                "FAUX"],
            correctAnswer: "VRAI",
            answeredCorrectly: false
        },
    ],
    hard: [
        {
            questionNum: "Question 1",
            questionText: "Lequel de ces quatres n'est pas un principes de la Programmation Orrientee Objet?",
            options: ["Abstration",
                "Polymorphisme",
                "principe de service",
                "Encapsulation"],
            correctAnswer: "principe de service",
            answeredCorrectly: false
        },
        {
            questionNum: "Question 2",
            questionText: "lequel de ces quatre n'est pas un specifateur d'acces en programmation",
            options: ["Protected",
                "Public",
                "Private",
                "Unaccessible"],
            correctAnswer: "Unaccessible",
            answeredCorrectly: false
        },
    ]
};

let currentQuestion = 0;
let currentLevel = 'easy';
const questionNumElement = document.getElementById('questionNum');
const questionTextElement = document.getElementById('questionText');
const optionsContainer = document.getElementById('options-container');
const prevButton = document.getElementById('prev-btn');
const nextButton = document.getElementById('next-btn');
const levelSelector = document.getElementById('level-selector');
const resultDiv = document.getElementById('result');

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

    if (currentQuestion === 0) {
        prevButton.disabled = true;
    } else {
        prevButton.disabled = false;
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
}

function nextQuestion() {
    currentQuestion++;
    if (currentQuestion >= quizData[currentLevel].length) {

        currentQuestion = 0;
    }
    showQuestion();
}

function prevQuestion() {
    currentQuestion--;
    if (currentQuestion < 0) {

        currentQuestion = quizData[currentLevel].length - 1;
    }
    showQuestion();
}

function changeLevel() {
    if (currentQuestion === 0) {
        currentLevel = levelSelector.value;
        showQuestion();
    }
}

showQuestion();