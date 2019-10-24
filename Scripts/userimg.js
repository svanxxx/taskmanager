class UserImage extends HTMLElement {
	connectedCallback() {
		var innerHTML =
			"<img " +
			"data-toggle='tooltip' " +
			"title='<img src=\"getUserImg.ashx?sz=150&ttid=" + this.userid + "\" />' " +
			"class='rounded-circle' " +
			"alt='Smile' " +
			"height=" + this.size +
			" width=" + this.size +
			" src='getUserImg.ashx?sz=" + this.size + "&ttid=" + this.userid + "'" +
			" />";
		this.innerHTML = innerHTML;
	}
	get size() {
		return this.getAttribute('size');
	}
	set size(val) {
		this.setAttribute('size', val);
	}
	get userid() {
		return this.getAttribute('userid');
	}
	set userid(val) {
		this.setAttribute('userid', val);
	}
	get ttuserid() {
		return this.getAttribute('ttuserid');
	}
	set ttuserid(val) {
		this.setAttribute('ttuserid', val);
	}
}
$(function () {
	customElements.define('user-image', UserImage);
});