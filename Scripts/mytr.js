﻿function stringToTime(st) {
	var vals = st.split(':');
	if (vals.length != 3)
		return new Date();
	return new Date(0, 0, 0, vals[0], vals[1], vals[2]);
}
function timeToString(dt) {
	return pad(dt.getHours(), 2) + ":" + pad(dt.getMinutes(), 2) + ":" + pad(dt.getSeconds(), 2);
}
$(function () {
	//var worker = new SharedWorker("scripts/events.js");
	//worker.port.addEventListener("message", function (e) {
	//	alert(e.data);
	//}, false);
	//worker.port.start();
	//var s = new WebSocket(((window.location.protocol === "https:") ? "wss://" : "ws://") + window.location.host + "/ws");

	var app = angular.module("mpsapplication", []);
	app.filter("getDispoById", getDispoById);
	app.filter("getMPSUserLoginById", getMPSUserLoginById);
	app.filter("getDispoColorById", getDispoColorById);
	app.filter("rawHtml", ["$sce", rawHtml]);
	app.filter("sumFormat", ["$sce", sumFormat]);

	var filter = {};
	createTasksFilter(filter);
	filter.users.push(ttUserID());
	$("#mytasks").attr("href", replaceUrlParam("ttrep.aspx", "filter", JSON.stringify(filter)));
	filter.users.splice(0);
	filter.createdUsers.push(ttUserID());
	$("#metasks").attr("href", replaceUrlParam("ttrep.aspx", "filter", JSON.stringify(filter)));

	$('.toast').toast('show');

	app.controller('mpscontroller', ["$scope", "$http", "$interval", function ($scope, $http, $interval) {

		/*var loc = window.location, new_uri;
		if (loc.protocol === "https:") {
			new_uri = "wss:";
		} else {
			new_uri = "ws:";
		}
		new_uri += "//" + loc.host;
		new_uri += loc.pathname + "/Events.ashx";

		if (typeof (WebSocket) !== 'undefined') {
			$scope.socket = new WebSocket(new_uri);
		} else {
			$scope.socket = new MozWebSocket(new_uri);
		}

		$scope.socket.onmessage = function (msg) {
			console.log(msg.data);
		};
		$scope.socket.onclose = function (event) {
			alert('Socket Cloesd!');
		};

		$interval(function () {
			if ($scope.socket.readyState === WebSocket.OPEN) {
				$scope.socket.send(new Date());
			}
		}, 2000);*/

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
		$scope.showTextRep = !($.cookie("showTextRep") === "false");
		
		$scope.changeShowTextRep = function () {
			$scope.showTextRep = !$scope.showTextRep;
			$.cookie("showTextRep", $scope.showTextRep, { expires: 365 });
		};
		$scope.storeData = function () {
			if (!($scope.trrec)) {
				return;
			}
			var copy = Object.assign({}, $scope.trrec);
			copy.DATE = DateToString(copy.DATE);
			copy.IN = timeToString(copy.IN);
			copy.OUT = timeToString(copy.OUT);
			copy.BREAK = timeToString(copy.BREAK);
			copy.TASKSEVENTS = [];
			copy.MODIFIEDTASKS = [];
			copy.CREATEDTASKS = [];
			copy.SCHEDULEDTASKS = [];

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
		$scope.percentdonestyle = "bg-danger";
		$scope.recalcPercent = function () {
			if ($scope.trrec) {
				var diff = $scope.trrec.OUT.getTime() - $scope.trrec.IN.getTime();
				$scope.percentdone = Math.ceil(diff / 1000 / 3600 / 9 * 100);
				var secs = diff / 1000.0;
				var hrs = Math.floor(secs / 3600.0);
				var mins = Math.floor(secs / 60.0 - hrs * 60.0);
				$scope.timedone = "In Office: " + hrs + ":" + mins + " hrs";
				if ($scope.percentdone < 25) {
					$scope.percentdonestyle = "bg-danger";
				} else if ($scope.percentdone < 50) {
					$scope.percentdonestyle = "bg-warning";
				} else if ($scope.percentdone < 75) {
					$scope.percentdonestyle = "bg-info";
				} else {
					$scope.percentdonestyle = "bg-success";
				}
			}
			else {
				$scope.percentdonestyle = "bg-danger";
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

		$scope.todayCommits = [];
		$scope.LoadTodayCommits = function () {
			$http.post("GitService.asmx/TodayCommits", JSON.stringify({}))
				.then(function (result) {
					$scope.todayCommits = result.data.d;
					$scope.todayCommits.forEach(function (c) {
						c.ID = c.TTID;
					});
				});
		}
		$interval(function () {
			$scope.LoadTodayCommits();
		}, 60000);
		$scope.LoadTodayCommits();

		$scope.versionTag = "";
		$scope.LoadversionTag = function () {
			$http.post("GitService.asmx/LastTag", JSON.stringify({}))
				.then(function (result) {
					$scope.versionTag = result.data.d;
				});
		}
		$interval(function () {
			$scope.LoadversionTag();
		}, 60000);
		$scope.LoadversionTag();

		$scope.GetFile = function (v, t) {
			window.open('getinstall.ashx?type=' + t + '&version=' + v);
		};

		$scope.progress = function () {
			return inProgress();
		};
		$scope.findRec = function () {
			$scope.storeData();
			$scope.loadData();
		};
		$scope.processTrRec = function (r) {
			$scope.trrec = r;
			if ($scope.trrec) {
				$scope.trrec.DONE = decodeURIComponent($scope.trrec.DONE);
				$scope.trrec.DATE = StringToDate($scope.trrec.DATE);
				$scope.trrec.IN = stringToTime($scope.trrec.IN);
				$scope.trrec.OUT = stringToTime($scope.trrec.OUT);
				$scope.trrec.BREAK = stringToTime($scope.trrec.BREAK);
				if ($scope.trrec.TASKSEVENTS) {
					$scope.trrec.TASKSEVENTS.forEach(function (e) {
						e.DEFECT.SUMMARY = decodeURIComponent(e.DEFECT.SUMMARY);
					});
				}

				$scope.recalcPercent();
			}
			$scope.status = "Saved.";
			$scope.datestring = $scope.date.toDateString();
		};
		$scope.loadEvents = function () {
			var prg = StartProgress("Loading events...");
			$http.post("DefectService.asmx/getDayEvents", JSON.stringify({ "date": DateToString($scope.date) }))
				.then(function (result) {
					if ($scope.trrec) {
						$scope.trrec.TASKSEVENTS = result.data.d;
					}
					EndProgress(prg);
					reActivateTooltips();
				});
		};
		$scope.spendEvent = function (id, hrs) {
			var prg = StartProgress("Updating event...");
			$http.post("DefectService.asmx/spendEvent", JSON.stringify({ "id": id, "hrs": hrs }))
				.then(function () {
					EndProgress(prg);
				});
		};
		$scope.deleteEvent = function (id) {
			var prg = StartProgress("Deleting event...");
			$http.post("DefectService.asmx/delEvent", JSON.stringify({ "id": id }))
				.then(function () {
					EndProgress(prg);
				});
		};
		$scope.loadData = function () {
			var taskprg = StartProgress("Loading data...");
			$scope.status = "Loading...";
			$http.post("trservice.asmx/gettrrec", JSON.stringify({ "date": DateToString($scope.date) }))
				.then(function (response) {
					$scope.processTrRec(response.data.d);
					EndProgress(taskprg);
				});
			$scope.loadEvents();
		};
		$scope.defects = [];
		$scope.unscheduled = [];
		$scope.loadTasks = function () {
			$http.post("PlanService.asmx/getplannedShort", JSON.stringify({ "userid": "" }))
				.then(function (response) {
					$scope.defects = response.data.d;
					reActivateTooltips();
				});
			$http.post("PlanService.asmx/getunplanned", JSON.stringify({ "userid": "" }))
				.then(function (response) {
					$scope.unscheduled = response.data.d;
				});
		};
		$scope.changeDispo = function (d, disp, hrs) {
			if ($scope.loaded()) {
				var url = hrs > 0 ? "DefectService.asmx/startEvent" : "DefectService.asmx/setTaskDispo";
				var par = hrs > 0 ? JSON.stringify({ "ttid": d.ID, "disp": disp.ID, "hrs": hrs, "date": DateToString($scope.date) }) : JSON.stringify({ "ttid": d.ID, "disp": disp.ID });
				$http.post(url, par).then(function (response) {
					$scope.loadEvents();
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
		$scope.getOptimalHoursUsed = function (d) {
			var total = 0;
			$scope.trrec.TASKSEVENTS.forEach(function (e) {
				total += parseInt(e.TIME);
			});
			if (total >= 7) {
				return 1;
			}
			if (total < 1) {
				return Math.min(8, parseInt(d.ESTIM));
			}
			return Math.min(8 - total, 8, d.ESTIM);
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
					$scope.changeDispo(d, $scope.dispos[index], $scope.getOptimalHoursUsed(d));
				}
			}
			killTooltips();
		};
		$scope.workTask = function (d) {
			if (d.SPENT >= d.ESTIM) {
				alert("This task is running out of time. You must give new correct estimation in order to continue!!!");
				return;
			}
			if ($scope.loaded()) {
				for (var i = 0; i < $scope.dispos.length; i++) {
					if ($scope.dispos[i].WORKING == 1) {
						$scope.changeDispo(d, $scope.dispos[i], $scope.getOptimalHoursUsed(d));
					}
				}
			}
			killTooltips();
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

		var defrec = document.getElementById("trrec").value;
		if (defrec !== "") {
			$scope.processTrRec(JSON.parse(defrec));
		}

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
		$scope.onCopyLastDay = function () {
			$scope.copylastday = !$scope.copylastday;
			$.cookie("copylastday", $scope.copylastday, { expires: 365 });
		};
		$scope.onChangeAutoTime = function () {
			$scope.autotime = !$scope.autotime;
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
		$scope.addTask = function () {
			var summary = prompt("Enter Summary For New Task:", "Free To Use");
			if (summary !== "" && summary !== null) {
				$http.post("trservice.asmx/newTask4MeNow", JSON.stringify({ "summary": summary }))
					.then(function (response) {
						openTask(response.data.d);
					});
			}
		};
		$scope.planTask = function () {
			var summary = prompt("Enter Summary For New Task:", "Free To Use");
			if (summary !== "" && summary !== null) {
				$http.post("trservice.asmx/planTask", JSON.stringify({ "summary": summary, "ttuserid": -1 }))
					.then(function (response) {
						openTask(response.data.d);
					});
			}
		};
		$scope.isTodayRecord = function () {
			var d = new Date();
			d.setHours(0, 0, 0, 0);
			return ($scope.trrec) && ($scope.date.getTime() === d.getTime());
		};

		var notifyHub = $.connection.notifyHub;
		notifyHub.client.onPlanChanged = function (userid) {
			if (userID() == userid) {
				$scope.loadTasks();
				$scope.loadEvents();
				$scope.$apply();
			}
		};
		$scope.roomUserClass = function (status) {
			if (status === 2) {
				return "btn-danger";
			} else if (status === 3) {
				return "btn-success";
			} else if (status === 4) {
				return "btn-primary";
			}
			return "'btn-light";
		};
		$scope.roomUsers = [];
		notifyHub.client.onRoomChanged = function (users) {
			if (angular.toJson($scope.roomUsers) != JSON.stringify(users)) {
				$scope.roomUsers = users;
			}
			$scope.$apply();
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
				});
			}, 5000); // Restart connection after 5 seconds.
		});

		$scope.notifyHub = notifyHub;
		$scope.congratulate = function () {
			$scope.notifyHub.server.sendMessage(userID(), $scope.birthdayID, "Congratulations!!!");
		};

		getMPSUsers($scope, "mpsusers", $http, function () {
			$scope.checkBirthday();
			$.connection.hub.start().done(function () {
				notifyHub.server.requestRoomUsers();
			});
		});
	}]);
});