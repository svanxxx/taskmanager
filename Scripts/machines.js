$(function () {
	var app = angular.module('mpsapplication', []);
	app.controller('mpscontroller', ["$scope", "$http", function ($scope, $http) {
		var taskprg = StartProgress("Loading data...");
		$scope.workmachine = undefined;
		$scope.machines = [];
		$http.post("trservice.asmx/getMachines", JSON.stringify({}))
			.then(function (result) {
				$scope.machines = result.data.d;
				EndProgress(taskprg);
			});
		$scope.setMachine = function (m) {
			if (m == '') {
				$scope.workmachine = undefined;
			} else {
				$scope.workmachine = m;
			}
		}
		$scope.hasMachine = function () {
			return (typeof $scope.workmachine !== "undefined");
		}
	}]);
})