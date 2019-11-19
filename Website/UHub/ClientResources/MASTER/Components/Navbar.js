(function () {

    var communities = [];
    var clubName = "Communities";
    var url = window.location.href;
    var currClubStr = url.split('/').slice(-1)[0];
    var urlRgxMatch = currClubStr.match(/^[0-9]+/);
    currClubID = -1;
    if (urlRgxMatch != null) {
        currClubID = urlRgxMatch[0];
    }


    var isClubPage = url.toLowerCase().indexOf("/schoolclub/") > 0;

    Vue.component('navbar-component', {
        data: function () {
            $.ajax({
                method: "POST",
                url: "/uhubapi/schoolclubs/GetAllBySchool",
                async: false,
                success: function (data) {

                    communities = data;
                    communities.sort(dynamicSort("Name"));

                    //traverse through club list
                    //If the url contains a valid clubID, then display that club name in the DropDownBox
                    //ALlows users to see their curent club context
                    var commLen = communities.length;
                    for (var i = 0; i < commLen; i++) {
                        if (currClubID == communities[i].ID) {
                            clubName = communities[i].Name;
                        }
                    }

                },
                error: function (error) {
                    console.log(error);
                }
            });
            return {
                isClubPage: isClubPage,
                currentClub: currClubID,
                clubName: clubName,
                communities: communities
            };
        },
        template:
            `
        <div >
            <div id="mainNav">
                <div class="logoWrapper">
                    <a href="/School" tabindex="-1">UHUB</a>
                </div>
                <nav>
                    <div id="navbarNavDropdown">
                        <ul class="navbar-nav mr-auto">
                            <li class="nav-item active dropdown">
                                <a class="nav-link dropdown-toggle"
                                    id="navbarDropdownMenuLink"
                                    aria-haspopup="true"
                                    aria-expanded="false">
                                    {{clubName}}
                                </a>
                            </li>
                        </ul>
                    </div>
                    <div class="searchWrapper">
                        <!--<input class="form-control" type="search" placeholder="Search" class="form-control" />-->
                        <div>
                            <!--<button class="form-control" type="submit" class="btn btn-secondary">Search</button>-->
                            <a href="/Account" class="btn btn-secondary acctLnk">My Account</a>
                            <a href="/Account/Logout" class="btn btn-secondary acctLnk">Logout</a>
                        </div>
                    </div>
                    
                </nav>
                <div id="navbarDropdownMenu" class="dropdown-menu-UHUB scrollable-menu" aria-labelledby="navbarDropdownMenuLink">
                    <a class="dropdown-item" href="/School/Clubs">— View All —</a>
                    <template v-if="isClubPage">
                        <span class="dropdown-item"
                            v-for="community in communities"
                            v-bind:href="'/SchoolClub/' + community.ID"
                            v-bind:data-ClubID="community.ID">
                            {{ community.Name }}
                        </span>
                    </template>
                    <template v-else>
                        <a class="dropdown-item"
                            v-for="community in communities"
                            v-bind:href="'/SchoolClub/' + community.ID"
                            v-bind:data-ClubID="community.ID">
                            {{ community.Name }}
                        </a>
                    </template>
                </div>
            </div>
        </div>
        `
    });

    new Vue({ el: "#navbar-uhub" });



    $("*:not(#navbarDropdownMenuLink, #navbarDropdownMenuLink *, #navbarDropdownMenu, #navbarDropdownMenu *)")
        .click(function () {
            $("#navbarDropdownMenu").scrollTop(0);
            $("#navbarDropdownMenu").removeClass("show");
        });


    $("#navbarDropdownMenuLink")
        .click(function (e) {
            e.stopPropagation();
            e.preventDefault();
            if ($("#navbarDropdownMenu").hasClass("show")) {
                $("#navbarDropdownMenu").scrollTop(0);
            }
            $("#navbarDropdownMenu").toggleClass("show");
        });

})();
