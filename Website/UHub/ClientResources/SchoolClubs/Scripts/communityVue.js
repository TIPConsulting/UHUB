(function () {

    var commID = encodeURIComponent(window.location.href.split('/').slice(-1)[0]);


    var mdConverter = new showdown.Converter();
    setShowdownDefaults(mdConverter);



    Vue.component('postlistcomp', {
        props: ['post'],
        template:
            `
           <li class="list-group-item mb-3" style="padding: .75rem 1.25rem">
                <div class="shadowBox"></div>                

                <template v-if="post.ID != undefined" >
                    Posted by <span>[<a v-bind:href="'/Account/find/' + post.CreatedBy">{{post.Username}}</a>]</span>
                    <template v-if="post.dateCreatedFromNow != null">
                        <span>{{post.dateCreatedFromNow}}</span>
                    </template>
                    <span v-else>{{post.postTime}}</span>
                    <p></p>
                    <a v-bind:href="'/Post/' + post.ID">
                        <h3> {{ post.Name }} </h3>
                    </a>
                </template>
                <h3 v-else> {{ post.Name }} </h3>

                <div v-html="post.Content"></div>
           </li>
        `
    });



    var vueClubInfo = new Vue({
        el: "#community-block",
        data: {
            community: {}
        },
        methods: {
            getCommunity() {
                var self = this;

                loadVueClubInfo();
            }
        },
        beforeMount() {
            this.getCommunity();
        }
    });


    var vuePostSet = new Vue({
        el: "#post-list",
        data: {
            posts: []
        },
        mounted: function () {
            var self = this;

            loadVuePostData();
        }
    });


    function loadVueClubInfo() {
        $.ajax({
            method: "POST",
            url: "/uhubapi/schoolclubs/GetByID?ClubID=" + encodeURIComponent(commID),
            statusCode: {
                200: function (data) {

                    $(navbarDropdownMenuLink).text(data.Name);
                    $("#club-name").text(data.Name);

                    vueClubInfo.community = data;
                },
                503: function () {
                    console.log("Internal Server Error");
                }
            }
        })
            .fail(function (error) {
                console.log(error);
            });
    }


    function loadVuePostData() {

        $("#body-content").style('display', "none", "important");

        $.ajax({
            method: "POST",
            url: "/uhubapi/posts/GetAllByClub?ClubID=" + encodeURIComponent(commID)
        })
            //AJAX -> /uhubapi/posts/GetAllByClub
            .done(function (formData) {

                if (formData.length > 0) {

                    for (var i = 0; i < formData.length; i++) {
                        formData[i].Content = mdConverter.makeHtml(formData[i].Content);


                        var now = moment();
                        var postTimeMoment = moment(formData[i].CreatedDate);
                        formData[i].postTime = postTimeMoment.local().format("YYYY-MM-DD HH:mm");
                        if (parseInt(now.diff(postTimeMoment, 'days')) < 7) {
                            formData[i].dateCreatedFromNow = postTimeMoment.fromNow();
                        }

                    }
                    formData.sort(dynamicSort("-CreatedDate"));
                    vuePostSet.posts = formData;

                }
                else {
                    vuePostSet.posts = [{
                        Name: "Nothing To See Here",
                        Content: "This club currently does not have any posts"
                    }];
                }

            })
            //AJAX -> /uhubapi/posts/GetAllByClub
            .fail(function (error) {
                console.log(error);

                vuePostSet.posts = [{
                    Name: "Nothing To See Here",
                    Content: "Unfortunately, an error occured while fetching posts"
                }];
            })
            //AJAX -> /uhubapi/posts/GetAllByClub
            .always(function () {

                $("#body-content").style('display', null);

            });
    }


    //dynamic content reloading
    //loads new community without refreshing the page
    $("#navbarDropdownMenu span.dropdown-item").click(function () {

        var id = parseInt($(this).attr("data-ClubID"));

        if (id > 0) {
            commID = id;
            window.history.pushState('School Club ' + id, 'School Club', '/SchoolClub/' + id);

            $("#btn_HeaderCreatePost").attr('href', '/SchoolClub/CreatePost/' + id);

            loadVuePostData();
            loadVueClubInfo();
        }

    });


})();