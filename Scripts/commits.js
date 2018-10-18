$(function () {
	var app = angular.module('mpsapplication', []);
	app.controller('mpscontroller', ["$scope", "$http", function ($scope, $http) {
		var prg = StartProgress("Loading data...");
		$scope.branch = getParameterByName("branch");
		$http.post("trservice.asmx/EnumCommits", JSON.stringify({ branch: $scope.branch }))
			.then(function (result) {
				$scope.commits = result.data.d;
				EndProgress(prg);
			});
	}]);
});