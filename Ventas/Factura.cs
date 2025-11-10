using iText.Kernel.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Layout.Borders;
using iText.Kernel.Colors;
using System.Data;


namespace Sistema_de_Gestión_Farmacéutica.Ventas
{
    public class Factura
    {
        public void GenerarFacturaPDF(string p_nombreCliente, DateTime p_fechaVenta, List<DetalleItem> p_detalles)
        {
            string carpeta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Facturas");
            Directory.CreateDirectory(carpeta);

            string archivo = Path.Combine(carpeta, $"Factura.pdf");

            using (PdfWriter writer = new PdfWriter(archivo))
            using (PdfDocument pdf = new PdfDocument(writer))
            using (Document doc = new Document(pdf))
            {
                // Encabezado
                Paragraph titulo = new Paragraph("Factura de Venta")
                    .SetFontSize(20)
                    .SimulateBold()
                    .SetTextAlignment(TextAlignment.CENTER);
                doc.Add(titulo);
                doc.Add(new Paragraph("\n"));


                // Fecha y Cliente
                Table infoTable = new Table(UnitValue.CreatePercentArray(new float[] { 70, 30 }))
                    .UseAllAvailableWidth()
                    .SetMarginBottom(10);

                    infoTable.AddCell(
                        new Cell()
                            .Add(new Paragraph($"Cliente: {p_nombreCliente}"))
                            .SetBorder(Border.NO_BORDER)
                            .SetTextAlignment(TextAlignment.LEFT)
                    );

                    infoTable.AddCell(
                        new Cell()
                            .Add(new Paragraph($"Fecha: {p_fechaVenta:dd/MM/yyyy}"))
                            .SetBorder(Border.NO_BORDER)
                            .SetTextAlignment(TextAlignment.RIGHT)
                    );

                    doc.Add(infoTable); 


                // Tabla de Productos
                Table tabla = new Table(new float[] { 3, 1, 2, 2 }).UseAllAvailableWidth();
                tabla.AddHeaderCell(new Cell().Add(new Paragraph("Producto")).SetBackgroundColor(ColorConstants.LIGHT_GRAY));
                tabla.AddHeaderCell(new Cell().Add(new Paragraph("Cant.")).SetBackgroundColor(ColorConstants.LIGHT_GRAY));
                tabla.AddHeaderCell(new Cell().Add(new Paragraph("Precio Unitario")).SetBackgroundColor(ColorConstants.LIGHT_GRAY));
                tabla.AddHeaderCell(new Cell().Add(new Paragraph("Subtotal")).SetBackgroundColor(ColorConstants.LIGHT_GRAY));

                decimal total = 0;

                foreach (var item in p_detalles)
                {
                    tabla.AddCell(new Paragraph(item.Nombre));
                    tabla.AddCell(new Paragraph(item.Cantidad.ToString()));
                    tabla.AddCell(new Paragraph(item.Precio.ToString("F2")));
                    tabla.AddCell(new Paragraph(item.Subtotal.ToString("F2")));

                    total += Convert.ToDecimal(item.Subtotal);
                }

                doc.Add(tabla);


                // Total
                doc.Add(new Paragraph($"\nTotal: ${total:F2}")
                    .SimulateBold()
                    .SetTextAlignment(TextAlignment.RIGHT)
                    .SetFontSize(14));

                // Mensaje Final
                doc.Add(new Paragraph("\nGracias por su compra.")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(12)
                    .SimulateItalic());

            }

            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = archivo,
                UseShellExecute = true
            });
        }


        public void GenerarReportePDF(DataTable p_ventas, Func<int, List<DetalleItem>> obtenerDetalles, string p_filtrosAplicados)
        {
            string carpeta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Reportes");
            Directory.CreateDirectory(carpeta);

            string archivo = Path.Combine(carpeta, $"ReporteVentas_{DateTime.Now:yyyyMMdd_HHmm}.pdf");

            using (PdfWriter writer = new PdfWriter(archivo))
            using (PdfDocument pdf = new PdfDocument(writer))
            using (Document doc = new Document(pdf))
            {
                // Título
                Paragraph titulo = new Paragraph("Reporte de Ventas")
                    .SetFontSize(20)
                    .SimulateBold()
                    .SetTextAlignment(TextAlignment.CENTER);
                doc.Add(titulo);
                doc.Add(new Paragraph($"Generado el {DateTime.Now:dd/MM/yyyy}")
                    .SetFontSize(10)
                    .SetTextAlignment(TextAlignment.CENTER));
                doc.Add(new Paragraph("\n"));

                // Filtros
                doc.Add(new Paragraph($"\nFiltros aplicados: {p_filtrosAplicados}")
                    .SetTextAlignment(TextAlignment.LEFT)
                    .SetFontSize(11)
                    .SetMarginBottom(10));

                decimal totalGeneral = 0;

                foreach (DataRow venta in p_ventas.Rows)
                {
                    int idVenta = Convert.ToInt32(venta["id_venta"]);
                    string vendedor = venta["nombre_vendedor"].ToString();
                    string cliente = venta["nombre_cliente"].ToString();
                    DateTime fechaVenta = Convert.ToDateTime(venta["fecha_venta"]);
                    decimal totalVenta = Convert.ToDecimal(venta["total"]);
                    totalGeneral += totalVenta;


                    // Encabezado de la venta
                    doc.Add(new Paragraph($"Venta #{idVenta} - Fecha: {fechaVenta:dd/MM/yyyy}")
                        .SimulateBold()
                        .SetFontSize(12));
                    doc.Add(new Paragraph($"Vendedor: {vendedor} | Cliente: {cliente}\n")
                        .SetFontSize(12));


                    // Tabla de detalles
                    Table tablaDetalles = new Table(new float[] { 3, 1, 2, 2 }).UseAllAvailableWidth();
                    tablaDetalles.AddHeaderCell(new Cell().Add(new Paragraph("Producto")).SetBackgroundColor(ColorConstants.LIGHT_GRAY));
                    tablaDetalles.AddHeaderCell(new Cell().Add(new Paragraph("Cant.")).SetBackgroundColor(ColorConstants.LIGHT_GRAY));
                    tablaDetalles.AddHeaderCell(new Cell().Add(new Paragraph("Precio Unit.")).SetBackgroundColor(ColorConstants.LIGHT_GRAY));
                    tablaDetalles.AddHeaderCell(new Cell().Add(new Paragraph("Subtotal")).SetBackgroundColor(ColorConstants.LIGHT_GRAY));

                    var detalles = obtenerDetalles(idVenta);
                    foreach (var d in detalles)
                    {
                        tablaDetalles.AddCell(new Paragraph(d.Nombre));
                        tablaDetalles.AddCell(new Paragraph(d.Cantidad.ToString()));
                        tablaDetalles.AddCell(new Paragraph(d.Precio.ToString("F2")));
                        tablaDetalles.AddCell(new Paragraph(d.Subtotal.ToString("F2"))); 
                    }

                    doc.Add(tablaDetalles);
                    doc.Add(new Paragraph($"Total venta: ${totalVenta:F2}\n\n")
                        .SetTextAlignment(TextAlignment.RIGHT)
                        .SimulateBold());

                }


                // Total General
                doc.Add(new Paragraph($"TOTAL GENERAL:{totalGeneral:F2}")
                    .SimulateBold()
                    .SetFontSize(14)
                    .SetTextAlignment(TextAlignment.RIGHT));

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = archivo,
                    UseShellExecute = true
                });

            }

        }
    }
}
