$(function () {
	var app = angular.module('mpsapplication', []);
	app.controller('mpscontroller', ["$scope", "$http", "$interval", function ($scope, $http, $interval) {
		$scope.builds = [];
		$scope.loadData = function () {
			var prg = StartProgress("Loading data...");
			$http.post("trservice.asmx/getBuildRequests", JSON.stringify({ from: $scope.showby * ($scope.page - 1) + 1, to: $scope.showby * $scope.page }))
				.then(function (result) {
					$scope.builds = result.data.d;
					$scope.updatePercent();
					EndProgress(prg);
				});
		};
		$scope.updatePercent = function () {
			for (var i = 0; i < $scope.builds.length; i++) {
				$scope.builds[i].PERCENT = 100;
				if ($scope.builds[i].DATEBUILD !== "") {
					var time = $scope.builds[i].DATEBUILD.split(" ")[1];
					var d = StringToTime(time);
					var now = new Date();
					d.setFullYear(now.getFullYear());
					d.setMonth(now.getMonth());
					d.setDate(now.getDate());
					var timeDiff = Math.abs(now.getTime() - d.getTime());
					var diffMins = Math.ceil(timeDiff / (1000 * 60));
					$scope.builds[i].PERCENT = diffMins / 30.0 * 100.0;
					if ($scope.builds[i].STARTED) {
						$scope.builds[i].DURATION = diffMins;
					}
				}
			}
		};
		$interval(function () {
			$scope.updatePercent();
		}, 5000);
		$scope.pushState = function () {
			var url = replaceUrlParam(replaceUrlParam(location.href, "showby", $scope.showby), "page", $scope.page);
			window.history.pushState({ showby: $scope.showby, page: $scope.page }, "page " + $scope.page + ", showby " + $scope.showby, url);
		};
		$scope.decPage = function () {
			if ($scope.page === 1) {
				return;
			}
			$scope.page--;
			$scope.loadData();
			$scope.pushState();
		};
		$scope.incPage = function () {
			if ($scope.builds.length === 0) {
				return;
			}
			$scope.page++;
			$scope.loadData();
			$scope.pushState();
		};
		$scope.changeShowBy = function () {
			$scope.loadData();
			$scope.pushState();
		};
		var page = getParameterByName("page");
		var showby = getParameterByName("showby");
		$scope.page = 1;
		$scope.showby = "5";
		$scope.showbys = ["5", "10", "15", "30", "60", "120"];
		if (page !== "") {
			$scope.page = parseInt(page);
		}
		if (showby !== "") {
			$scope.showby = "" + parseInt(showby);
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