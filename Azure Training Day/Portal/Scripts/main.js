(function () {
    var app = angular.module("Contacts", []);

    app.controller("MainCtrl", function($scope, $http) {

        function refresh() {
            $http.get("/api/Contacts").then(function(r) {
                $scope.contacts = r.data;
            });
        }

        refresh();

        $.connection.newContactsHub.client.notify =
            function (name) {
                
                toastr.info("Welcome " + name + "!");
                refresh();
            };
        $.connection.hub.start();

    });
})();