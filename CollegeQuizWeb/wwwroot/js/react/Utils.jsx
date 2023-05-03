export const alertInfo = message => ({ active: true, style: 'alert-success', message });
export const alertDanger = message => ({ active: true, style: 'alert-danger', message });
export const alertWarning = message => ({ active: true, style: 'alert-warning', message });
export const alertOff = () => ({ active: false, style: 'alert-success', message: '' });

export const WAITING_SCREEN = "WAITING_SCREEN";
export const COUNTING_SCREEN = "COUNTING_SCREEN";
export const IN_GAME = "IN_GAME";
export const QUESTION_RESULT_SCREEN = "QUESTION_RESULT_SCREEN";
export const CORRECT_ANSWERS_SCREEN = "CORRECT_ANSWERS_SCREEN";
export const MOBILE_CHECKPOINT = "MOBILE_CHECKPOINT";

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
    "/gfx/blueCard.svg", "/gfx/greenCard.svg", "/gfx/darkblueCard.svg",
    "/gfx/tealCard.svg", "/gfx/oliveCard.svg", "/gfx/darkgreenCard.svg"
];