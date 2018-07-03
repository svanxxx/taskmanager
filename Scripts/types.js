$(function () {
	var app = angular.module('mpsapplication', []);
	app.controller('mpscontroller', ["$scope", "$http", function ($scope, $http) {
		$scope.discard = function () {
			window.location.reload();
		}
		$scope.save = function () {
			var prg = StartProgress("Saving data...");
			var types = [];
			for (var i = 0; i < $scope.types.length; i++) {
				var ch = $scope.types[i].changed;
				if (ch) {
					delete $scope.types[i].changed;
					types.push($scope.types[i])
				}
			}
			$http.post("trservice.asmx/settasktypes", angular.toJson({ "data": types }), )
				.then(function (response) {
					EndProgress(prg);
					$scope.changed = false;
				});
		}

		getTypes($scope, "types", $http);

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