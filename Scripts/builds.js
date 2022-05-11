$(function () {
	var app = angular.module('mpsapplication', []);
	app.controller('mpscontroller', ["$scope", "$http", "$interval", function ($scope, $http, $interval) {
		InitBuildHelpers($scope, $interval, $http, undefined);
		$scope.updatePercent = function () {
			upadteBuildProgress($scope.builds, $scope.buildtime);
		};
		$interval(function () {
			$scope.updatePercent();
		}, 5000);
		$scope.pushState = function () {
			var url = replaceUrlParam(replaceUrlParam(location.href, "showby", $scope.buildsstate.showby), "page", $scope.buildsstate.page);
			window.history.pushState({ showby: $scope.buildsstate.showby, page: $scope.buildsstate.page }, "page " + $scope.buildsstate.page + ", showby " + $scope.buildsstate.showby, url);
		};
		$scope.decPage = function () {
			if ($scope.buildsstate.page === 1) {
				return;
			}
			$scope.buildsstate.page--;
			$scope.loadBuilds();
			$scope.pushState();
		};
		$scope.incPage = function () {
			if ($scope.builds.length === 0) {
				return;
			}
			$scope.buildsstate.page++;
			$scope.loadBuilds();
			$scope.pushState();
		};
		$scope.changeShowBy = function () {
			$scope.loadBuilds();
			$scope.pushState();
		};
		var page = getParameterByName("page");
		var showby = getParameterByName("showby");
		$scope.buildsstate = {};
		$scope.buildsstate.page = 1;
		$scope.buildsstate.showby = "8";
		$scope.buildsstate.showbys = ["5", "8", "10", "15", "30", "60", "120"];
		if (page !== "") {
			$scope.buildsstate.page = parseInt(page);
		}
		if (showby !== "") {
			$scope.buildsstate.showby = "" + parseInt(showby);
		}

		$scope.loadBuilds();
		$scope.conntectToBuildBroker(true);
	}]);
});