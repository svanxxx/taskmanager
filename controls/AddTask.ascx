<%@ Control Language="C#" AutoEventWireup="true" %>

<script>
	$(function () {
		$('#createttbtn').click(function () {
			createTT($('#tttext')[0].value);
			$('#addtask').modal('hide');
		});
		$('#tttext').keydown(function (key) {
			if (key.which === 13) {
				createTT($('#tttext')[0].value);
				$('#addtask').modal('hide');
			}
		});
		$('#addtask').on('shown.bs.modal', function () {
			$('#tttext').focus();
		});
		$('#footerp').html("&copy; " + (new Date().getFullYear()) + " MPS tasks control system");
		$('a').each(function () {
			if (window.location.href.toUpperCase().indexOf($(this).prop('href').toUpperCase()) != -1) {
				$(this).addClass('active'); $(this).parents('li').addClass('active');
			}
		});
	});
</script>

<div class="modal" id="addtask">
	<div class="modal-dialog modal-dialog-centered">
		<div class="modal-content">
			<div class="modal-header">
				<h4 class="modal-title">You are about to create new task</h4>
				<button type="button" class="close" data-dismiss="modal">&times;</button>
			</div>
			<div class="modal-body">
				<div class="form-group">
					<label for="usr">Task Summary:</label>
					<input type="text" class="form-control" id="tttext" value="Free To Use">
				</div>
			</div>
			<div class="modal-footer">
				<button type="button" class="btn btn-primary" id="createttbtn">Create</button>
				<button type="button" class="btn btn-danger" data-dismiss="modal">Cancel</button>
			</div>
		</div>
	</div>
</div>
