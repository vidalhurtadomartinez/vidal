﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SIGAA.Models;
using SIGAA.Commons;
using MvcFlash.Core;
using MvcFlash.Core.Extensions;
using SIGAA.Etiquetas;
using SIGAA.ViewModels;

namespace SIGAA.Controllers
{
    [Autenticado]
    public class PermisoDenegadoPorRolController : Controller
    {
        private SeguridadContext db = new SeguridadContext();

        [Permiso(Permiso = RolesPermisos.SEGU_permisoDenegadoPorRol_puedeVerIndice)]
        //public ActionResult Index(string criterio = null)
        //{
        //    var pdr = db.PermisoDenegadoPorRol.Where(p => p.iEliminado_fl == 1).Include(g => g.Rol).ToList();
        //    var pdrFil = pdr.Where(t => criterio == null ||
        //                           t.Rol.Nombre.ToLower().Contains(criterio.ToLower()) ||
        //                           t.Permiso.Modulo.ToLower().Contains(criterio.ToLower()) ||
        //                           t.Permiso.Descripcion.ToString().ToLower().Contains(criterio.ToLower())
        //                           ).ToList();
        //    var pdrFilFilOr = pdrFil.OrderBy(ef => ef.Rol.Nombre);
        //    if (Request.IsAjaxRequest())
        //    {
        //        if (!String.IsNullOrWhiteSpace(criterio))
        //        {
        //            Flash.Instance.Success("RESULTADO DE BUSQUEDA", "Se han encontrado " + pdrFilFilOr.Count() + " registros con los criterios especificados.");
        //        }
        //        return PartialView("_Index", pdrFilFilOr);
        //    }

        //    if (!String.IsNullOrWhiteSpace(criterio))
        //    {
        //        Flash.Instance.Success("RESULTADO DE BUSQUEDA", "Se han encontrado " + pdrFilFilOr.Count() + " registros con los criterios especificados.");
        //    }
        //    return View(pdrFilFilOr);
        //}

        public ActionResult Index()
        {
            Rol primerRol = db.Rol.FirstOrDefault();
            PermisoDenegadoPorRolViewModel pdprvw = new PermisoDenegadoPorRolViewModel();
            var permisosS = TraerVarios_Permisos_PorModulo("SEGU");
            var primerProcesoS = permisosS.LastOrDefault();           

            var acciones = new List<MetodoDeAccionViewModel>();
            acciones = TreerVarios_Acciones_PorProcesoYnemonico(primerRol.iRol_id, "SEGU", primerProcesoS.Proceso);

            pdprvw.iRol_id = primerRol.iRol_id;
            pdprvw.MetodosDeAccionDeProcesoSeleccionado = acciones;
            pdprvw.ProcesosSEGU = TreerVarios_PrcesosVM_PorProcesoYnemonico("SEGU", primerRol.iRol_id);
            pdprvw.ProcesosEGRE = TreerVarios_PrcesosVM_PorProcesoYnemonico("EGRE", primerRol.iRol_id);
            pdprvw.ProcesosCONV = TreerVarios_PrcesosVM_PorProcesoYnemonico("CONV", primerRol.iRol_id);
            pdprvw.ProcesosCRM = TreerVarios_PrcesosVM_PorProcesoYnemonico("CRM", primerRol.iRol_id);
            pdprvw.ProcesosOYM = TreerVarios_PrcesosVM_PorProcesoYnemonico("OYM", primerRol.iRol_id);

            ViewBag.iRol_id = new SelectList(db.Rol, "iRol_id", "Nombre", primerRol.iRol_id);
            return View(pdprvw);
        }

