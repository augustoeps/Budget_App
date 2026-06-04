using System;
using System.Collections.Generic;

namespace Presupuesto.Helper
{
    public class PeriodoHelper
    {
        // DTO simple para UI
        public class MesDTO
        {
            public int MesNumero { get; set; }
            public string Nombre { get; set; }

            // 👇 lo vamos a usar luego con BD
            public bool ExisteEnBD { get; set; }
            public bool EsSeleccionado { get; set; }

            public int Anio { get; set; }
        }

        // 👇 ahora sí: el año entra como contexto real
        public static List<MesDTO> GenerarMeses(int anio)
        {
            return new List<MesDTO>
    {
        new MesDTO { MesNumero = 1,  Nombre = "Enero",      Anio = anio },
        new MesDTO { MesNumero = 2,  Nombre = "Febrero",    Anio = anio },
        new MesDTO { MesNumero = 3,  Nombre = "Marzo",      Anio = anio },
        new MesDTO { MesNumero = 4,  Nombre = "Abril",      Anio = anio },
        new MesDTO { MesNumero = 5,  Nombre = "Mayo",       Anio = anio },
        new MesDTO { MesNumero = 6,  Nombre = "Junio",      Anio = anio },
        new MesDTO { MesNumero = 7,  Nombre = "Julio",      Anio = anio },
        new MesDTO { MesNumero = 8,  Nombre = "Agosto",     Anio = anio },
        new MesDTO { MesNumero = 9,  Nombre = "Septiembre", Anio = anio },
        new MesDTO { MesNumero = 10, Nombre = "Octubre",    Anio = anio },
        new MesDTO { MesNumero = 11, Nombre = "Noviembre",  Anio = anio },
        new MesDTO { MesNumero = 12, Nombre = "Diciembre",  Anio = anio }
    };
        }

        
    }
}