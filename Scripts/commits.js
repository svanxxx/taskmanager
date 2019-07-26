$(function () {
	var app = angular.module('mpsapplication', []);
	app.filter('rawHtml', ['$sce', rawHtml]);

	app.controller('mpscontroller', ["$scope", "$http", function ($scope, $http) {
		$scope.loadData = function () {
			var prg = StartProgress("Loading data...");
			$scope.branch = getParameterByName("branch");
			$http.post("GitService.asmx/EnumCommits", JSON.stringify({ branch: $scope.branch, from: $scope.commitsstate.showby * ($scope.commitsstate.page - 1) + 1, to: $scope.commitsstate.showby * $scope.commitsstate.page }))
				.then(function (result) {
					$scope.commits = result.data.d;
					reActivateTooltips();
					EndProgress(prg);
				});
		};
		$scope.loadCommit = function (c, member) { loadCommit(c, $scope, $http, member); };
		$scope.pushState = function () {
			var url = replaceUrlParam(replaceUrlParam(location.href, "showby", $scope.commitsstate.showby), "page", $scope.commitsstate.page);
			window.history.pushState({ showby: $scope.commitsstate.showby, page: $scope.commitsstate.page }, "page " + $scope.commitsstate.page + ", showby " + $scope.commitsstate.showby, url);
		};
		$scope.decPage = function () {
			if ($scope.commitsstate.page === 1) {
				return;
			}
			$scope.commitsstate.page--;
			$scope.loadData();
			$scope.pushState();
		};
		$scope.incPage = function () {
			if ($scope.commits.length === 0) {
				return;
			}
			$scope.commitsstate.page++;
			$scope.loadData();
			$scope.pushState();
		};
		$scope.changeShowBy = function () {
			$scope.loadData();
			$scope.pushState();
		};
		var page = getParameterByName("page");
		var showby = getParameterByName("showby");
		$scope.commitsstate = {};
		$scope.commitsstate.page = 1;
		$scope.commitsstate.showby = "10";
		$scope.commitsstate.showbys = ["10", "15", "30", "60", "120"];
		if (page !== "") {
			$scope.commitsstate.page = parseInt(page);
		}
		if (showby !== "") {
			$scope.commitsstate.showby = "" + parseInt(showby);
		}

		$scope.loadData();
	}]);
});