function CreateChart() {
	var config = {
		type: "pie",
		data: {
			datasets: [{
				data: [],
				backgroundColor: [],
				label: "Completion chart (hours)"
			}],
			labels: []
		},
		options: {
			animation: false,
			responsive: true,
			title: {
				display: true,
				position: "bottom",
				text: "Completion chart (hours)"
			}
		}
	};
	var chartctx = document.getElementById("chartpie").getContext("2d");
	window.chartpie = new Chart(chartctx, config);
}
function DrawChart($scope) {
	var display = {};
	for (var i = 0; i < $scope.defects.length; i++) {
		var d = $scope.defects[i];
		var val = display[d.DISPO];
		if (val != undefined) {
			val += d.ESTIM;
			display[d.DISPO] = val;
		} else {
			display[d.DISPO] = d.ESTIM;
		}
	}
	var d = window.chartpie.data;
	d.labels = [];
	d.datasets.splice(0, 1);
	var newDataset = {
		backgroundColor: [],
		data: [],
		label: "Completion chart (hours)",
	};
	var labels = [];

	window.chartpie.update();

	for (var propertyName in display) {
		newDataset.data.push(display[propertyName]);
		newDataset.backgroundColor.push(getDispoColorById()(propertyName, $scope)["background-color"]);
		labels.push(getDispoNameById()(propertyName, $scope) + " : " + display[propertyName]);
	}
	d.labels = labels;
	d.datasets.push(newDataset);
	window.chartpie.update();
}

