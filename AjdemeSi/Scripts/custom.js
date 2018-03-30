$(document).ready(function (event) {

});

$(document).on('click tap', '#sidebar-colapse', function (event) {
    $('#page-wrapper').toggleClass('collapsed');
    $('.sidebar').toggleClass('collapsed');
    $(this).toggleClass('glyphicon-arrow-left').toggleClass('glyphicon-arrow-right');
});