$(function () {
	var app = angular.module('mpsapplication', []);
	app.filter('rawHtml', ['$sce', function ($sce) {
		return function (val) {
			return $sce.trustAsHtml(val);
		};
	}]);
	app.filter('getUserById', getUserById);
	app.controller('mpscontroller', ["$scope", "$http", "$interval", function ($scope, $http, $interval) {
		$scope.progress = true;
		$scope.lockid = null;
		$scope.gitStatus = "";
		$scope.rebaseStatus = "";
		$scope.pushStatus = "";
		$scope.branch = getParameterByName("branch");
		$scope.ttid = getParameterByName("ttid");
		$scope.readonly = !IsAdmin() || $scope.branch === "" || $scope.ttid === "";
		$scope.progress = function () {
			return inProgress();
		};
		$interval(function () {
			$http.post("BuildService.asmx/GetMergerID", JSON.stringify({}))
				.then(function (result) {
					$scope.lockid = result.data.d;
					$scope.readonly = $scope.ttid === "" || $scope.branch === "" || !IsAdmin() || ($scope.lockid != null && $scope.lockid != userID());
				});
		}, 2000);
		$scope.start = function () {
			$scope.gitStatus = "";
			$scope.rebaseStatus = "";
			$scope.pushStatus = "";
			var prg = StartProgress("Loading updates...");
			$http.post("BuildService.asmx/getUpdateMergeGit", JSON.stringify({ branch: $scope.branch }))
				.then(function (result) {
					$scope.gitStatus = result.data.d;
					EndProgress(prg);
				});
		};
		$scope.rebase = function () {
			$scope.pushStatus = "";
			var prg = StartProgress("Rebasing Branch...");
			$http.post("BuildService.asmx/rebaseBranch", JSON.stringify({ branch: $scope.branch }))
				.then(function (result) {
					$scope.rebaseStatus = result.data.d;
					EndProgress(prg);
				});
		};
		$scope.push2Master = function () {
			var prg = StartProgress("Pushing to master...");
			$http.post("BuildService.asmx/pushMerger2Master", JSON.stringify({ ttid: $scope.ttid }))
				.then(function (result) {
					$scope.pushStatus = result.data.d;
					EndProgress(prg);
				});
		};
	}]);
});