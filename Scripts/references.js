function getDispos() {
	if (localStorage.dispos) {
		return JSON.parse(localStorage.dispos);
	}
	return null;
}
function setDispos(d) {
	localStorage.dispos = JSON.stringify(d);
}
function getUsers() {
	if (localStorage.users) {
		return JSON.parse(localStorage.users);
	}
	return null;
}
function setUsers(u) {
	localStorage.users = JSON.stringify(u);
}
function getTypes() {
	if (localStorage.types) {
		return JSON.parse(localStorage.types);
	}
	return null;
}
function setTypes(u) {
	localStorage.types = JSON.stringify(u);
}
function getSevers() {
	if (localStorage.severs) {
		return JSON.parse(localStorage.severs);
	}
	return null;
}
function setSevers(u) {
	localStorage.severs = JSON.stringify(u);
}
function getProducts() {
	if (localStorage.products) {
		return JSON.parse(localStorage.products);
	}
	return null;
}
function setProducts(u) {
	localStorage.products = JSON.stringify(u);
}
function getPriorities() {
	if (localStorage.priorities) {
		return JSON.parse(localStorage.priorities);
	}
	return null;
}
function setPriorities(u) {
	localStorage.priorities = JSON.stringify(u);
}
function getComps() {
	if (localStorage.comps) {
		return JSON.parse(localStorage.comps);
	}
	return null;
}
function setComps(u) {
	localStorage.comps = JSON.stringify(u);
}