$(function () {
	var app = angular.module('mpsapplication', []);
	app.controller('mpscontroller', ["$scope", "$http", function ($scope, $http) {
		$scope.discard = function () {
			window.location.reload();
		}
		$scope.save = function () {
			var prg = StartProgress("Saving data...");
			var priors = [];
			for (var i = 0; i < $scope.priors.length; i++) {
				var ch = $scope.priors[i].changed;
				if (ch) {
					delete $scope.priors[i].changed;
					priors.push($scope.priors[i])
				}
			}
			$http.post("trservice.asmx/settaskpriorities", angular.toJson({ "data": priors }), )
				.then(function (response) {
					EndProgress(prg);
					$scope.changed = false;
				});
		}

		getPriorities($scope, "priors", $http);

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
	}]);
})