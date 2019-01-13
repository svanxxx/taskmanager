$(function () {
	var app = angular.module('mpsapplication', []);
	app.controller('mpscontroller', ["$scope", "$http", "$interval", function ($scope, $http, $interval) {
		$scope.buildtime = parseInt(document.getElementById("buildtime").value);
		$scope.builds = [];
		$scope.loadData = function () {
			var prg = StartProgress("Loading data...");
			$http.post("trservice.asmx/getBuildRequests", JSON.stringify({ from: $scope.state.showby * ($scope.state.page - 1) + 1, to: $scope.state.showby * $scope.state.page }))
				.then(function (result) {
					$scope.builds = result.data.d;
					$scope.updatePercent();
					EndProgress(prg);
				});
		};
		$scope.updatePercent = function () {
			upadteBuildProgress($scope.builds, $scope.buildtime);
		};
		$interval(function () {
			$scope.updatePercent();
		}, 5000);
		$scope.pushState = function () {
			var url = replaceUrlParam(replaceUrlParam(location.href, "showby", $scope.state.showby), "page", $scope.state.page);
			window.history.pushState({ showby: $scope.state.showby, page: $scope.state.page }, "page " + $scope.state.page + ", showby " + $scope.state.showby, url);
		};
		$scope.decPage = function () {
			if ($scope.state.page === 1) {
				return;
			}
			$scope.state.page--;
			$scope.loadData();
			$scope.pushState();
		};
		$scope.incPage = function () {
			if ($scope.builds.length === 0) {
				return;
			}
			$scope.state.page++;
			$scope.loadData();
			$scope.pushState();
		};
		$scope.changeShowBy = function () {
			$scope.loadData();
			$scope.pushState();
		};
		var page = getParameterByName("page");
		var showby = getParameterByName("showby");
		$scope.state = {};
		$scope.state.page = 1;
		$scope.state.showby = "15";
		$scope.showbys = ["5", "10", "15", "30", "60", "120"];
		if (page !== "") {
			$scope.state.page = parseInt(page);
		}
		if (showby !== "") {
			$scope.state.showby = "" + parseInt(showby);
		}

		$scope.loadData();

		var notifyHub = $.connection.notifyHub;
		notifyHub.client.OnBuildChanged = function () {
			$scope.loadData();
			$scope.$apply();
		};
		$.connection.hub.disconnected(function () {
			setTimeout(function () { $.connection.hub.start(); }, 5000); // Restart connection after 5 seconds.
		});
		$.connection.hub.start();
	}]);
});