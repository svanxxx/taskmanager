Date.prototype.monthDays = function () {
	var d = new Date(this.getFullYear(), this.getMonth() + 1, 0);
	return d.getDate();
}

$(function () {
	var headertable = document.getElementById("headertable");
	var datatable = document.getElementById("datatable");
	var sticky = headertable.offsetTop;
	window.onscroll = function () {
		if (window.pageYOffset > sticky) {
			headertable.classList.add("sticky");
			headertable.style.width = datatable.clientWidth + "px";
		} else {
			headertable.classList.remove("sticky");
		}
	}

	var app = angular.module('mpsapplication', []);
	app.controller('mpscontroller', ["$scope", "$http", function ($scope, $http) {
		$scope.numOfDays = function (d) {
			return new Date(d.getFullYear(), d.getMonth(), 0).getDate();
		}
		$scope.loadData = function () {
			$scope["loaders"] = 1;
			$scope.users = [];

			$scope.today = new Date();
			$scope.today.setHours(0, 0, 0, 0);

			$scope.vacations = [];
			$scope.days = [];
			$scope.monthNames = monthNames;
			var d = new Date();
			$scope.daterep = new Date(d.getFullYear(), d.getMonth(), 1);
			$scope.getVacation = function (u, d) {
				for (var i = 0; i < u.scheduled.length; i++) {
					if (u.scheduled[i].DATE == DateToString(d)) {
						return u.scheduled[i].ID;
					}
				}
				return "";
			}
			$scope.hasVacation = function (u, d) {
				for (var i = 0; i < u.scheduled.length; i++) {
					if (u.scheduled[i].DATE == DateToString(d)) {
						return true;
					}
				}
				return false;
			}

			$scope.cleanUsers = function () {
				for (var i = 0; i < $scope.users.length; i++) {
					$scope.users[i].unscheduled = [];
					$scope.users[i].scheduled = [];
				}
			}
			var usersprg = StartProgress("Loading users...");
			$http.post("trservice.asmx/getMPSusers", JSON.stringify({ "active": true }))
				.then(function (result) {
					$scope.users = result.data.d;
					EndProgress(usersprg); $scope["loaders"]--;
					$scope.days = [];
					var stopday = new Date($scope.daterep.getFullYear() + 1, $scope.daterep.getMonth(), $scope.daterep.getDate());
					for (var d = new Date($scope.daterep.getFullYear(), $scope.daterep.getMonth(), 1); d < stopday; d.setDate(d.getDate() + 1)) {
						$scope.days.push(new Date(d));
					}
					$scope.cleanUsers();
					var vacationprg = StartProgress("Loading vacations..."); $scope["loaders"]++
					$http.post("trservice.asmx/EnumCloseVacations", JSON.stringify({ "start": DateToString($scope.daterep), "days" : 366 }))
						.then(function (result) {
							$scope.cleanUsers();
							for (var i = 0; i < result.data.d.length; i++) {
								var v = result.data.d[i];
								for (var j = 0; j < $scope.users.length; j++) {
									var u = $scope.users[j];
									if (u.TTUSERID == v.AUSER) {
										u.scheduled.push(v);
										break;
									}
								}
							}
							EndProgress(vacationprg); $scope["loaders"]--;
						})
				})
		}
		$scope.loadData();
	}]);
})