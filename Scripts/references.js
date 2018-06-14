var storageversion = "0";
function loadReference($scope, member, $http, localmember, functionname) {
	var m = localmember + "_storageversion";
	if (localStorage[m] != storageversion) {
		localStorage.removeItem(localmember);
		localStorage[m] = storageversion;
	}

	if (localStorage[localmember]) {
		$scope[member] = JSON.parse(localStorage[localmember]);
	} else {
		if (!("loaders" in $scope)) {
			$scope["loaders"] = 0;
		}
		var prgtypes = StartProgress("Loading " + localmember + "..."); $scope["loaders"]++;
		$http.post("trservice.asmx/" + functionname, JSON.stringify({}))
			.then(function (result) {
				$scope[member] = result.data.d;
				localStorage[localmember] = JSON.stringify($scope[member]);
				EndProgress(prgtypes); $scope["loaders"]--;
			});
	}
}
function getTypes($scope, member, $http) {
	loadReference($scope, member, $http, "types", "gettasktypes");
}
function getDispos($scope, member, $http) {
	loadReference($scope, member, $http, "dispos", "gettaskdispos");
}
function getUsers($scope, member, $http) {
	loadReference($scope, member, $http, "users", "gettaskusers");
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