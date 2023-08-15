﻿var g_filrid = "filterid";
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
			$scope.filters = [{ ID: 0, NAME: " - Select - " }];
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
			let proccessed = $scope.getServiceFilter();
			localStorage.DefectsFilter = JSON.stringify(proccessed);
			let o = Object.assign({}, proccessed);
			let url = removeUrlParam(replaceUrlParam(location.href, "filter", localStorage.DefectsFilter), g_filrid);
			window.history.pushState(o, "filter:" + localStorage.DefectsFilter, url);
			$scope.loadData();
		};
		$scope.updateFilter = function (filter) {
			if (!confirm("Update existing '" + filter.NAME + "' filter? ")) {
				return;
			}
			$http.post("trservice.asmx/udpateFilter", JSON.stringify({ "id": filter.ID, "filter": $scope.getServiceFilter() }))
				.then(function (response) {
					let f = response.data.d;
					let i = $scope.filters.findIndex(function (item) { return item.ID === f.ID; });
					if (i >= 0) {
						$scope.filters[i] = response.data.d;
					}
					EndProgress(prg);
				});
		};
		$scope.saveFilter = function (personal) {
			msgBox("Please enter filter name", "New Filter", function (txt) {
				let filter = $scope.filters.find(function (item) { return item.NAME === txt });
				if (filter != null) {
					$scope.updateFilter(filter);
					return;
				}
				let prg = StartProgress("Storing filter...");
				$http.post("trservice.asmx/saveFilter", JSON.stringify({ "name": txt, "personal": personal, "filter": $scope.getServiceFilter() }))
					.then(function (response) {
						$scope.filters.push(response.data.d);
						$scope.selectedFilter = response.data.d;
						EndProgress(prg);
					});
			});
		};
		$scope.deleteFilter = function () {
			if ($scope.selectedFilter === null) {
				return;
			}
			var r = confirm("Are you sure you want to delete currently selected filter: " + $scope.selectedFilter.NAME + " ?");
			if (r === true) {
				var prg = StartProgress("Deleting filter...");
				$http.post("trservice.asmx/deleteFilter", JSON.stringify({ "id": $scope.selectedFilter.ID }))
					.then(function () {
						$scope.filters = $scope.filters.filter(function (f) { return f.ID !== $scope.selectedFilter.ID; });
						$scope.selectedFilter = null;
						EndProgress(prg);
					});
			}
		};
		$scope.resetFilter = function () {
			$scope.DefectsFilter = {};
			$scope.selectedFilter = null;
			createTasksFilter($scope.DefectsFilter);
			$scope.applyfilter();
		};
		$scope.filterCopyurl = function () {
			copyurl(window.location.href.split('?')[0] + '?' + g_filrid + '=' + $scope.selectedFilter.ID);
		};
		$scope.applySelectedFilter = function (id) {
			if (id === 0) {
				$scope.selectedFilter = null;
				return;
			}
			$scope.selectedFilter = $scope.filters.find(function (f) { return f.ID === id; });
			if ($scope.selectedFilter) {
				$http.post("trservice.asmx/savedFilterData", JSON.stringify({ "id": $scope.selectedFilter.ID }))
					.then(function (response) {
						$scope.DefectsFilter = response.data.d;
						createTasksFilter($scope.DefectsFilter);
						$scope.applyfilter();
					});
			} else {
				$scope.selectedFilter = null;//not undefined
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
				if (typeof $scope.DefectsFilter === "undefined") {
					return "";
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
							let re = new RegExp($scope.apply.summary.value1, "gi");
							copy.SUMMARY = copy.SUMMARY.replace(re, $scope.apply.summary.value2);
						} else if ($scope.apply.summary.value2 !== "") {
							copy.SUMMARY = $scope.apply.summary.value2;
						}
					}
					if ($scope.apply.details.use) {
						if ($scope.apply.details.value) {
							copy.add_details = $scope.apply.details.value;
						}
					}
					delete copy["checked"];
					updated.push(copy);
				}
			});
			if (confirm("Are you sure you want to change " + updated.length + " defects ?")) {
				StartProgress("Updating tasks...");
				$http.post("trservice.asmx/settaskBase", JSON.stringify({ "defects": updated })).then(function () {
					window.location.reload();
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

		$scope.initFilters(JSON.parse(document.getElementById("filters").value));
		$scope.selectedFilter = null;
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
		$scope.apply.details = { "use": false, "value": "" };
		//loading:
		getUsers($scope, "users", $http, function () {
			$scope.updateUsersFilter();
			var fid = getParameterByName(g_filrid);
			if (fid) {
				$scope.applySelectedFilter(parseInt(fid));
			} else {
				$scope.loadData();
			}
		});

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