// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

/*
 * Funckja do pokazywania modala po wylogowaniu
 */
function showModal() {
    const modal = document.getElementById('logout-modal');
    if (modal !== null) new bootstrap.Modal(modal, {}).show();
}

function onLoad() {
    showModal();
    $('[data-bs-toggle="tooltip"]').toArray().forEach(function(el) {
        new bootstrap.Tooltip(el);
    });
}

$(window).on('load', onLoad);
