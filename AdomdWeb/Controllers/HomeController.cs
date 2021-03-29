using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AnalysisServices.AdomdClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using mvc.Models;

namespace mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDaxService _dax;

        public HomeController(ILogger<HomeController> logger, IDaxService daxService)
        {
            _logger = logger;
            _dax = daxService;
        }

        public IActionResult Index()
        {
            var model = BuildIndexModel();
            return View(model);
        }

        private HomeIndexViewModel BuildIndexModel()
        {
            _dax.OpenConnection();

            var model = new HomeIndexViewModel();

            string daxQuery = @"
                DEFINE
                    VAR maxYear = MAX ( Calendario[Año] )
                EVALUATE
                    CALCULATETABLE (
                        ROW (
                            ""Unidades Vendidas"", [Unidades Vendidas],
                            ""Importe"", [Importe],
                            ""% Beneficiario"", [% Beneficio]
                        ),
                        Calendario[Año] = maxYear
                    )";

            using (var reader = _dax.GetDaxResult(daxQuery))
            {
                if (reader.Read())
                {
                    model.UnidadesVendidas = reader.GetInt64(0);
                    model.Importe = reader.GetDecimal(1);
                    model.Beneficio = reader.GetDecimal(2);
                }
                reader.Close();
            }

            _dax.CloseConnection();

            return model;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
