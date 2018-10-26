$(function () {
	var app = angular.module('mpsapplication', []);
	app.controller('mpscontroller', ["$scope", "$http", function ($scope, $http) {
		$scope.getTTLink = function () {

		}
		var prg = StartProgress("Loading data...");
		$http.post("trservice.asmx/enumbranches", JSON.stringify({}))
			.then(function (result) {
				$scope.branches = result.data.d;
				EndProgress(prg);
			});
	}]);
});