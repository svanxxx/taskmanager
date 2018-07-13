function enterTT() {
	var ttid = prompt("Please enter TT ID", getParameterByName("ttid"));
	if (ttid != null) {
		//window.location = replaceUrlParam(location.href, "ttid", ttid);
		return true;
	}
	return false;
}
var _callsettname = "JColResizer0";
function getTabKey() {
	return GetPageName() + _callsettname;
}

$(function () {

	sessionStorage[_callsettname] = localStorage[getTabKey()];

	$("table").colResizable({
		liveDrag: true,
		postbackSafe: true,
		onResize: function (target) {
			setTimeout(function () {
				localStorage[getTabKey()] = sessionStorage[_callsettname];
			}, 1000);
		}
	});

	$("table thead tr th").css("overflow", "visible");

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

		$scope.onGo = function (keyEvent) {
			if (keyEvent.which === 13) {
				$scope.applyfilter();
				keyEvent.preventDefault();
			}
		}

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
			if (!("text" in $scope.DefectsFilter)) {
				$scope.DefectsFilter.text = "";
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
		$scope.discardfilter = function () {
			window.location.reload();
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