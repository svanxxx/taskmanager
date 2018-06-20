﻿$(function () {
	var app = angular.module('mpsapplication', []);
	app.controller('mpscontroller', ["$scope", "$http", function ($scope, $http) {
		$scope["loaders"] = 0;
		$scope.today = new Date();
		$scope.yesterday = new Date();
		$scope.yesterday.setDate($scope.yesterday.getDate() - 1);
		$scope.loadData = function () {
			var taskprg = StartProgress("Loading data..."); $scope["loaders"]++;
			$scope.users = [];
			$http.post("trservice.asmx/getMPSusers", JSON.stringify({ "active": true }))
				.then(function (result) {
					EndProgress(taskprg); $scope["loaders"]--;
					$scope.users = result.data.d;
					var reportskprg = StartProgress("Loading reports..."); $scope["loaders"]++
					$http.post("trservice.asmx/getreports", JSON.stringify({ "dates": [DateToString($scope.today), DateToString($scope.yesterday)] }))
						.then(function (result) {
							EndProgress(reportskprg); $scope["loaders"]--;
							var d1 = DateToString($scope.today);
							var d2 = DateToString($scope.yesterday);
							var recs = result.data.d;
							for (var r = 0; r < recs.length; r++) {
								for (var u = 0; u < $scope.users.length; u++) {
									if (recs[r].USER == $scope.users[u].ID) {
										var txts = recs[r].DONE.split(/\r?\n/);
										if (recs[r].DATE == d1) {
											$scope.users[u].TODAY = txts;
										} else {
											$scope.users[u].YESTERDAY = txts;
										}
									}
								}
							}
						});
					$scope.users.forEach(function (user) {
						var user4proc = user;
						var newuserprog = StartProgress("Loading plan for " + user4proc.PERSON_NAME + "..."); $scope["loaders"]++
						$http.post("trservice.asmx/getplannedShort", JSON.stringify({ "userid": user4proc.TTUSERID }))
							.then(function (result) {
								user4proc.PLAN = result.data.d;
								EndProgress(newuserprog); $scope["loaders"]--;
							})
					});
				});
		}
		$scope.planLoaded = function (id) {
			for (var i = 0; i < $scope.users.length; i++) {
				if ($scope.users[i].ID == id) {
					return $scope.users[i].PLAN !== undefined;
				}
			}
			return false;
		}
		$scope.loaded = function () {
			return $scope["loaders"] == 0;
		}
		$scope.changeDate = function () {
			$scope.yesterday = new Date($scope.today.getTime());
			$scope.yesterday.setDate($scope.yesterday.getDate() - 1);
			$scope.loadData();
		}
		$scope.loadData();
	}]);
})