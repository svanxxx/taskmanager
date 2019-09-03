$(function () {
	$('ul#tasktabs li a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
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
	app.filter('rawHtml', ['$sce', rawHtml]);

	app.controller('mpscontroller', ["$scope", "$http", "$interval", "$window", function ($scope, $http, $interval, $window) {
		$window.onbeforeunload = function () {
			$scope.notifyHub.server.unLockTask(ttid, $scope.currentlock);
		};
		$scope.cliplabl = function () {
			copyurl("TT" + $scope.defect.ID + " " + $scope.defect.SUMMARY);
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
		$scope.loadMasterCommits = function () {
			$scope.mastercommits = [];
			$http.post("GitService.asmx/QueryCommits", JSON.stringify({ "pattern": "TT" + ttid }))
				.then(function (result) {
					if (JSON.stringify($scope.mastercommits) !== JSON.stringify(result.data.d)) {
						$scope.mastercommits = result.data.d;
						reActivateTooltips();
					}
				});
		};
		$scope.loadCommits = function () {
			$scope.commits = [];
			$scope.gitbranchhash = "";
			$http.post("GitService.asmx/BranchHash", JSON.stringify({ "branch": $scope.defect.BRANCH }))
				.then(function (result) {
					$scope.gitbranchhash = result.data.d;
				});

			$http.post("GitService.asmx/EnumCommits", JSON.stringify({ "branch": $scope.defect.BRANCH, from: $scope.commitsstate.showby * ($scope.commitsstate.page - 1) + 1, to: $scope.commitsstate.showby * $scope.commitsstate.page }))
				.then(function (result) {
					if (JSON.stringify($scope.commits) !== JSON.stringify(result.data.d)) {
						$scope.commits = result.data.d;
						reActivateTooltips();
					}
				});
		};
		$scope.loadBuilds = function () {
			$interval(function () {
				$scope.updatePercent();
			}, 5000);
			if (!Array.isArray($scope.builds)) {
				$scope.builds = [];
			}
			$http.post("BuildService.asmx/getBuildsByTask", JSON.stringify({ from: $scope.buildsstate.showby * ($scope.buildsstate.page - 1) + 1, to: $scope.buildsstate.showby * $scope.buildsstate.page, "ttid": ttid }))
				.then(function (result) {
					if (JSON.stringify($scope.builds) !== JSON.stringify(result.data.d)) {
						$scope.builds = result.data.d;
						$scope.updatePercent();
						reActivateTooltips();
					}
				});
		};
		$scope.getDispoColor = function () {
			if ($scope.defect && $scope.dispos) {
				return "background-color:" + $scope.dispos.filter(function (x) { return x.ID == $scope.defect.DISPO; })[0].COLOR;
			}
			return "";
		};
		$scope.chgOrder = function (inc) {
			if (typeof (inc) === "undefined") {
				$scope.defect.ORDER = undefined;
			} else if (inc) {
				if (!$scope.defect.ORDER) {
					$scope.defect.ORDER = 1;
				} else {
					$scope.defect.ORDER++;
				}
			} else {
				if (!$scope.defect.ORDER) {
					$scope.defect.ORDER = 1;
				} else {
					if ($scope.defect.ORDER > 1) {
						$scope.defect.ORDER--;
					}
				}
			}
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
				if (id > 0) {
					$scope.attachs[index].deleted = true;
					$scope.changed = true;
				} else {
					$scope.attachs.splice(index, 1);
					$scope.changed = true;
				}
			}
		};
		$scope.specsStyle = function () {
			var active = $("ul#tasktabs li a.active")[0].innerText.trim() === $scope.tab_specs;
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
			var deftext = $scope.builds.length > 0 ? $scope.builds[0].NOTES : "";
			var comments = prompt("Please enter comments:", deftext);
			if (comments === null) {
				return;
			}
			$http.post("trservice.asmx/addBuildByTask", JSON.stringify({ "ttid": ttid, "notes": comments }))
				.then(function (result) {
					$scope.loadBuilds();
				});
		};
		$scope.loadCommit = function (c, member) { loadCommit(c, $scope, $http, member); };
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
				$http.post("GitService.asmx/deleteBranch", JSON.stringify({ "branch": $scope.defect.BRANCH }))
					.then(function () {
						EndProgress(delprg);
						$scope.loadCommits();
					});
			}
		};
		$scope.gotoAlarm = function () {
			$('[href="#alarm"]')[0].click();
		};
		$scope.addFile = function () {
			var file = $('<input type="file" name="filefor" style="display: none;" />');
			file.on('input', function (e) {
				var att = { "ID": -(new Date()).getTime(), "FILENAME": this.files[0].name, "ARCHIVE": "", "DATE": "", "SIZE": this.files[0].size, "newfile": this.files[0] };
				$scope.attachs.push(att);
				$scope.changed = true;
				$scope.$apply();
			});
			file.trigger('click');
		};
		document.onpaste = function (event) {
			if (!$scope.canChangeDefect()) return;
			var items = (event.clipboardData || event.originalEvent.clipboardData).items;
			for (index in items) {
				var item = items[index];
				if (item.kind === 'file') {
					var blob = item.getAsFile();
					var reader = new FileReader();
					reader.onload = function (event) {
						var postfix = -1;
						var prefix = "TT" + ttid + "_attach#";
						$scope.attachs.forEach(function (a) {
							var fname = a.FILENAME;
							if (fname.endsWith(".png")) { fname = fname.substring(0, fname.length - 4); }
							if (fname.startsWith(prefix)) {
								var num = parseInt(fname.replace(prefix, ""));
								if (!isNaN(num)) {
									postfix = Math.max(num, postfix);
								}
							}
						});
						postfix = postfix < 0 ? 1 : postfix + 1;
						var att = { "ID": -(new Date()).getTime(), "FILENAME": prefix + postfix + ".png", "ARCHIVE": "", "DATE": "", "SIZE": event.target.result.length, "newblob": event.target.result };
						$scope.attachs.push(att);
						$scope.changed = true;
						$scope.$apply();
					};
					reader.readAsDataURL(blob);
				}
			}
		};
		$scope.discardDefect = function () {
			window.location.reload();
		};
		$scope.canChangeDefect = function () {
			return $scope.defect != null && !inProgress() && $scope.currentlock === $scope.globallock;
		};
		$scope.saving = false;
		$scope.saveDefect = function () {
			//updating object to convert date
			$scope.saving = true;
			$scope.loadTasksources(true);
			var alarmfire = !$scope.defect.FIRE;
			$scope.defect.FIRE = $scope.defect.TIMER != null && $scope.today.getTime() <= $scope.defect.TIMER.getTime();
			alarmfire = alarmfire && $scope.defect.FIRE;
			var copy = Object.assign({}, $scope.defect);
			copy.DATE = DateToString(copy.DATE);
			copy.TIMER = DateToString(copy.TIMER);
			if (!copy.ORDER || copy.ORDER < 1) {
				copy.ORDER = -1;
			}

			var prgsaving = StartProgress("Saving task...");

			$scope.attachsinprogress = 0;
			if ($scope.attachs) {
				$scope.attachs.forEach(function (a) {
					if (a.deleted) {
						$http.post("trservice.asmx/delfileupload", angular.toJson({ "ttid": $scope.defect.ID, "id": a.ID })).then(function () {
							$scope.attachsinprogress--;
							if ($scope.attachsinprogress == 0) {
								$scope.loadAttachments();
							}
						});
						$scope.attachsinprogress++;
					}
					else if (a.newfile) {
						var r = new FileReader();
						r.attfilename = a.newfile.name;
						r.onloadend = function (e) {
							var data = e.target.result;
							var fileupload = StartProgress("Uploading file " + this.attfilename + "...");
							$http.post("trservice.asmx/newfileupload", angular.toJson({ "ttid": ttid, "filename": this.attfilename, "data": data }))
								.then(function () {
									EndProgress(fileupload);
									$scope.attachsinprogress--;
									if ($scope.attachsinprogress == 0) {
										$scope.loadAttachments();
									}
								});
							$scope.attachsinprogress++;
						};
						r.readAsDataURL(a.newfile);
					}
					else if (a.newblob) {
						var fileupload = StartProgress("Uploading file " + a.FILENAME + "...");
						$http.post("trservice.asmx/newfileupload", angular.toJson({ "ttid": ttid, "filename": a.FILENAME, "data": a.newblob }))
							.then(function () {
								EndProgress(fileupload);
								$scope.attachsinprogress--;
								if ($scope.attachsinprogress == 0) {
									$scope.loadAttachments();
								}
							});
						$scope.attachsinprogress++;
					};
				});
			}
			$scope.defect.REQUESTRESET = false;

			$http.post("DefectService.asmx/settask", angular.toJson({ "d": copy }))
				.then(function () {
					EndProgress(prgsaving);
					$scope.changed = false;
					$scope.history = null;
					$scope.events = null;
					$scope.loadHistory();
					$scope.loadEvents();
					$scope.saving = false;
					if ($scope.commented && $scope.commented_alarmuser) {
						$http.post("trservice.asmx/NotifyDefect", angular.toJson({ "ttid": ttid, "message": $scope.commented_txt, "img": $scope.commented_img, "alsoteam": $scope.commented_alarmgroup }));
					}
					$scope.commented = false;
					if (alarmfire) {
						$scope.invite($scope.defect.AUSER);
					}
				});
		};
		$scope.changetab = function (event) {
			var tab = event.target.tagName === "A" ? event.target.innerText : event.target.parentElement.innerText;
			if (tab === $scope.tab_builds) {
				if (!$scope.commits) {
					$scope.loadCommits();
				}
				if (!$scope.builds) {
					$scope.loadBuilds();
				}
			} else if (tab === $scope.tab_attachs) {
				if (!$scope.attachs) {
					$scope.loadAttachments();
				}
			} else if (tab === $scope.tab_history) {
				if (!$scope.history) {
					$scope.loadHistory();
				}
			} else if (tab === $scope.tab_git) {
				if (!$scope.commits) {
					$scope.loadCommits();
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
			if (document.querySelector("#bsttabs li a.active").id === $scope.bsttab_bat) {
				var preb = $scope.defect.BSTBATCHES === "" ? "" : "\n";
				$scope.defect.BSTBATCHES += preb + batch;
			} else if (document.querySelector("#bsttabs li a.active").id === $scope.bsttab_com) {
				var bcom = StartProgress("Loading commands...");
				$http.post("trservice.asmx/getBSTBatchData", JSON.stringify({ batch: batch }))
					.then(function (response) {
						EndProgress(bcom);
						for (var i = 0; i < response.data.d.length; i++) {
							var prec = $scope.defect.BSTCOMMANDS === "" ? "" : "\n";
							$scope.defect.BSTCOMMANDS += prec + response.data.d[i];
						}
					});
			} else {
				alert("Please select correct tab");
			}
		};
		$scope.showTests = function (guid) {
			var pr = StartProgress("Redirecting...");
			$http.post("trservice.asmx/getTestID", JSON.stringify({ requestGUID: guid }))
				.then(function (response) {
					window.open($scope.testlink + response.data.d);
					EndProgress(pr);
				});
		};
		$scope.invite = function (usr) {
			$scope.notifyHub.server.sendMessage(userID(), getUserTRIDById()(usr, $scope), location.href + "&tstamp=" + (new Date()).getTime());
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
		};
		//data section
		$scope.loadData = function () {
			var taskprg = StartProgress("Loading task...");
			$http.post("trservice.asmx/gettask", JSON.stringify({ "ttid": ttid }))
				.then(function (response) {
					$scope.defect = response.data.d;
					if ($scope.defect) {
						var vals = $scope.defect.SUMMARY.split("@");
						$scope.defectsumm = "";
						if (vals.length > 0) {
							$scope.defectsumm = vals[0];
						}
						$scope.defecteml = "";
						if (vals.length > 1) {
							$scope.defecteml = vals[1];
						}
						$scope.defect.TIMER = StringToDate($scope.defect.TIMER);
						$scope.defect.DATE = StringToDate($scope.defect.DATE);
						$scope.defect.CREATEDBY = "" + $scope.defect.CREATEDBY;
						$scope.defect.ESTIMBY = "" + $scope.defect.ESTIMBY;
						if ($scope.defect.ORDER == -1) {
							$scope.defect.ORDER = null;
						}
					}
					document.title = "Task: #" + ttid;
					document.getElementById("firealarm").style.backgroundImage = "url('images/fire.gif')";
					EndProgress(taskprg);
				});
		};

		$scope.reloading = false;
		$scope.loadData();

		$scope.generateSlots = function () {
			var slotcap = 17;
			$scope.batchesslots = [];
			var arr = [];
			var bts = $scope.batches;
			var src = $scope.batchsearch.toUpperCase();
			for (var i = 0; i < bts.length; i++) {
				if (bts[i].toUpperCase().includes(src)) {
					arr.push(bts[i]);
				}
			}
			for (i = 0; i < arr.length; i++) {
				if ($scope.batchesslots.length === 0) {
					$scope.batchesslots.push([]);
				}
				if ($scope.batchesslots[$scope.batchesslots.length - 1].length === slotcap) {
					$scope.batchesslots.push([]);
				}
				$scope.batchesslots[$scope.batchesslots.length - 1].push(arr[i]);
			}
		};
		$scope.loadHistory = function () {
			$http.post("trservice.asmx/gettaskhistory", JSON.stringify({ "ttid": ttid }))
				.then(function (result) {
					$scope.history = result.data.d;
				});
		};
		$scope.loadBatches = function () {
			$http.post("trservice.asmx/getBSTBatches", JSON.stringify({}))
				.then(function (result) {
					$scope.batches = result.data.d;
					$scope.generateSlots();
				});
		};
		$scope.loadEvents = function () {
			$http.post("DefectService.asmx/gettaskevents", JSON.stringify({ "ttid": ttid }))
				.then(function (result) {
					$scope.events = result.data.d;
				});
		};
		$scope.$watchCollection('defect', function (newval, oldval) {
			if (newval && oldval) {
				if (!$scope.reloading) {
					if (!newval.ESTIM || newval.ESTIM < 1) {
						$scope.defect.ESTIM = 1;
					}
					if (newval.ORDER != null && newval.ORDER < 1) {
						$scope.defect.ORDER = 1;
					}
					$scope.changed = true;
				} else {
					$scope.reloading = false;
				}
			}
		});
		$scope.$watchCollection('batchsearch', function (newval, oldval) {
			if (newval !== oldval) {
				$scope.generateSlots();
			}
		});
		$scope.releaseRequest = function () {
			$scope.notifyHub.server.sendMessage(userID(), $scope.lockedby, "Please release TT" + $scope.defect.ID + "!!!");
		};
		$scope.releaseForce = function () {
			$scope.notifyHub.server.sendMessage(userID(), $scope.lockedby, "You was disconnected from TT" + $scope.defect.ID + " task!");
			$scope.notifyHub.server.lockTaskForce(ttid, $scope.currentlock, userID());
		};
		$scope.locktask = function () {
			$scope.notifyHub.server.lockTask(ttid, $scope.currentlock, userID());
		};
		$scope.resettask = function () {
			if (!confirm("Are you sure you want to delete all task information and reset all the fields?")) {
				return;
			}
			$scope.defect.SUMMARY = "Free To Use";
			$scope.defect.DESCR = "";
			$scope.defect.AUSER = "";
			$scope.defect.SPECS = "";
			$scope.defect.ESTIM = 0;
			$scope.defect.REFERENCE = "";
			$scope.defect.BSTBATCHES = "";
			$scope.defect.BSTCOMMANDS = "";
			$scope.defect.REQUESTRESET = true;

			$scope.defect.DISPO = "" + $scope.defectDefaults.DISP;
			$scope.defect.TYPE = "" + $scope.defectDefaults.TYPE;
			$scope.defect.PRODUCT = "" + $scope.defectDefaults.PRODUCT;
			$scope.defect.PRIO = "" + $scope.defectDefaults.PRIO;
			$scope.defect.COMP = "" + $scope.defectDefaults.COMP;
			$scope.defect.SEVE = "" + $scope.defectDefaults.SEVR;
			$scope.defect.ESTIM = $scope.defectDefaults.ESTIMATED;

			if ($scope.attachs) {
				$scope.attachs.forEach(function (a) {
					$scope.deleteAttach(a.ID);
				});
			}
		};
		$scope.normtext = function () {
			var lines = $scope.defect.DESCR.split("\n");
			for (var i = 0; i < lines.length; i++) {
				lines[i] = lines[i].trim();
				if (i > 0 && lines[i] === "" && lines[i - 1] === "") {
					lines.splice(i, 1);
					i--;
				}
			}
			$scope.defect.DESCR = lines.join("\n");
		};
		$scope.adddesc = function (text, img) {
			msgBox("Add Comment", text, function (txt) {
				$scope.commented = true;
				$scope.commented_txt = txt;
				$scope.commented_img = img;
				var d = new Date();
				$scope.defect.DESCR = "<" + userLogin() + " time='" +
					(d.getMonth() + 1) + '/' + d.getDate() + '/' + d.getFullYear() + " " + d.getHours() + ":" + d.getMinutes()
					+ "'>\n" + txt + "\n</" + userLogin() + ">\n\n" + $scope.defect.DESCR;
				$scope.$apply();
			});
		};
		$scope.duplicate = function () {
			$http.post("trservice.asmx/copyTask", JSON.stringify({ "ttid": ttid }))
				.then(function (response) {
					openTask(response.data.d);
				});
		};
		$scope.updateDefSum = function () {
			$scope.defect.SUMMARY = $scope.defectsumm + "@" + $scope.defecteml;
			$scope.changed = true;
		};
		$scope.updateDefEml = function () {
			$scope.defect.SUMMARY = $scope.defectsumm + "@" + $scope.defecteml;
			$scope.changed = true;
		};
		//start
		$scope.defectDefaults = JSON.parse(document.getElementById("defectdefaults").value);
		$scope.currentlock = guid();
		$scope.buildtime = parseInt(document.getElementById("buildtime").value);
		$scope.testlink = document.getElementById("testlink").value;
		$scope.addresses = document.getElementById("deflist").value;

		$scope.today = new Date();
		$scope.today.setHours(0, 0, 0, 0)
		$scope.defectsumm = "";
		$scope.defecteml = "";
		$scope.bsttab_bat = "bsttabs-batches";
		$scope.bsttab_com = "bsttabs-command";
		$scope.bsttab_his = "bsttabs-history";
		$scope.tab_builds = "Builds";
		$scope.tab_git = "Git";
		$scope.tab_attachs = "Attachs";
		$scope.tab_history = "History";
		$scope.tab_bst = "BST";
		$scope.tab_workflow = "Workflow";
		$scope.tab_specs = "Specs";
		$scope.buildpriorities = [{ ID: 1, DESCR: "1 (Low)" }, { ID: 2, DESCR: "2 (Programmer big release)" }, { ID: 3, DESCR: "3 (Release)" }, { ID: 4, DESCR: "4 (Programmer)" }, { ID: 5, DESCR: "5 (High)" }];
		$scope.globallock = "";
		$scope.batches = null;
		$scope.batchesslots = [];
		$scope.changed = false;
		$scope.isrelease = function () {
			return $scope.defect !== undefined && document.getElementById("releasettid").value == $scope.defect.ID;
		};

		$scope.loadTasksources = function (save) {
			if (!save) {
				$scope.tasksources = [];
				if (localStorage.tasksources) {
					$scope.tasksources = JSON.parse(localStorage.tasksources);
				}
			} else {
				if (!$scope.tasksources.includes($scope.defecteml)) {
					if ($scope.tasksources.push($scope.defecteml) > 20) {
						$scope.tasksources.shift();
					}
				}
				localStorage.tasksources = JSON.stringify($scope.tasksources);
			}
		};
		$scope.loadTasksources(false);

		$scope.batchsearch = "";
		$scope.commented = false;
		$scope.commented_txt = "";
		$scope.commented_img = "";
		$scope.commented_alarmuser = true;
		$scope.commented_alarmgroup = true;
		$scope.teststring = "&#64";

		$scope.buildsstate = {};
		$scope.buildsstate.page = 1;
		$scope.buildsstate.showby = "5";
		$scope.buildsstate.filter = "";
		$scope.buildsstate.showbys = ["5", "10", "15"];

		$scope.commitsstate = {};
		$scope.commitsstate.page = 1;
		$scope.commitsstate.showby = "5";
		$scope.commitsstate.filter = "";
		$scope.commitsstate.showbys = ["5", "10", "15"];

		$scope.mastercommitsstate = {};
		$scope.mastercommitsstate.page = 1;
		$scope.mastercommitsstate.showby = "5";
		$scope.mastercommitsstate.filter = "";
		$scope.mastercommitsstate.showbys = ["5", "10", "15"];

		$scope.loadonepage = function (state) {
			if (state === "commitsstate") {
				$scope.loadCommits();
			} else if (state === "mastercommitsstate") {
				$scope.loadMasterCommits();
			} else {
				$scope.loadBuilds();
			}
		};

		$scope.decPage = function (state) {
			if ($scope[state].page === 1) {
				return;
			}
			$scope[state].page--;
			$scope.loadonepage(state);
		};
		$scope.incPage = function (state) {
			if ($scope[state].length === 0) {
				return;
			}
			$scope[state].page++;
			$scope.loadonepage(state);
		};
		$scope.changeShowBy = function (state) {
			$scope.loadonepage(state);
		};

		$.connection.hub.disconnected(function () {
			setTimeout(function () { $.connection.hub.start(); }, 5000); // Restart connection after 5 seconds.
		});
		$scope.notifyHub = $.connection.notifyHub;
		$scope.notifyHub.client.onDefectChanged = function (defectid) {
			if (!$scope.saving && ttid == defectid) {
				$scope.reloading = true;
				$scope.loadData();
				$scope.$apply();
			}
		};
		$scope.notifyHub.client.onLockTask = function (li) {
			$scope.globallock = li.globallock;
			$scope.lockedby = li.lockedby;
			$scope.$apply();
		};
		$scope.notifyHub.client.OnBuildChanged = function () {
			$scope.loadBuilds();
			$scope.$apply();
		};
		$.connection.hub.start().done(function () {
			$scope.locktask();
		});
	}]);
});