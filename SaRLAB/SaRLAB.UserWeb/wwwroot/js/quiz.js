// ________FAKE_DATA_______________
let questions = [
    {
        quiz_id: 1,
        question:
            "You can learn a lot about the local _______ by talking to local people.",
        answers: ["territory", "area", "land", "nation"],
    },
    {
        quiz_id: 2,
        question:
            "It's good to have someone to ________ you when you are visiting a new place.",
        answers: ["lead", "take", "guide", "bring"],
    },
    {
        quiz_id: 3,
        question:
            "When you ______ your destination, your tour guide will meet you at the airport.",
        answers: ["arrive", "reach", "get", "achieve"],
    },
    {
        quiz_id: 4,
        question: "It can be quite busy here during the tourist ______",
        answers: ["season", "phase", "period", "stage"],
    },
    {
        quiz_id: 5,
        question:
            "Make sure you _______ a hotel before you come to our island, especially in the summer.",
        answers: ["book", "keep", "put", "buy"],
    },
    {
        quiz_id: 6,
        question: "Captain Cook discovered Australia on a _______ to the Pacific.",
        answers: ["vacation", "travel", "cruise", "voyage"],
    },
    {
        quiz_id: 7,
        question:
            " Most tourist attractions in London charge an admission ________.",
        answers: ["fare", "ticket", "fee", "pay"],
    },
    {
        quiz_id: 8,
        question: "The hotel where we are _______ is quite luxurious.",
        answers: ["living", "existing", "remaining", "staying"],
    },
    {
        quiz_id: 9,
        question: "Is English an ________ language in your country?",
        answers: ["mother", "official", "living", "old"],
    },
    {
        quiz_id: 10,
        question: "He spoke a ______ of French that we found hard to understand.",
        answers: ["slang", "jargon", "dialect", "language"],
    },
    {
        quiz_id: 11,
        question: "Which planet is known as the Red Planet?",
        answers: ["Earth", "Mars", "Jupiter", "Venus"],
    },
    {
        quiz_id: 12,
        question: "What is the largest ocean on Earth?",
        answers: ["Atlantic", "Indian", "Arctic", "Pacific"],
    },
    {
        quiz_id: 13,
        question: "What is the capital of Japan?",
        answers: ["Beijing", "Seoul", "Tokyo", "Bangkok"],
    },
    {
        quiz_id: 14,
        question: "Which element has the chemical symbol 'O'?",
        answers: ["Oxygen", "Gold", "Silver", "Iron"],
    },
    {
        quiz_id: 15,
        question: "What is the speed of light?",
        answers: ["300,000 km/s", "150,000 km/s", "1,000 km/s", "30,000 km/s"],
    },
    {
        quiz_id: 16,
        question: "Who wrote 'Hamlet'?",
        answers: ["Charles Dickens", "William Shakespeare", "Mark Twain", "Jane Austen"],
    },
    {
        quiz_id: 17,
        question: "What is the smallest prime number?",
        answers: ["1", "2", "3", "5"],
    },
    {
        quiz_id: 18,
        question: "Which country is known as the Land of the Rising Sun?",
        answers: ["China", "Japan", "India", "Thailand"],
    },
    {
        quiz_id: 19,
        question: "What is the capital of Australia?",
        answers: ["Sydney", "Melbourne", "Canberra", "Brisbane"],
    },
    {
        quiz_id: 20,
        question: "Which is the longest river in the world?",
        answers: ["Amazon", "Nile", "Yangtze", "Mississippi"],
    },
];
const results = [
    {
        quiz_id: 1,
        answer: "area",
    },
    {
        quiz_id: 3,
        answer: "reach",
    },
    {
        quiz_id: 2,
        answer: "guide",
    },
    {
        quiz_id: 4,
        answer: "season",
    },
    {
        quiz_id: 5,
        answer: "book",
    },
    {
        quiz_id: 6,
        answer: "voyage",
    },
    {
        quiz_id: 7,
        answer: "fee",
    },
    {
        quiz_id: 8,
        answer: "staying",
    },
    {
        quiz_id: 9,
        answer: "official",
    },
    {
        quiz_id: 10,
        answer: "dialect",
    },
    {
        quiz_id: 11,
        answer: "Mars",
    },
    {
        quiz_id: 12,
        answer: "Pacific",
    },
    {
        quiz_id: 13,
        answer: "Tokyo",
    },
    {
        quiz_id: 14,
        answer: "Oxygen",
    },
    {
        quiz_id: 15,
        answer: "300,000 km/s",
    },
    {
        quiz_id: 16,
        answer: "William Shakespeare",
    },
    {
        quiz_id: 17,
        answer: "2",
    },
    {
        quiz_id: 18,
        answer: "Japan",
    },
    {
        quiz_id: 19,
        answer: "Canberra",
    },
    {
        quiz_id: 20,
        answer: "Nile",
    },
];

$(document).ready(function () {
    const questionsPerPage = 10;
    let currentPage = 1;
    const totalPages = Math.ceil(questions.length / questionsPerPage);

    function renderQuestions(page) {
        let container = $('#questions-container');
        container.empty();

        let start = (page - 1) * questionsPerPage;
        let end = start + questionsPerPage;

        questions.slice(start, end).forEach((question, index) => {
            let questionHtml = `<div class="question">
                <p>${start + index + 1}. ${question.question}</p>`;
            question.answers.forEach(answer => {
                questionHtml += `<input type="radio" name="question${question.quiz_id}" value="${answer}"> ${answer}<br>`;
            });
            questionHtml += `</div>`;
            container.append(questionHtml);
        });
    }

    function renderPagination() {
        let paginationContainer = $('#pagination-container');
        paginationContainer.empty();

        for (let i = 1; i <= totalPages; i++) {
            let buttonHtml = `<button onclick="changePage(${i})" class="${i === currentPage ? 'active' : ''}">${i}</button>`;
            paginationContainer.append(buttonHtml);
        }
    }

    window.changePage = function (page) {
        if (page < 1 || page > totalPages) return;
        currentPage = page;
        renderQuestions(page);
        renderPagination();
    }


    // Kiểm tra đáp án khi submit form
    $('#quiz-form').on('submit', function (event) {
        event.preventDefault();

        let correctAnswers = 0;
        let totalQuestions = questions.length;

        questions.forEach(question => {
            let selectedAnswer = $(`input[name="question${question.quiz_id}"]:checked`).val();
            let correctAnswer = results.find(result => result.quiz_id === question.quiz_id).answer;

            if (selectedAnswer === correctAnswer) {
                correctAnswers++;
            }
        });

        let message = `Bạn đã trả lời đúng ${correctAnswers}/${totalQuestions} câu hỏi.`;
        /*let modalImgSrc = "~/images/ic_warning.png";*/
        let modalAction = '<a class="btn btn-danger" href="/MultipleQuestions/GetAllQuestion">Làm lại</a>';

        if (correctAnswers === totalQuestions) {
            message = 'Bạn đã trả lời đúng tất cả các câu hỏi!';
           /* modalImgSrc = "~/images/ic_success.png";*/
            modalAction = '<a class="btn btn-success" href="/HomePage/index">Truy cập</a>';
        }

        $('#message').html(message);
        /*$('#modal-img').attr('src', '@Url.Content("' + modalImgSrc + '")');*/
        $('#modal-action').html(modalAction);
        showModal();
    });

    // Hiển thị modal
    function showModal() {
        $('#resultModal').css('display', 'block');
    }

    // Đóng modal
    window.closeModal = function () {
        $('#resultModal').css('display', 'none');
    }

    renderQuestions(currentPage);
    renderPagination();
});