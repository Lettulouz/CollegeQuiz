export const alertInfo = (message) => ({ active: true, style: 'alert-success', message });
export const alertDanger = (message) => ({ active: true, style: 'alert-danger', message });
export const alertOff = () => ({ active: false, style: 'alert-success', message: '' });

export const WAITING_SCREEN = "WAITING_SCREEN";
export const COUNTING_SCREEN = "COUNTING_SCREEN";
export const IN_GAME = "IN_GAME";

export const getCommonFetchObj = method => ({
    method,
    credentials: 'same-origin',
    headers: {
        'Accept': 'application/json',
        'Content-Type': 'application/json'
    },
});