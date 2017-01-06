﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SIGAA.Areas.CONV.Models;
namespace SIGAA.Areas.CONV.Controllers
{
    public class TipoCargaHorariasController : Controller
    {
        private ConvalidacionesContext db = new ConvalidacionesContext();

        // GET: TipoCargaHorarias
        public ActionResult Index()
        {
            return View(db.TipoCargaHorarias.ToList());
        }

        // GET: TipoCargaHorarias/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoCargaHoraria tipoCargaHoraria = db.TipoCargaHorarias.Find(id);
            if (tipoCargaHoraria == null)
            {
                return HttpNotFound();
            }
            return View(tipoCargaHoraria);
        }

        // GET: TipoCargaHorarias/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TipoCargaHorarias/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "lTipoCargaHoraria_fl,sDescripcion,bActivo")] TipoCargaHoraria tipoCargaHoraria)
        {
            if (ModelState.IsValid)
            {
                db.TipoCargaHorarias.Add(tipoCargaHoraria);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tipoCargaHoraria);
        }

        // GET: TipoCargaHorarias/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoCargaHoraria tipoCargaHoraria = db.TipoCargaHorarias.Find(id);
            if (tipoCargaHoraria == null)
            {
                return HttpNotFound();
            }
            return View(tipoCargaHoraria);
        }

        // POST: TipoCargaHorarias/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "lTipoCargaHoraria_fl,sDescripcion,bActivo")] TipoCargaHoraria tipoCargaHoraria)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tipoCargaHoraria).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tipoCargaHoraria);
        }

        // GET: TipoCargaHorarias/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoCargaHoraria tipoCargaHoraria = db.TipoCargaHorarias.Find(id);
            if (tipoCargaHoraria == null)
            {
                return HttpNotFound();
            }
            return View(tipoCargaHoraria);
        }

        // POST: TipoCargaHorarias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TipoCargaHoraria tipoCargaHoraria = db.TipoCargaHorarias.Find(id);
            db.TipoCargaHorarias.Remove(tipoCargaHoraria);
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