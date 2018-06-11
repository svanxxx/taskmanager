$(function () {
	var app = angular.module('mpsapplication', []);
	app.controller('mpscontroller', function ($scope, $http) {
		$scope.discard = function () {
			window.location.reload();
		}
		$scope.save = function () {
			var prg = StartProgress("Saving data...");
			var severs = [];
			for (var i = 0; i < $scope.severs.length; i++) {
				var ch = $scope.severs[i].changed;
				if (ch) {
					delete $scope.severs[i].changed;
					severs.push($scope.severs[i])
				}
			}
			$http.post("trservice.asmx/setsevers", angular.toJson({ "severs": severs }), )
				.then(function (response) {
					EndProgress(prg);
					$scope.changed = false;
				});
		}
		var taskprg = StartProgress("Loading data...");
		$scope.severs = [];
		$http.post("trservice.asmx/gettasksevers", JSON.stringify({}))
			.then(function (result) {
				$scope.severs = result.data.d;
			});
		$scope.changed = false;
		$scope.enterdata = function (object, prop) {
			var oldval = object[prop];
			var newvalue = window.prompt("Please enter the value", oldval);
			if (newvalue == null || newvalue == "") {
				return;
			}
			if (newvalue != oldval) {
				object[prop] = newvalue;
				object.changed = true;
				$scope.changed = true;
			}
		}
		EndProgress(taskprg);
	});
})