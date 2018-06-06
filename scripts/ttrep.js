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
			return $scope.dispos.filter(x => x.ID == id)[0].DESCR;
		};
	});
	app.filter('getDispoColorById', function () {
		return function (id, $scope) {
			var col = $scope.dispos.filter(x => x.ID == id)[0].COLOR;
			return { "background-color": col };
		};
	});

	app.controller('mpscontroller', function ($scope, $http, $interval) {

		var taskprg = StartProgress("Loading tasks...");
		$scope.defects = [];
		$http.post("trservice.asmx/gettasks", JSON.stringify({}))
			.then(function (response) {
				$scope.defects = response.data.d;
				EndProgress(taskprg);;
			});

		if (sessionStorage.types) {
			$scope.types = JSON.parse(sessionStorage.types);
		} else {
			var prgtypes = StartProgress("Loading types..."); $scope.loaders++;
			$scope.types = [];
			$http.post("trservice.asmx/gettasktypes", JSON.stringify({}))
				.then(function (result) {
					$scope.types = result.data.d;
					sessionStorage.types = JSON.stringify(result.data.d);
					EndProgress(prgtypes); $scope.loaders--;
				});
		}

		if (sessionStorage.products) {
			$scope.products = JSON.parse(sessionStorage.products);
		} else {
			var prgproducts = StartProgress("Loading products..."); $scope.loaders++;
			$scope.products = [];
			$http.post("trservice.asmx/gettaskproducts", JSON.stringify({}))
				.then(function (result) {
					$scope.products = result.data.d;
					sessionStorage.products = JSON.stringify(result.data.d);
					EndProgress(prgproducts); $scope.loaders--;
				});
		}

		$scope.dispos = getDispos();
		if (!$scope.dispos) {
			var prgdispos = StartProgress("Loading dispositions..."); $scope.loaders++;
			$scope.dispos = [];
			$http.post("trservice.asmx/gettaskdispos", JSON.stringify({}))
				.then(function (result) {
					$scope.dispos = result.data.d;
					setDispos($scope.dispos);
					EndProgress(prgdispos); $scope.loaders--;
				});
		}

		if (sessionStorage.priorities) {
			$scope.priorities = JSON.parse(sessionStorage.priorities);
		} else {
			var prgprio = StartProgress("Loading priorities..."); $scope.loaders++;
			$scope.priorities = [];
			$http.post("trservice.asmx/gettaskpriorities", JSON.stringify({}))
				.then(function (result) {
					$scope.priorities = result.data.d;
					sessionStorage.priorities = JSON.stringify(result.data.d);
					EndProgress(prgprio); $scope.loaders--;
				});
		}

		if (sessionStorage.comps) {
			$scope.comps = JSON.parse(sessionStorage.comps);
		} else {
			var prgcompo = StartProgress("Loading components..."); $scope.loaders++;
			$scope.comps = [];
			$http.post("trservice.asmx/gettaskcomps", JSON.stringify({}))
				.then(function (result) {
					$scope.comps = result.data.d;
					sessionStorage.comps = JSON.stringify(result.data.d);
					EndProgress(prgcompo); $scope.loaders--;
				});
		}

		if (sessionStorage.severs) {
			$scope.severs = JSON.parse(sessionStorage.severs);
		} else {
			var prgseve = StartProgress("Loading severities..."); $scope.loaders++;
			$scope.severs = [];
			$http.post("trservice.asmx/gettasksevers", JSON.stringify({}))
				.then(function (result) {
					$scope.severs = result.data.d;
					sessionStorage.severs = JSON.stringify(result.data.d);
					EndProgress(prgseve); $scope.loaders--;
				});
		}

		if (sessionStorage.users) {
			$scope.users = JSON.parse(sessionStorage.users);
		} else {
			var prgusers = StartProgress("Loading users..."); $scope.loaders++;
			$scope.users = [];
			$http.post("trservice.asmx/gettaskusers", JSON.stringify({}))
				.then(function (result) {
					$scope.users = result.data.d;
					sessionStorage.users = JSON.stringify(result.data.d);
					EndProgress(prgusers); $scope.loaders--;
				});
		}
	});
})