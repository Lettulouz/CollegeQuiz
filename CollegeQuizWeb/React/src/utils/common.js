export const alertInfo = message => ({ active: true, style: 'alert-success', message });
export const alertDanger = message => ({ active: true, style: 'alert-danger', message });
export const alertWarning = message => ({ active: true, style: 'alert-warning', message });
export const alertOff = () => ({ active: false, style: 'alert-success', message: '' });

export const WAITING_SCREEN = "WAITING_SCREEN";
export const COUNTING_SCREEN = "COUNTING_SCREEN";
export const IN_GAME = "IN_GAME";
export const QUESTION_RESULT_SCREEN = "QUESTION_RESULT_SCREEN";
export const CORRECT_ANSWERS_SCREEN = "CORRECT_ANSWERS_SCREEN";

export const getCommonFetchObj = method => ({
    method,
    credentials: 'same-origin',
    headers: {
        'Accept': 'application/json',
        'Content-Type': 'application/json'
    },
});

export const getCommonFetchObjWithBody = (method, body) => ({
    method,
    credentials: 'same-origin',
    headers: {
        'Accept': 'application/json',
        'Content-Type': 'application/json'
    },
    body: JSON.stringify(body)
});

export const getCommonFetchObjWithFormData = (method, formData) => ({
    method,
    credentials: 'same-origin',
    body: formData
});

export const getCropperConfig = () => ({
    aspectRatio: 1,
    viewMode: 1,
    dragMode: 'move',
    autoCropArea: 1,
    cropBoxMovable: true,
    cropBoxResizable: true,
    scalable: true,
    restore: false,
    minContainerWidth: 700,
    minContainerHeight: 500,
    minCanvasWidth: 700,
    minCanvasHeight: 500,
});

export const ANSWER_LETTERS = [ "A", "B", "C", "D", "E", "F" ];
export const ANSWER_SVGS = [
    "https://cdn.quizazu.com/static/gfx/card/blueCard.svg",
    "https://cdn.quizazu.com/static/gfx/card/greenCard.svg",
    "https://cdn.quizazu.com/static/gfx/card/darkblueCard.svg",
    "https://cdn.quizazu.com/static/gfx/card/tealCard.svg",
    "https://cdn.quizazu.com/static/gfx/card/oliveCard.svg",
    "https://cdn.quizazu.com/static/gfx/card/darkgreenCard.svg"
];

export const RESULT_CARD_COLORS = [
    { r: 31, g: 210, b: 230 },
    { r: 41, g: 228, b: 197 },
    { r: 85, g: 229, b: 160 },
    { r: 187, g: 245, b: 205 },
    { r: 210, g: 239, b: 219 },
];

export const generateColor = i => {
    const { r, g, b } = RESULT_CARD_COLORS[i];
    return `rgba(${r}, ${g}, ${b}, .8)`
};

export const convertSecondsToTime = seconds => {
    const secondsNr = Number(seconds);
    const minutesT = Math.floor(secondsNr % 3600 / 60);
    const secondsT = Math.floor(secondsNr % 3600 % 60);
    return ('0' + minutesT).slice(-2) + ":" + ('0' + secondsT).slice(-2);
};

export const playSound = counter => {
    if(counter <= 5 && counter > 0){
        var audio = new Audio(`https://cdn.quizazu.com/static/sfx/${counter}.mp3`);
        audio.volume = 0.8
        audio.play();
    }
};

export const generateErrorMessage = code => {
    switch (code) {
        case "404":
            return "Podany zasób którego szukasz nie istnieje bądź został usunięty.";
        case "403":
        case "401":
            return "Sesja wygasła. Zaloguj się ponownie, aby uzyskać dostęp do zasobu.";
        default:
            return "Wystąpił bład po stronie serwera. Spróbuj ponownie później.";
    }
};