        [Permiso(Permiso = RolesPermisos.SEGU_permisoDenegadoPorRol_puedeVerDetalle)]
        public ActionResult Details(int Rol_id = 0, int Permiso_id = 0)
        {

            if (Rol_id == 0 && Permiso_id == 0)
            {
                Flash.Instance.Error("ERROR", "El paramettro Rol_id ó Permiso_id  no puede ser nulo");
                return RedirectToAction("Index");
            }
            RolesPermisos permiso = (RolesPermisos)Enum.ToObject(typeof(RolesPermisos), Permiso_id);//convierte a un enum en base a su ID
            PermisoDenegadoPorRol permisoDenegadoPorRol = db.PermisoDenegadoPorRol.Find(Rol_id, permiso);
            if (permisoDenegadoPorRol == null)
            {
                Flash.Instance.Error("ERROR", "No existe un permiso dengado por rol con Ids" + Rol_id + " ," + Permiso_id);
                return RedirectToAction("Index");
            }
            return View(permisoDenegadoPorRol);
        }

        [Permiso(Permiso = RolesPermisos.SEGU_permisoDenegadoPorRol_puedeCrearNuevo)]
        public ActionResult Create()
        {
            PermisoDenegadoPorRol pdr = new PermisoDenegadoPorRol();
            Rol rol = db.Rol.FirstOrDefault();

            //List<PermisoDenegadoPorRol> pdrs = db.PermisoDenegadoPorRol.Where(pd => pd.iRol_id == rol.iRol_id).ToList();
            //List<Permiso> permisos = db.Permiso.ToList();

            //var permisoFiltradoPorRol = (from perm in permisos
            //                             where !pdrs.Any(m => m.iPermiso_id == perm.iPermiso_id)
            //                             select perm
            //                             ).ToList();
            var permisoFiltrado = filtrarPermisoDenegadoPorRol(rol.iRol_id);

            ViewBag.iPermiso_id = new SelectList(permisoFiltrado, "iPermiso_id", "Descripcion");
            ViewBag.iRol_id = new SelectList(db.Rol, "iRol_id", "Nombre");
            return View(pdr);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "iRol_id,iPermiso_id,iEstado_fl,iEliminado_fl,sCreado_by,iConcurrencia_id")] PermisoDenegadoPorRol permisoDenegadoPorRol)
        {
            try
            {
                permisoDenegadoPorRol.iEstado_fl = true;
                permisoDenegadoPorRol.iEliminado_fl = 1;
                permisoDenegadoPorRol.sCreado_by = FrontUser.Get().usr_login;
                permisoDenegadoPorRol.iConcurrencia_id = 1;

                if (ModelState.IsValid)
                {
                    db.PermisoDenegadoPorRol.Add(permisoDenegadoPorRol);
                    db.SaveChanges();
                    ModelState.Clear();
                    permisoDenegadoPorRol = null;
                    Flash.Instance.Success("CORRECTO", "El dato se ha guradado correctamente.");
                    return RedirectToAction("Index");
                }

                ViewBag.iPermiso_id = new SelectList(db.Permiso, "iPermiso_id", "Descripcion", permisoDenegadoPorRol.iPermiso_id);
                ViewBag.iRol_id = new SelectList(db.Rol, "iRol_id", "Nombre", permisoDenegadoPorRol.iRol_id);
                Flash.Instance.Error("ERROR", "No se pudo Guardar el dato porque ha ocurrido un error al validar el modelo, por favor verifique que los campos estén correctamente llenados.");
                return View(permisoDenegadoPorRol);
            }
            catch (Exception ex)
            {
                Flash.Instance.Error("ERROR", "No se pudo Guardar el dato porque ha ocurrido el siguiente error " + ex.Message);
                return RedirectToAction("Index");
            }
        }

        [Permiso(Permiso = RolesPermisos.SEGU_permisoDenegadoPorRol_puedeEditar)]
        public ActionResult Edit(int Rol_id = 0, int Permiso_id = 0)
        {
            if (Rol_id == 0 && Permiso_id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "LOS PARAMETROS NO SON CORRECTOS");
            }
            RolesPermisos permiso = (RolesPermisos)Enum.ToObject(typeof(RolesPermisos), Permiso_id);//convierte a un enum en base a su ID
            PermisoDenegadoPorRol permisoDenegadoPorRol = db.PermisoDenegadoPorRol.Find(Rol_id, permiso);
            if (permisoDenegadoPorRol == null)
            {
                return HttpNotFound();
            }

            Session["permisoId"] = (int)Permiso_id;//envio a session para luego remplazar por el nuevo valor
            //List<PermisoDenegadoPorRol> pdrs = db.PermisoDenegadoPorRol.Where(pd => pd.iRol_id == Rol_id).ToList();
            //List<Permiso> permisos = db.Permiso.ToList();

            //var permisoFiltradoPorRol = (from perm in permisos
            //                             where !pdrs.Any(m => m.iPermiso_id == perm.iPermiso_id)
            //                             select perm
            //                             ).ToList();

            var permisoFiltrados = filtrarPermisoDenegadoPorRol(Rol_id);

            ViewBag.iPermiso_id = new SelectList(permisoFiltrados, "iPermiso_id", "Descripcion", permisoDenegadoPorRol.iPermiso_id);
            ViewBag.iRol_id = new SelectList(db.Rol, "iRol_id", "Nombre", Rol_id);
            return View(permisoDenegadoPorRol);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "iRol_id,iPermiso_id,iEstado_fl,iEliminado_fl,sCreado_by,iConcurrencia_id")] PermisoDenegadoPorRol permisoDenegadoPorRol)
        {
            try
            {
                int permisoIDanterior = (int)Session["permisoId"];
                RolesPermisos permisoAnterior = (RolesPermisos)Enum.ToObject(typeof(RolesPermisos), permisoIDanterior);//convierte a un enum en base a su ID
                if (ModelState.IsValid)
                {
                    var PermisosDenegos = db.PermisoDenegadoPorRol.Where(a => a.iRol_id == permisoDenegadoPorRol.iRol_id && a.iPermiso_id == permisoAnterior).ToList();

                    foreach (var s in PermisosDenegos)
                    {
                        db.PermisoDenegadoPorRol.Remove(s);
                    }
                    permisoDenegadoPorRol.iEliminado_fl = 1;
                    permisoDenegadoPorRol.sCreado_by = FrontUser.Get().usr_login;
                    permisoDenegadoPorRol.iConcurrencia_id += 1;

                    db.PermisoDenegadoPorRol.Add(permisoDenegadoPorRol);
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception en)
                    {
                        Flash.Instance.Error("ERROR", "No se pudo Modificar el dato proque ha ocurrido el siguiente error: " + en.Message);
                        return RedirectToAction("Index");
                    }

                    //db.Entry(permisoDenegadoPorRol).State = EntityState.Modified;
                    //db.SaveChanges();
                    Flash.Instance.Success("CORRECTO", "El dato ha sido Modificado correctamente.");
                    return RedirectToAction("Index");
                }
                ViewBag.iPermiso_id = new SelectList(db.Permiso, "iPermiso_id", "Descripcion", permisoDenegadoPorRol.iPermiso_id);
                ViewBag.iRol_id = new SelectList(db.Rol, "iRol_id", "Nombre", permisoDenegadoPorRol.iRol_id);
                Flash.Instance.Error("ERROR", "No se pudo Modificar el dato porque ha ocurrido un error al validar el modelo, por favor verifique que los campos estén correctamente llenados.");
                return View(permisoDenegadoPorRol);
            }
            catch (Exception ex)
            {
                Flash.Instance.Error("ERROR", "No se pudo Modificar el dato proque ha ocurrido el siguiente error: " + ex.Message);
                return RedirectToAction("Index");
            }
        }

        [Permiso(Permiso = RolesPermisos.SEGU_permisoDenegadoPorRol_puedeEliminar)]
        public ActionResult Delete(int Rol_id = 0, int Permiso_id = 0)
        {
            if (Rol_id == 0 && Permiso_id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "LOS PARAMETROS NO SON CORRECTOS");
            }
            RolesPermisos permiso = (RolesPermisos)Enum.ToObject(typeof(RolesPermisos), Permiso_id);//convierte a un enum en base a su ID
            PermisoDenegadoPorRol permisoDenegadoPorRol = db.PermisoDenegadoPorRol.Find(Rol_id, permiso);
            if (permisoDenegadoPorRol == null)
            {
                return HttpNotFound();
            }
            return View(permisoDenegadoPorRol);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int Rol_id = 0, int Permiso_id = 0)
        {
            using (var transaccion = db.Database.BeginTransaction())
            {
                try
                {
                    RolesPermisos permiso = (RolesPermisos)Enum.ToObject(typeof(RolesPermisos), Permiso_id);//convierte a un enum en base a su ID
                    PermisoDenegadoPorRol permisoDenegadoPorRol = db.PermisoDenegadoPorRol.Find(Rol_id, permiso);
                    permisoDenegadoPorRol.iEstado_fl = false;
                    permisoDenegadoPorRol.iEliminado_fl = 2;
                    permisoDenegadoPorRol.sCreado_by = FrontUser.Get().usr_login;
                    permisoDenegadoPorRol.iConcurrencia_id += 1;

                    db.Entry(permisoDenegadoPorRol).State = EntityState.Modified;
                    db.SaveChanges();

                    db.PermisoDenegadoPorRol.Remove(permisoDenegadoPorRol);
                    db.SaveChanges();
                    Flash.Instance.Success("CORRECTO", "El dato ha sido eliminado correctamente.");
                    transaccion.Commit();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Flash.Instance.Error("ERROR", "No se pudo eliminar el dato porque ha ocurrido el siguiente error: " + ex.Message);
                    transaccion.Rollback();
                    return RedirectToAction("Index");
                }
            }
        }

        private List<Permiso> filtrarPermisoDenegadoPorRol(int rol_id)
        {
            List<Permiso> resultado = new List<Permiso>();
            List<Permiso> permisosTodos = db.Permiso.ToList();
            if (rol_id > 0)
            {
                var pdrs = db.PermisoDenegadoPorRol.Where(pd => pd.iRol_id == rol_id).ToList();
                if (pdrs.Count > 0)
                {
                    resultado = (from perm in permisosTodos
                                 where !pdrs.Any(m => m.iPermiso_id == perm.iPermiso_id)
                                 select perm).ToList();
                }
                else {
                    resultado = (from perm in permisosTodos
                                 select perm
                                                 ).ToList();
                }
            }
            return resultado;
        }

        [HttpPost]
        public JsonResult PermisosPorRol(int rol_id)
        {
            var permisos = filtrarPermisoDenegadoPorRol(rol_id).Select(a => new { iPermiso_id = a.iPermiso_id.ToString(), Descripcion = a.Descripcion }).ToList();
            return Json(permisos, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InsertarEnBloquePorModulo(int idRol, string nemonicoModulo)
        {
            List<Permiso> permisos = db.Permiso.Where(p => p.Nemonico.ToUpper().Trim().Equals(nemonicoModulo.ToUpper().Trim())).ToList();
            List<PermisoDenegadoPorRol> pdrs = new List<PermisoDenegadoPorRol>();
            foreach (var permiso in permisos)
            {
                PermisoDenegadoPorRol pdr = new PermisoDenegadoPorRol();
                pdr.iConcurrencia_id = 1;
                pdr.iEliminado_fl = 1;
                pdr.iEstado_fl = true;
                pdr.iPermiso_id = permiso.iPermiso_id;
                pdr.iRol_id = idRol;
                pdr.sCreado_by = FrontUser.Get().usr_login;
                if (ModelState.IsValid)
                {
                    db.PermisoDenegadoPorRol.Add(pdr);
                    db.SaveChanges();
                    ModelState.Clear();
                    pdrs.Add(pdr);
                }

                Flash.Instance.Success("CORRECTO", string.Format("{0} {1} {2}", "La denegacion a los metodos de accion del Modulo ", nemonicoModulo.ToUpper(), "se estableció correctamente."));
            }
            return Json(pdrs, JsonRequestBehavior.AllowGet);
        }
        public JsonResult InsertarEnBloquePorProceso(int idRol, string nombreProceso)
        {
            List<Permiso> permisos = db.Permiso.Where(p => p.Proceso.ToLower().Equals(nombreProceso.ToLower())).ToList();
            List<PermisoDenegadoPorRol> pdrs = new List<PermisoDenegadoPorRol>();
            foreach (var permiso in permisos)
            {
                PermisoDenegadoPorRol pdr = new PermisoDenegadoPorRol();
                pdr.iConcurrencia_id = 1;
                pdr.iEliminado_fl = 1;
                pdr.iEstado_fl = true;
                pdr.iPermiso_id = permiso.iPermiso_id;
                pdr.iRol_id = idRol;
                pdr.sCreado_by = FrontUser.Get().usr_login;
                if (ModelState.IsValid)
                {

                    db.PermisoDenegadoPorRol.Add(pdr);
                    db.SaveChanges();
                    ModelState.Clear();
                    pdrs.Add(pdr);
                }

                Flash.Instance.Success("CORRECTO", string.Format("{0} {1} {2}", "La denegacion a los metodos de accion del proceso ", nombreProceso.ToUpper(), "se estableció correctamente."));
            }
            return Json(pdrs, JsonRequestBehavior.AllowGet);
        }
        public JsonResult InsertarUnMetodoDeAccion(int idRol, int permisoId)
        {
            PermisoDenegadoPorRol pdr = new PermisoDenegadoPorRol();
            try
            {
                pdr.iConcurrencia_id = 1;
                pdr.iEliminado_fl = 1;
                pdr.iEstado_fl = true;
                pdr.iPermiso_id = (RolesPermisos)Enum.ToObject(typeof(RolesPermisos), permisoId);
                pdr.iRol_id = idRol;
                pdr.sCreado_by = FrontUser.Get().usr_login;
                if (ModelState.IsValid)
                {
                    db.PermisoDenegadoPorRol.Add(pdr);
                    db.SaveChanges();
                    ModelState.Clear();
                    Flash.Instance.Success("CORRECTO", "La denegacion al metodo de accion es correcto.");
                }
            }
            catch (Exception ex)
            {
                Flash.Instance.Error("ERROR", "No se pudo Guardar el dato porque ha ocurrido el siguiente error " + ex.Message);
                //return RedirectToAction("Index");
            }
            return Json(pdr, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EliminarEnBloquePorModulo(int idRol, string nemonicoModulo)
        {
            List<Permiso> permisos = db.Permiso.Where(p => p.Nemonico.ToUpper().Trim().Equals(nemonicoModulo.ToUpper().Trim())).ToList();
            List<PermisoDenegadoPorRol> pdrs = new List<PermisoDenegadoPorRol>();
            foreach (var permiso in permisos)
            {
                PermisoDenegadoPorRol pdr = new PermisoDenegadoPorRol();
                pdr.iConcurrencia_id = 1;
                pdr.iEliminado_fl = 1;
                pdr.iEstado_fl = true;
                pdr.iPermiso_id = permiso.iPermiso_id;
                pdr.iRol_id = idRol;
                pdr.sCreado_by = FrontUser.Get().usr_login;

                db.PermisoDenegadoPorRol.Remove(pdr);
                db.SaveChanges();
                pdrs.Add(pdr);

                Flash.Instance.Success("CORRECTO", string.Format("{0} {1} {2}", "La habilitación a los metodos de accion del Modulo ", nemonicoModulo.ToUpper(), "se estableció correctamente."));
            }
            return Json(pdrs, JsonRequestBehavior.AllowGet);
        }
        public JsonResult EliminarEnBloquePorProceso(int idRol, string nombreProceso)
        {
            List<Permiso> permisos = db.Permiso.Where(p => p.Proceso.ToLower().Equals(nombreProceso.ToLower())).ToList();
            List<PermisoDenegadoPorRol> pdrs = new List<PermisoDenegadoPorRol>();
            foreach (var permiso in permisos)
            {
                PermisoDenegadoPorRol pdr = new PermisoDenegadoPorRol();
                pdr.iConcurrencia_id = 1;
                pdr.iEliminado_fl = 1;
                pdr.iEstado_fl = true;
                pdr.iPermiso_id = permiso.iPermiso_id;
                pdr.iRol_id = idRol;
                pdr.sCreado_by = FrontUser.Get().usr_login;
                if (ModelState.IsValid)
                {
                    db.PermisoDenegadoPorRol.Remove(pdr);
                    db.SaveChanges();
                    pdrs.Add(pdr);
                }

                Flash.Instance.Success("CORRECTO", string.Format("{0} {1} {2}", "La habilitación a los metodos de accion del proceso ", nombreProceso.ToUpper(), "se estableció correctamente."));
            }
            return Json(pdrs, JsonRequestBehavior.AllowGet);
        }
        public JsonResult EliminarUno_PDR_PorIdRonEidPermiso(int idRol, int permisoId)
        {
            PermisoDenegadoPorRol pdr = new PermisoDenegadoPorRol();
            try
            {
                pdr.iConcurrencia_id = 1;
                pdr.iEliminado_fl = 1;
                pdr.iEstado_fl = true;
                pdr.iPermiso_id = (RolesPermisos)Enum.ToObject(typeof(RolesPermisos), permisoId);
                pdr.iRol_id = idRol;
                pdr.sCreado_by = FrontUser.Get().usr_login;
                if (ModelState.IsValid)
                {
                    db.PermisoDenegadoPorRol.Remove(pdr);
                    db.SaveChanges();
                    Flash.Instance.Success("CORRECTO", "La habilitación al metodo de accion es correcto.");
                }
            }
            catch (Exception ex)
            {
                Flash.Instance.Error("ERROR", "No se pudo Guardar el dato porque ha ocurrido el siguiente error " + ex.Message);
                //return RedirectToAction("Index");
            }
            return Json(pdr, JsonRequestBehavior.AllowGet);
        }

        private List<Permiso> TraerVarios_Permisos_PorModulo(string nemonico)
        {
            var permisos = new List<Permiso>();
            permisos = db.Permiso.Where(e => e.iEliminado_fl == 1).Where(p => p.Nemonico.Trim().ToUpper().Equals(nemonico.Trim().ToUpper())).ToList();
            return permisos;
        }
        private List<Permiso> TraerVarios_Permisos_PorProceso(string nombreProceso)
        {
            var permisos = new List<Permiso>();
            permisos = db.Permiso.Where(e => e.iEliminado_fl == 1).Where(p => p.Proceso.Trim().ToUpper().Equals(nombreProceso.Trim().ToUpper())).ToList();
            return permisos;
        }
        private List<Permiso> TraerVarios_Permisos_PorModuloYproceso(string nemonico, string nombreProceso)
        {
            var permisosPorProcYnem = new List<Permiso>();
            var perisosPorNemonico = TraerVarios_Permisos_PorModulo(nemonico);
            permisosPorProcYnem = perisosPorNemonico.Where(p => p.Proceso.Trim().ToUpper().Equals(nombreProceso.Trim().ToUpper())).ToList();

            return permisosPorProcYnem;
        }

        private List<PermisoDenegadoPorRol> TraerVarios_PDR_porModulo(string nemonico, int idRol)
        {
            var pdr = new List<PermisoDenegadoPorRol>();
            pdr = db.PermisoDenegadoPorRol.Where(e => e.iEliminado_fl == 1).Where(r => r.iRol_id == idRol).Where(p => p.Permiso.Nemonico.Trim().ToUpper().Equals(nemonico.Trim().ToUpper())).ToList();
            return pdr;
        }
        private List<PermisoDenegadoPorRol> TraerVarios_PDR_porProceso(string nombreProceso)
        {
            var pdr = new List<PermisoDenegadoPorRol>();
            pdr = db.PermisoDenegadoPorRol.Where(e => e.iEliminado_fl == 1).Where(p => p.Permiso.Proceso.Trim().ToUpper().Equals(nombreProceso.Trim().ToUpper())).ToList();
            return pdr;
        }
        private List<PermisoDenegadoPorRol> TraerVarios_PDR_porModuloYproceso(string nemonico, string nombreProceso, int idRol)
        {
            var pdrPorNemYproc = new List<PermisoDenegadoPorRol>();
            var pdrPorNemonico = TraerVarios_PDR_porModulo(nemonico, idRol);
            pdrPorNemYproc = pdrPorNemonico.Where(p => p.Permiso.Proceso.Trim().ToUpper().Equals(nombreProceso.Trim().ToUpper())).ToList();
            return pdrPorNemYproc;
        }
        private PermisoDenegadoPorRol TraerUno_PDR_porIdRolEidPermiso(int idRol, RolesPermisos idPermiso)
        {
            var pdr = new PermisoDenegadoPorRol();
            pdr = db.PermisoDenegadoPorRol.Where(r => r.iRol_id == idRol).Where(p => p.iPermiso_id == idPermiso).FirstOrDefault();
            return pdr;
        }


        private List<string> TraerVarios_NombreNemonicosDeModulos()
        {
            List<string> modulos = new List<string>();
            var result = db.Permiso.GroupBy(test => test.Nemonico)
                   .Select(grp => grp.First())
                   .ToList();
            modulos = result.Select(se => se.Nemonico).ToList();

            //var nemonicosUnicos = (from dbo in db.Permiso
            //                    //where dbo.Property == true
            //                    select dbo.Nemonico).Distinct().OrderBy(name => name);
            return modulos;
        }
        private List<string> TraerVarios_NombreProcesos_PorModulo(string nemonicoModulo)
        {
            List<string> modulos = new List<string>();
            //var result = db.Permiso.GroupBy(test => test.Proceso)
            //       .Select(grp => grp.First())
            //       .Where(e => e.Nemonico.Equals(nemonicoModulo))
            //       .ToList();
            //modulos = result.Select(se => se.Proceso).ToList();

            modulos = (from perm in db.Permiso
                       where perm.Nemonico.Equals(nemonicoModulo)
                       select perm.Proceso).Distinct().OrderBy(name => name).ToList();
            return modulos;


        }
        private List<string> TraerVarios_NombreAcciones_PorProcesoYmodulo(string nombreProceso, string nemonico)
        {
            List<string> acciones = new List<string>();
            //var result = db.Permiso.GroupBy(test => test.Proceso)
            //       .Select(grp => grp.First())
            //       .Where(e => e.Nemonico.Equals(nemonicoModulo))
            //       .ToList();
            //modulos = result.Select(se => se.Proceso).ToList();

            acciones = (from perm in db.Permiso
                        where perm.Proceso.Equals(nombreProceso) && perm.Nemonico.Equals(nemonico)
                        select perm.Descripcion).Distinct().OrderBy(name => name).ToList();
            return acciones;


        }
        public ActionResult TreerVarios_Acciones_PorProcesoYnemonico_json(int idRol, string nemonico, string nombreProceso)
        {
            var res = TreerVarios_Acciones_PorProcesoYnemonico(idRol, nemonico, nombreProceso);
            if (Request.IsAjaxRequest())
            {
                return PartialView("_IndexPanelAcciones", res);
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        private List<ProcesoViewModel> TreerVarios_PrcesosVM_PorProcesoYnemonico(string nemonico, int rolid)
        {
            //  modulo CRM
            var procesosCR = new List<ProcesoViewModel>();
            var permisosCr = TraerVarios_Permisos_PorModulo(nemonico);
            var pdrsCr = TraerVarios_PDR_porModulo(nemonico, rolid);//se utilizara para ver si chequeamos el select all.
            var nombresProcesosCr = TraerVarios_NombreProcesos_PorModulo(nemonico);
            foreach (var nombreProceso in nombresProcesosCr)
            {
                var procesos = TraerVarios_Permisos_PorModuloYproceso(nemonico, nombreProceso);
                var pdr = TraerVarios_PDR_porModuloYproceso(nemonico, nombreProceso, rolid);

                ProcesoViewModel procesoVM = new ProcesoViewModel();
                procesoVM.sNombre = nombreProceso;
                procesoVM.bEstadoSeleccionado = pdr.Count < procesos.Count ? false : true;

                procesosCR.Add(procesoVM);
            }
            return procesosCR;
            //var res = TreerVarios_Acciones_PorProcesoYnemonico(idRol, nemonico, nombreProceso);
            //if (Request.IsAjaxRequest())
            //{
            //    return PartialView("_IndexPanelAcciones", res);
            //}

            //return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index_json(int iRol_id)
        {
            Rol primerRol = db.Rol.Where(r =>r.iRol_id == iRol_id).FirstOrDefault();
            PermisoDenegadoPorRolViewModel pdprvw = new PermisoDenegadoPorRolViewModel();
            var permisosS = TraerVarios_Permisos_PorModulo("SEGU");
            var primerProcesoS = permisosS.FirstOrDefault();

            var acciones = new List<MetodoDeAccionViewModel>();
            acciones = TreerVarios_Acciones_PorProcesoYnemonico(primerRol.iRol_id, "SEGU", primerProcesoS.Proceso);

            pdprvw.iRol_id = primerRol.iRol_id;
            pdprvw.MetodosDeAccionDeProcesoSeleccionado = acciones;
            pdprvw.ProcesosSEGU = TreerVarios_PrcesosVM_PorProcesoYnemonico("SEGU", primerRol.iRol_id);
            pdprvw.ProcesosEGRE = TreerVarios_PrcesosVM_PorProcesoYnemonico("EGRE", primerRol.iRol_id);
            pdprvw.ProcesosCONV = TreerVarios_PrcesosVM_PorProcesoYnemonico("CONV", primerRol.iRol_id);
            pdprvw.ProcesosCRM = TreerVarios_PrcesosVM_PorProcesoYnemonico("CRM", primerRol.iRol_id);
            pdprvw.ProcesosOYM = TreerVarios_PrcesosVM_PorProcesoYnemonico("OYM", primerRol.iRol_id);

            if (Request.IsAjaxRequest())
            {
                return PartialView("_IndexPaneles", pdprvw);
            }
            return Json(pdprvw, JsonRequestBehavior.AllowGet);
        }
        private List<MetodoDeAccionViewModel> TreerVarios_Acciones_PorProcesoYnemonico(int idRol, string nemonico, string nombreProceso)
        {
            var acciones = new List<MetodoDeAccionViewModel>();
            var permisos = TraerVarios_Permisos_PorModuloYproceso(nemonico, nombreProceso);
            foreach (var permiso in permisos)
            {
                MetodoDeAccionViewModel metodoAccion = new MetodoDeAccionViewModel();
                metodoAccion.iRol_id = idRol;
                metodoAccion.iPermiso_id = (int)permiso.iPermiso_id;
                metodoAccion.sNombreAccion = permiso.Descripcion;
                var pdr = TraerUno_PDR_porIdRolEidPermiso(idRol, permiso.iPermiso_id);
                metodoAccion.bEstadoSeleccionado = pdr == null ? false : true;

                acciones.Add(metodoAccion);
            }
            return acciones;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
