$(function () {
	var app = angular.module('mpsapplication', []);
	app.controller('mpscontroller', ["$scope", "$http", function ($scope, $http) {

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
			if ($scope.branches.length === 0) {
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
		$scope.loadData = function () {
			var prg = StartProgress("Loading data...");
			$http.post("trservice.asmx/enumbranches", JSON.stringify({ from: $scope.showby * ($scope.page - 1) + 1, to: $scope.showby * $scope.page }))
				.then(function (result) {
					$scope.branches = result.data.d;
					EndProgress(prg);
				});
		};

		var page = getParameterByName("page");
		var showby = getParameterByName("showby");
		$scope.page = 1;
		$scope.showby = "15";
		$scope.showbys = ["15", "30", "60", "120"];
		if (page !== "") {
			$scope.page = parseInt(page);
		}
		if (showby !== "") {
			$scope.showby = "" + parseInt(showby);
		}
		$scope.loadData();
	}]);
});