(function () {


    var jsonClubDataOld = null;
    var oldResponseErr = null;



    function setWaitState() {
        $("#btn_CreateClub").attr("disabled", "disabled");
        $("html").css({ cursor: "default" });
    }

    function clearWaitState() {
        $("#btn_CreateClub").removeAttr("disabled");
        $("html").css({ cursor: "default" });
    }



    function GetDataObj() {

        return {
            Name: $("#txt_Name").val(),
            Description: $("#txt_Description").val()
        };

    }



    function processInputValidation(formData) {

        if (!formData.Name.match(RgxPtrns.Club.NAME)) {
            oldResponseErr = 'Club Name Invalid';
            alert(oldResponseErr);
            return false;
        }
        else if (!formData.Description.match(RgxPtrns.Club.DESCRIPTION)) {
            oldResponseErr = 'Club Description Invalid';
            alert(oldResponseErr);
            return false;
        }

        return true;
    }


    function SendData(data) {

        setWaitState();


        var jsonClubData = JSON.stringify(data);


        if (jsonClubData == jsonClubDataOld) {
            alert(oldResponseErr);
            clearWaitState();
            return;
        }
        jsonClubDataOld = jsonClubData;


        if (!processInputValidation(data)) {
            clearWaitState();
            return;
        }


        var recapVal = grecaptcha.getResponse();



        $.ajax({
            method: "POST",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            url: "/uhubapi/schoolClubs/Create",
            headers: {
                "g-recaptcha-response": recapVal
            },
            data: jsonClubData
        })
            //AJAX -> /uhubapi/schoolClubs/Create
            .done(function (data) {

                Name: $("#txt_Name").val("");
                Description: $("#txt_Description").val("");

                alert("Club Created");

                window.location.href = "/SchoolClub/" + data;

            })
            //AJAX -> /uhubapi/schoolClubs/Create
            .fail(function (data) {
                oldResponseErr = data.responseJSON;
                alert(oldResponseErr);
            })
            //AJAX -> /uhubapi/schoolClubs/Create
            .always(function () {
                clearWaitState();
            });
    }




    $("#btn_CreateClub").click(function () {
        var data = GetDataObj();
        SendData(data);
    });



    registerInputValidator($("#txt_Name"), RgxPtrns.Club.NAME);
    registerInputValidator($("#txt_Description"), RgxPtrns.Club.DESCRIPTION, true);

})();