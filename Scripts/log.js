$(function () {
	var app = angular.module('mpsapplication', []);
	app.controller('mpscontroller', ["$scope", "$http", function ($scope, $http) {

		$scope.page = 1;
		$scope.by = 50;
		$scope.inc = function () {
			$scope.page++;
			$scope.loadData();
		};
		$scope.dec = function () {
			if ($scope.page < 2) {
				return;
			}
			$scope.page--;
			$scope.loadData();
		};
		$scope.clear = function () {
			var taskprg = StartProgress("Loading data...");
			$http.post("trservice.asmx/clearLog", JSON.stringify({}))
				.then(function (result) {
					$scope.text = result.data.d;
					EndProgress(taskprg);
				});
		};
		$scope.loadData = function () {
			var taskprg = StartProgress("Loading data...");
			$http.post("trservice.asmx/getLog", JSON.stringify({ from: ($scope.page - 1) * $scope.by, to: ($scope.page) * $scope.by - 1 }))
				.then(function (result) {
					$scope.text = result.data.d;
					EndProgress(taskprg);
				});
		};
		$scope.loadData();
	}]);
});