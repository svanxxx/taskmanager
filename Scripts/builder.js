$(function () {
	var app = angular.module('mpsapplication', []);
	app.filter('rawHtml', ['$sce', function ($sce) {
		return function (val) {
			return $sce.trustAsHtml(val);
		};
	}]);
	app.controller('mpscontroller', ["$scope", "$http", function ($scope, $http) {
		$scope.progress = true;
		$scope.readonly = !IsAdmin();
		$scope.gitStatus = "";
		$scope.psStatus = "";
		$scope.pushStatus = "";
		$scope.progress = function () {
			return inProgress();
		};
		$scope.start = function () {
			$scope.gitStatus = "";
			$scope.psStatus = "";
			$scope.pushStatus = "";
			var prg = StartProgress("Loading updates...");
			$http.post("trservice.asmx/getUpdateWorkGit", JSON.stringify({}))
				.then(function (result) {
					$scope.gitStatus = result.data.d;
					EndProgress(prg);
				});
		};
		$scope.release = function () {
			$scope.pushStatus = "";
			var prg = StartProgress("Incrementing version...");
			$http.post("trservice.asmx/versionIncrement", JSON.stringify({}))
				.then(function (result) {
					$scope.psStatus = result.data.d;
					EndProgress(prg);
				});
		};
		$scope.push2Master = function () {
			var prg = StartProgress("Pushing to master...");
			$http.post("trservice.asmx/push2Master", JSON.stringify({}))
				.then(function (result) {
					$scope.pushStatus = result.data.d;
					EndProgress(prg);
				});
		};
	}]);
});