$(function () {
	var app = angular.module('mpsapplication', []);
	app.controller('mpscontroller', ["$scope", "$http", function ($scope, $http) {
		window.addEventListener("popstate", function (event) {
			if (event.state) {
				$scope.state = JSON.parse(JSON.stringify(event.state));
			} else {
				$scope.init();
				$scope.loadData(false);
			}
			$scope.$apply();
		});
		$scope.loadData = function (pushstate) {
			var prg = StartProgress("Loading data...");
			$http.post("trservice.asmx/enumbranches", JSON.stringify({ from: $scope.state.showby * ($scope.state.page - 1) + 1, to: $scope.state.showby * $scope.state.page, user: $scope.state.filter }))
				.then(function (result) {
					$scope.state.branches = result.data.d;
					EndProgress(prg);
					if (pushstate) {
						$scope.pushState();
					}
				});
		};
		$scope.pushState = function () {
			var url = replaceUrlParam(replaceUrlParam(replaceUrlParam(location.href, "showby", $scope.state.showby), "page", $scope.state.page), "filter", $scope.state.filter);
			window.history.pushState(JSON.parse(JSON.stringify($scope.state)), "page " + $scope.state.page + ", showby " + $scope.state.showby + ", filter " + $scope.state.filter, url);
		};
		$scope.decPage = function () {
			if ($scope.state.page === 1) {
				return;
			}
			$scope.state.page--;
			$scope.loadData(true);
		};
		$scope.incPage = function () {
			if ($scope.state.branches.length === 0 || $scope.state.branches.length < parseInt($scope.state.showby)) {
				return;
			}
			$scope.state.page++;
			$scope.loadData(true);
		};
		$scope.changeShowBy = function () {
			$scope.loadData(true);
		};
		$scope.filterby = function (name) {
			$scope.state.page = 1;
			$scope.state.filter = name;
			$scope.loadData(true);
		};
		$scope.init = function () {
			var page = getParameterByName("page");
			var showby = getParameterByName("showby");
			$scope.state = {};
			$scope.state.page = 1;
			$scope.state.showby = "15";
			$scope.state.filter = getParameterByName("filter");
			$scope.showbys = ["15", "30", "60", "120"];
			if (page !== "") {
				$scope.state.page = parseInt(page);
			}
			if (showby !== "") {
				$scope.showby = "" + parseInt(showby);
			}
		};
		$scope.init();
		$scope.loadData(false);
	}]);
});