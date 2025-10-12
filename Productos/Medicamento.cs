using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_de_Gestión_Farmacéutica
{
    public class Medicamento
    {
        public int id_medicamento {  get; set; }
        public required string nombre_comercial { get; set; }
        public float precio_unitario { get; set; }
        public required string presentacion { get; set; }
        public required string laboratorio { get; set; }
        public int stock { get; set; }
        public int stock_minimo { get; set; }
        public int activado { get; set; }

        public Medicamento() { }

        public Medicamento(int id_medicamento, string nombre_comercial, float precio_unitario, string presentacion, string laboratorio, int stock, int stock_minimo, int activado)
        {
            this.id_medicamento = id_medicamento;
            this.nombre_comercial = nombre_comercial;
            this.precio_unitario = precio_unitario;
            this.presentacion = presentacion;
            this.laboratorio = laboratorio;
            this.stock = stock;
            this.stock_minimo = stock_minimo;
            this.activado = activado;
        }
    }
}
