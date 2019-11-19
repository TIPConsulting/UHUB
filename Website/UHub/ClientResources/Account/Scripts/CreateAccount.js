
(function () {



    var oldEmail = "";
    var userObjOld = null;
    var oldResponseErr = null;



    function setWaitState() {
        $("#btn_CreateUser").attr("disabled", "disabled");
        $("html").css({ cursor: "default" });
    }

    function clearWaitState() {
        $("#btn_CreateUser").removeAttr("disabled");
        $("html").css({ cursor: "default" });
    }


    function processInputValidation(formData) {


        if (!formData.Email.match(RgxPtrns.User.EMAIL)) {
            oldResponseErr = 'Email Invalid';
            alert(oldResponseErr);
            return false;
        }
        else if (!formData.Username.match(RgxPtrns.User.USERNAME)) {
            oldResponseErr = 'Username Invalid';
            alert(oldResponseErr);
            return false;
        }
        else if (!formData.Password.match(RgxPtrns.User.PSWD)) {
            oldResponseErr = 'Password Invalid';
            alert(oldResponseErr);
            return false;
        }
        else if (!formData.Name.match(RgxPtrns.User.NAME)) {
            oldResponseErr = 'Name Invalid';
            alert(oldResponseErr);
            return false;
        }
        else if (!formData.Major.match(RgxPtrns.User.MAJOR)) {
            oldResponseErr = 'Major Invalid';
            alert(oldResponseErr);
            return false;
        }
        else if (formData.PhoneNumber != "" && !formData.PhoneNumber.match(RgxPtrns.User.PHONE)) {
            oldResponseErr = 'Phone Invalid';
            alert(oldResponseErr);
            return false;
        }
        else if (formData.GradDate != "" && !formData.GradDate.match(RgxPtrns.User.GRAD_DATE)) {
            oldResponseErr = 'Grad Date Invalid';
            alert(oldResponseErr);
            return false;
        }
        else if (!formData.Year.match(RgxPtrns.User.YEAR)) {
            oldResponseErr = 'Year Invalid';
            alert(oldResponseErr);
            return false;
        }
        else if (!formData.Company.match(RgxPtrns.User.COMPANY)) {
            oldResponseErr = 'Company Invalid';
            alert(oldResponseErr);
            return false;
        }
        else if (!formData.JobTitle.match(RgxPtrns.User.JOB_TITLE)) {
            oldResponseErr = 'Job Title Invalid';
            alert(oldResponseErr);
            return false;
        }

        return true;
    }





    var createUser = new Vue({
        el: "#userForm",
        data: {
            Username: "",
            Password: "",
            Email: "",
            Name: "",
            Major: "",
            PhoneNumber: "",
            Year: "",
            GradDate: "",
            Company: "",
            JobTitle: ""
        },
        methods: {
            readRefs: function () {
                let refs = this.$refs;
                this.Username = refs.inputUSR.value;
                this.Password = refs.inputPWD.value;
                this.Email = refs.inputEmail.value;
                this.Name = refs.inputFirst.value + " " + refs.inputLast.value;
                this.Major = refs.inputMajor.value;
                this.PhoneNumber = refs.inputPhone.value;
                this.Year = refs.inputYear.value;
                this.GradDate = refs.inputGrad.value;
                this.Company = refs.inputCompany.value;
                this.JobTitle = refs.inputJob.value;
                this.sendObj = JSON.stringify(this.$data);
            },
            sendData: function () {
                setWaitState();
                var recapVal = grecaptcha.getResponse();

                var userObj = createUser.sendObj;

                if (userObj == userObjOld) {
                    alert(oldResponseErr);
                    clearWaitState();
                    return;
                }
                userObjOld = userObj;


                if (!processInputValidation(this)) {
                    clearWaitState();
                    return;
                }



                $.ajax({
                    method: "POST",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    url: "/uhubapi/Account/CreateUser",
                    headers: {
                        "g-recaptcha-response": recapVal
                    },
                    data: userObj
                })
                    //AJAX -> "/uhubapi/Account/CreateUser"
                    .done(function (data) {

                        alert(data.status);

                        if (data.canLogin === true) {
                            window.location.href = "/";
                        }
                        else {
                            window.location.href = "/Account/Confirm/New";
                        }
                    })
                    //AJAX -> "/uhubapi/Account/CreateUser"
                    .fail(function (data) {
                        oldResponseErr = data.responseJSON.status;
                        alert(oldResponseErr);
                    })
                    //AJAX -> "/uhubapi/Account/CreateUser"
                    .always(function () {
                        $("#btn_CreateUser").removeAttr("disabled");
                        $("html").css({ cursor: "default" });
                    });
            }
        }
    });
    /*
        When email is unfocused, sends ajax request to get list of majors to supply the dropdown
     */
    $("#email").blur(function () {
        var majornames = [];
        var val = $(this).val();
        if (val == undefined || val == null || val == "") {
            return;
        }
        if (val === oldEmail) {
            return;
        }

        oldEmail = val;

        $.ajax({
            method: "get",
            url: "/uhubapi/schoolmajors/GetAllByEmail",
            data: {
                email: val
            }
        })
            //AJAX -> /uhubapi/schoolmajors/GetAllByEmail
            .done(function (data) {
                for (var i = 0; i < data.length; i++) {
                    if (data[i].IsEnabled === true) {
                        majornames.push(data[i].Name);
                    }
                }
                majornames.sort();

                $("#autocomplete").autocomplete({
                    source: majornames
                });
            });
    });



    $(function () {
        $("#grad-date").datepicker({
            changeMonth: true,
            changeYear: true
        });
        $("#grad-date").datepicker("option", "dateFormat", "yy-mm-dd");
    });



    registerInputValidator($("#email"), RgxPtrns.User.EMAIL);
    registerInputValidator($("#usr"), RgxPtrns.User.USERNAME);
    registerInputValidator($("#pwd"), RgxPtrns.User.PSWD);
    registerInputValidator($("#firstname"), RgxPtrns.User.NAME);
    registerInputValidator($("#lastname"), RgxPtrns.User.NAME);
    registerInputValidator($("#autocomplete"), RgxPtrns.User.MAJOR);
    registerInputValidator($("#phone"), RgxPtrns.User.PHONE, true);
    registerInputValidator($("#grad-date"), RgxPtrns.User.GRAD_DATE, true);
    registerInputValidator($("#year"), RgxPtrns.User.YEAR);
    registerInputValidator($("#company"), RgxPtrns.User.COMPANY, true);
    registerInputValidator($("#job-title"), RgxPtrns.User.JOB_TITLE, true);


})();