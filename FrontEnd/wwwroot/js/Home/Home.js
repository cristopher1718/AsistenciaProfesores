jsDashboard = {

    objetos: {
    },
    controles: {
        txtCodigoProfesor: '#txtCodigoProfesor'
    },

    Tablas: {
    },

    botones: {
        btnBuscarProfesorInfo: '#btnBuscarProfesorInfo'
    },

    variables: {

    },
    metodos: {

        BuscarProfesorInfo: async function() {
            try {
                var codigo = $('#txtCodigoProfesor').val();

                var profesorInfoResponse = await fetch('/Home/ObtenerInfoProfesor?codigo=' + encodeURIComponent(codigo));

                var respuesta = await profesorInfoResponse.json();

                if (respuesta.ok && respuesta.data != null) {

                    var horario = ``;

                    if (respuesta.data.ubicaciones.length == 0) {
                        horario = `<span>El profesor no tiene clases establecidas el dia de hoy</span>`
                    } else {
                        horario = `<span>
                                Aqui vamos a poner los horarios de clases del profesor
                                </span>`
                    }

                    var divInfo = `
                            <div style="display:flex; flex-direction: column; ">
                                <span>
                                ${respuesta.data.codigoInstitucional}
                                </span>
                                <span>
                                ${respuesta.data.nombreCompleto}
                                </span>
                                <span>
                                ${respuesta.data.especialidad}
                                </span>
                                <span>
                                ${respuesta.data.diaSemana}
                                </span>
                                <span>
                                ${horario}
                                </span>
                            </div>
                              `;

                    Swal.fire({
                        icon: 'success',
                        title: 'Éxito',
                        html: divInfo
                    });
                } else {
                    Swal.fire({
                        icon: 'info',
                        title: 'Error al obtener información',
                        html: respuesta.message
                    });
                }
            } catch (error) {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'Ocurrió un error al buscar la información del profesor: ' + error.message,
                });
            }
        }
        
    },

    eventos:
        function() {
            $(jsDashboard.botones.btnBuscarProfesorInfo).click(function(e) {
                e.preventDefault();
                jsDashboard.metodos.BuscarProfesorInfo();
            });
        }
}

$(function() {
    jsDashboard.eventos();
});