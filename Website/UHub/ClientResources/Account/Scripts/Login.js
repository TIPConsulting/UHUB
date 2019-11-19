(function () {
    $(function () { $("#Email").focus(); });


    registerInputValidator($("#Email"), RgxPtrns.User.EMAIL);
    registerInputValidator($("#Password"), RgxPtrns.User.PSWD);


    window.setInterval(function () {

        if ($("#Email").attr('data-isValid') === 'true' && $("#Password").attr('data-isValid') === 'true') {
            $("input[type=submit]").removeAttr("disabled");
        }
        else {
            $("input[type=submit]").attr("disabled", "disabled");
        }

    }, 100);
})();
