$(function () {
    $("form").preventDoubleSubmission();

    $("#btn_Submit").prop('disabled', false);
    $(document).on("keypress", ":input:not(textarea)", function (event) {
        if (event.keyCode === 13) {
            event.preventDefault();
        }
    });
});