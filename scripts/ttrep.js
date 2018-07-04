function enterTT() {
	var ttid = prompt("Please enter TT ID", getParameterByName("ttid"));
	if (ttid != null) {
		//window.location = replaceUrlParam(location.href, "ttid", ttid);
		return true;
	}
	return false;
}
$(function () {
	var app = angular.module('mpsapplication', []);
	app.filter('getUserById', function () {
		return function (id, $scope) {
			if (!$scope.users) {
				return "";
			}
			for (i = 0; i < $scope.users.length; i++) {
				if ($scope.users[i].ID == id) {
					return $scope.users[i].FIRSTNAME + " " + $scope.users[i].LASTNAME;
				}
			}
			return "";
		};
	});

	app.filter('getCompById', getCompById);
	app.filter('getDispoById', getDispoById);
	app.filter('getDispoColorById', getDispoColorById);

	app.controller('mpscontroller', ["$scope", "$http", function ($scope, $http) {

		//references section
		getDispos($scope, "dispos", $http);
		getUsers($scope, "users", $http);
		getTypes($scope, "types", $http);
		getPriorities($scope, "priorities", $http);
		getSevers($scope, "severs", $http);
		getProducts($scope, "products", $http);
		getComps($scope, "comps", $http);

		$scope.loadData = function () {
			var taskprg = StartProgress("Loading tasks...");
			$scope.changed = false;
			$scope.DefectsFilter = {};
			if (localStorage.DefectsFilter) {
				$scope.DefectsFilter = JSON.parse(localStorage.DefectsFilter);
			}
			if (!("dispositions" in $scope.DefectsFilter)) {
				$scope.DefectsFilter.dispositions = [];
			}
			if (!("components" in $scope.DefectsFilter)) {
				$scope.DefectsFilter.components = [];
			}
			if (!("users" in $scope.DefectsFilter)) {
				$scope.DefectsFilter.users = [];
			}
			$http.post("trservice.asmx/gettasks", JSON.stringify({ "f": $scope.DefectsFilter }))
				.then(function (response) {
					$scope.defects = response.data.d;
					EndProgress(taskprg);;
				});
		}

		$scope.loadData();

		$scope.applyfilter = function () {
			localStorage.DefectsFilter = JSON.stringify($scope.DefectsFilter);
			$scope.loadData();
		}
		$scope.referenceFiltered = function (id, refname) {
			return $scope.DefectsFilter[refname].findIndex(function (x) { return x == id; }) > -1;
		}
		$scope.changeReferenceFilter = function (id, refname) {
			$scope.changed = true;
			var index = $scope.DefectsFilter[refname].findIndex(function (x) { return x == id; });
			if (index > -1) {
				$scope.DefectsFilter[refname].splice(index, 1);
			} else {
				$scope.DefectsFilter[refname].push(id);
			}
		}
	}]);
})