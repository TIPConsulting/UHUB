new Vue({
    el: "#club-list",
    data: {
        clubs: []
    },
    mounted: function () {
        var self = this;

        $.ajax({
            method: "POST",
            url: "/uhubapi/SchoolClubs/GetAllBySchool"
        })
            //AJAX -> /uhubapi/SchoolClubs/GetAllBySchool
            .done(function (data) {
                self.clubs = data;
                self.clubs.sort(dynamicSort("Name"));

                $("#club-list").style('display', null);
            })
            //AJAX -> /uhubapi/SchoolClubs/GetAllBySchool
            .fail(function (error) {
                console.log(error);
            });
    }
});
