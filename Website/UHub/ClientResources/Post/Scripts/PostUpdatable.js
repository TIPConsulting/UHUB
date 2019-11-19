(function () {

    var simplemde;
    var postRawData;
    var jsonPostDataOld = null;
    var oldResponseErr = null;
    var rawCommentSet = [];
    var currentUser = null;


    var postID = encodeURIComponent(window.location.href.split('/').slice(-1)[0]);
    postID = postID.match(/^[0-9]+/)[0];


    var mdConverter = new showdown.Converter();
    setShowdownDefaults(mdConverter);

    function getToday() {
        return moment().format();
    }

    function getTodayDateStr() {
        var today = new Date();
        var dd = today.getDate();
        var mm = today.getMonth() + 1; //January is 0!
        var yyyy = today.getFullYear();
        return yyyy.toString().padStart(4, '0') + '-' + mm.toString().padStart(2, '0') + '-' + dd.toString().padStart(2, '0');
    }


    function getPostUpdateData() {
        postRawData.Content = simplemde.value();
        postRawData.CanComment = $("#chk_CanComment")[0].checked;

        return {
            ID: postRawData.ID,
            Name: postRawData.Name,
            Content: postRawData.Content,
            IsPublic: postRawData.IsPublic,
            CanComment: postRawData.CanComment
        };
    }


    function execCreateComment(jsonData) {

        return $.ajax({
            method: "POST",
            url: "/uhubapi/comments/Create",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonData
        })
            .fail(function (jqAjax, errorText) {
                alert("Error" + errorText);
            });
    }



    // getCommentById and getCommentDepth  are helper functions for arrangeCommentTree
    // Returns matching comment object
    function getCommentById(cmtID, cmtList) {
        for (var j = 0; j < cmtList.length; j++) {
            if (cmtList[j].ID == cmtID) {
                return cmtList[j];
            }
        }
    }
    // Finds the comment's degrees of separation from post
    function getCommentDepth(theCmt, cmtList) {

        var depthLevel = 0;
        while (theCmt.ParentID != postID) {
            theCmt = getCommentById(theCmt.ParentID, cmtList);
            depthLevel++;
        }
        return depthLevel;
    }
    // Arranges array so that children are within parent comments
    function arrangeCommentTree(cmtList) {
        var maxDepth = 0;
        var newCmtList = cmtList.slice();

        var listLength = newCmtList.length;

        //Remove disabled comments
        for (var i = listLength - 1; i >= 0; i--) {
            if (!newCmtList[i].IsEnabled) {
                newCmtList.splice(i, 1);
            }
        }
        listLength = newCmtList.length;


        //Get depth level for each comment
        for (var i = 0; i < listLength; i++) {
            newCmtList[i].cmt_children = [];
            newCmtList[i].DepthLevel = getCommentDepth(newCmtList[i], newCmtList);
            if (newCmtList[i].DepthLevel > maxDepth) {
                maxDepth = newCmtList.DepthLevel;
            }
        }

        //create hierarchy
        //iterate through all comments in rawSet
        for (var j = 0; j < listLength; j++) {

            if (newCmtList[j].DepthLevel == 0) {
                continue;
            }

            //iterate through all comments again
            //Looking for parent of [j]
            for (var k = 0, kIsGood = true; k < listLength && kIsGood; k++) {


                if (newCmtList[k].ID == newCmtList[j].ParentID) {
                    var added = false;
                    //add new child if none exist
                    var chldCnt = newCmtList[k].cmt_children.length;

                    //Add new child (sorted by ID descending) if a child already exists in list
                    //TODO: Convert to binary insert sort
                    for (var z = 0, zIsGood = true; z < chldCnt && zIsGood; z++) {
                        if (newCmtList[j].ID > newCmtList[k].cmt_children[z].ID) {
                            newCmtList[k].cmt_children.splice(z, 0, newCmtList[j]);
                            added = true;
                            zIsGood = false;
                        }
                    }
                    if (!added) {
                        newCmtList[k].cmt_children.push(newCmtList[j]);
                    }
                    kIsGood = false;
                }
            }
        }


        for (var i = (newCmtList.length - 1); i >= 0; i--) {
            if (newCmtList[i].DepthLevel != 0) {
                newCmtList.splice(i, 1);
            }
        }

        //sort by newest on top
        newCmtList.sort(dynamicSort('-ID'));
        return newCmtList;
    }


    function showCommentReply() {
        if (rawCommentSet.length == 0) {
            return;
        }

        if (!postRawData.CanComment) {
            $("[data-reply=reply]").style('display', 'none', 'important');
            return;
        }

        var cmtInterval = window.setInterval(function () {
            var cmtBtnSet = $("[data-reply=reply]");
            if (cmtBtnSet.length == 0) {
                return;
            }
            for (var i = 0; i < cmtBtnSet.length; i++) {
                var depthStr = $(cmtBtnSet[i]).attr('data-cmtDepth');
                var depth = parseInt(depthStr);

                //max 5 levels of comments
                if (depth < 4) {
                    $(cmtBtnSet[i]).style('display', null);
                }
            }
            window.clearInterval(cmtInterval);
        }, 10);
    }


    Vue.component('comment-component', {
        props: ['comment'],
        template: '<div class="container">' +
            '            <div class="container-fluid">' +
            '                <div>' +
            '                    <span class="m-2 mr-0" style="margin-right:0 !important">Posted by</span>' +
            '                    <span v-if="comment.CreatedBy > 0">[<a v-bind:href="\'/Account/find/\' + comment.CreatedBy">{{ comment.Username }}</a>]</span>' +
            '                    <span v-else>[<a href="/Account">{{ comment.Username }}</a>]</span>' +
            '                    <template v-if="comment.dateCreatedFromNow != null">' +
            '                       <span>{{comment.dateCreatedFromNow}}</span>' +
            '                    </template>' +
            '                    <span v-else>{{ comment.postTime }}</span>' +
            '                </div>' +
            '                <div class="border border-dark rounded m-2 py-2">' +
            '                    <span class="text-body p-2">' +
            '                        {{ comment.Content }}' +
            '                    </span>' +
            '                </div>' +
            '                <button type="button" class="btn-sm btn-outline-dark m-2 mb-1" v-on:click="emit(comment.ID)" data-reply="reply" :data-cmtDepth="comment.DepthLevel" style="display:none !important">Reply</button>' +
            '                <div>' +
            '                    <div v-bind:data-cmtID="comment.ID" class="form-group" style="display: none;">' +
            '                        <textarea rows="2" class="mx-auto form-control" ref="commentReply"></textarea>' +
            '                        <button v-bind:data-submitID="comment.ID" type="button" class="btn-sm btn-outline-primary m-2 mb-1" v-on:click="submitComment">Submit</button>' +
            '                    </div>' +
            '                </div>' +
            '               <div>' +
            '                   <comment-component v-for="comment in comment.cmt_children" v-on:custom-click="buttonHandler" :key="comment.id" v-bind:comment="comment"><comment-component>' +
            '               </div>' +
            '            </div>' +
            '        </div>',
        methods: {
            emit: function (cmtID) {
                this.$emit('custom-click', cmtID);
            },
            buttonHandler: function (commentID) {
                $("[data-cmtID=" + commentID + "]").toggle();
            },
            submitComment: function () {

                var formData = {
                    Content: this.$refs.commentReply.value,
                    ParentID: this.comment.ID
                };

                var jsonData = JSON.stringify(formData);


                execCreateComment(jsonData)
                    //AJAX -> /uhubapi/comments/Create
                    .done(function (data) {
                        $("[data-cmtID=" + formData.ParentID + "] textarea").val("");
                        $("[data-cmtID=" + formData.ParentID + "]").toggle();


                        var now = moment();
                        var postTime = now.format("YYYY-MM-DD HH:mm");
                        var cmtTimeMoment = now;
                        var fromNow = null;
                        if (parseInt(now.diff(cmtTimeMoment, 'days')) < 7) {
                            fromNow = cmtTimeMoment.fromNow();
                        }


                        var newCommentCreatorID = -1;
                        var newCommentCreatorName = "me";
                        if (currentUser != null) {
                            newCommentCreatorID = currentUser.ID;
                            newCommentCreatorName = currentUser.Username;
                        }


                        var newCmt = {
                            ID: data,
                            ParentID: formData.ParentID,
                            CreatedBy: newCommentCreatorID,
                            Username: newCommentCreatorName,
                            CreatedDate: getToday(),
                            postTime: postTime,
                            dateCreatedFromNow: fromNow,
                            Content: formData.Content,
                            IsEnabled: true
                        };

                        rawCommentSet.push(newCmt);
                        var cmtArrangedList = arrangeCommentTree(rawCommentSet);
                        vueInstance.comments = cmtArrangedList;

                        showCommentReply();
                    });

            }
        }
    });



    new Vue({
        el: "#post-container",
        data: {
            postID: postID,
            parentID: "",
            commName: "",
            postCreater: "",
            createdBy: "",
            title: "",
            content: "",
            postCanComment: false,
            postTime: "",
            modifiedDate: "",
            dateCreatedFromNow: null,
            likeCount: -1,
            comments: []
        },
        methods: {
            buttonHandler: function (commentID) {
                $("[data-cmtID=" + commentID + "]").toggle();
            },
            postReply: function () {
                $("#post-reply").toggle();
            },
            submitCommentPost: function () {


                var formData = {
                    Content: this.$refs.postReplyText.value,
                    ParentID: postID
                };

                var jsonData = JSON.stringify(formData);


                execCreateComment(jsonData)
                    //AJAX -> /uhubapi/comments/Create
                    .done(function (data) {
                        $("#post-reply textarea").val("");
                        $("#post-reply").toggle();


                        var now = moment();
                        var postTime = now.format("YYYY-MM-DD HH:mm");
                        var cmtTimeMoment = now;
                        var fromNow = null;
                        if (parseInt(now.diff(cmtTimeMoment, 'days')) < 7) {
                            fromNow = cmtTimeMoment.fromNow();
                        }

                        var newCommentCreatorID = -1;
                        var newCommentCreatorName = "me";
                        if (currentUser != null) {
                            newCommentCreatorID = currentUser.ID;
                            newCommentCreatorName = currentUser.Username;
                        }


                        var newCmt = {
                            ID: data,
                            ParentID: formData.ParentID,
                            CreatedBy: newCommentCreatorID,
                            Username: newCommentCreatorName,
                            CreatedDate: getToday(),
                            postTime: postTime,
                            dateCreatedFromNow: fromNow,
                            Content: formData.Content,
                            IsEnabled: true
                        };

                        rawCommentSet.push(newCmt);
                        var cmtArrangedList = arrangeCommentTree(rawCommentSet);
                        vueInstance.comments = cmtArrangedList;

                        showCommentReply();
                    });

            }
        },
        mounted: function () {
            vueInstance = this;
            var self = this;


            $.ajax({
                method: "POST",
                url: "/uhubapi/posts/GetByID?PostID=" + encodeURIComponent(postID)
            })
                ///AJAX -> uhubapi/posts/GetByID
                .done(function (pstData) {
                    postRawData = pstData;

                    var clubID = pstData.ParentID;

                    self.parentID = pstData.ParentID;
                    self.title = htmlEncode(pstData.Name);
                    window.setTimeout(function () {
                        simplemde.value(pstData.Content);
                        //set old value to prevent clean updates
                        oldResponseErr = 'Nothing to Update';
                        jsonPostDataOld = JSON.stringify(getPostUpdateData());
                    }, 1);
                    self.postCanComment = pstData.CanComment;
                    self.postTime = moment(pstData.CreatedDate).local().format("YYYY-MM-DD HH:mm");
                    self.createdBy = pstData.CreatedBy;
                    self.modifiedDate = pstData.ModifiedDate;
                    self.postCreater = pstData.Username;
                    self.likeCount = pstData.LikeCount;


                    var postTimeMoment = moment(pstData.CreatedDate);
                    var now = moment();
                    if (parseInt(now.diff(postTimeMoment, 'days')) < 7) {
                        self.dateCreatedFromNow = postTimeMoment.fromNow();
                    }

                    $("#post-container").style('display', null);
                    if (pstData.CanComment) {
                        $("#btn_ToggleReply").style('display', null);
                    }


                    //fetch comments if necessary
                    if (self.title != undefined && self.title != null && self.title != "") {
                        $.ajax({
                            method: "POST",
                            url: "/uhubapi/comments/GetByPost?PostID=" + encodeURIComponent(postID)
                        })
                            //AJAX -> /uhubapi/comments/GetByPost
                            .done(function (cmtData) {
                                rawCommentSet = cmtData;
                                rawCommentSet.forEach(comment => {
                                    comment.postTime = moment(comment.CreatedDate).local().format("YYYY-MM-DD HH:mm");

                                    var cmtTimeMoment = moment(comment.CreatedDate);
                                    var now = moment();
                                    if (parseInt(now.diff(cmtTimeMoment, 'days')) < 7) {
                                        comment.dateCreatedFromNow = cmtTimeMoment.fromNow();
                                    }
                                });
                                var cmtArrangedList = arrangeCommentTree(rawCommentSet);
                                //console.log(JSON.parse(JSON.stringify(cmtArrangedList)));

                                self.comments = cmtArrangedList;

                                showCommentReply();


                            })
                            //AJAX -> /uhubapi/comments/GetByPost
                            .fail(function (jqAjax, errorText) {
                                alert("Error" + errorText);
                            });
                    }




                    //set navbar title to current club
                    var comms = $("#navbarDropdownMenu a");
                    var commLen = comms.length;
                    for (var i = 0; i < commLen; i++) {
                        if (clubID == $(comms[i]).attr("data-ClubID")) {
                            $("#navbarDropdownMenuLink").text($(comms[i]).text());
                        }
                    }


                    //get club info
                    $.ajax({
                        method: "POST",
                        url: "/uhubapi/schoolclubs/GetByID?ClubID=" + clubID
                    })
                        //-->/uhubapi/schoolclubs/GetByID
                        .done(function (clubData) {

                            self.commName = clubData.Name;

                        })
                        //-->/uhubapi/schoolclubs/GetByID
                        .fail(function (jqAjax, errorText) {
                            alert("Error" + errorText);
                        });


                    //get current user
                    $.ajax({
                        method: "POST",
                        url: "/uhubapi/Users/GetMe"
                    })
                        .done(function (data) {
                            currentUser = data;
                        });


                })
                ///AJAX -> uhubapi/posts/GetByID
                .fail(function (jqAjax, errorText) {
                    alert("Error " + errorText);
                });
        }
    });


    simplemde = new SimpleMDE(
        {
            autosave: {
                enabled: false
            },
            element: document.getElementById("txt_PostArea"),
            previewRender: function (plainText) {
                return mdConverter.makeHtml(plainText);
            },
            promptURLs: true,
            spellChecker: true,
            status: ["lines", "words",
                {
                    className: "charCount",
                    defaultValue: function (el) {
                        this.charCount = 0;
                        el.innerHTML = "Characters: 0 (10 - 10k)";
                    },
                    onUpdate: function (el) {
                        var ct = simplemde.value().length;
                        el.innerHTML = "Characters: " + ct + " (10 - 10k)";
                    }
                },
                {
                    className: "viewAs",
                    defaultValue: function (el) {
                        el.innerHTML = '[<a href="?Preview" target="_blank">View As</a>]';
                    }
                }]
        });




    function setWaitState() {
        $("#btn_UpdatePost").attr("disabled", "disabled");
        $("html").css({ cursor: "default" });
    }

    function clearWaitState() {
        $("#btn_UpdatePost").removeAttr("disabled");
        $("html").css({ cursor: "default" });
    }


    function processInputValidation(formData) {
        if (!formData.Content.match(RgxPtrns.Post.CONTENT)) {
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
            return false;
        }



        $.ajax({
            method: "POST",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonPostData,
            url: "/uhubapi/posts/Update"
        })
            //AJAX -> /uhubapi/posts/Update
            .done(function (data) {
                oldResponseErr = 'Nothing to Update';

                if (postRawData.CanComment) {
                    $("#btn_ToggleReply").style('display', null);
                }
                showCommentReply();

                alert(data);
            })
            //AJAX -> /uhubapi/posts/Update
            .fail(function (data) {
                oldResponseErr = data.responseJSON;
                alert(oldResponseErr);
            })
            //AJAX -> /uhubapi/posts/Update
            .always(function (data) {
                clearWaitState();
            });
    }



    $("#btn_UpdatePost").click(function () {
        var data = getPostUpdateData();
        sendData(data);
    });




    registerInputValidator($("#txt_PostArea"), RgxPtrns.Post.CONTENT);



})();
