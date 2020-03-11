function moretasks(elem) {
	setTimeout(function () { $(elem).tooltip("hide"); }, 200);
	var items = elem.parentElement.querySelectorAll(".task-last");
	items.forEach(function (i) {
		if (i.style.display === "none" || i.style.display === "") {
			i.style.display = "block";
		} else {
			i.style.display = "none";
		}
	});
}
$(function () {
	var app = angular.module('mpsapplication', []);
	app.filter('getDispoColorById', getDispoColorById);
	app.filter("sumFormat", ["$sce", sumFormat]);

	app.controller('mpscontroller', ["$scope", "$http", "$interval", function ($scope, $http, $interval) {
		$scope["loaders"] = 0;
		$scope.today = new Date();
		if (getParameterByName("date") !== "") {
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
						difference = difference === 0 ? currd : Math.min(currd, difference);
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
			$scope.personsOnline = 0;
			$scope.personsVacation = 0;

			$scope.todaystring = DateToString($scope.today);
			$scope.yesterdaystring = DateToString($scope.yesterday);

			var vacationprg = StartProgress("Loading vacations..."); $scope["loaders"]++;
			$http.post("trservice.asmx/enumCloseVacations", JSON.stringify({ "start": DateToString($scope.yesterday), "days": 31 }))
				.then(function (result) {
					var today = new Date();
					$scope.vacations = result.data.d;
					$scope.vacations.forEach(function (vac) {
						vac.nextin = [];
						if (vac.DATE === $scope.todaystring) {
							$scope.personsVacation++;
						}
						var vacdate = StringToDate(vac.DATE);
						vac.order = Math.round((vacdate.getTime() - today.getTime()) / 1000 / 60 / 60 / 24) - 1;
						var closevac = 999;
						for (var i = 0; i < $scope.vacations.length; i++) {
							var curr = $scope.vacations[i];
							if (curr !== vac && curr.AUSER === vac.AUSER) {
								var currdate = StringToDate(curr.DATE);
								var diff = Math.round((currdate.getTime() - vacdate.getTime()) / 1000 / 60 / 60 / 24);
								if (diff > 0) {
									closevac = Math.min(closevac, diff);
								}
							}
						}
						if (closevac < 999) {
							vac.nextin = new Array(closevac - 1);
						}
						for (i = 0; i < vac.nextin.length; i++) {
							var cd = new Date();
							cd.setDate(vacdate.getDate() + i + 1);
							vac.nextin[i] = cd.getDay() === 0 || cd.getDay() === 6;
						}
					});
					EndProgress(vacationprg); $scope["loaders"]--;
					reActivateTooltips();
				});

			var reportskprg = StartProgress("Loading reports..."); $scope["loaders"]++;
			$http.post("trservice.asmx/getreports", JSON.stringify({ "dates": [DateToString($scope.today), DateToString($scope.yesterday)] }))
				.then(function (result) {
					var d1 = DateToString($scope.today);
                    var recs = result.data.d;
                    $scope.mpsusers.forEach(function (u) {
                        u.TODAY = undefined;
                        u.TODAYIN = undefined;
                        u.TODAYOUT = undefined;
                        u.CREATEDTASKS2 = undefined;
                        u.SCHEDULEDTASKS2 = undefined;
                        u.MODIFIEDTASKS2 = undefined;
                        u.TASKSEVENTS2 = undefined;
                        u.YESTERDAY = undefined;
                        u.CREATEDTASKS1 = undefined;
                        u.SCHEDULEDTASKS1 = undefined;
                        u.MODIFIEDTASKS1 = undefined;
                        u.TASKSEVENTS1 = undefined;
                    });
					for (var r = 0; r < recs.length; r++) {
						var rec = recs[r];
						for (var u = 0; u < $scope.mpsusers.length; u++) {
							var user = $scope.mpsusers[u];
							if (rec.USER == user.ID) {
								var txts = rec.DONE.split(/\r?\n/);
								if (rec.DATE == d1) {
									$scope.personsOnline++;
									user.TODAY = txts;
									user.TODAYIN = rec.IN.substring(0, 5);
									user.TODAYOUT = rec.OUT.substring(0, 5);
									user.CREATEDTASKS2 = rec.CREATEDTASKS;
									user.SCHEDULEDTASKS2 = rec.SCHEDULEDTASKS;
									user.MODIFIEDTASKS2 = rec.MODIFIEDTASKS;
									user.TASKSEVENTS2 = rec.TASKSEVENTS;
								} else {
									user.YESTERDAY = txts;
									user.CREATEDTASKS1 = rec.CREATEDTASKS;
									user.SCHEDULEDTASKS1 = rec.SCHEDULEDTASKS;
									user.MODIFIEDTASKS1 = rec.MODIFIEDTASKS;
									user.TASKSEVENTS1 = rec.TASKSEVENTS;
								}
							}
						}
					}
					EndProgress(reportskprg); $scope["loaders"]--;
					reActivateTooltips();
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
                $http.post("PlanService.asmx/getplannedShort", JSON.stringify({ "userid": user4proc.TTUSERID }))
					.then(function (result) {
						user4proc.PLAN = result.data.d;
						EndProgress(newuserprog); $scope["loaders"]--;
					});
			});
			reActivateTooltips();
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
		$scope.createDSFilter = createDSFilter;
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