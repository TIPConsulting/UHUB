(function () {
    var mdConverter = new showdown.Converter();
    setShowdownDefaults(mdConverter);

    $.ajax({
        method: "GET",
        url: "/uhubapi/schools/GetMine"
    })
        .done(function (data) {
            $("#school-name").text(data.Name);
        });

    new Vue({
        el: "#post-list",
        data: {
            posts: []
        },
        mounted: function () {
            var self = this;


            $.ajax({
                method: "POST",
                url: "/uhubapi/posts/GetAllBySchool"
            })
                //AJAX -> /uhubapi/posts/GetAllBySchool
                .done(function (data) {

                    if (data.length > 0) {
                        data.sort(dynamicSort("-CreatedDate"));

                        //set parentName
                        var clubSet = $("#navbarDropdownMenu a.dropdown-item");
                        var parentNameDict = {}
                        for (var j = 0; j < clubSet.length; j++) {
                            var key = parseInt($(clubSet[j]).attr("data-clubID"));
                            var val = $(clubSet[j]).text();

                            parentNameDict[key] = val;
                        }


                        for (var i = 0; i < data.length; i++) {
                            data[i].Content = mdConverter.makeHtml(data[i].Content);

                            
                            var now = moment();
                            var postTimeMoment = moment(data[i].CreatedDate);
                            data[i].postTime = postTimeMoment.local().format("YYYY-MM-DD HH:mm");
                            if (parseInt(now.diff(postTimeMoment, 'days')) < 7) {
                                data[i].dateCreatedFromNow = postTimeMoment.fromNow();
                            }


                            var parentName = parentNameDict[data[i].ParentID];
                            if (parentName != undefined) {
                                data[i].ClubName = parentName;
                            }
                        }


                        self.posts = data;
                    }
                    else {
                        self.posts = [{
                            Name: "Nothing To See Here",
                            Content: "This school currently does not have any posts"
                        }];
                    }




                })
                //AJAX -> /uhubapi/posts/GetAllBySchool
                .fail(function (error) {
                    console.log(error);

                    self.posts = [{
                        Name: "Nothing To See Here",
                        Content: "Unfortunately, an error occured while fetching posts"
                    }];
                })
                //AJAX -> /uhubapi/posts/GetAllBySchool
                .always(function () {
                    $("#post-list").style('display', null);

                });
        }
    });

})();