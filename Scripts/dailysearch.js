$(function () {
	var app = angular.module('mpsapplication', []);
	app.filter('rawHtml', ['$sce', function ($sce) {
		return function (val) {
			return $sce.trustAsHtml(val);
		};
	}]);
	app.controller('mpscontroller', ["$scope", "$http", function ($scope, $http) {
		window.addEventListener("popstate", function (event) {
			$scope.state = event.state;
			$scope.$apply();
		});
		$scope.pushState = function () {
			var url = replaceUrlParam(location.href, "filter", JSON.stringify($scope.state.filter));
			window.history.pushState(JSON.parse(JSON.stringify($scope.state)), "", url);
		};
		$scope.loadData = function (pop) {
			if (!pop) {
				$scope.pushState();
			}

			$scope.state.reports = [];
			var d1 = DateToString($scope.state.filter.startdate);
			var diff = Math.round(($scope.state.filter.enddate - $scope.state.filter.startdate) / (24 * 3600 * 1000));
			var repsprg = StartProgress("Loading reports...");
			var txt = $scope.state.filter.text;
			if (txt === undefined) txt = "";
			$http.post("trservice.asmx/getreports4Person", JSON.stringify({ 'personid': $scope.state.filter.userid, start: d1, days: diff, text: txt}))
				.then(function (result) {
					$scope.state.reports = result.data.d;
					var reps = $scope.state.reports;
					for (var i = 0; i < reps.length; i++) {
						var lines = reps[i].DONE.split(/\r?\n/);
						reps[i].DONE = [];
						for (var j = 0; j < lines.length; j++) {
							var l = lines[j];
							if (!l) {
								continue;
							}
							var match = l.match("TT\\d+");
							if (match !== null) {
								var ttid = match[0];
								ttid = ttid.substring(2);
								var url = '<a href="showtask.aspx?ttid=' + ttid + '" target="_blank"><span class="badge badge-pill badge-secondary">TT' + ttid + '</span></a>';
								reps[i].DONE.push(l.replace(match[0], url));
								continue;
							}
							reps[i].DONE.push(l);
						}
					}
					EndProgress(repsprg);
				});
		};
		$scope.onGo = function (key) {
			if (key.which === 13) {
				$scope.loadData(false);
				key.preventDefault();
			}
		};
		$scope.mpsusers = [];
		getMPSUsers($scope, "mpsusers", $http, function () {
			$scope.state = {};
			$scope.state.reports = [];
			$scope.state.filter = createDSFilter(userID());
			var fltr = getParameterByName("filter");
			if (fltr !== "") {
				$scope.state.filter = JSON.parse(fltr);
				$scope.state.filter.startdate = new Date($scope.state.filter.startdate);
				$scope.state.filter.enddate = new Date($scope.state.filter.enddate);
				$scope.state.filter.userid = "" + $scope.state.filter.userid;
			}
			$scope.loadData(false);
		});
	}]);
});