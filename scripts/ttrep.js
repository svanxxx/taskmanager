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
	app.filter('getDispoById', function () {
		return function (id, $scope) {
			if (!$scope.dispos) {
				return "";
			}
			return $scope.dispos.filter(x => x.ID == id)[0].DESCR;
		};
	});
	app.filter('getDispoColorById', function () {
		return function (id, $scope) {
			var col = $scope.dispos.filter(x => x.ID == id)[0].COLOR;
			return { "background-color": col };
		};
	});

	app.controller('mpscontroller', function ($scope, $http) {

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
			return $scope.DefectsFilter[refname].findIndex(x => x == id) > -1;
		}
		$scope.changeReferenceFilter = function (id, refname) {
			$scope.changed = true;
			var index = $scope.DefectsFilter[refname].findIndex(x => x == id);
			if (index > -1) {
				$scope.DefectsFilter[refname].splice(index, 1);
			} else {
				$scope.DefectsFilter[refname].push(id);
			}
		}
	});
})