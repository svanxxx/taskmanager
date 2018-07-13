$(function () {
	var app = angular.module('mpsapplication', []);
	app.filter('mcol', function () {
		return function (m, $scope) {
			if (m.ping == true) {
				return { "background-color": "#337ab7" };				
			} else {
				return { "background-color": "gray" };
			}
		};
	});
	app.controller('mpscontroller', ["$scope", "$http", function ($scope, $http) {
		var taskprg = StartProgress("Loading data...");
		$scope.workmachine = undefined;
		$scope.searchMachine = false;
		$scope.machines = [];

		$scope.unpinged = function (m) {
			return (typeof m.ping === "undefined");
		}

		$http.post("trservice.asmx/getMachines", JSON.stringify({}))
			.then(function (result) {
				$scope.machines = result.data.d;
				EndProgress(taskprg);
			});
		var dompckprg = StartProgress("Loading domain computers...");
		$http.post("trservice.asmx/getDomainComputers", JSON.stringify({}))
			.then(function (result) {
				$scope.domainComputers = result.data.d;
				EndProgress(dompckprg);
			});
		$scope.setMachine = function (m) {
			if (m == '') {
				$scope.searchMachine = false;
				$scope.workmachine = undefined;
			} else {
				$scope.workmachine = m;
			}
		}

		$scope.mLoadSignal = new EventSource("machinesping.ashx");
		$scope.mLoadSignal.addEventListener("machine", function (e) {
			var data = e.data.split("-");
			if (data.length > 1) {
				for (var i = 0; i < $scope.machines.length; i++) {
					if ($scope.machines[i].NAME == data[0]) {
						var val = (data[1] == "Success");
						if ($scope.machines[i].ping !== val) {
							$scope.machines[i].ping = val;
							$scope.$apply();
						}
						break;
					}
				}
			}
		}, false);

		$scope.shutMachine = function () {
			var scankprg = StartProgress("Shutting down...");
			$http.post("trservice.asmx/shutMachine", JSON.stringify({ "m": $scope.workmachine }))
				.then(function (result) {
					window.location.reload();
				});
		}
		$scope.wakeMachine = function () {
			var scankprg = StartProgress("Waking up...");
			$http.post("trservice.asmx/wakeMachine", JSON.stringify({ "m": $scope.workmachine }))
				.then(function (result) {
					window.location.reload();
				});
		}
		$scope.scanMachine = function () {
			var scankprg = StartProgress("Scanning...");
			$http.post("trservice.asmx/scanMachine", JSON.stringify({ "m": $scope.workmachine }))
				.then(function (result) {
					window.location.reload();
				});
		}
		$scope.remMachine = function () {
			var scankprg = StartProgress("Removing...");
			$http.post("trservice.asmx/remMachine", JSON.stringify({ "m": $scope.workmachine }))
				.then(function (result) {
					window.location.reload();
				});
		}
		$scope.hasMachine = function () {
			return (typeof $scope.workmachine !== "undefined");
		}
	}]);
})