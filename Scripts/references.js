var _admin = undefined;
function IsAdmin() {
	if (typeof _admin === "undefined") {
		_admin = document.getElementById("isadmin").value;
	}
	return _admin === "True";
}
function userID() {
	return parseInt(document.getElementById("userid").value);
}
function ttUserID() {
	return parseInt(document.getElementById("ttuserid").value);
}
function resetReferenceVersion() {
	document.getElementById("referenceid").value = "";
}
function userLogin() {
	return document.getElementById("userlogin").value;
}
function loadReference($scope, member, $http, localmember, functionname, params, func) {
	params = params || {};
	var m = localmember + "_storageversion";
	var curr = document.getElementById("referenceid").value;
	if (localStorage[m] != curr) {
		localStorage.removeItem(localmember);
		localStorage[m] = curr;
	}

	if (localStorage[localmember]) {
		$scope[member] = JSON.parse(localStorage[localmember]);
		if (func) func();
	} else {
		if (!("loaders" in $scope)) {
			$scope["loaders"] = 0;
		}
		var prgtypes = StartProgress("Loading " + localmember + "...");
		$scope["loaders"]++;
		$http.post("trservice.asmx/" + functionname, JSON.stringify(params))
			.then(function (result) {
				$scope[member] = result.data.d;
				localStorage[localmember] = JSON.stringify($scope[member]);
				EndProgress(prgtypes); $scope["loaders"]--;
				if (func) func();
			});
	}
}
var defDispCol = { "background-color": "white" };
function getDispoColorById() {
	return function (id, $scope) {
		if ($scope.dispos.length < 1) {
			return defDispCol;
		}
		var res = $scope.dispos.filter(function (x) { return x.ID == id; });
		if (res.length < 1) {
			return defDispCol;
		}
		return { "background-color": res[0].COLOR };
	};
}
function rawHtml($sce) {
	return function (val) {
		return $sce.trustAsHtml(val);
	};
}
function getDispoById() {
	return function (id, $scope) {
		if ($scope.dispos.length < 1) {
			return "";
		}
		var res = $scope.dispos.filter(function (x) { return x.ID == id; });
		if (res.length < 1) {
			return "";
		}
		return res[0].DESCR;
	};
}
function getCompById() {
	return function (id, $scope) {
		if ($scope.comps.length < 1) {
			return "";
		}
		var res = $scope.comps.filter(function (x) { return x.ID == id; });
		if (res.length < 1) {
			return "";
		}
		return res[0].DESCR;
	};
}
function getSeveById() {
	return function (id, $scope) {
		var r = $scope.severs.filter(function (x) { return x.ID == id; });
		if (r.length > 0) {
			return r[0].DESCR;
		}
		return "";
	};
}
function getUserById() {
	return function (id, $scope) {
		if (!$scope.users) {
			return "";
		}
		var r = $scope.users.filter(function (x) { return x.ID == id; });
		if (r.length > 0) {
			return r[0].FULLNAME;
		}
		return "";
	};
}
function getUserTRIDById() {
	return function (id, $scope) {
		if (!$scope.users) {
			return "";
		}
		var r = $scope.users.filter(function (x) { return x.ID == id; });
		if (r.length > 0) {
			return r[0].TRID;
		}
		return "";
	};
}

