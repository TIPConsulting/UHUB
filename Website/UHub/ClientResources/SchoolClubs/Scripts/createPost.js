(function () {

    var jsonPostDataOld = null;
    var oldResponseErr = null;


    var url = window.location.href;
    var seperated = url.split('/');
    var clubID = seperated.slice(-1)[0];


    var mdConverter = new showdown.Converter();
    setShowdownDefaults(mdConverter);


    $("#auxNav a.btn").attr('href', '/SchoolClub/' + clubID);



    var simplemde = new SimpleMDE(
        {
            autosave: {
                enabled: false
            },
            element: document.getElementById("inputContent"),
            previewRender: function (plainText) {
                return mdConverter.makeHtml(plainText);
            },
            promptURLs: true,
            spellChecker: true,
            status: ["lines", "words", {
                className: "charCount",
                defaultValue: function (el) {
                    this.charCount = 0;
                    el.innerHTML = "Characters: 0 (10 - 10k)";
                },
                onUpdate: function (el) {
                    var ct = simplemde.value().length;
                    el.innerHTML = "Characters: " + ct + " (10 - 10k)";
                }
            }]
        });


    function setWaitState() {
        $("#btn_CreatePost").attr("disabled", "disabled");
        $("html").css({ cursor: "default" });
    }

    function clearWaitState() {
        $("#btn_CreatePost").removeAttr("disabled");
        $("html").css({ cursor: "default" });
    }


    function getFormData() {

        return {
            Name: $("#inputTitle").val(),
            Content: simplemde.value(),
            IsPublic: !$("#inputMakePrivate")[0].checked,
            CanComment: $("#inputCanComment")[0].checked,
            ParentID: clubID
        };
    }


    function processInputValidation(formData) {

        if (!formData.Name.match(RgxPtrns.Post.NAME)) {
            oldResponseErr = 'Post Name Invalid';
            alert(oldResponseErr);
            return false;
        }
        else if (!formData.Content.match(RgxPtrns.Post.CONTENT)) {
            oldResponseErr = 'Post Content Invalid';
            alert(oldResponseErr);
            return false;
        }

        return true;
    }


    function sendData(formData) {
        setWaitState();

        var jsonPostData = JSON.stringify(formData);


        if (jsonPostData == jsonPostDataOld) {
            alert(oldResponseErr);
            clearWaitState();
            return;
        }
        jsonPostDataOld = jsonPostData;


        if (!processInputValidation(formData)) {
            clearWaitState();
            return;
        }


        $.ajax({
            method: "POST",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonPostData,
            url: "/uhubapi/posts/Create"
        })
            .done(function (data) {
                $("#inputTitle").val("");
                simplemde.value("");
                $("#inputMakePrivate")[0].checked = false;
                $("#inputCanComment")[0].checked = true;

                alert("Post Created");

                window.location.href = "/Post/" + data;
            })
            .fail(function (resp) {
                oldResponseErr = resp.responseJSON;
                alert(oldResponseErr);
            })
            .always(function () {
                clearWaitState();
            });

    }



    $("#btn_CreatePost").click(function () {
        var data = getFormData();
        sendData(data);
    });



    registerInputValidator($("#inputTitle"), RgxPtrns.Post.NAME);
    registerInputValidator($("#inputContent"), RgxPtrns.Post.CONTENT);


})();