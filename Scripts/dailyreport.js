$(function () {
	var app = angular.module('mpsapplication', []);
	app.filter('getDispoColorById', getDispoColorById);

	app.controller('mpscontroller', ["$scope", "$http", "$interval", function ($scope, $http, $interval) {
		$scope["loaders"] = 0;
		$scope.today = new Date();
		if (getParameterByName("date") != "") {
			$scope.today = StringToDate(getParameterByName("date"));
		}
		$scope.mpsusers = [];
		$scope.today.setHours(0, 0, 0, 0);
		$scope.yesterday = new Date();
		$scope.yesterday.setDate($scope.yesterday.getDate() - 1);
		$scope.yesterday.setHours(0, 0, 0, 0);

		$scope.vacations = [];

		getDispos($scope, "dispos", $http);
		getMPSUsers($scope, "mpsusers", $http);

		$scope.getUpcomingdays = function (u) {
			var difference = 0;
			for (var i = 0; i < $scope.vacations.length; i++) {
				var vac = $scope.vacations[i];
				if (vac.AUSER == u.TTUSERID) {
					var d = StringToDate(vac.DATE);
					if (d > $scope.today) {
						var currd = d - $scope.today;
						//looking for nearest vacation
						difference = (difference == 0) ? currd : Math.min(currd, difference);
					}
				}
			}
			difference = Math.floor(difference / 1000 / 60 / 60 / 24);
			if (difference < 2) {
				return "";
			} else {
				return "Upcoming in " + difference + " days:";
			}
		};

		$scope.loadData = function () {
			$scope.todaystring = DateToString($scope.today);
			$scope.yesterdaystring = DateToString($scope.yesterday);

			var vacationprg = StartProgress("Loading vacations..."); $scope["loaders"]++;
			$http.post("trservice.asmx/enumCloseVacations", JSON.stringify({ "start": DateToString($scope.yesterday), "days": 15 }))
				.then(function (result) {
					$scope.vacations = result.data.d;
					EndProgress(vacationprg); $scope["loaders"]--;
				});

			var reportskprg = StartProgress("Loading reports..."); $scope["loaders"]++;
			$http.post("trservice.asmx/getreports", JSON.stringify({ "dates": [DateToString($scope.today), DateToString($scope.yesterday)] }))
				.then(function (result) {
					var d1 = DateToString($scope.today);
					var recs = result.data.d;
					for (var r = 0; r < recs.length; r++) {
						for (var u = 0; u < $scope.mpsusers.length; u++) {
							if (recs[r].USER == $scope.mpsusers[u].ID) {
								var txts = recs[r].DONE.split(/\r?\n/);
								if (recs[r].DATE == d1) {
									$scope.mpsusers[u].TODAY = txts;
									$scope.mpsusers[u].CREATEDTASKS2 = recs[r].CREATEDTASKS;
									$scope.mpsusers[u].SCHEDULEDTASKS2 = recs[r].SCHEDULEDTASKS;
									$scope.mpsusers[u].MODIFIEDTASKS2 = recs[r].MODIFIEDTASKS;
								} else {
									$scope.mpsusers[u].YESTERDAY = txts;
									$scope.mpsusers[u].CREATEDTASKS1 = recs[r].CREATEDTASKS;
									$scope.mpsusers[u].SCHEDULEDTASKS1 = recs[r].SCHEDULEDTASKS;
									$scope.mpsusers[u].MODIFIEDTASKS1 = recs[r].MODIFIEDTASKS;
								}
							}
						}
					}
					EndProgress(reportskprg); $scope["loaders"]--;
				});
			$scope.mpsusers.forEach(function (user) {
				user.CREATEDTASKS1 = [];
				user.SCHEDULEDTASKS1 = [];
				user.MODIFIEDTASKS1 = [];
				user.CREATEDTASKS2 = [];
				user.SCHEDULEDTASKS2 = [];
				user.MODIFIEDTASKS2 = [];
				var user4proc = user;
				var newuserprog = StartProgress("Loading plan for " + user4proc.PERSON_NAME + "..."); $scope["loaders"]++;
				$http.post("trservice.asmx/getplannedShort", JSON.stringify({ "userid": user4proc.TTUSERID }))
					.then(function (result) {
						user4proc.PLAN = result.data.d;
						EndProgress(newuserprog); $scope["loaders"]--;
					});
			});
		};
		$scope.planLoaded = function (id) {
			for (var i = 0; i < $scope.mpsusers.length; i++) {
				if ($scope.mpsusers[i].ID == id) {
					return $scope.mpsusers[i].PLAN !== undefined;
				}
			}
			return false;
		};
		$scope.loaded = function () {
			return $scope.mpsusers.length > 0 && $scope["loaders"] === 0;
		};
		$scope.changeDate = function () {
			$scope.yesterday = new Date($scope.today.getTime());
			$scope.yesterday.setDate($scope.yesterday.getDate() - 1);
			window.history.pushState("date" + $scope.today, "date" + $scope.today, replaceUrlParam(location.href, "date", DateToString($scope.today)));
			$scope.loadData();
		};

		$scope.loadData();

		var pageisloading = $interval(function () {
			if ($scope.loaded) {
				var s = $("#pageloadnotify").attr("src");
				s = s.substring(s.indexOf("id=") + 3);
				$http.post("trservice.asmx/pageLoadedComplete", JSON.stringify({ "id": s }));
				$interval.cancel(pageisloading);
			}
		}, 1000);
	}]);
});