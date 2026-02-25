using Microsoft.AspNetCore.Mvc;
using MvcCoreSessionEmpleados.Extensions;
using MvcCoreSessionEmpleados.Models;
using MvcCoreSessionEmpleados.Repositories;

namespace MvcCoreSessionEmpleados.Controllers
{
    public class EmpleadosController : Controller
    {
        RepositoryEmpleados repo;
        public EmpleadosController(RepositoryEmpleados repo)
        {
            this.repo = repo;
        }

        public async Task<IActionResult> SessionSalarios(int? salario)
        {
            if(salario != null)
            {
                //QUEREMOS ALMACENAR LA SUMA TOTAL DE SALARIOS
                //QUE TENGAMOS EN SESSION
                int sumaTotal = 0;
                if(HttpContext.Session.GetString("SUMASALARIAL") != null)
                {
                    //SI YA TENEMOS DATOS ALMACENADOS, LOS RECUPERAMOS
                    sumaTotal = HttpContext.Session.GetObject<int>("SUMASALARIAL");
                }
                //SUMAMOS EL NUEVO SALARIO A LA SUMA TOTAL
                sumaTotal += salario.Value;
                //ALMACENAMOS EL VALOR DENTRO DE SESSION
                HttpContext.Session.SetObject("SUMASALARIAL", sumaTotal);
                ViewData["MENSAJE"] = "Salario almacenado: " + salario;
            }
            List<Empleado> empleados = await this.repo.GetEmpleadosAsync();
            return View(empleados);
        }
        public IActionResult SumaSalarial()
        {
            return View();   
        }
        public IActionResult Index()
        {
            return View();   
        }
        
        public async Task<IActionResult> SessionEmpleados(int? idEmpleado)
        {
            if (idEmpleado != null)
            {
                Empleado empleado = await this.repo.FindEmpleadoAsync(idEmpleado.Value);
                //EN SESSION TENDREMOS ALMACENADOS UN CONJUNTO DE EMPLEADOS
                List<Empleado> empleadosList;

                //DEBEMOS PREGUNTAR SI YA TENEMOS EMPLEADOS EN SESSION
                if (HttpContext.Session.GetObject<List<Empleado>>("EMPLEADOS") != null)
                {
                    //RECUPERAMOS LA LISTA DE SESSION
                    empleadosList = HttpContext.Session.GetObject<List<Empleado>>("EMPLEADOS");
                }
                else
                {
                    //CREAMOS UNA NUEVA LISTA PARA ALMACENAR LOS EMPLEADOS
                    empleadosList = new List<Empleado>();
                }
                //AGREGAMOS EL EMPELADO AL LIST
                empleadosList.Add(empleado);
                //ALMACENAMOS LA LISTA EN SESSION
                HttpContext.Session.SetObject("EMPLEADOS", empleadosList);
                ViewData["MENSAJE"] = "Empleado " + empleado.Apellido + " almacenado correctamente";
            }
            List<Empleado> empleados = await this.repo.GetEmpleadosAsync();
            return View(empleados);
        }

        public IActionResult EmpleadosAlmacenados()
        {
            return View();
        }
    }
}