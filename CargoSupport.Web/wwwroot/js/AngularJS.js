var app = angular.module('myApp', []);

app.controller('dataController', function ($scope, $http) {
    $http.get("/api/public/get")
        .then(function (response) { $scope.names = response.data; $scope.extractDrivers(response.data); });

    $scope.extractDrivers = function (allObjects) {
        let arrayOfDrivers = []
        for (var i = 0; i < allObjects.length; i++) {
            arrayOfDrivers.push(allObjects[i].driver)
        }
        $scope.driverName = arrayOfDrivers;
    }

    $scope.refreshData = function () {
        console.log("Data updated remotely!");
        $http.get("/Routes/GetRoutes")
            .then(function (response) { $scope.names = response.data; });
    }
    $scope.updateBooleans = function (id, object) {
        $http.post('/Routes/EditDriver/', object)
            .then(function () { UpdateMessage(); });
    }
    $scope.updateDriverNames = function (id, object) {
    }
});