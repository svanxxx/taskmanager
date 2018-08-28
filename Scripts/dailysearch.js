$(function () {
	var app = angular.module('mpsapplication', []);
	app.filter('rawHtml', ['$sce', function ($sce) {
		return function (val) {
			return $sce.trustAsHtml(val);
		};
	}]);
	app.controller('mpscontroller', ["$scope", "$http", function ($scope, $http) {
		window.addEventListener("popstate", function (event) {
			$scope.selectedpersonID = event.state;
			if (!$scope.selectedpersonID) {
				$scope.selectedpersonID = userID();
			}
			$scope.loadData(true);
		});

		$scope.mpsusers = [];

		$scope.startdate = new Date();
		$scope.startdate.setDate(1);
		$scope.startdate.setHours(0, 0, 0, 0);
		$scope.enddate = new Date($scope.startdate.getFullYear(), $scope.startdate.getMonth() + 1, 0);
		$scope.selectedpersonID = userID();
		$scope.addHistory = function () {
			window.history.pushState($scope.selectedpersonID, "userid:" + $scope.selectedpersonID, replaceUrlParam(location.href, "userid", $scope.selectedpersonID));
		};

		$scope.loadData = function (pop) {
			if (!pop) {
				$scope.addHistory();
			}

			$scope.reports = [];
			var d1 = DateToString($scope.startdate);
			var diff = ($scope.enddate - $scope.startdate) / (24 * 3600 * 1000);
			var repsprg = StartProgress("Loading reports...");
			$http.post("trservice.asmx/getreports4Person", JSON.stringify({ 'personid': $scope.selectedpersonID, start: d1, days: diff }))
				.then(function (result) {
					$scope.reports = result.data.d;
					for (var i = 0; i < $scope.reports.length; i++) {
						var lines = $scope.reports[i].DONE.split(/\r?\n/);
						$scope.reports[i].DONE = [];
						for (var j = 0; j < lines.length; j++) {
							var l = lines[j];
							if (!l) {
								continue;
							}
							var match = l.match("TT\\d+");
							if (match != null) {
								var ttid = match[0];
								ttid = ttid.substring(2);
								var url = '<a href="showtask.aspx?ttid=' + ttid + '" target="_blank"><span class="badge">TT' + ttid + '</span></a>';
								$scope.reports[i].DONE.push(l.replace(match[0], url));
								continue;
							}
							$scope.reports[i].DONE.push(l);
						}
					}
					EndProgress(repsprg);
				});
		};

		getMPSUsers($scope, "mpsusers", $http, function () {
			$scope.selectedpersonID = getParameterByName("userid");
			if ($scope.selectedpersonID === "") {
				$scope.selectedpersonID = "" + $scope.mpsusers[0].ID;
			}
			$scope.loadData(false);
		});
	}]);
})