$('#exampleModal').on('show.bs.modal', function (event) {
    var button = $(event.relatedTarget)
    var recipient = button.data('whatever')
    var modal = $(this)
    modal.find('.modal-title').text('New message to ' + recipient)
    modal.find('.modal-body input').val(recipient)
})

$('#VerSolicitudes').on('show.bs.modal', function (event) {
    var button = $(event.relatedTarget)
    var recipient = button.data('whatever')
    var modal = $(this)
    modal.find('.modal-title').text('New message to ' + recipient)
    modal.find('.modal-body input').val(recipient)
})

function aceptarSolicitud(usuario, invitacion) {
    $.ajax({
        type: 'GET',
        url: '@Url.Action("AceptarSolicitud", "Principal")',

        dataType: 'json',
        data: {
            usuario: usuario,
            invitado: invitacion
        },
        success: function (states) {
            alert("Actualizado");
        },
        error: function (ex) {
            alert("Error al actualizar");
        }
    })
}

function rechazarSolicitud(usuario, invitacion) {
    $.ajax({
        type: 'GET',
        url: '@Url.Action("RechazarSolicitud", "Principal")',

        dataType: 'json',
        data: {
            usuario: usuario,
            invitado: invitacion
        },
        success: function (states) {
            alert("Actualizado");
        },
        error: function (ex) {
            alert("Error al actualizar");
        }
    })
}

function actualizarPagina() {
    console.log("adentro");
    setTimeout(function () {
        location.reload();
    }, 5000);
}