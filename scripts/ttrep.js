$(function () {
	//fix after resizable columns support:
	$("table thead tr th").css("overflow", "visible");

	$(".refmenu").click(function (event) {
		var el = event.target.parentElement.querySelector(".refselector");
		setTimeout(function () { el.focus(); }, 100);
	});
	$(".refselector").keyup(function (event) {
		if (event.keyCode === 27) { //esc
			event.target.parentElement.parentElement.classList.remove("open");
			return;
		}
		var txt = event.target.value.toLowerCase();
		var items = event.target.parentElement.parentElement.querySelectorAll("label");
		if (txt === "") {
			items.forEach(function (i) {
				i.style.display = "";
			});
		}
		else {
			items.forEach(function (i) {
				if (i.innerText.toLowerCase().includes(txt)) {
					i.style.display = "";
				} else {
					i.style.display = "none";
				}
			});
		}
	});

	var app = angular.module('mpsapplication', []);
	app.filter('getUserById', getUserById);
	app.filter('getCompById', getCompById);
	app.filter('getSeveById', getSeveById);
	app.filter('getDispoById', getDispoById);
	app.filter('getDispoColorById', getDispoColorById);
	app.filter('getUserTRIDById', getUserTRIDById);
	app.filter("sumFormat", ["$sce", sumFormat]);
	app.filter('rawHtml', ['$sce', rawHtml]);

	app.controller('mpscontroller', ["$scope", "$http", function ($scope, $http) {
		var f = getParameterByName("filter");
		if (f) {
			localStorage.DefectsFilter = f;
		}
		window.addEventListener("popstate", function (event) {
			localStorage.DefectsFilter = JSON.stringify(Object.assign({}, event.state));
			$scope.loadData();
		});

		$scope.onGo = function (keyEvent) {
			if (keyEvent.which === 13) {
				$scope.applyfilter();
				keyEvent.preventDefault();
			}
		};

		//references section
		getDispos($scope, "dispos", $http);
		getTypes($scope, "types", $http);
		getPriorities($scope, "priorities", $http);
		getSevers($scope, "severs", $http);
		getProducts($scope, "products", $http);
		getComps($scope, "comps", $http);
		getPriorities($scope, "prios", $http);

		$scope.getServiceFilter = function () {
			var o = Object.assign({}, $scope.DefectsFilter);
			o.startDateEnter = o.startDateEnter === "" ? "" : DateToString(o.startDateEnter);
			o.endDateEnter = o.endDateEnter === "" ? "" : DateToString(o.endDateEnter);
			o.startDateCreated = o.startDateCreated === "" ? "" : DateToString(o.startDateCreated);
			o.endDateCreated = o.endDateCreated === "" ? "" : DateToString(o.endDateCreated);
			return o;
		};
		$scope.updateFilterObjects = function () {
			$scope.users.forEach(function (u) {
				u.filter = false;
				u.createdFilter = false;
				$scope.DefectsFilter.users.forEach(function (id) {
					if (u.ID == id) {
						u.filter = true;
					}
				});
				$scope.DefectsFilter.createdUsers.forEach(function (id) {
					if (u.ID == id) {
						u.createdFilter = true;
					}
				});
			});
			$scope.dispos.forEach(function (d) {
				d.filter = false;
				$scope.DefectsFilter.dispositions.forEach(function (id) {
					if (d.ID == id) {
						d.filter = true;
					}
				});
			});
			$scope.comps.forEach(function (d) {
				d.filter = false;
				$scope.DefectsFilter.components.forEach(function (id) {
					if (d.ID == id) {
						d.filter = true;
					}
				});
			});
			$scope.severs.forEach(function (d) {
				d.filter = false;
				$scope.DefectsFilter.severities.forEach(function (id) {
					if (d.ID == id) {
						d.filter = true;
					}
				});
			});
		};
		$scope.loadData = function () {
			$scope.defectsselected = false;
			var taskprg = StartProgress("Loading tasks...");
			$scope.changed = false;
			$scope.DefectsFilter = {};
			if (localStorage.DefectsFilter) {
				$scope.DefectsFilter = JSON.parse(localStorage.DefectsFilter);
			}
			createTasksFilter($scope.DefectsFilter);
			$http.post("trservice.asmx/gettasks", JSON.stringify({ "f": $scope.getServiceFilter() }))
				.then(function (response) {
					$scope.defects = response.data.d;
					var efforts = {};
					for (var i = 0; i < $scope.defects.length; i++) {
						var d = $scope.defects[i];
						d.checked = false;
						var dsp = "" + d.DISPO;
						if (dsp in efforts) {
							efforts[dsp] += d.ESTIM;
						} else {
							efforts[dsp] = d.ESTIM;
						}
					}
					$scope.effort = "";
					for (var eff in efforts) {
						var clr = getDispoColorById()(eff, $scope)["background-color"];
						$scope.effort += "<span class='badge' style='background-color:" + clr + "'>" + efforts[eff] + "</span>";
					}
					EndProgress(taskprg);
				});
			$scope.updateFilterObjects();
		};
		$scope.initFilters = function (filters) {
			$scope.filters = [{ ID: 0, NAME: "<Select>" }];
			if (filters) {
				filters.forEach(function (f) {
					$scope.filters.push(f);
				});
			}
		};
		$scope.filterusers = true;
		$scope.updateUsersFilter = function () {
			$scope.users.forEach(function (u) {
				u.show = !$scope.filterusers || (u.ACTIVE && u.TRID >= 1);
			});
		};
		$scope.duplicate = function () {
			var r = confirm("Are you sure you want to duplicate all currently selected tasks?");
			if (r === true) {
				var prg = StartProgress("Duplicating...");
				var ids = [];
				$scope.defects.forEach(function (d) {
					if (d.checked) {
						ids.push(d.ID);
					}
				});
				$http.post("trservice.asmx/copyTasks", JSON.stringify({ "ttids": ids.join(",") }))
					.then(function () {
						EndProgress(prg);
						$scope.applyfilter();
					});
			}
		};
		getUsers($scope, "users", $http, function () {
			$scope.loadData();
			$scope.updateUsersFilter();
		});

		$scope.initFilters(JSON.parse(document.getElementById("filters").value));
		$scope.selectedFilter = "0";
		$scope.effort = "";
		$scope.apply = {};
		$scope.apply.disposition = { "use": false, "value": -1 };
		$scope.apply.component = { "use": false, "value": -1 };
		$scope.apply.severity = { "use": false, "value": -1 };
		$scope.apply.user = { "use": false, "value": -1 };
		$scope.apply.estim = { "use": false, "value": 8 };
		$scope.apply.priority = { "use": false, "value": -1 };
		$scope.apply.date = { "use": false, "value": "" };
		$scope.apply.summary = { "use": false, "value1": "", "value2": "" };

		$scope.checkall = function () {
			if ($scope.defects.length < 1) {
				return;
			}
			var check = !$scope.defects[0].checked;
			$scope.defects.forEach(function (d) {
				d.checked = check;
			});
			if ($scope.defectsselected !== check) {
				$scope.defectsselected = check;
			}
		};
		$scope.applyfilter = function () {
			var proccessed = $scope.getServiceFilter();
			localStorage.DefectsFilter = JSON.stringify(proccessed);
			var o = Object.assign({}, proccessed);
			window.history.pushState(o, "filter:" + localStorage.DefectsFilter, replaceUrlParam(location.href, "filter", localStorage.DefectsFilter));
			$scope.loadData();
		};
		$scope.saveFilter = function () {
			var name = prompt("Please enter filter name", "New Filter");
			if (name === null) {
				return;
			}
			var prg = StartProgress("Storing filter...");
			$http.post("trservice.asmx/saveFilter", JSON.stringify({ "name": name, "filter": $scope.getServiceFilter() }))
				.then(function (response) {
					$scope.filters.push(response.data.d);
					$scope.selectedFilter = "" + response.data.d.ID;
					EndProgress(prg);
				});
		};
		$scope.deleteFilter = function () {
			if ($scope.selectedFilter === "0") {
				return;
			}
			var flt = $scope.filters.filter(function (f) { return f.ID == $scope.selectedFilter; })[0];
			var name = flt.NAME;
			var id = flt.ID;
			var r = confirm("Are you sure you want to delete currently selected filter: " + name + " ?");
			if (r === true) {
				var prg = StartProgress("Deleting filter...");
				$http.post("trservice.asmx/deleteFilter", JSON.stringify({ "id": id }))
					.then(function () {
						$scope.filters = $scope.filters.filter(function (f) { return f.ID !== id; });
						$scope.selectedFilter = "0";
						EndProgress(prg);
					});
			}
		};
		$scope.resetFilter = function () {
			$scope.DefectsFilter = {};
			$scope.selectedFilter = "0";
			createTasksFilter($scope.DefectsFilter);
			$scope.applyfilter();
		};
		$scope.applySelectedFilter = function () {
			if ($scope.selectedFilter !== "0") {
				$http.post("trservice.asmx/savedFilterData", JSON.stringify({ "id": $scope.selectedFilter }))
					.then(function (response) {
						$scope.DefectsFilter = response.data.d;
						createTasksFilter($scope.DefectsFilter);
						$scope.applyfilter();
					});
			}
		};
		$scope.discardfilter = function () {
			window.location.reload();
		};
		$scope.referenceFiltered = function (id, refname) {
			return $scope.DefectsFilter[refname].findIndex(function (x) { return x == id; }) > -1;
		};
		$scope.classFiltered = function (refname1, refname2) {
			var arr = [refname1, refname2];
			for (var i = 0; i < 2; i++) {
				if (!arr[i]) {
					continue;
				}
				var o = $scope.DefectsFilter[arr[i]];
				if (Array.isArray(o)) {
					if (o.length > 0) {
						return "filteredCol";
					}
				} else {
					if (o !== "") {
						return "filteredCol";
					}
				}
			}
			return "";
		};
		$scope.changeDefects = function () {
			var updated = [];
			$scope.defects.forEach(function (d) {
				if (d.checked) {
					var copy = Object.assign({}, d);
					if ($scope.apply.disposition.use && $scope.apply.disposition.value > 0) {
						copy.DISPO = $scope.apply.disposition.value;
					}
					if ($scope.apply.component.use && $scope.apply.component.value > 0) {
						copy.COMP = $scope.apply.component.value;
					}
					if ($scope.apply.severity.use && $scope.apply.severity.value > 0) {
						copy.SEVE = $scope.apply.severity.value;
					}
					if ($scope.apply.user.use && $scope.apply.user.value > 0) {
						copy.AUSER = $scope.apply.user.value;
					}
					if ($scope.apply.estim.use && $scope.apply.estim.value > 0) {
						copy.ESTIM = $scope.apply.estim.value;
					}
					if ($scope.apply.priority.use && $scope.apply.priority.value > 0) {
						copy.PRIO = $scope.apply.priority.value;
					}
					if ($scope.apply.date.use && $scope.apply.date.value !== "") {
						copy.DATE = DateToString($scope.apply.date.value);
					}
					if ($scope.apply.summary.use) {
						if ($scope.apply.summary.value1 !== "") {
							var re = new RegExp($scope.apply.summary.value1, "gi");
							copy.SUMMARY = copy.SUMMARY.replace(re, $scope.apply.summary.value2);
						} else if ($scope.apply.summary.value2 !== "") {
							copy.SUMMARY = $scope.apply.summary.value2;
						}
					}
					delete copy["checked"];
					updated.push(copy);
				}
			});
			if (confirm("Are you sure you want to change " + updated.length + " defects ?")) {
				var updatingprg = StartProgress("Updating tasks...");
				$http.post("trservice.asmx/settaskBase", JSON.stringify({ "defects": updated }))
					.then(function (response) {
						$scope.loadData();
						EndProgress(updatingprg);
					});
			} else {
				// Do nothing!
			}
			$('#batchchanges').modal('hide');
		};
		$scope.resetReferenceFilter = function (refname, obj) {
			$(obj.target).parent().parent().parent().parent().find("input").prop("checked", false);
			$scope.changed = true;
			if (typeof $scope.DefectsFilter[refname] === "string") {
				$scope.DefectsFilter[refname] = "";
			}
			else {
				$scope.DefectsFilter[refname] = [];
			}
			$scope.updateFilterObjects();
		};
		$scope.ChangeDate = function (dateparam) {
			$scope.changed = true;
			if ($scope.DefectsFilter[dateparam] === "") {
				$scope.DefectsFilter[dateparam] = new Date();
			} else {
				$scope.DefectsFilter[dateparam] = "";
			}
		};
		$scope.ChangeNum = function (numparam) {
			$scope.changed = true;
			if ($scope.DefectsFilter[numparam] === "") {
				$scope.DefectsFilter[numparam] = 8;
			} else {
				$scope.DefectsFilter[numparam] = "";
			}
		};
		$scope.changeReferenceFilter = function (id, refname) {
			$scope.changed = true;
			var index = $scope.DefectsFilter[refname].findIndex(function (x) { return x == id; });
			if (index > -1) {
				$scope.DefectsFilter[refname].splice(index, 1);
			} else {
				$scope.DefectsFilter[refname].push(id);
			}
		};
		$scope.$watch("defects", function (newval, oldval) {
			if (newval && oldval && !inProgress()) {
				var checked = false;
				$scope.defects.forEach(function (d) {
					if (d.checked) {
						checked = true;
					}
				});
				$scope.defectsselected = checked;
			}
		}, true);
		$scope.$watch("DefectsFilter", function (newVal, oldVal) {
			if (newVal && oldVal && !inProgress()) {
				$scope.changed = true;
			}
		}, true);

		$.connection.hub.disconnected(function () {
			setTimeout(function () { $.connection.hub.start(); }, 5000); // Restart connection after 5 seconds.
		});
		$scope.notifyHub = $.connection.notifyHub;
		$scope.notifyHub.client.onDefectChanged = function (defectid) {
			for (var i = 0; i < $scope.defects.length; i++) {
				if ($scope.defects[i].ID == defectid) {
					$scope.loadData();
					return;
				}
			}
		};
		$.connection.hub.start();
	}]);
});