$(function () {
	var app = angular.module('mpsapplication', []);
	app.controller('mpscontroller', ["$scope", "$http", function ($scope, $http) {
		$scope.loadData = function () {
			var prg = StartProgress("Loading data...");
			$http.post("trservice.asmx/getBuildRequests", JSON.stringify({}))
				.then(function (result) {
					$scope.builds = result.data.d;
					EndProgress(prg);
				});
		};
		
		$scope.loadData();

		var notifyHub = $.connection.notifyHub;
		notifyHub.client.onBuildChanged = function (id) {
			$scope.loadData();
			$scope.$apply();
		};
		$.connection.hub.disconnected(function () {
			setTimeout(function () {$.connection.hub.start();}, 5000); // Restart connection after 5 seconds.
		});
		$.connection.hub.start();
	}]);
});