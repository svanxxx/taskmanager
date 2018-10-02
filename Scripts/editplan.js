function isElementInViewport(el) {
	var rect = el.getBoundingClientRect();
	return (
		rect.top >= 0 &&
		rect.left >= 0 &&
		rect.bottom <= (window.innerHeight || document.documentElement.clientHeight) && /*or $(window).height() */
		rect.right <= (window.innerWidth || document.documentElement.clientWidth) /*or $(window).width() */
	);
}
$(function () {
	var app = angular.module('mpsapplication', []);

	app.filter('getDispoColorById', getDispoColorById);

	app.controller('mpscontroller', ["$scope", "$http", "$timeout", function ($scope, $http, $timeout) {
		$scope.Math = window.Math;
		window.addEventListener("popstate", function (event) {
			if (event.state.userid) {
				$scope.currentuserid = event.state.userid;
				$scope.changeuser($scope.users.find(function (x) { return x.ID == $scope.currentuserid; }), false);
				$scope.$apply();
			}
		});

		$scope.currentuserid = -1;
		if (getParameterByName("userid") != "") {
			$scope.currentuserid = parseInt(getParameterByName("userid"));
		}

		$scope.currentuser = {};
		getDispos($scope, "dispos", $http);

		$scope.discardDefects = function () {
			$scope.changeuser($scope.currentuser, false);
		};
		$scope.scheduletask = function (d) {
			for (var i = $scope.unscheduled.length - 1; i >= 0; i--) {
				if ($scope.unscheduled[i].ID == d.ID) {
					$scope.unscheduled.splice(i, 1);
					d.orderchanged = true;
					$scope.defects.splice(0, 0, d);
					$scope.changed = true;
					return;
				}
			}
		};
		$scope.unscheduletask = function (d) {
			for (var i = $scope.defects.length - 1; i >= 0; i--) {
				if ($scope.defects[i].ID == d.ID) {
					$scope.defects.splice(i, 1);
					d.orderchanged = true;
					$scope.unscheduled.splice(0, 0, d);
					$scope.changed = true;
					return;
				}
			}
		};
		$scope.copyurl = function () {
			var $temp = $("<input>");
			$("body").append($temp);
			$temp.val(window.location.href).select();
			document.execCommand("copy");
			$temp.remove();
			var p = StartProgress("The link has been copied.");
			setTimeout(function () { EndProgress(p); }, 2000);
		};
		$scope.saveDefects = function () {
			var recs = [];
			for (var i = $scope.defects.length - 1; i >= 0; i--) {
				var d = $scope.defects[i];
				var ord = $scope.defects.length - i;

				if (d.BACKORDER == ord) {
					continue;
				}
				var rec = {};
				rec.ttid = d.ID;
				rec.backorder = ord;
				rec.moved = ("orderchanged" in d);
				recs.push(rec);
			}

			var unsched = [];
			for (var i = $scope.unscheduled.length - 1; i >= 0; i--) {
				var d = $scope.unscheduled[i];
				if ("orderchanged" in d) {
					unsched.push(d.ID);
				}
			}

			$http.post("trservice.asmx/setschedule", JSON.stringify({ "ttids": recs, "unschedule": unsched })).then(function (result) {
				$scope.changeuser($scope.currentuser, false);
			});
		};
		$scope.tasktotop = function (d) {
			for (var i = 0; i < $scope.defects.length; i++) {
				if (d.ID == $scope.defects[i].ID && i > 0) {
					$scope.defects[i].orderchanged = true;
					for (var j = i; j > 0; j--) {
						var tempo = $scope.defects[j - 1];
						$scope.defects[j - 1] = $scope.defects[j];
						$scope.defects[j] = tempo;
					}
					$scope.changed = true;
				}
			}
		};
		$scope.taskMove = function (d, $event) {
			if (($event.keyCode == 38 || $event.keyCode == 40) && $event.ctrlKey == true) {
				$event.preventDefault();
				for (var i = 0; i < $scope.defects.length; i++) {
					if (d.ID == $scope.defects[i].ID) {
						var index = i + 1;
						if ($event.keyCode == 38) {
							index = i - 1;
						}
						if (index == -1 || index == $scope.defects.length) {
							break;
						}
						var tempo = $scope.defects[index];
						$scope.defects[index] = $scope.defects[i];
						$scope.defects[i] = tempo;
						$scope.defects[index].orderchanged = true;
						$scope.changed = true;
						setTimeout(function () {
							if (!isElementInViewport($event.target)) {
								$event.target.scrollIntoView();
							}
						}, 200);
						break;
					}

				}
				$timeout(function () {
					$("input.taskselector:checked").focus();
				}, 10);
			}
		};
		$scope.changeuser = function (u, state) {
			$scope.defects = [];
			$scope.unscheduled = [];
			$scope.currentuserid = u.ID;
			$scope.currentuser = u;
			var prgtasks = StartProgress("Loading tasks...");
			if (state) {
				window.history.pushState({ userid: $scope.currentuserid }, "user: " + u.LOGIN, replaceUrlParam(location.href, "userid", u.ID));
			}
			$http.post("trservice.asmx/getplanned", JSON.stringify({ "userid": u.TTUSERID }))
				.then(function (result) {
					$scope.defects = result.data.d;
					EndProgress(prgtasks);
					$scope.changed = false;
					reActivateTooltips();
				});
			$http.post("trservice.asmx/getunplanned", JSON.stringify({ "userid": u.TTUSERID }))
				.then(function (response) {
					$scope.unscheduled = response.data.d;
					reActivateTooltips();
				});
		};

		getMPSUsers($scope, "users", $http, function () {
			if ($scope.currentuserid < 0) {
				$scope.currentuserid = userID();
			}
			$scope.changeuser($scope.users.find(function (x) { return x.ID == $scope.currentuserid; }), true);
		});
		$scope.changed = false;
	}]);
});