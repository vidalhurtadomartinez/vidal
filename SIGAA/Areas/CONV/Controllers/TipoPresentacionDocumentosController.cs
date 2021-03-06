﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SIGAA.Areas.CONV.Models;
using SIGAA.Etiquetas;
using SIGAA.Commons;

namespace SIGAA.Areas.CONV.Controllers
{
    [Autenticado]
    public class TipoPresentacionDocumentosController : Controller
    {
        private ConvalidacionesContext db = new ConvalidacionesContext();

        [Permiso(Permiso = RolesPermisos.CONV_TipoPresentacionDocumento_VerListado)]
        public ActionResult Index()
        {
            return View(db.TipoPresentacionDocumentos.ToList());
        }

        [Permiso(Permiso = RolesPermisos.CONV_TipoPresentacionDocumento_VerDetalle)]
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoPresentacionDocumento tipoPresentacionDocumento = db.TipoPresentacionDocumentos.Find(id);
            if (tipoPresentacionDocumento == null)
            {
                return HttpNotFound();
            }
            return View(tipoPresentacionDocumento);
        }

        [Permiso(Permiso = RolesPermisos.CONV_TipoPresentacionDocumento_CrearNuevo)]
        public ActionResult Create()
        {
            return View();
        }

        // POST: TipoPresentacionDocumentos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "sTipoPresentacion_fl,sDescripcion,bActivo")] TipoPresentacionDocumento tipoPresentacionDocumento)
        {
            if (ModelState.IsValid)
            {
                db.TipoPresentacionDocumentos.Add(tipoPresentacionDocumento);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tipoPresentacionDocumento);
        }

        [Permiso(Permiso = RolesPermisos.CONV_TipoPresentacionDocumento_Editar)]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoPresentacionDocumento tipoPresentacionDocumento = db.TipoPresentacionDocumentos.Find(id);
            if (tipoPresentacionDocumento == null)
            {
                return HttpNotFound();
            }
            return View(tipoPresentacionDocumento);
        }

        // POST: TipoPresentacionDocumentos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "sTipoPresentacion_fl,sDescripcion,bActivo")] TipoPresentacionDocumento tipoPresentacionDocumento)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tipoPresentacionDocumento).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tipoPresentacionDocumento);
        }

        [Permiso(Permiso = RolesPermisos.CONV_TipoPresentacionDocumento_Eliminar)]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoPresentacionDocumento tipoPresentacionDocumento = db.TipoPresentacionDocumentos.Find(id);
            if (tipoPresentacionDocumento == null)
            {
                return HttpNotFound();
            }
            return View(tipoPresentacionDocumento);
        }

        // POST: TipoPresentacionDocumentos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TipoPresentacionDocumento tipoPresentacionDocumento = db.TipoPresentacionDocumentos.Find(id);
            db.TipoPresentacionDocumentos.Remove(tipoPresentacionDocumento);
            db.SaveChanges();
            return RedirectToAction("Index");
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
