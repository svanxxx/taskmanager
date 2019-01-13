$(function () {
	var app = angular.module('mpsapplication', []);
	app.filter('rawHtml', ['$sce', rawHtml]);

	app.controller('mpscontroller', ["$scope", "$http", function ($scope, $http) {
		$scope.loadData = function () {
			var prg = StartProgress("Loading data...");
			$scope.branch = getParameterByName("branch");
			$http.post("trservice.asmx/EnumCommits", JSON.stringify({ branch: $scope.branch, from: $scope.state.showby * ($scope.state.page - 1) + 1, to: $scope.state.showby * $scope.state.page }))
				.then(function (result) {
					$scope.commits = result.data.d;
					EndProgress(prg);
				});
		};
		$scope.loadCommit = function (c) { loadCommit(c, $scope, $http); };
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
			if ($scope.commits.length === 0) {
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
		$scope.showbys = ["15", "30", "60", "120"];
		if (page !== "") {
			$scope.state.page = parseInt(page);
		}
		if (showby !== "") {
			$scope.state.showby = "" + parseInt(showby);
		}

		$scope.loadData();
	}]);
});