using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebStoryService.Areas.MyApi.Controllers
{
    [RoutePrefix("story")]
    public class ChapterApiController : Controller
    {
        [Route("chapter")]
        [HttpGet]
        // GET: MyApi/ChapterApi
        public ActionResult GetChapter()
        {
            return View();
        }

        // GET: MyApi/ChapterApi/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: MyApi/ChapterApi/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MyApi/ChapterApi/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: MyApi/ChapterApi/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: MyApi/ChapterApi/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: MyApi/ChapterApi/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: MyApi/ChapterApi/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