$(function () {
	CreateChart();
	var app = angular.module('mpsapplication', []);
	app.filter('getDispoColorById', getDispoColorById);
	app.filter('getUserById', getUserById);
	app.filter("sumFormat", ["$sce", sumFormat]);
	app.filter("defectsTrackFilter", function () {
		return function (defects, sort) {
			if (sort === 0) {
				return defects;
			}

			if (defects) {
				var defectsRet = [];
				for (var i = 0; i < defects.length; i++) {
					let d = defects[i];
					if (sort === 1 && d.EDD && d.VERSION == "") {
						defectsRet.push(d);
					} else if (sort === 2 && d.VERSION != "") {
						defectsRet.push(d);
					}
				}
				if (sort === 1) {
					defectsRet.sort(function (a, b) {
						return a.EDD.getTime() - b.EDD.getTime();
					});
				} else if (sort === 2) {
					defectsRet.sort(function (a, b) {
						let av = a.VERSION.split(".");
						let bv = b.VERSION.split(".");
						if (av.length === 3 && bv.length === 3) {
							if (av[0] !== bv[0]) {
								return av[0] > bv[0] ? -1 : 1;
							}
							let v1 = parseInt(av[2]);
							let v2 = parseInt(bv[2]);
							if (v1 === v2) {
								return 0;
							}
							return v1 > v2 ? -1 : 1;
						}
						return true;
					});
				}
				return defectsRet;
			}
		};
	});

	app.controller('mpscontroller', ["$scope", "$http", "$interval", function ($scope, $http, $interval) {
		$scope.isadmin = IsAdmin();
		getDispos($scope, "dispos", $http);
		getUsers($scope, "users", $http);
		$scope.filters = [];
		$scope.defects = [];
		$scope.id = getParameterByName("id");
		$scope.sort = 0;
		if ($scope.id != "") {
			$.cookie("trackerid", $scope.id, { expires: 365 });
		}
		var tmp = document.getElementById("trackers").value;
		if (tmp === "") {
			$scope.trackers = [];
		} else {
			$scope.trackers = JSON.parse(tmp);
			if ($scope.id == "") {
				$scope.id = $.cookie("trackerid");
				if ($scope.id == undefined) {
					$scope.id = "";
				}
			}
			if ($scope.trackers.length > 0) {
				if ($scope.id != "") {
					var exist = false;
					for (var i = 0; i < $scope.trackers.length; i++) {
						if ($scope.id == $scope.trackers[i].ID) {
							exist = true;
							break;
						}
					}
					if (!exist) {
						$scope.id = "";
					}
				}
				if ($scope.id == "") {
					$scope.id = $scope.trackers[0].ID;
				}
				window.history.pushState(null, "", replaceUrlParam(location.href, "id", $scope.id));
			}
		}

		if ($scope.isadmin) {
			var prgfltr = StartProgress("Loading filters...");
			$http.post("TrackerService.asmx/getFilters", JSON.stringify({ "user": ttUserID() }))
				.then(function (res) {
					$scope.filters = res.data.d;
					EndProgress(prgfltr);
				});
		}
		$scope.taskActions = function (ttid) {
			if (!$scope.isadmin) {
				return;
			}
			$('.taskrect').on('contextmenu', function (e) {
				let ttid = e.target.getAttribute("ttid");
				var top = e.pageY - 10;
				var left = e.pageX - 90;
				$("#context-menu").css({
					display: "block",
					top: top,
					left: left
				}).addClass("show");
				return false; //blocks default Webbrowser right click menu
			}).on("click", function () {
				$("#context-menu").removeClass("show").hide();
			});

			$("#context-menu a").on("click", function () {
				$(this).parent().removeClass("show").hide();
			});
		};
		$scope.assignToClient = function (track, user) {
			var prgass = StartProgress("Assigning...");
			$http.post("TrackerService.asmx/assignTracker", JSON.stringify({ "id": track, "userid": user }))
				.then(function (res) {
					$scope.trackers = res.data.d;
					EndProgress(prgass);
				});
		};
		$scope.delTracker = function (id) {
			var r = confirm("Are you sure you want to delete this tracker?");
			if (r !== true) {
				return;
			}
			var delprg = StartProgress("Deleting...");
			$http.post("TrackerService.asmx/delTracker", JSON.stringify({ "id": id }))
				.then(function (res) {
					$scope.trackers = $scope.trackers.filter(function (item) {
						return (item.ID != id);
					});
					EndProgress(delprg);
				});
		};
		$scope.addTracker = function (id, inputname) {
			if (!inputname) {
				var date = new Date();
				var uname = userLogin();
				var sdate = monthNames[date.getMonth()] + " " + date.getDate() + ", " + date.getFullYear();
				inputname = uname.charAt(0).toUpperCase() + uname.slice(1) + " Requests " + sdate;
			}
			var name = prompt("Please enter the name", inputname);
			if (name !== "" && name !== null) {
				if (!id) {
					id = -1;
				}
				$http.post("TrackerService.asmx/newTracker", JSON.stringify({ "name": name, "user": ttUserID(), "filter": id }))
					.then(function (res) {
						$scope.trackers.push(res.data.d);
						EndProgress(prgfltr);
					});
			}
		};
		$scope.loadData = function () {
			if ($scope.id !== "") {
				var taskprg = StartProgress("Loading data...");
				var tr = $scope.trackers.find(function (item) { return item.ID == $scope.id });
				$http.post("TrackerService.asmx/getItems", JSON.stringify({ "trackerid": tr.ID }))
					.then(function (res) {
						$scope.defects = res.data.d.ITEMS;
						$scope.defects.forEach(function (val) {
							if (val.EDD !== "" && typeof val.EDD !== "undefined") {
								let prts = val.EDD.split("/");
								if (prts.length === 3) {
									val.EDD = new Date(prts[2], prts[0] - 1, prts[1]);
								}
							} else {
								val.EDD = undefined;
							}
						});
						tr.Completes = res.data.d.TRACKER.Completes;
						EndProgress(prgfltr);
						DrawChart($scope);
					});
				EndProgress(taskprg);
				reActivateTooltips();
			}
		};
		$scope.trackUserID = function () {
			if ($scope.id == "") {
				return -1;
			} else {
				return $scope.trackers.find(function (item) { return item.ID == $scope.id }).IDCLIENT;
			}
		};
		$scope.trackUserName = function () {
			var id = $scope.trackUserID();
			if (id < 0) {
				return "";
			} else {
				return $scope.trackers.find(function (item) { return item.ID == $scope.id }).IDCLIENT;
			}
		};
		$scope.messageKey = function (event) {
			if (event.keyCode === 13) {
				if (!$scope.newtask) {
					return;
				}
				$scope.lasttaskinput = new Date().getTime();
				$http.post("TrackerService.asmx/newTask", JSON.stringify({ summary: $scope.newtask, trackerid: $scope.id }))
					.then(function (res) {
						$scope.defects.unshift(res.data.d);
						$scope.lastloaded = "internal event";
					});
				$scope.newtask = "";
			}
		};

		var references = $interval(function () {
			if (!inProgress()) {
				$scope.loadData();
				$interval.cancel(references);
			}
		}, 200);

		$scope.pageName = "Task Tracker";
		$scope.simpleTracker = false;
		$scope.lastloaded = "";
		var dt = new Date();
		dt.setHours(dt.getHours() - 1);
		$scope.lasttaskinput = dt.getTime();
		$scope.requireSynch = function () {
			var timeout = new Date().getTime() - $scope.lasttaskinput;
			if (timeout < 1000 * 20) { //ask for updates only after delay to allow sql server update text indexes
				return false;
			}
			return true;
		};
		if ($scope.id != "") {
			var tracker = $scope.trackers.find(function (item) { return item.ID == $scope.id });
			if (tracker) {
				$scope.pageName = tracker.NAME;
				$scope.simpleTracker = tracker.IDFILTER < 0;
			} else {
				$scope.pageName = "There are no trackers assigned to this user";
				$scope.simpleTracker = false;
			}
			$http.post("TrackerService.asmx/getTrackerModified", JSON.stringify({ "id": $scope.id }))
				.then(function (res) {
					$scope.lastloaded = res.data.d;
					console.log("fist loaded " + $scope.lastloaded);
				});

			$interval(function () {
				if (!$scope.requireSynch()) {
					return;
				}
				$http.post("TrackerService.asmx/getTrackerModified", JSON.stringify({ "id": $scope.id }))
					.then(function (res) {
						console.log(res.data.d);
						if ($scope.lastloaded === "") {
							$scope.lastloaded = "" + res.data.d;
							return;
						}
						if ($scope.lastloaded !== res.data.d) {
							$scope.lastloaded = "" + res.data.d;
							$scope.loadData();
						}
					});

			}, 5000);
			$interval(function () {
				if (!$scope.requireSynch()) {
					return;
				}
				$scope.loadData();
			}, 300000);
		}
		setTimeout(function () { $("#sortbtn").tooltip('show'); }, 2000);
		setTimeout(function () { $("#sortbtn").tooltip('hide'); }, 5000);
		//$('.toast').toast('show');
	}]);
	app.directive('ngRightClick', function ($parse) {
		return function (scope, element, attrs) {
			var fn = $parse(attrs.ngRightClick);
			element.bind('contextmenu', function (event) {
				scope.$apply(function () {
					event.preventDefault();
					fn(scope, { $event: event });
				});
			});
		};
	});
});