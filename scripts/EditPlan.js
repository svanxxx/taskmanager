$(function () {
	var app = angular.module('mpsapplication', []);

	app.filter('getUserImgById', function () {
		return function (id, $scope) {
			for (i = 0; i < $scope.users.length; i++) {
				if ($scope.users[i].ID == id) {
					return $scope.getPersonImg($scope.users[i].EMAIL);
				}
			}
			return "";
		};
	});
	app.filter('getDispoColorById', function () {
		return function (id, $scope) {
			var col = ($scope.dispos && $scope.dispos.length > 0) ? $scope.dispos.filter(x => x.ID == id)[0].COLOR : "white";
			return { "background-color": col };
		};
	});

	app.controller('mpscontroller', function ($scope, $http) {
		$scope.Math = window.Math;
		$scope.getPersonImg = function (email) {
			if ($scope.users) {
				return "images/personal/" + email + ".jpg";
			}
			return "";
		}

		$scope.changeuser = function (u) {
			$scope.defects = [];
			$scope.currentuser = u.TTUSERID;
			var prgtasks = StartProgress("Loading tasks...");
			$http.post("trservice.asmx/getplanned", JSON.stringify({ "userid": $scope.currentuser}))
				.then(function (result) {
					$scope.defects = result.data.d;
					EndProgress(prgtasks);
				});

			$scope.unscheduled = [];
			$http.post("trservice.asmx/getunplanned", JSON.stringify({ "userid": $scope.currentuser }))
				.then(function (response) {
					$scope.unscheduled = response.data.d;
				});
		}

		$scope.dispos = getDispos();
		if (!$scope.dispos) {
			var prgdispos = StartProgress("Loading dispositions...");
			$scope.dispos = [];
			$http.post("trservice.asmx/gettaskdispos", JSON.stringify({}))
				.then(function (result) {
					$scope.dispos = result.data.d;
					setDispos($scope.dispos);
					EndProgress(prgdispos);
				});
		}

		var userskprg = StartProgress("Loading users...");
		$scope.users = [];
		$http.post("trservice.asmx/getMPSusers", JSON.stringify({}))
			.then(function (result) {
				$scope.users = result.data.d;
				$scope.currentuser = $scope.users[0].TTUSERID;
				$scope.changeuser($scope.users[0]);
				EndProgress(userskprg);
			});
		$scope.changed = false;
	})
})