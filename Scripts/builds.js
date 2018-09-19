$(function () {
	var app = angular.module('mpsapplication', []);
	app.controller('mpscontroller', ["$scope", "$http", function ($scope, $http) {
		var prg = StartProgress("Loading data...");
		$http.post("trservice.asmx/getBuildRequests", JSON.stringify({}))
			.then(function (result) {
				$scope.builds = result.data.d;
				EndProgress(prg);
			});
	}]);
});