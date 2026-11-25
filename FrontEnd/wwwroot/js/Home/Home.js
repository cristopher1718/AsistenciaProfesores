jsDashboard = {

    objetos: {
    },
    controles: {
        txtCodigoProfesor: '#txtCodigoProfesor'
    },

    Tablas: {
    },

    botones: {
        btnBuscarProfesorInfo: '#btnBuscarProfesorInfo',
        btnModificarHorario: '#btnModificarHorario',
        btnEditarProfesor: '#btnEditarProfesor'
    },

    variables: {

    },
    metodos: {

        BuscarProfesorInfo: async function() {
            try {
                var codigo = $('#txtCodigoProfesor').val();

                var profesorInfoResponse = await fetch('/Home/ObtenerInfoProfesor?codigo=' + encodeURIComponent(codigo));

                var respuesta = await profesorInfoResponse.json();

                console.log({respuesta})

                if (respuesta.ok && respuesta.data != null) {

                    let horario = ``;

                    var ubicaciones = respuesta.data.ubicaciones


                    if (respuesta.data.ubicaciones.length == 0) {
                        horario = `<span>El profesor no tiene clases establecidas el dia de hoy</span>`
                    } else {

                        horario = ``
                        ubicaciones.forEach(u => {

                            let aula = `<span>
                                ${u.aula}
                                </span>`;

                            let tableRow = ``

                            u.leccionesActivas.forEach(l => {

                                tableRow += `<th>L${l}</th>`
                            })

                            let table = `<table style="border: 1px solid;">
                                                <thead>
                                                    <tr>
                                                        ${tableRow}
                                                    </tr>
                                                </thead>
                                            </table>`

                            horario += `<span>
                                ${aula}
                                </span>
                                ${table}
                                `
                        });

                        
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
                                ${horario}
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
        },

        ModifiarHorarioProfesor: async function () {
            Swal.fire({
                title: "Ingrese el codigo del profesor",
                input: "text",
                inputAttributes: { autocapitalize: "off" },
                showCancelButton: true,
                confirmButtonText: "Buscar",
                showLoaderOnConfirm: true,
                preConfirm: async (codigo) => {
                    try {
                        window.location.href = `/Ubicacion/EditarHorario?codigo=${codigo}`
                    } catch (error) {
                        Swal.showValidationMessage(`Request failed: ${error}`);
                    }
                },
                allowOutsideClick: () => !Swal.isLoading()
            })
        },

        EditarProfesor: async function () {
            Swal.fire({
                title: "Ingrese el codigo del profesor",
                input: "text",
                inputAttributes: { autocapitalize: "off" },
                showCancelButton: true,
                confirmButtonText: "Buscar",
                showLoaderOnConfirm: true,
                preConfirm: async (codigo) => {
                    try {
                        window.location.href = `/Profesor/Edit/${codigo}`
                    } catch (error) {
                        Swal.showValidationMessage(`Request failed: ${error}`);
                    }
                },
                allowOutsideClick: () => !Swal.isLoading()
            })
        }


    },

    eventos:
        function() {
            $(jsDashboard.botones.btnBuscarProfesorInfo).click(function(e) {
                e.preventDefault();
                jsDashboard.metodos.BuscarProfesorInfo();
            });

            $(jsDashboard.botones.btnModificarHorario).click(function () {
                jsDashboard.metodos.ModifiarHorarioProfesor();
            })
            $(jsDashboard.botones.btnEditarProfesor).click(function () {
                jsDashboard.metodos.EditarProfesor();
            })
        }
}

$(function() {
    jsDashboard.eventos();
});