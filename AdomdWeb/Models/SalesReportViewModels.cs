using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace mvc.Models
{
    public class SalesReportIndexViewModel {
        public int Year { get; set; }
        public List<SelectListItem> Years { get; set; }
    }
    
    public class SalesReportDataViewModel
    {
        public long UnidadesVendidas { get; set; }
        public decimal Importe { get; set; }        
        public decimal Beneficio { get; set; }

        public List<Tuple<string,long>> UnidadesPorCategoria { get; set; }
        public List<Tuple<string,decimal>> ImportePorCategoria { get; set; }
        public List<Tuple<string,decimal>> BeneficioPorCategoria { get; set; }

        public List<Tuple<string,long>> UnidadesPorMes { get; set; }
        public List<Tuple<string,decimal>> ImportePorMes { get; set; }
        public List<Tuple<string,decimal>> BeneficioPorMes { get; set; }
    }
}
