(function () {
    var postID = encodeURIComponent(window.location.href.split('/').slice(-1)[0]);
    postID = postID.match(/^[0-9]+/)[0];

    var mdConverter = new showdown.Converter();
    setShowdownDefaults(mdConverter);


    new Vue({
        el: "#post-versions",
        data: {
            postVersions: []
        },
        mounted: function () {
            var self = this;
            $.ajax({
                method: "POST",
                url: "/uhubapi/posts/GetRevisionsByID?PostID=" + encodeURIComponent(postID)
            })
            //AJAX -> /uhubapi/posts/GetRevisionsByID
            .done(function (data) {
                if (data.length > 1) {

                    for (var i = 0; i < data.length; i++) {
                        data[i].Content = mdConverter.makeHtml(data[i].Content);
                    }
                    data.sort(dynamicSort("-ModifiedDate"));

                    self.postVersions = data;
                } else {
                    self.postVersions = [{
                        Name: "Nothing To See Here",
                        Content: "This post only has one version."
                    }];
                }
            })
            //AJAX -> /uhubapi/posts/GetRevisionsByID
            .fail(function (error) {
                console.log(error);

                self.postVersions = [{
                    Name: "Nothing To See Here",
                    Content: "Unfortunately, an error occured while fetching this post's previous versions"
                }];
            })
            //AJAX -> /uhubapi/posts/GetRevisionsByID
            .always(function () {
                $("#post-versions").style('display', null);
            });
        }
    });
})();