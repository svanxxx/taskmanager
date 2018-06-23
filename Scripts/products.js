$(function () {
	var app = angular.module('mpsapplication', []);
	app.controller('mpscontroller', ["$scope", "$http", function ($scope, $http) {
		$scope.discard = function () {
			window.location.reload();
		}
		$scope.save = function () {
			var prg = StartProgress("Saving data...");
			var products = [];
			for (var i = 0; i < $scope.products.length; i++) {
				var ch = $scope.products[i].changed;
				if (ch) {
					delete $scope.products[i].changed;
					products.push($scope.products[i])
				}
			}
			$http.post("trservice.asmx/settaskproducts", angular.toJson({ "data": products }), )
				.then(function (response) {
					EndProgress(prg);
					$scope.changed = false;
				});
		}

		getProducts($scope, "products", $http);

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