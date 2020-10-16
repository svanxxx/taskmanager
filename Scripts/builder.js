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
		$scope.readonly = !IsAdmin();
		$scope.gitStatus = "";
		$scope.psStatus = "";
		$scope.pushStatus = "";
		$scope.progress = function () {
			return inProgress();
		};
		$interval(function () {
			$http.post("BuildService.asmx/GetBuilderID", JSON.stringify({}))
				.then(function (result) {
					$scope.lockid = result.data.d;
					$scope.readonly = !IsAdmin() || ($scope.lockid != null && $scope.lockid != userID());
				});
		}, 2000);
		$scope.start = function () {
			$scope.gitStatus = "";
			$scope.psStatus = "";
			$scope.pushStatus = "";
			var prg = StartProgress("Loading updates...");
			$http.post("BuildService.asmx/getUpdateWorkGit", JSON.stringify({}))
				.then(function (result) {
					$scope.gitStatus = result.data.d;
					EndProgress(prg);
				});
		};
		$scope.release = function () {
			$scope.pushStatus = "";
			var prg = StartProgress("Incrementing version...");
			$http.post("BuildService.asmx/versionIncrement", JSON.stringify({}))
				.then(function (result) {
					$scope.psStatus = result.data.d;
					EndProgress(prg);
				});
		};
		$scope.push2Master = function () {
			var prg = StartProgress("Pushing to master...");
			$http.post("BuildService.asmx/push2Master", JSON.stringify({}))
				.then(function (result) {
					$scope.pushStatus = result.data.d;
					EndProgress(prg);
				});
		};
		$scope.applySchedule = function () {
			var prg = StartProgress("Applying schedule...");
			var sb = JSON.parse(JSON.stringify($scope.scheduledBuild));
			var dt = (new Date(sb.TIME));
			sb.TIME = dt.getHours() * 60 * 60 * 1000 + dt.getMinutes() * 60 * 1000;
			$http.post("BuildService.asmx/setSchedule", JSON.stringify({ sb: sb }))
				.then(function (result) {
					$scope.setSchedule(result.data.d);
					EndProgress(prg);
				});
		};
		$scope.setSchedule = function (ret) {
			$scope.scheduledBuild = ret;
			var d = new Date(0, 0, 0, 0, 0, 0, $scope.scheduledBuild.TIME);
			$scope.scheduledBuild.TIME = d;
			$('.toast').toast('show');
		}
		$scope.scheduledBuild = [];
		$http.post("BuildService.asmx/getSchedule", JSON.stringify({}))
			.then(function (result) {
				$scope.setSchedule(result.data.d);
			});
	}]);
});