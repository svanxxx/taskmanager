$(function () {
	var app = angular.module('mpsapplication', []);
	app.filter('rawHtml', ['$sce', rawHtml]);

	app.controller('mpscontroller', ["$scope", "$http", function ($scope, $http) {
		$scope.loadData = function () {
			var prg = StartProgress("Loading data...");
			$scope.branch = getParameterByName("branch");
			$http.post("trservice.asmx/EnumCommits", JSON.stringify({ branch: $scope.branch, from: $scope.showby * ($scope.page - 1) + 1, to: $scope.showby * $scope.page }))
				.then(function (result) {
					$scope.commits = result.data.d;
					EndProgress(prg);
				});
		};
		$scope.loadCommit = function (c) { loadCommit(c, $scope, $http); };
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
			if ($scope.commits.length === 0) {
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