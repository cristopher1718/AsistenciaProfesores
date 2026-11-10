using Entities.DTOS;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BusinessLogicLevel.Servicios
{
    public class PdfReporteService
    {
        public static byte[] GenerarReportePdf(List<ReporteAsistenciaDTO> datos, DateOnly desde, DateOnly hasta)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Lato"));

                    // 1. ENCABEZADO
                    page.Header().Element(header =>
                    {
                        header.Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text("Colegio Técnico Profesional de Pococí").FontSize(18).SemiBold().FontColor("#0A2540");
                                col.Item().Text("Reporte Consolidado de Asistencia").FontSize(14).FontColor("#D4AF37");
                                col.Item().Text($"Periodo: {desde:dd/MM/yyyy} al {hasta:dd/MM/yyyy}").FontSize(11);
                                col.Item().Text($"Fecha de Emisión: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(9).FontColor(Colors.Grey.Medium);
                            });
                        });
                    });

                    // 2. CONTENIDO (La Tabla)
                    page.Content().PaddingVertical(1, Unit.Centimetre).Element(content =>
                    {
                        content.Table(table =>
                        {
                            // Definir columnas
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2); // Código
                                columns.RelativeColumn(4); // Nombre
                                columns.RelativeColumn(2); // Tardías < 5
                                columns.RelativeColumn(2); // Tardías > 5
                                columns.RelativeColumn(2); // Aus. Justificadas
                                columns.RelativeColumn(2); // Aus. Injustificadas
                            });

                            // Encabezados de la tabla
                            table.Header(header =>
                            {
                                // Estilo base para los encabezados
                                static IContainer CellStyle(IContainer container) => container.DefaultTextStyle(x => x.SemiBold().FontColor(Colors.White)).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);

                                header.Cell().Background("#0A2540").Element(CellStyle).Text("Código");
                                header.Cell().Background("#0A2540").Element(CellStyle).Text("Docente");
                                header.Cell().Background("#0A2540").Element(CellStyle).AlignCenter().Text("Tardías +5");
                                header.Cell().Background("#0A2540").Element(CellStyle).AlignCenter().Text("Tardías -5");
                                header.Cell().Background("#0A2540").Element(CellStyle).AlignCenter().Text("Aus. Just.");
                                header.Cell().Background("#0A2540").Element(CellStyle).AlignCenter().Text("Aus. Injust.");
                            });

                            // Filas de datos
                            foreach (var item in datos)
                            {
                                static IContainer CellStyle(IContainer container) => container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);

                                table.Cell().Element(CellStyle).Text(item.CodigoInstitucional);
                                table.Cell().Element(CellStyle).Text(item.NombreCompleto);
                                table.Cell().Element(CellStyle).AlignCenter().Text(item.TardiasMenores5.ToString());
                                table.Cell().Element(CellStyle).AlignCenter().Text(item.TardiasMayores5.ToString());
                                table.Cell().Element(CellStyle).AlignCenter().Text(item.AusenciasJustificadas.ToString());
                                table.Cell().Element(CellStyle).AlignCenter().Text(item.AusenciasInjustificadas.ToString());
                            }
                        });
                    });

                    // 3. PIE DE PÁGINA
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                        x.Span(" de ");
                        x.TotalPages();
                    });
                });
            });

            // Retorna el PDF en formato de bytes para descargarlo
            return document.GeneratePdf();
        }
    }
}