function getTypes($scope, member, $http) {
	loadReference($scope, member, $http, "types", "gettasktypes");
}
function getDispos($scope, member, $http) {
	loadReference($scope, member, $http, "dispos", "gettaskdispos");
}
function getUsers($scope, member, $http, func) {
	loadReference($scope, member, $http, "users", "gettaskusers", null, func);
}
function getMPSUsers($scope, member, $http, func) {
	$scope[member] = [];
	loadReference($scope, member, $http, "MPSUsers", "getMPSUsers", { "active": true }, func);
}
function getPriorities($scope, member, $http) {
	loadReference($scope, member, $http, "priorities", "gettaskpriorities");
}
function getSevers($scope, member, $http) {
	loadReference($scope, member, $http, "severs", "gettasksevers");
}
function getProducts($scope, member, $http) {
	loadReference($scope, member, $http, "products", "gettaskproducts");
}
function getComps($scope, member, $http) {
	loadReference($scope, member, $http, "comps", "gettaskcomps");
}
function loadCommit(c, $scope, $http, member) {
	if (c.DIFF) {
		c.DIFF = null;
	} else {
		var commdiff = StartProgress("Loading commit details...");
		$http.post("GitService.asmx/getCommitDiff", JSON.stringify({ "commit": c.COMMIT }))
			.then(function (result) {
				EndProgress(commdiff);
				if (!member) {
					member = "commits";
				}
				$scope[member].forEach(function (co) {
					if (co.COMMIT === c.COMMIT) {
						co.DIFF = result.data.d.join("<br/>");
					}
				});
			});
	}
}
function createTasksFilter(filter) {
	if (!("dispositions" in filter)) {
		filter.dispositions = [];
	}
	if (!("ID" in filter)) {
		filter.ID = "";
	}
	if (!("components" in filter)) {
		filter.components = [];
	}
	if (!("severities" in filter)) {
		filter.severities = [];
	}
	if (!("createdUsers" in filter)) {
		filter.createdUsers = [];
	}
	if (!("users" in filter)) {
		filter.users = [];
	}
	if (!("text" in filter)) {
		filter.text = "";
	}
	if (!("startDateEnter" in filter)) {
		filter.startDateEnter = "";
	} else {
		if (filter.startDateEnter !== "") {
			filter.startDateEnter = StringToDate(filter.startDateEnter);
		}
	}
	if (!("endDateEnter" in filter)) {
		filter.endDateEnter = "";
	} else {
		if (filter.endDateEnter !== "") {
			filter.endDateEnter = StringToDate(filter.endDateEnter);
		}
	}
	if (!("startDateCreated" in filter)) {
		filter.startDateCreated = "";
	} else {
		if (filter.startDateCreated !== "") {
			filter.startDateCreated = StringToDate(filter.startDateCreated);
		}
	}
	if (!("endDateCreated" in filter)) {
		filter.endDateCreated = "";
	} else {
		if (filter.endDateCreated !== "") {
			filter.endDateCreated = StringToDate(filter.endDateCreated);
		}
	}
	if (!("startEstim" in filter)) {
		filter.startEstim = "";
	}
	if (!("endEstim" in filter)) {
		filter.endEstim = "";
	}

}
function createDSFilter(userid) {
	var filter = {};
	var d = new Date(); d.setDate(1); d.setHours(0, 0, 0, 0);
	filter.text = "";
	filter.startdate = d;
	filter.enddate = new Date(d.getFullYear(), d.getMonth() + 1, 0);
	filter.userid = "" + userid;
	return filter;
}
function openTask(ttid) {
	window.open("showtask.aspx?ttid=" + ttid, '_blank');
}
function enterTT() {
	var ttid = parseInt(prompt("Please enter TT ID"));
	if (!isNaN(ttid)) {
		openTask(ttid);
	}
}
function allowPosNumbers(e) {
	if (!(
				(e.keyCode > 95 && e.keyCode < 106) //numpad numbers
			|| (e.keyCode > 47 && e.keyCode < 58)  //numbers
			|| (e.keyCode === 8)							//backspace
			|| (e.keyCode === 9)							//tab
			|| (e.keyCode === 13)						//enter
			|| (e.keyCode === 45)						//insert - for pasting
			|| (e.keyCode > 36 && e.keyCode < 41)	//left right up and down
		)) {
		e.preventDefault();
		console.log(e.keyCode);
		return false;
	}
}
function createTT(summary) {
	if (summary !== "" && summary !== null) {
		$.ajax({
			type: "POST",
			url: GetSitePath() + "trservice.asmx/newTask",
			data: JSON.stringify({ "summary": summary }),
			contentType: "application/json; charset=utf-8",
			dataType: "json",
			success: function (mess) {
				openTask(mess.d);
			}
		});
	}
}
function reActivateTooltips() {
	setTimeout(function () { $('.tooltip').remove(); $('[data-toggle="tooltip"]').tooltip({ html: true }); }, 1000);//when data loaded - activate tooltip.
}
function sumFormat($sce) {
	return function (val) {
		var parts = val.split("@");
		if (parts.length < 2) {
			return $sce.trustAsHtml(val);
		}
		return $sce.trustAsHtml(parts[0] + "&nbsp;<sup><small><i>" + parts[1] + "</small></i></sup>");
	};
}
function killTooltips() {
	setTimeout(function () {
		var elements = document.getElementsByClassName("tooltip");
		while (elements.length > 0) {
			elements[0].parentNode.removeChild(elements[0]);
		}
	}, 500);
}
function copyurl(txt) {
	var $temp = $("<input>");
	$("body").append($temp);
	var stamp = "timestamp=" + (new Date()).getTime();
	if (txt === undefined) {
		var hr = window.location.href;
		if (hr.includes("?")) {
			hr += "&" + stamp;
		} else {
			hr += "?" + stamp;
		}
		$temp.val(hr).select();
	} else {
		$temp.val(txt).select();
	}
	document.execCommand("copy");
	$temp.remove();

	var options = {
		icon: "~/images/fist.png"
	};
	var n = new Notification("The link has been copied to clipboard.", options);
	setTimeout(n.close.bind(n), 2000);
}
$(function () {
	$(document).bind("keydown", function (e) {
		if (e.keyCode === 188 && event.ctrlKey) {
			enterTT();
		}
	});
	reActivateTooltips();
});