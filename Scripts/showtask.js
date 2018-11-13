function deflist() {
	return document.getElementById("deflist").value;
}
$(function () {
	$('a[data-toggle="pill"]').on('shown.bs.tab', function (e) {
		localStorage.taskactivetab = $(e.target).attr("href");
	});
	if (localStorage.taskactivetab) {
		$('[href="' + localStorage.taskactivetab + '"]').tab('show');
		setTimeout(function () { $('[href="' + localStorage.taskactivetab + '"]')[0].click(); }, 1000);
	}

	var ttid = getParameterByName("ttid");
	if (ttid === "") {
		ttid = parseInt(prompt("Please enter TT ID", getParameterByName("ttid")));
		if (isNaN(ttid)) {
			window.location.href = GetSitePath();
		}
	}

	var app = angular.module('mpsapplication', []);
	app.filter('getUserById', getUserById);
	app.filter('getUserTRIDById', getUserTRIDById);
	app.filter('getDispoColorById', getDispoColorById);

	app.controller('mpscontroller', ["$scope", "$http", "$interval", "$window", function ($scope, $http, $interval, $window) {
		$window.onbeforeunload = function () {
			$scope.notifyHub.server.unLockTask(ttid, $scope.currentlock, userID());
		};
		$scope.cliplabl = function () {
			var $temp = $("<input>");
			$("body").append($temp);
			$temp.val("TT" + $scope.defect.ID + " " + $scope.defect.SUMMARY).select();
			document.execCommand("copy");
			$temp.remove();
		};
		$scope.loadAttachments = function () {
			var prgattach = StartProgress("Loading attachments...");
			$http.post("trservice.asmx/getattachsbytask", JSON.stringify({ "ttid": ttid }))
				.then(function (result) {
					$scope.attachs = result.data.d;
					EndProgress(prgattach);
				});
		};
		$scope.getfileext = function (filename) {
			return (/(?:\.([^.]+))?$/).exec(filename)[1];
		};
		$scope.updatePercent = function () {
			upadteBuildProgress($scope.builds, $scope.buildtime);
		};
		$scope.loadBuilds = function () {
			$interval(function () {
				$scope.updatePercent();
			}, 5000);

			if (!Array.isArray($scope.builds)) {
				$scope.builds = [];
			}
			if (!Array.isArray($scope.commits)) {
				$scope.commits = [];
			}
			$http.post("trservice.asmx/getbuildsbytask", JSON.stringify({ "ttid": ttid }))
				.then(function (result) {
					if (JSON.stringify($scope.builds) !== JSON.stringify(result.data.d)) {
						$scope.builds = result.data.d;
						$scope.updatePercent();
					}
				});
			$http.post("trservice.asmx/EnumCommits", JSON.stringify({ "branch": $scope.defect.BRANCH, from: 1, to: 20 }))
				.then(function (result) {
					if (JSON.stringify($scope.commits) !== JSON.stringify(result.data.d)) {
						$scope.commits = result.data.d;
					}
				});
		};
		$scope.getDispoColor = function () {
			if ($scope.defect && $scope.dispos) {
				return "background-color:" + $scope.dispos.filter(function (x) { return x.ID == $scope.defect.DISPO; })[0].COLOR;
			}
			return "";
		};
		$scope.sendEmail = function () {
			var emailpr = StartProgress("Sending email...");
			$http.post("trservice.asmx/alarmEmail", JSON.stringify({ "ttid": ttid, "addresses": $scope.addresses }))
				.then(function (result) {
					EndProgress(emailpr);
					alert(result.data.d);
				});
		};
		$scope.deleteAttach = function (id) {
			var index = $scope.attachs.findIndex(function (x) { return x.ID == id; });
			if (index > -1) {
				$scope.attachs[index].deleted = true;
				$scope.changed = true;
			}
		};
		$scope.specsStyle = function () {
			var active = $("ul#tasktabs li.active")[0].innerText.trim() === $scope.tab_specs;
			return !active && $scope.defect !== undefined && $scope.defect.SPECS.trim().length > 0 ? 'blink_me' : '';
		};
		$interval(function () {
			$scope.locktask();
		}, 5000);
		$scope.canBuild = function () {
			return ($scope.defect && $scope.defect.BRANCH.toUpperCase() === 'RELEASE') || ($scope.commits && $scope.commits.length > 0);
		};
		$scope.testTask = function () {
			for (var i = 0; i < $scope.builds.length; i++) {
				if ($scope.builds[i].STATUS.indexOf("wait") > -1) {
					alert("Already waiting for build!");
					return;
				}
			}
			var deftext = $scope.builds.length > 0 ? $scope.builds[$scope.builds.length - 1].NOTES : "";
			var comments = prompt("Please enter comments:", deftext);
			if (comments === null) {
				return;
			}
			$http.post("trservice.asmx/addBuildByTask", JSON.stringify({ "ttid": ttid, "notes": comments }))
				.then(function (result) {
					$scope.loadBuilds();
				});
		};
		$scope.abortTest = function () {
			for (var i = 0; i < $scope.builds.length; i++) {
				if ($scope.builds[i].STATUS.indexOf("wait") > -1 || $scope.builds[i].STATUS.indexOf("progress") > -1) {
					$http.post("trservice.asmx/cancelBuildByTask", JSON.stringify({ "ttid": ttid }))
						.then(function () {
							$scope.loadBuilds();
						});
					return;
				}
			}
			alert("Threre are no waiting for build requests!");
		};
		$scope.deleteBranch = function () {
			if (confirm("Are you sure you want to delete branch related to this task? The operation cannot be undone.")) {
				var delprg = StartProgress("Deleting branch...");
				$scope.commits = null;
				$http.post("trservice.asmx/deleteBranch", JSON.stringify({ "branch": "TT" + ttid }))
					.then(function () {
						EndProgress(delprg);
						$scope.loadBuilds();
					});
			}
		};
		$scope.addFile = function () {
			var file = $('<input type="file" name="filefor" style="display: none;" />');
			file.on('input', function (e) {
				var att = { "ID": -1, "FILENAME": this.files[0].name, "ARCHIVE": "", "DATE": "", "SIZE": this.files[0].size, "newfile": this.files[0] };
				$scope.attachs.push(att);
				$scope.changed = true;
				$scope.$apply();
			});
			file.trigger('click');
		};
		$scope.discardDefect = function () {
			window.location.reload();
		}
		$scope.canChangeDefect = function () {
			return ($scope.defect != null) && (!inProgress()) && ($scope.currentlock == $scope.globallock);
		};
		$scope.saveDefect = function () {
			//updating object to convert date
			var copy = Object.assign({}, $scope.defect);
			copy.DATE = DateToString(copy.DATE);
			if (!copy.ORDER) {
				copy.ORDER = -1;
			}

			var prgsaving = StartProgress("Saving task...");

			$scope.attachsinprogress = 0;
			if ($scope.attachs) {
				for (var a = 0; a < $scope.attachs.length; a++) {
					if ($scope.attachs[a].deleted) {
						$http.post("trservice.asmx/delfileupload", angular.toJson({ "ttid": $scope.defect.ID, "id": $scope.attachs[a].ID })).then(function () {
							$scope.attachsinprogress--;
							if ($scope.attachsinprogress == 0) {
								$scope.loadAttachments();
							}
						});
						$scope.attachsinprogress++;
						$scope.attachs.splice(a, 1);
						a--;
					}
					else if ($scope.attachs[a].newfile) {
						var r = new FileReader();
						r.attfilename = $scope.attachs[a].newfile.name;
						r.onloadend = function (e) {
							var data = e.target.result;
							var fileupload = StartProgress("Uploading file " + this.attfilename + "...");
							$http.post("trservice.asmx/newfileupload", angular.toJson({ "ttid": ttid, "filename": this.attfilename, "data": data }))
								.then(function (response) {
									EndProgress(fileupload);
									$scope.attachsinprogress--;
									if ($scope.attachsinprogress == 0) {
										$scope.loadAttachments();
									}
								});
							$scope.attachsinprogress++;
						};
						r.readAsDataURL($scope.attachs[a].newfile);
					}
				}
			}
			$http.post("trservice.asmx/settask", angular.toJson({ "d": copy }))
				.then(function (response) {
					EndProgress(prgsaving);
					$scope.changed = false;
					$scope.history = null;
					$scope.events = null;
					$scope.loadHistory();
					$scope.loadEvents();
				});
		};
		$scope.changetab = function (event) {
			var tab = event.target.innerText;
			if (tab === $scope.tab_builds) {
				$scope.loadBuilds();
			} else if (tab === $scope.tab_attachs) {
				if (!$scope.attachs) {
					$scope.loadAttachments();
				}
			} else if (tab === $scope.tab_history) {
				if (!$scope.history) {
					$scope.loadHistory();
				}
			} else if (tab === $scope.tab_bst) {
				if (!$scope.batches) {
					$scope.loadBatches();
				}
				if (!$scope.builds) {
					$scope.loadBuilds();
				}
			} else if (tab === $scope.tab_workflow) {
				if (!$scope.events) {
					$scope.loadEvents();
				}
			}
		};
		$scope.add2Bst = function (batch) {
			if (!$scope.canChangeDefect()) {
				return;
			}
			var pre = $scope.defect.BST === "" ? "" : "\n";
			$scope.defect.BST += pre + batch;
		};
		//references secion:
		getMPSUsers($scope, "mpsusers", $http);
		getUsers($scope, "users", $http);
		getDispos($scope, "dispos", $http);
		getTypes($scope, "types", $http);
		getPriorities($scope, "priorities", $http);
		getSevers($scope, "severs", $http);
		getProducts($scope, "products", $http);
		getComps($scope, "comps", $http);

		$scope.getMPSUserName = function (id) {
			var users = $scope["mpsusers"];
			if (!users) {
				return "ghost";
			}
			for (var i = 0; i < users.length; i++) {
				if (users[i].ID == id) {
					return users[i].PERSON_NAME;
				}
			}
			return "ghost";
		}
		//data section
		var taskprg = StartProgress("Loading task...");
		$http.post("trservice.asmx/gettask", JSON.stringify({ "ttid": ttid }))
			.then(function (response) {
				$scope.defect = response.data.d;
				if ($scope.defect) {
					$scope.defect.DATE = StringToDate($scope.defect.DATE);
					$scope.defect.CREATEDBY = "" + $scope.defect.CREATEDBY;
					if ($scope.defect.ORDER == -1) {
						$scope.defect.ORDER = null;
					}
				}
				document.title = "Task: #" + ttid;
				EndProgress(taskprg);
			});

		$scope.loadHistory = function () {
			$http.post("trservice.asmx/gettaskhistory", JSON.stringify({ "ttid": ttid }))
				.then(function (result) {
					$scope.history = result.data.d;
				});
		};
		$scope.loadBatches = function () {
			$http.post("trservice.asmx/getBSTBatches", JSON.stringify({ }))
				.then(function (result) {
					$scope.batches = result.data.d;
					var slotcap = 14;
					$scope.batchesslots = [];
					for (var i = 0; i < $scope.batches.length; i++) {
						if ($scope.batchesslots.length === 0) {
							$scope.batchesslots.push([]);
						}
						if ($scope.batchesslots[$scope.batchesslots.length - 1].length === slotcap) {
							$scope.batchesslots.push([]);
						}
						$scope.batchesslots[$scope.batchesslots.length - 1].push($scope.batches[i]);
					}
				});
		};
		$scope.loadEvents = function () {
			$http.post("trservice.asmx/gettaskevents", JSON.stringify({ "ttid": ttid }))
				.then(function (result) {
					$scope.events = result.data.d;
				});
		};
		$scope.$watchCollection('defect', function (newval, oldval) {
			if (newval && oldval) {
				$scope.changed = true;
			}
		});
		$scope.releaseRequest = function () {
			$scope.notifyHub.server.sendMessage(userID(), $scope.lockedby, "Please release TT" + $scope.defect.ID + "!!!");
		};
		$scope.locktask = function () {
			$scope.notifyHub.server.lockTask(ttid, $scope.currentlock, userID());
		};

		//start
		$scope.buildtime = parseInt(document.getElementById("buildtime").value);
		$scope.tab_builds = "Builds";
		$scope.tab_attachs = "Attachments";
		$scope.tab_history = "History";
		$scope.tab_bst = "BST";
		$scope.tab_workflow = "Workflow";
		$scope.tab_specs = "Specification";
		$scope.currentlock = guid();
		$scope.globallock = "";
		$scope.batches = null;
		$scope.batchesslots = [];
		$scope.changed = false;
		$.connection.hub.start().done(function () {
			$scope.locktask();
		});
		$.connection.hub.disconnected(function () {
			setTimeout(function () { $.connection.hub.start(); }, 5000); // Restart connection after 5 seconds.
		});
		$scope.notifyHub = $.connection.notifyHub;
		$scope.notifyHub.client.onLockTask = function (li) {
			$scope.globallock = li.globallock;
			$scope.lockedby = li.lockedby;
			$scope.$apply();
		};
		$scope.notifyHub.client.OnBuildChanged = function () {
			$scope.loadBuilds();
			$scope.$apply();
		};
		
		$scope.addresses = deflist();
	}]);
});