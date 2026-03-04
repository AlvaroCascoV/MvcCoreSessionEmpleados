using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MvcCoreSessionEmpleados.Extensions;
using MvcCoreSessionEmpleados.Models;
using MvcCoreSessionEmpleados.Repositories;
using System.Threading.Tasks;

namespace MvcCoreSessionEmpleados.Controllers
{
    public class EmpleadosController : Controller
    {
        RepositoryEmpleados repo;
        IMemoryCache memoryCache;
        public EmpleadosController(RepositoryEmpleados repo, IMemoryCache memoryCache)
        {
            this.repo = repo;
            this.memoryCache = memoryCache;
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
                //EN SESSION TENDREMOS ALMACENADOS UN CONJUNTO DE EMPLEADOS (ineficaz, mejor guardar datos como ids)
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
        
        public async Task<IActionResult> SessionEmpleadosOk(int? idempleado)
        {
            if (idempleado != null)
            {
                //ALMACENAMOS LO MINIMO...
                List<int> idsEmpleados;
                if(HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS") != null)
                {
                    //RECUPERAMOS LA COLECCION
                    idsEmpleados = HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
                }
                else
                {
                    //CREAMOS LA COLECCION
                    idsEmpleados = new List<int>();
                }
                //ALMACENAMOS EL ID DEL EMPLEADO
                idsEmpleados.Add(idempleado.Value);
                //ALMACENAMOS EN SESSION LOS DATOS
                HttpContext.Session.SetObject("IDSEMPLEADOS", idsEmpleados);
                ViewData["MENSAJE"] = "Empleados almacenados: " + idsEmpleados.Count;
            }
            List<Empleado> empleados = await this.repo.GetEmpleadosAsync();
            return View(empleados);
        }
        public async Task<IActionResult> EmpleadosAlmacenadosOk()
        {
            //para mostrar empleados en vez de ids
            //RECUPERAMOS LA COLECCION DE SESSION
            List<int> idsEmpleados = HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
            if(idsEmpleados == null)
            {
                ViewData["MENSAJE"] = "No existen empleados en Session";
                return View();
            }
            else
            {
                List<Empleado> empleados = await this.repo.GetEmpleadosSessionAsync(idsEmpleados);
                return View(empleados);
            }
        }
        //ahora que se borre los que tengo almacenados
        public async Task<IActionResult> SessionEmpleadosV4(int? idempleado)
        {
            if (idempleado != null)
            {
                List<int> idsEmpleadosList;
                if (HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS") != null)
                {
                    idsEmpleadosList = HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
                }
                else
                {
                    idsEmpleadosList = new List<int>();
                }
                idsEmpleadosList.Add(idempleado.Value);
                HttpContext.Session.SetObject("IDSEMPLEADOS", idsEmpleadosList);
                ViewData["MENSAJE"] = "Empleados almacenados: " + idsEmpleadosList.Count;
            }
            //REVISAMOS SI EXISTE LA SESSION
            List<int> idsEmpleados = HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
            if (idsEmpleados == null)
            {
                List<Empleado> empleados = await this.repo.GetEmpleadosAsync();
                return View(empleados);
            }
            else
            {
                List<Empleado> empleados = await this.repo.GetEmpleadosNotSessionAsync(idsEmpleados);
                return View(empleados);
            }
        }
        public async Task<IActionResult> EmpleadosAlmacenadosV4()
        {
            //para mostrar empleados en vez de ids
            //RECUPERAMOS LA COLECCION DE SESSION
            List<int> idsEmpleados = HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
            if(idsEmpleados == null)
            {
                ViewData["MENSAJE"] = "No existen empleados en Session";
                return View();
            }
            else
            {
                List<Empleado> empleados = await this.repo.GetEmpleadosSessionAsync(idsEmpleados);
                return View(empleados);
            }
        }
        //deshabilitar el boton y mostrar otra cosa
        //añadimos cache basico, vemos que no funcionan juntas
        //[ResponseCache(Duration = 80,Location = ResponseCacheLocation.Client)]
        //ahora añadimos el memorycache para favoritos
        public async Task<IActionResult> SessionEmpleadosV5(int? idempleado, int? idfavorito)
        {
            if (idempleado != null)
            {
                List<int> idsEmpleadosList;
                if (HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS") != null)
                {
                    idsEmpleadosList = HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
                }
                else
                {
                    idsEmpleadosList = new List<int>();
                }
                idsEmpleadosList.Add(idempleado.Value);
                HttpContext.Session.SetObject("IDSEMPLEADOS", idsEmpleadosList);
                ViewData["MENSAJE"] = "Empleados almacenados: " + idsEmpleadosList.Count;
            }

            //añadimos memoryCache
            if(idfavorito != null)
            {
                //COMO ESTOY ALMACENANDO EN CACHE, VAMOS A GUARDAR
                //DIRECTAMENTE LOS OBJETOS EN LUGAR DE LOS IDS            
                List<Empleado> empleadosFavoritos;
                if(this.memoryCache.Get("FAVORITOS") == null)
                {
                    //si no existe, lo creamos
                    empleadosFavoritos = new List<Empleado>();
                }
                else
                {
                    //si hay, lo recuperamos
                    empleadosFavoritos = this.memoryCache.Get<List<Empleado>>("FAVORITOS");
                }
                //buscamos al empleado para guardarlo
                Empleado empleadoFavorito = await this.repo.FindEmpleadoAsync(idfavorito.Value);
                empleadosFavoritos.Add(empleadoFavorito);
                this.memoryCache.Set("FAVORITOS", empleadosFavoritos);
            }

            List<Empleado> empleados = await this.repo.GetEmpleadosAsync();
            return View(empleados);
        }
        //añadimos cache basico, vemos que no funcionan juntas
        //[ResponseCache(Duration = 80, Location = ResponseCacheLocation.Client)]
        public async Task<IActionResult> EmpleadosAlmacenadosV5(int? ideliminar)
        {
            List<int> idsEmpleados = HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
            if(idsEmpleados == null)
            {
                ViewData["MENSAJE"] = "No existen empleados en Session";
                return View();
            }
            else
            {
                //PREGUNTAMOS SI HEMOS RECIBIDO EL ID DEL EMPLEADO A ELIMINAR
                if (ideliminar != null)
                {
                    idsEmpleados.Remove(ideliminar.Value);
                    //SI NO TENEMOS EMPLEADOS EN SESSION,
                    //NUESTRA COLECCION SIGUE EXISTIENDO PERO SE QUEDA A 0
                    //HAY QUE ELIMINAR LA SESSION
                    if(idsEmpleados.Count == 0)
                    {
                        HttpContext.Session.Remove("IDSEMPLEADOS");
                        return View();
                    }
                    else
                    {
                        //ACTUALIZAMOS SESSION
                        HttpContext.Session.SetObject("IDSEMPLEADOS", idsEmpleados);
                    }
                }
                List<Empleado> empleados = await this.repo.GetEmpleadosSessionAsync(idsEmpleados);
                return View(empleados);
            }
        }

        public IActionResult EmpleadosFavoritos()
        {
            //if(this.memoryCache.Get("FAVORITOS") == null)
            //{
            //    ViewData["MENSAJE"] = "No tenemos favoritos";
            //    return View();
            //}
            //else
            //{
            //    List<Empleado> favoritos = this.memoryCache.Get<List<Empleado>>("FAVORITOS");
            //    return View(favoritos);
            //}
            //vamos a pintarlos desde la view
            return View();
        }
    }
}