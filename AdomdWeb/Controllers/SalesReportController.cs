using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using mvc.Models;
using Microsoft.AnalysisServices.AdomdClient;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace mvc.Controllers
{
    public class SalesReportController : Controller
    {
        private readonly IDaxService _dax;

        public SalesReportController(IDaxService daxService)
        {
            _dax = daxService;
        }

        public IActionResult Index()
        {
            var model = BuildIndexModel();
            return View(model);
        }

        [HttpPost]
        public JsonResult Data(int year)
        {
            var model = BuildDataModel(year);
            return Json(model);
        }

        private SalesReportIndexViewModel BuildIndexModel()
        {
            _dax.OpenConnection();

            string daxQuery = @"
                EVALUATE
                VALUES(Calendario[Año])
                ORDER BY Calendario[Año] DESC";

            var reader = _dax.GetDaxResult(daxQuery);

            List<SelectListItem> years = new();
            while (reader.Read())
            {
                string year = reader[0].ToString();
                years.Add(new SelectListItem(year, year));
            }
            reader.Close();

            _dax.CloseConnection();
            return new SalesReportIndexViewModel { Years = years };
        }

        private SalesReportDataViewModel BuildDataModel(int year)
        {
            var model = new SalesReportDataViewModel();

            var daxParameters = new List<AdomdParameter> {
                new AdomdParameter("Y", year)
            };

            _dax.OpenConnection();

            model = CalculateTotals(model, daxParameters);
            model = CalculateValuesPerCategory(model, daxParameters);
            model = CalculateValuesPerMonth(model, daxParameters);

            _dax.CloseConnection();

            return model;
        }

        private SalesReportDataViewModel CalculateTotals(SalesReportDataViewModel model, List<AdomdParameter> daxParameters)
        {
            string daxQuery = @"
                EVALUATE
                CALCULATETABLE(
                    ROW(
                        ""Unidades Vendidas"",[Unidades Vendidas],
                        ""Importe"", [Importe],
                        ""% Beneficiario"", [% Beneficio]
                    ),
                    Calendario[Año] = @Y
                )";

            using (var reader = _dax.GetDaxResult(daxQuery, daxParameters))
            {
                if (reader.Read())
                {
                    model.UnidadesVendidas = reader.GetInt64(0);
                    model.Importe = reader.GetDecimal(1);
                    model.Beneficio = reader.GetDecimal(2);
                }
                reader.Close();

            }
            return model;
        }

        private SalesReportDataViewModel CalculateValuesPerCategory(SalesReportDataViewModel model, List<AdomdParameter> daxParameters)
        {
            string daxQuery = @"
                EVALUATE
                SUMMARIZECOLUMNS(
                    Productos[Categoria],
                    FILTER( Calendario, Calendario[Año]  = @Y),
                    ""Unidades Vendidas"", [Unidades Vendidas],
                    ""Importe"", [Importe],
                    ""% Beneficio"", [% Beneficio]
                )";

            var unidades = new List<Tuple<string, long>>();
            var importes = new List<Tuple<string, decimal>>();
            var beneficios = new List<Tuple<string, decimal>>();

            using (var reader = _dax.GetDaxResult(daxQuery, daxParameters))
            {
                while (reader.Read())
                {
                    unidades.Add(new Tuple<string, long>(reader.GetString(0), reader.GetInt64(1)));
                    importes.Add(new Tuple<string, decimal>(reader.GetString(0), reader.GetDecimal(2)));
                    beneficios.Add(new Tuple<string, decimal>(reader.GetString(0), reader.GetDecimal(3)));
                }
                reader.Close();
            }

            model.UnidadesPorCategoria = unidades.OrderByDescending(t => t.Item2).ToList();
            model.ImportePorCategoria = importes.OrderByDescending(t => t.Item2).ToList();
            model.BeneficioPorCategoria = beneficios.OrderByDescending(t => t.Item2).ToList();

            return model;
        }

        private SalesReportDataViewModel CalculateValuesPerMonth(SalesReportDataViewModel model, List<AdomdParameter> daxParameters)
        {
            string daxQuery = @"
                EVALUATE
                SUMMARIZECOLUMNS(
                    Calendario[Mes],
                    FILTER( Calendario, Calendario[Año]  = @Y),
                    ""Unidades Vendidas"", [Unidades Vendidas],
                    ""Importe"", [Importe],
                    ""% Beneficio"", [% Beneficio]
                )";

            var unidades = new List<Tuple<string, long>>();
            var importes = new List<Tuple<string, decimal>>();
            var beneficios = new List<Tuple<string, decimal>>();

            using (var reader = _dax.GetDaxResult(daxQuery, daxParameters))
            {
                while (reader.Read())
                {
                    unidades.Add(new Tuple<string, long>(reader.GetString(0), reader.GetInt64(1)));
                    importes.Add(new Tuple<string, decimal>(reader.GetString(0), reader.GetDecimal(2)));
                    beneficios.Add(new Tuple<string, decimal>(reader.GetString(0), reader.GetDecimal(3)));
                }
                reader.Close();
            }

            model.UnidadesPorMes = unidades;
            model.ImportePorMes = importes;
            model.BeneficioPorMes = beneficios;

            return model;
        }
    }
}
