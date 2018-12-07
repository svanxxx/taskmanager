$(function () {
	var app = angular.module('mpsapplication', []);
	app.controller('mpscontroller', ["$scope", "$http", function ($scope, $http) {

		getProducts($scope, "products", $http);
		getTypes($scope, "types", $http);
		getDispos($scope, "dispos", $http);
		getPriorities($scope, "prios", $http);
		getSevers($scope, "severs", $http);
		getComps($scope, "comps", $http);

		$scope.readonly = !IsAdmin();
		$scope.discard = function () {
			window.location.reload();
		};
		$scope.save = function () {
			var prg = StartProgress("Saving data...");
			$http.post("trservice.asmx/setDefaults", angular.toJson({ s: $scope.deffObj }))
				.then(function () {
					EndProgress(prg);
					$scope.loadData();
				});
		};

		$scope.loadData = function () {
			var taskprg = StartProgress("Loading data...");
			$scope.changed = false;
			$scope.deffObj = {};
			$http.post("trservice.asmx/getDefaults", JSON.stringify({}))
				.then(function (result) {
					$scope.deffObj = result.data.d;
					EndProgress(taskprg);
				});
		};
		$scope.itemchanged = function (r) {
			r.changed = true;
			$scope.changed = true;
		};

		$scope.loadData();
	}]);
});