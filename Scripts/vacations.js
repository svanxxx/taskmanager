Date.prototype.monthDays = function () {
	var d = new Date(this.getFullYear(), this.getMonth() + 1, 0);
	return d.getDate();
};

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
	};

	var app = angular.module('mpsapplication', []);
	app.controller('mpscontroller', ["$scope", "$http", function ($scope, $http) {

		getDispos($scope, "dispos", $http);
		$scope.isVacationScheduled = function (v) {
			var idxDisp = $scope.dispos.findIndex(function (x) { return x.ID == v.DISPO; });
			return !$scope.dispos[idxDisp].CANNOTSTART;
		};

		$scope.numOfDays = function (d) {
			return new Date(d.getFullYear(), d.getMonth(), 0).getDate();
		};
		$scope.getVacation = function (u, d) {
			var arr = [];
			for (var i = 0; i < u.scheduled.length; i++) {
				if (u.scheduled[i].DATE == DateToString(d)) {
					arr.push(u.scheduled[i].ID);
				}
			}
			return arr;
		};
		$scope.hasVacation = function (u, d) {
			var res = 0;
			for (var i = 0; i < u.scheduled.length; i++) {
				if (u.scheduled[i].DATE == DateToString(d)) {
					res++;
				}
			}
			return res;
		};
		$scope.hasVacationSick = function (u, d) {
			for (var i = 0; i < u.scheduled.length; i++) {
				if (u.scheduled[i].DATE == DateToString(d)) {
					return u.scheduled[i].SICK;
				}
			}
			return false;
		};
		$scope.hasWorkRec = function (u, d) {
			for (var i = 0; i < u.workrecs.length; i++) {
				if (u.workrecs[i].DATE == DateToString(d)) {
					return true;
				}
			}
			return false;
		};

		$scope.cleanUsers = function () {
			for (var i = 0; i < $scope.users.length; i++) {
				$scope.users[i].unscheduled = [];
				$scope.users[i].scheduled = [];
				$scope.users[i].workrecs = [];
			}
		};

		$scope.scheduleVacation = function (u, d) {
			if (u.unscheduled.length < 1) {
				alert("User has no free vacations!");
				return;
			}

			var toSchedule = u.unscheduled[0];
			$http.post("trservice.asmx/scheduletask", JSON.stringify({ "ttid": toSchedule.ID, "date": DateToString(d) }))
				.then(function (result) {
					u.unscheduled.splice(0, 1);
					u.scheduled.push(result.data.d);
				});
		};
		$scope.getColor = function (u, d) {
			if (u) {
				if ($scope.hasWorkRec(u, d))
					return "#0000ff26";
				var vacs = $scope.hasVacation(u, d);
				if (vacs > 0) {
					if ($scope.hasVacationSick(u, d)) {
						return "green";
					}
					return vacs == 1 ? "#ffa50045" : "red";
				}
			}
			if (d.valueOf() === $scope.today.valueOf())
				return "yellow";
			if (d.getDay() === 6 || d.getDay() === 0)
				return "DodgerBlue";
		};

		var d = new Date();
		$scope.daterep = new Date(d.getFullYear(), d.getMonth(), 1);
		$scope.changed = false;
		$scope.loadReady = function () {
			$scope.changed = true;
		};

		$scope.loadData = function () {
			$scope.changed = false;
			$scope["loaders"]++;

			$scope.today = new Date();
			$scope.today.setHours(0, 0, 0, 0);

			$scope.vacations = [];
			$scope.days = [];
			$scope.monthNames = monthNames;

			$scope.daterepend = new Date();
			$scope.daterepend.setDate($scope.daterep.getDate() + 366);
			$scope.users = [];

			getMPSUsers($scope, "users", $http, function () {
				$scope.cleanUsers();
				$scope.days = [];
				var stopday = new Date($scope.daterep.getFullYear() + 1, $scope.daterep.getMonth(), $scope.daterep.getDate());
				for (var d = new Date($scope.daterep.getFullYear(), $scope.daterep.getMonth(), 1); d < stopday; d.setDate(d.getDate() + 1)) {
					$scope.days.push(new Date(d));
				}
				var vacationprg = StartProgress("Loading vacations..."); $scope["loaders"]++;
				$http.post("trservice.asmx/enumCloseVacations", JSON.stringify({ "start": DateToString($scope.daterep), "days": 366 }))
					.then(function (result) {
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
					});
				var trprg = StartProgress("Loading records..."); $scope["loaders"]++;
				$http.post("trservice.asmx/enumTRSignal", JSON.stringify({ "from": DateToString($scope.daterep), "to": DateToString($scope.daterepend) }))
					.then(function (result) {
						for (var i = 0; i < result.data.d.length; i++) {
							var v = result.data.d[i];
							for (var j = 0; j < $scope.users.length; j++) {
								var u = $scope.users[j];
								if (u.ID == v.USER) {
									u.workrecs.push(v);
									break;
								}
							}
						}
						EndProgress(trprg); $scope["loaders"]--;
					});
				var freevacationprg = StartProgress("Loading free vacations..."); $scope["loaders"]++;
				$http.post("trservice.asmx/enumUnusedVacations", JSON.stringify({}))
					.then(function (result) {
						for (var i = 0; i < result.data.d.length; i++) {
							var v = result.data.d[i];
							for (var j = 0; j < $scope.users.length; j++) {
								var u = $scope.users[j];
								if (u.TTUSERID == v.AUSER) {
									u.unscheduled.push(v);
									break;
								}
							}
						}
						EndProgress(freevacationprg); $scope["loaders"]--;
					});
			});
		};
		$scope.loadData();
	}]);
});