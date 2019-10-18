$(function () {
    var app = angular.module('mpsapplication', []);
    app.filter('getDispoColorById', getDispoColorById);

    app.controller('mpscontroller', ["$scope", "$http", function ($scope, $http) {
        $scope.getDispoColor = function () {
            if ($scope.defect && $scope.dispos) {
                return "background-color:" + $scope.dispos.filter(function (x) { return x.ID == $scope.defect.DISPO; })[0].COLOR;
            }
            return "";
        };

        getDispos($scope, "dispos", $http);

		$scope.loadData = function () {
			var taskprg = StartProgress("Loading data...");
            EndProgress(taskprg);
		};
		$scope.loadData();
	}]);
});