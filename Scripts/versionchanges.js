$(function () {
	var app = angular.module('mpsapplication', []);
	app.filter("sumFormat", ["$sce", sumFormat]);

	app.controller('mpscontroller', ["$scope", "$http", "$timeout", function ($scope, $http, $timeout) {
		$scope.isClient = document.getElementById("isclient").value.toLowerCase() == "true";
		$scope.versions = [];
		$scope["loaders"] = 0;
		$scope.GetFile = function (v, t) {
			window.open('getinstall.ashx?type=' + t + '&version=' + v.version);
		};
		$scope.copyurl = function (txt) {
			copyurl(replaceUrlParam(location.href, "version", txt));
		};
		$scope.isadmin = IsAdmin();
		$scope.alertVersion = function () {
			$http.post("BuildService.asmx/alertVersion", JSON.stringify({}));
		};

		$scope.testFilter = document.getElementById("userfilter").value;
		$scope.tests;

		$scope.selectTestFilter = function (filter) {
			$scope.tests = undefined;
			$scope.testFilter = filter;
			$scope.loadTests();
		};
		$scope.selectTestVersion = function (version) {
			$scope.tests = undefined;
			$scope.testVersion = version;
			$scope.loadTests();
		};
		$scope.loadFilters = function () {
			if (!$scope.filters) {
				url = new URL(document.getElementById("TESTAPIURL").value);
				url = new URL(url.toString() + "v1/filters");
				$http.get(url.toString())
					.then(function (result) {
						$scope.filters = result.data.data;
						if (!$scope.isClient && !$scope.testFilter && $scope.filters.length > 0) {
							$scope.testFilter = $scope.filters[0].name;
						}
					});
			}
		};
		$scope.loadFilters();
		$scope.loadTests = function () {
			let url = new URL(document.getElementById("TESTAPIURL").value);
			url = new URL(url.toString() + "v1/runs");
			url.searchParams.set("filter", $scope.testFilter);
			url.searchParams.set("version", $scope.testVersion);
			$http.get(url.toString())
				.then(function (result) {
					$scope.tests = result.data.data;
				});
		};
		var taskprg = StartProgress("Loading data..."); $scope["loaders"]++;
		$http.post("GitService.asmx/getVersionLog", JSON.stringify({}))
			.then(function (result) {
				for (var i = 0; i < result.data.d.length; i++) {
					var txt = result.data.d[i].trim();
					if (txt.startsWith("=")) {
						txt = txt.replace(/=/g, "").trim();
						if (txt.length > 0) {
							let testVersion = (new Date()).getFullYear();
							let items = txt.split(".");
							testVersion += ".8" + items[0].replace(/\d/g, '').toUpperCase();
							testVersion += "." + items[0].replace(/\D/g, '');
							testVersion += "." + items[1];
							testVersion += "." + items[2];
							testVersion += ".AUTOBOT";
							$scope.versions.push({ version: txt, testVersion: testVersion, changes: [] });
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
				var ver = getParameterByName("version");
				if (ver !== "") {
					window.location.hash = "";
					setTimeout(function () { window.location.hash = ver; }, 10);
				}
			});
	}]);
});