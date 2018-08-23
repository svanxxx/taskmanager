$(function () {
	var app = angular.module('mpsapplication', []);

	app.filter('getDispoColorById', getDispoColorById);

	app.controller('mpscontroller', ["$scope", "$http", "$timeout", function ($scope, $http, $timeout) {
		$scope.Math = window.Math;
		$scope.currentuserid = -1;
		$scope.currentuser = {};
		getDispos($scope, "dispos", $http);

		$scope.discardDefects = function () {
			$scope.changeuser($scope.currentuser);
		};
		$scope.scheduletask = function (d) {
			for (var i = $scope.unscheduled.length - 1; i >= 0; i--) {
				if ($scope.unscheduled[i].ID == d.ID) {
					$scope.unscheduled.splice(i, 1);
					d.orderchanged = true;
					$scope.defects.splice(0, 0, d);
					$scope.changed = true;
					return;
				}
			}
		}
		$scope.unscheduletask = function (d) {
			for (var i = $scope.defects.length - 1; i >= 0; i--) {
				if ($scope.defects[i].ID == d.ID) {
					$scope.defects.splice(i, 1);
					d.orderchanged = true;
					$scope.unscheduled.splice(0, 0, d);
					$scope.changed = true;
					return;
				}
			}
		}
		$scope.saveDefects = function () {
			var recs = [];
			for (var i = $scope.defects.length - 1; i >= 0; i--) {
				var d = $scope.defects[i];
				var ord = $scope.defects.length - i;

				if (d.BACKORDER == ord) {
					continue;
				}
				var rec = {};
				rec.ttid = d.ID;
				rec.backorder = ord;
				rec.moved = ("orderchanged" in d);
				recs.push(rec);
			}

			var unsched = [];
			for (var i = $scope.unscheduled.length - 1; i >= 0; i--) {
				var d = $scope.unscheduled[i];
				if ("orderchanged" in d) {
					unsched.push(d.ID);
				}
			}

			$http.post("trservice.asmx/setschedule", JSON.stringify({ "ttids": recs, "unschedule": unsched })).then(function (result) {
				$scope.changeuser($scope.currentuser);
			});
		}
		$scope.taskMove = function (d, $event) {
			if (($event.keyCode == 38 || $event.keyCode == 40) && $event.ctrlKey == true) {
				$event.preventDefault();
				for (var i = 0; i < $scope.defects.length; i++) {
					if (d.ID == $scope.defects[i].ID) {
						var index = i + 1;
						if ($event.keyCode == 38) {
							index = i - 1;
						}
						if (index == -1 || index == $scope.defects.length) {
							break;
						}
						var tempo = $scope.defects[index];
						$scope.defects[index] = $scope.defects[i];
						$scope.defects[i] = tempo;
						$scope.defects[index].orderchanged = true;
						$scope.changed = true;
						break;
					}
				}
				$timeout(function () {
					$("input.taskselector:checked").focus();
				}, 10);
			}
		}
		$scope.changeuser = function (u) {
			$scope.defects = [];
			$scope.currentuserid = u.TTUSERID;
			$scope.currentuser = u;
			var prgtasks = StartProgress("Loading tasks...");
			$http.post("trservice.asmx/getplanned", JSON.stringify({ "userid": $scope.currentuserid }))
				.then(function (result) {
					$scope.defects = result.data.d;
					EndProgress(prgtasks);
					$scope.changed = false;
				});

			$scope.unscheduled = [];
			$http.post("trservice.asmx/getunplanned", JSON.stringify({ "userid": $scope.currentuserid }))
				.then(function (response) {
					$scope.unscheduled = response.data.d;
				});
		};

		getMPSUsers($scope, "users", $http, function () {
			$scope.currentuserid = userID();
			$scope.changeuser($scope.users.find(function (x) { return x.ID == $scope.currentuserid;}));
		});
		$scope.changed = false;
	}]);
});