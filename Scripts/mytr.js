function stringToTime(st) {
	var vals = st.split(':');
	if (vals.length != 3)
		return new Date();
	return new Date(0, 0, 0, vals[0], vals[1], vals[2]);
}
function timeToString(dt) {
	return pad(dt.getHours(), 2) + ":" + pad(dt.getMinutes(), 2) + ":" + pad(dt.getSeconds(), 2);
}
$(function () {
	var app = angular.module('mpsapplication', []);
	app.filter('getDispoById', getDispoById);
	app.filter('getDispoColorById', getDispoColorById);

	var filter = {};
	createTasksFilter(filter);
	filter.users.push(ttUserID());
	$("#mytasks").attr("href", replaceUrlParam("ttrep.aspx", "filter", JSON.stringify(filter)));
	filter.users.splice(0);
	filter.createdUsers.push(ttUserID());
	$("#metasks").attr("href", replaceUrlParam("ttrep.aspx", "filter", JSON.stringify(filter)));

	app.controller('mpscontroller', ["$scope", "$http", "$interval", function ($scope, $http, $interval) {

		getDispos($scope, "dispos", $http);

		$scope.checkBirthday = function () {
			$scope.haveBirthday = false;
			$scope.birthdayID = "-1";
			$scope.birthdayYears = 0;
			var today = DateToString(new Date());
			$scope.mpsusers.forEach(function (u) {
				if (u.INWORK && u.BIRTHDAY.substring(0, 5) === today.substring(0, 5)) {
					$scope.haveBirthday = true;
					$scope.birthdayID = u.ID;
					$scope.birthdayYears = (new Date()).getFullYear() - u.BIRTHDAY.substring(6, 10);
				}
			});
		};

		var d = new Date();
		d.setHours(0, 0, 0, 0);
		$scope.date = d;
		$scope.status = "Working...";

		$scope.storeData = function () {
			if (!($scope.trrec)) {
				return;
			}
			var copy = Object.assign({}, $scope.trrec);
			copy.DATE = DateToString(copy.DATE);
			copy.IN = timeToString(copy.IN);
			copy.OUT = timeToString(copy.OUT);
			copy.BREAK = timeToString(copy.BREAK);

			var storeprg = StartProgress("Storing data...");
			$scope.status = "Saving...";
			$http.post("trservice.asmx/settrrec", JSON.stringify({ "rec": copy })).then(function (response) {
				EndProgress(storeprg);
				$scope.status = "Saved.";
			});
			$scope.changed = false;
		};
		$interval(function () {
			if ($scope.changed) {
				$scope.storeData();
			}
		}, 2000);
		$interval(function () {
			if ("hidden" in document && document.hidden) {
				return;
			}
			$http.post("trservice.asmx/gettrrec", JSON.stringify({ "date": DateToString($scope.date) }))
				.then(function (response) {
					if (response.data.d && $scope.trrec) {
						$scope.trrec.CREATEDTASKS = response.data.d.CREATEDTASKS;
						$scope.trrec.SCHEDULEDTASKS = response.data.d.SCHEDULEDTASKS;
						$scope.trrec.MODIFIEDTASKS = response.data.d.MODIFIEDTASKS;
					}
				});

		}, 60000);
		$scope.percentdonestyle = "progress-bar-danger";
		$scope.recalcPercent = function () {
			if ($scope.trrec) {
				var diff = $scope.trrec.OUT.getTime() - $scope.trrec.IN.getTime();
				$scope.percentdone = Math.ceil(diff / 1000 / 3600 / 9 * 100);
				var secs = diff / 1000.0;
				var hrs = Math.floor(secs / 3600.0);
				var mins = Math.floor(secs / 60.0 - hrs * 60.0);
				$scope.timedone = "presence: " + hrs + ":" + mins;
				if ($scope.percentdone < 25) {
					$scope.percentdonestyle = "progress-bar-danger";
				} else if ($scope.percentdone < 50) {
					$scope.percentdonestyle = "progress-bar-warning";
				} else if ($scope.percentdone < 75) {
					$scope.percentdonestyle = "progress-bar-info";
				} else {
					$scope.percentdonestyle = "progress-bar-success";
				}
			}
			else {
				$scope.percentdonestyle = "progress-bar-danger"
				$scope.percentdone = 0;
			}
		};
		$scope.out = function () {
			var d = new Date();
			d = new Date(0, 0, 0, d.getHours(), d.getMinutes());
			$scope.trrec.OUT = d;
		};

		$interval(function () {
			if ($scope.autotime && $scope.isTodayRecord()) {
				$scope.out();
			}
		}, 30000);

		$scope.findRec = function () {
			$scope.storeData();
			$scope.loadData();
		};
		$scope.loadData = function () {
			var taskprg = StartProgress("Loading data...");
			$scope.status = "Loading...";
			$http.post("trservice.asmx/gettrrec", JSON.stringify({ "date": DateToString($scope.date) }))
				.then(function (response) {
					$scope.trrec = response.data.d;
					if ($scope.trrec) {
						$scope.trrec.DATE = StringToDate($scope.trrec.DATE);
						$scope.trrec.IN = stringToTime($scope.trrec.IN);
						$scope.trrec.OUT = stringToTime($scope.trrec.OUT);
						$scope.trrec.BREAK = stringToTime($scope.trrec.BREAK);
						$scope.recalcPercent();
					}
					$scope.status = "Saved.";
					$scope.datestring = $scope.date.toDateString();
					EndProgress(taskprg);
				});
		};
		$scope.defects = [];
		$scope.unscheduled = [];
		$scope.loadTasks = function () {
			$http.post("trservice.asmx/getplanned", JSON.stringify({ "userid": "" }))
				.then(function (response) {
					$scope.defects = response.data.d;
					reActivateTooltips();
				});
			$http.post("trservice.asmx/getunplanned", JSON.stringify({ "userid": "" }))
				.then(function (response) {
					$scope.unscheduled = response.data.d;
				});
		};
		$scope.changeDispo = function (d, disp) {
			if ($scope.loaded()) {
				$http.post("trservice.asmx/settaskdispo", JSON.stringify({ "ttid": d.ID, "disp": disp.ID })).then(function (response) {
					if (response.data.d) {
						var idxDisp = $scope.dispos.findIndex(function (x) { return x.ID == disp.ID; });
						if (!$scope.dispos[idxDisp].REQUIREWORK) {
							var idx = $scope.defects.findIndex(function (x) { return x.ID == d.ID; });
							$scope.defects.splice(idx, 1);
						} else {
							for (var i = 0; i < $scope.defects.length; i++) {
								if ($scope.defects[i].ID == response.data.d.ID) {
									$scope.defects[i] = response.data.d;
									return;
								}
							}
						}
					} else {
						alert("The task is locked - cannot change it - please go to task details and see who has locked it!");
					}
				});
			}
		};
		$scope.workTaskUns = function (d) {
			if ($scope.loaded()) {
				var index = $scope.dispos.findIndex(function (x) { return x.WORKING == 1; });
				if (index > -1) {
					d.ORDER = 1;
					d.DISPO = $scope.dispos[index].ID;
					var di = $scope.unscheduled.findIndex(function (x) { return x == d; });
					$scope.unscheduled.splice(di, 1);
					$scope.defects.unshift(d);
					$scope.trrec.DONE = "TT" + d.ID + "(" + d.ESTIM + ") " + d.SUMMARY + "\n" + $scope.trrec.DONE;
					$scope.changeDispo(d, $scope.dispos[index]);
				}
			}
		};
		$scope.workTask = function (d) {
			if ($scope.loaded()) {
				for (var i = 0; i < $scope.dispos.length; i++) {
					if ($scope.dispos[i].WORKING == 1) {
						$scope.trrec.DONE = "TT" + d.ID + "(" + d.ESTIM + ") " + d.SUMMARY + "\n" + $scope.trrec.DONE;
						$scope.changeDispo(d, $scope.dispos[i]);
					}
				}
			}
		};
		$scope.changed = false;
		$scope.$watchCollection('trrec', function (newval, oldval) {
			if (newval && oldval) {
				$scope.changed = true;
				$scope.recalcPercent();
				$scope.status = "Working...";
			}
		});

		$scope.loadTasks();
		$scope.loadData();

		$scope.todayRec = function () {
			$(".tooltip").tooltip("hide");
			$http.post("trservice.asmx/todayrrec", JSON.stringify({ "lastday": $scope.copylastday })).then(function () {
				var d = new Date();
				d.setHours(0, 0, 0, 0);
				$scope.date = d;
				$scope.loadData();
				$scope.checkBirthday();
				$.connection.notifyHub.server.requestRoomUsers();
			});
		};

		$scope.autotime = $.cookie("autotime") === "true";
		$scope.copylastday = $.cookie("copylastday") === "true";
		$scope.oncopylastday = function () {
			$.cookie("copylastday", $scope.copylastday, { expires: 365 });
		};
		$scope.changeAutoDate = function () {
			$.cookie("autotime", $scope.autotime, { expires: 365 });
		};

		$scope.loaded = function () {
			if ($scope.trrec)
				return true;
			return false;
		};
		$scope.deleteRec = function () {
			if (confirm("Are you sure you want to delete current record?")) {
				$http.post("trservice.asmx/deltrrec", JSON.stringify({ "id": $scope.trrec.ID })).then(function () {
					$scope.loadData();
				});
			}
		};
		$scope.addRec = function () {
			$http.post("trservice.asmx/addrec", JSON.stringify({ "date": DateToString($scope.date), "lastday": 1 })).then(function () {
				$scope.loadData();
			});
		};

		$scope.isTodayRecord = function () {
			var d = new Date();
			d.setHours(0, 0, 0, 0);
			return ($scope.trrec) && ($scope.date.getTime() === d.getTime());
		};

		$http.post("trservice.asmx/getcurrentuser", JSON.stringify({}))
			.then(function (response) {
				$scope.user = response.data.d;
			});

		var notifyHub = $.connection.notifyHub;
		notifyHub.client.onPlanChanged = function (userid) {
			if (userID() == userid) {
				$scope.loadTasks();
				$scope.$apply();
			}
		};
		notifyHub.client.onRoomChanged = function (users) {
			for (var i = 0; i < users.length; i++) {
				for (var j = 0; j < $scope.mpsusers.length; j++) {
					if ($scope.mpsusers[j].ID == users[i].ID) {
						$scope.mpsusers[j].STATUS = users[i].STATUS;
					}
				}
			}
			$scope.$apply();
		};
		notifyHub.client.onBuildStatusChanged = function (id, ttid, userid, message) {
			if (userid == ttUserID()) {
				if (Notification.permission !== "granted") {
					Notification.requestPermission();
				}
				var notification = new Notification('Your build request TT' + ttid + ' has changed its status!', {
					icon: 'images/fist.png',
					body: message
				});
				notification.onclick = function () {
					window.open("showtask.aspx?ttid=" + ttid);
				};
			}
		};
		notifyHub.client.onMessage = function (fromID, message) {
			if (Notification.permission !== "granted") {
				Notification.requestPermission();
			}
			var notification = new Notification('message', {
				icon: 'getUserImg.ashx?id=' + fromID,
				body: message
			});
		};

		$.connection.hub.disconnected(function () {
			setTimeout(function () {
				$.connection.hub.start().done(function () {
					notifyHub.server.requestRoomUsers();
					notifyHub.server.registerMessenger(userID());
				});
			}, 5000); // Restart connection after 5 seconds.
		});

		getMPSUsers($scope, "mpsusers", $http, function () {
			$scope.checkBirthday();
			$.connection.hub.start().done(function () {
				notifyHub.server.requestRoomUsers();
				notifyHub.server.registerMessenger(userID());
			});
		});
	}]);
});