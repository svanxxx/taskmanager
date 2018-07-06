function enterTT() {
	var ttid = prompt("Please enter TT ID", getParameterByName("ttid"));
	if (ttid != null) {
		window.location = replaceUrlParam(location.href, "ttid", ttid);
		return true;
	}
	return false;
}
$(function () {

	$('a[data-toggle="pill"]').on('shown.bs.tab', function (e) {
		localStorage.taskactivetab = $(e.target).attr("href")
	});
	if (localStorage.taskactivetab) {
		$('[href="' + localStorage.taskactivetab + '"]').tab('show');
	}

	var ttid = getParameterByName("ttid");
	if (ttid == "") {
		if (enterTT() == false) {
			window.location.href = GetSitePath();
		}
	}

	$(document).bind('keydown', function (e) {
		if (e.keyCode == 188 && event.ctrlKey) {
			enterTT();
		}
	});

	var app = angular.module('mpsapplication', []);
	app.filter('getUserById', function () {
		return function (id, $scope) {
			for (i = 0; i < $scope.users.length; i++) {
				if ($scope.users[i].ID == id) {
					return $scope.users[i].FIRSTNAME + " " + $scope.users[i].LASTNAME;
				}
			}
			return "";
		};
	});
	app.filter('getUserImgById', function () {
		return function (id, $scope) {
			for (i = 0; i < $scope.users.length; i++) {
				if ($scope.users[i].ID == id) {
					return $scope.getPersonImg($scope.users[i].EMAIL);
				}
			}
			return "";
		};
	});

	app.controller('mpscontroller', ["$scope", "$http", "$interval", "$window", function ($scope, $http, $interval, $window) {

		$window.onbeforeunload = function () {
			$http.post("trservice.asmx/unlocktask", angular.toJson({ "ttid": ttid, "lockid": $scope.currentlock }));
		};
		$scope.loadAttachments = function () {
			var prgattach = StartProgress("Loading attachments..."); $scope.loaders++;
			$scope.attachs = [];
			$http.post("trservice.asmx/getattachsbytask", JSON.stringify({ "ttid": ttid }))
				.then(function (result) {
					$scope.attachs = result.data.d;
					EndProgress(prgattach); $scope.loaders--;
				});
		};

		$scope.currentlock = guid();
		$scope.globallock = "";
		$scope.locktask = function () {
			$http.post("trservice.asmx/locktask", angular.toJson({ "ttid": ttid, "lockid": $scope.currentlock }))
				.then(function (response) {
					$scope.globallock = response.data.d.globallock;
					$scope.lockedby = response.data.d.lockedby;
				});
		};

		$scope.getDispoColor = function () {
			if ($scope.defect && $scope.dispos) {
				var col = $scope.dispos.filter(function (x) { return x.ID == $scope.defect.DISPO; })[0].COLOR;
				return "background-color: " + col;
			}
			return "";
		};

		$scope.locktask();
		$interval(function () {
			$scope.locktask();
		}, 20000);

		$scope.getPersonImg = function (email) {
			if ($scope.lockedby) {
				return "images/personal/" + email + ".jpg";
			}
			return "";
		};

		$scope.deleteAttach = function (id) {
			var index = $scope.attachs.findIndex(function (x) { return x.ID == id; });
			if (index > -1) {
				$scope.attachs[index].deleted = true;
				$scope.changed = true;
			}
		}

		$scope.addFile = function () {
			var file = $('<input type="file" name="filefor" style="display: none;" />');
			file.on('input', function (e) {
				var att = { "ID": -1, "FILENAME": this.files[0].name, "ARCHIVE": "", "DATE": "", "SIZE": this.files[0].size, "newfile": this.files[0] };
				$scope.attachs.push(att);
				$scope.changed = true;
				$scope.$apply();
			});
			file.trigger('click');
		}
		$scope.discardDefect = function () {
			window.location.reload();
		}
		$scope.canChangeDefect = function () {
			return ($scope.defect != null) && ($scope.loaders == 0) && ($scope.currentlock == $scope.globallock);
		}
		$scope.loaders = 0;
		$scope.saveDefect = function () {
			//updating object to convert date
			var copy = Object.assign({}, $scope.defect);
			copy.DATE = DateToString(copy.DATE);
			if (!copy.ORDER) {
				copy.ORDER = -1;
			}

			var prgsaving = StartProgress("Saving task..."); $scope.loaders++;

			$scope.attachsinprogress = 0;
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
					reloadattachments = true;
				}
				else if ($scope.attachs[a].newfile) {
					var r = new FileReader();
					r.attfilename = $scope.attachs[a].newfile.name;
					r.onloadend = function (e) {
						var data = e.target.result;
						var fileupload = StartProgress("Uploading file " + this.attfilename + "..."); $scope.loaders++;
						$http.post("trservice.asmx/newfileupload", angular.toJson({ "ttid": ttid, "filename": this.attfilename, "data": data }))
							.then(function (response) {
								EndProgress(fileupload); $scope.loaders--;
								$scope.attachsinprogress--;
								if ($scope.attachsinprogress == 0) {
									$scope.loadAttachments();
								}
							});
						$scope.attachsinprogress++;
					}
					r.readAsDataURL($scope.attachs[a].newfile);
					reloadattachments = true;
				}
			}

			$http.post("trservice.asmx/settask", angular.toJson({ "d": copy }), )
				.then(function (response) {
					EndProgress(prgsaving); $scope.loaders--;
					$scope.changed = false;
					$http.post("trservice.asmx/gettaskhistory", JSON.stringify({ "ttid": ttid }))
						.then(function (result) {
							$scope.history = result.data.d;
						});
					$http.post("trservice.asmx/gettaskevents", JSON.stringify({ "ttid": ttid }))
						.then(function (result) {
							$scope.events = result.data.d;
						});
				});
		}

		//references secion:
		getUsers($scope, "users", $http);
		getDispos($scope, "dispos", $http);
		getTypes($scope, "types", $http);
		getPriorities($scope, "priorities", $http);
		getSevers($scope, "severs", $http);
		getProducts($scope, "products", $http);
		getComps($scope, "comps", $http);

		//data section
		var taskprg = StartProgress("Loading task..."); $scope.loaders++;
		$http.post("trservice.asmx/gettask", JSON.stringify({ "ttid": ttid }))
			.then(function (response) {
				$scope.defect = response.data.d;
				if ($scope.defect) {
					$scope.defect.DATE = StringToDate($scope.defect.DATE);
					if ($scope.defect.ORDER == -1) {
						$scope.defect.ORDER = null;
					}
				}
				document.title = "Task: #" + ttid;
				EndProgress(taskprg); $scope.loaders--;
			});

		var prghistory = StartProgress("Loading history..."); $scope.loaders++;
		$scope.history = [];
		$http.post("trservice.asmx/gettaskhistory", JSON.stringify({ "ttid": ttid }))
			.then(function (result) {
				$scope.history = result.data.d;
				EndProgress(prghistory); $scope.loaders--;
			});

		var prgevents = StartProgress("Loading events..."); $scope.loaders++;
		$scope.events = [];
		$http.post("trservice.asmx/gettaskevents", JSON.stringify({ "ttid": ttid }))
			.then(function (result) {
				$scope.events = result.data.d;
				EndProgress(prgevents); $scope.loaders--;
			});

		$scope.loadAttachments();

		$scope.changed = false;
		$scope.$watchCollection('defect', function (newval, oldval) {
			if (newval && oldval) {
				$scope.changed = true;
			}
		});
	}]);
})