$(function () {
	var app = angular.module('mpsapplication', []);
	app.controller('mpscontroller', ["$scope", "$http", function ($scope, $http) {
		$scope.discard = function () {
			window.location.reload();
		}
		$scope.save = function () {
			var prg = StartProgress("Saving data...");
			var comps = [];
			for (var i = 0; i < $scope.comps.length; i++) {
				var ch = $scope.comps[i].changed;
				if (ch) {
					delete $scope.comps[i].changed;
					comps.push($scope.comps[i])
				}
			}
			$http.post("trservice.asmx/settaskcomps", angular.toJson({ "data": comps }), )
				.then(function (response) {
					EndProgress(prg);
					$scope.changed = false;
				});
		}
		getComps($scope, "comps", $http);
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