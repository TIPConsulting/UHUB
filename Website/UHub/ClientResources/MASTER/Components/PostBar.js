(function () {

    Vue.component('postbar-component', {

        data: function () {
            var url = window.location.href;
            var seperated = url.split('/');
            var currCommID = seperated.slice(-1)[0];

            var createURL = "/SchoolClub/CreatePost/" + currCommID;

            return {
                createURL
            }
        },

        template:
            `
         <div id="auxNav" class="container-fullwidth">
            <ul class="nav navbar-expand-md bg-light">
                <li class="w-uhub-offset">
                <li>
                <li class="nav-item">
                    <div class="text-center">
                        <a :href="createURL" id="btn_HeaderCreatePost" class="btn btn-secondary acctLnk" style="padding:3px;margin:5px 0 5px 0">Create Post</a>
                    </div>
                </li>
            </ul>
        </div>
        `
    });

    new Vue({ el: "#postbar-uhub" });

})();
