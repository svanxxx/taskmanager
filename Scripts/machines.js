$(function () {
	var app = angular.module('mpsapplication', []);
	app.filter('rawHtml', ['$sce', rawHtml]);
	app.filter('mcol', function () {
		return function (m, $scope) {
			var style = {};
			if ($scope.userLogin.toUpperCase() === m.PCNAME.toUpperCase()) {
				style["border"] = "solid 4px black";
			}
			if (m.ping == true) {
				style["background-color"] = "#337ab7";
			} else {
				style["background-color"] = "gray";
			}
			return style;
		};
	});
	app.controller('mpscontroller', ["$scope", "$http", function ($scope, $http) {
		var taskprg = StartProgress("Loading data...");
		$scope.workmachine = undefined;
		$scope.searchMachine = false;
		$scope.machines = [];
		$scope.categories = [];
		$scope.userLogin = userLogin();
		$scope.unpinged = function (m) {
			return (typeof m.ping === "undefined");
		};

		$http.post("MachinesService.asmx/getMachines", JSON.stringify({}))
			.then(function (result) {
				$scope.loadMachines(result.data.d);
				EndProgress(taskprg);
			});
		var dompckprg = StartProgress("Loading domain computers...");
		$http.post("MachinesService.asmx/getDomainComputers", JSON.stringify({}))
			.then(function (result) {
				$scope.domainComputers = result.data.d;
				EndProgress(dompckprg);
			});
		$scope.setMachine = function (m) {
			if (m == undefined) {
				$scope.searchMachine = false;
				$scope.workmachine = undefined;
			} else {
				$scope.workmachine = m;
			}
		};

		$scope.mLoadSignal = new EventSource("machinesping.ashx");
		$scope.mLoadSignal.addEventListener("machine", function (e) {
			var data = e.data.split("-");
			if (data.length > 1) {
				for (var i = 0; i < $scope.machines.length; i++) {
					if ($scope.machines[i].PCNAME == data[0]) {
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

		$scope.loadMachines = function (ms) {
			$scope.machines = ms;
			$scope.categories = [""];
			ms.forEach(function (m) {
				if (m.CATEGORY) {
					var uc = m.CATEGORY.toUpperCase();
					if ($scope.categories.indexOf(uc) < 0) {
						$scope.categories.push(uc);
					}
				} else {
					m.CATEGORY = "";
				}
			});
			$scope.categories.sort();
		};
		$scope.shutMachine = function () {
			StartProgress("Shutting down...");
			$http.post("MachinesService.asmx/shutMachine", JSON.stringify({ "m": $scope.workmachine.PCNAME }))
				.then(function () {
					window.location.reload();
				});
		};
		$scope.wakeMachine = function () {
			StartProgress("Waking up...");
			$http.post("MachinesService.asmx/wakeMachine", JSON.stringify({ "m": $scope.workmachine.PCNAME }))
				.then(function () {
					window.location.reload();
				});
		};
		$scope.scanMachine = function () {
			StartProgress("Scanning...");
			$http.post("MachinesService.asmx/scanMachine", JSON.stringify({ "m": $scope.workmachine.PCNAME }))
				.then(function () {
					window.location.reload();
				});
		};
		$scope.catMachine = function () {
			var cat = prompt("Please enter category", "");
			if (cat !== null) {
				StartProgress("Updating...");
				$http.post("MachinesService.asmx/catMachine", JSON.stringify({ "m": $scope.workmachine.PCNAME, "category": cat }))
					.then(function () {
						window.location.reload();
					});
			}
		};
		$scope.remMachine = function () {
			StartProgress("Removing...");
			$http.post("MachinesService.asmx/remMachine", JSON.stringify({ "m": $scope.workmachine.PCNAME }))
				.then(function () {
					window.location.reload();
				});
		};
		$scope.hasMachine = function () {
			return typeof $scope.workmachine !== "undefined";
		};
		$scope.scanAllMachines = function () {
			waitForProcess();
			$http.post("MachinesService.asmx/scanAllMachines", JSON.stringify({}))
				.then(function (result) {
					$scope.loadMachines(result.data.d);
					waitForProcessEnd();
				});
		};
		$scope.reScanMachines = function () {
			waitForProcess();
			$http.post("MachinesService.asmx/reScanMachines", JSON.stringify({}))
				.then(function (result) {
					$scope.loadMachines(result.data.d);
					waitForProcessEnd();
				});
		};
	}]);
})