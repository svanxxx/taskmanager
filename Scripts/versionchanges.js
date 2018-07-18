$(function () {
	var app = angular.module('mpsapplication', []);
	app.controller('mpscontroller', ["$scope", "$http", function ($scope, $http) {
		$scope.versions = [];
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
						if (match != null) {
							ttid = match[0];
							ttid = ttid.substring(2);
							summary = match.input;
						}
						$scope.versions[$scope.versions.length - 1].changes.push({ ttid: ttid, summary: summary })
					}
				}
			})
	}]);
})