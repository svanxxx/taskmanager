$(function () {
	var app = angular.module('mpsapplication', []);
	app.filter("sumFormat", ["$sce", sumFormat]);

	app.controller('mpscontroller', ["$scope", "$http", function ($scope, $http) {
		$scope.versions = [];
		$scope["loaders"] = 0;
		$scope.GetFile = function (v, t) {
			window.open('getinstall.ashx?type=' + t + '&version=' + v.version);
		};
		$scope.copyurl = function (txt) {
			copyurl(txt);
		};
		$scope.isadmin = IsAdmin();
		$scope.alertVersion = function () {
			$http.post("BuildService.asmx/alertVersion", JSON.stringify({}))
		};

		var taskprg = StartProgress("Loading data..."); $scope["loaders"]++;
		$http.post("trservice.asmx/getVersionLog", JSON.stringify({}))
			.then(function (result) {
				for (var i = 0; i < result.data.d.length; i++) {
					var txt = result.data.d[i].trim();
					if (txt.startsWith("=")) {
						txt = txt.replace(/=/g, "").trim();
						if (txt.length > 0) {
							$scope.versions.push({ version: txt, changes: [] });
						}
					} else if (txt.length > 0 && $scope.versions.length > 0) {
						var summary = txt;
						ttid = "00000";
						var match = txt.match("TT\\d+");
						if (match !== null) {
							ttid = match[0];
							ttid = ttid.substring(2);
							summary = match.input.substring(2 + ttid.length, match.input.length);
						}
						$scope.versions[$scope.versions.length - 1].changes.push({ ttid: ttid, summary: summary });
					}
				}
				$scope.versions.reverse();
				EndProgress(taskprg); $scope["loaders"]--;
			});
	}]);
})