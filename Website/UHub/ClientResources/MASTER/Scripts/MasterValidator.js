function registerInputValidator(obj, rgxStr, allowEmpty) {

    if (allowEmpty == undefined || allowEmpty == null) {
        allowEmpty = false;
    }

    var outline = $(obj).style('outline-color');
    var border = $(obj).style('border-color');
    var shadow = $(obj).style('box-shadow');
    $(obj).attr('data-baseStyles', outline + "|" + border + "|" + shadow);


    function validate() {

        var col = null;
        var rgb = null;
        var isValid = false;

        if (allowEmpty && $(obj).val() == "") {
            var baseStyles = $(obj).attr('data-baseStyles');
            var bStyleSet = baseStyles.split('|');

            $(obj).style('outline-color', bStyleSet[0]);
            $(obj).style('border-color', bStyleSet[1]);
            $(obj).style('box-shadow', bStyleSet[2]);

            $(obj).attr('data-isValid', 'true');

            return;
        }
        else {
            if ($(obj).val().match(rgxStr)) {
                isValid = 'true';
                col = 'green';
                rgb = 'rgba(0, 255, 0, .2)';
            }
            else {
                isValid = 'false';
                col = 'red';
                rgb = 'rgba(255, 0, 0, .2)';
            }
        }

        $(obj).attr('data-isValid', isValid);
        $(obj).style('outline-color', col, "important");
        $(obj).style('border-color', col, "important");
        $(obj).style('box-shadow', "0 0 .1rem 0.2rem " + rgb, "important");
    }



    if ($(obj).val() != "") {
        validate();
    }
    $(obj).on('change keydown keyup', validate);


}