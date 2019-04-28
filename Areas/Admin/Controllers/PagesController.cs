using CmsShop.Models.Data;
using CmsShop.Models.ViewModels.Pages;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CmsShop.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            // Declaration of PageVM list
            List<PageVM> pagesList;


            using (Db db = new Db())
            {
                //List initial
                pagesList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();
            }

            //Return pages to view 
            return View(pagesList);
        }

        // GET: Admin/Pages/AddPage
        [HttpGet]
        public ActionResult AddPage()
        {
            return View();
        }

        // POST: Admin/Pages/AddPage
        [HttpPost]
        public ActionResult AddPage(PageVM model)
        {
            //Check state model
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                string slug;

                //PageDTO initialization
                PageDTO dto = new PageDTO();

                //If we dont have page address, we assign title
                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }

                //prevent to add the same name of page
                if (db.Pages.Any(x => x.Title == model.Title) || db.Pages.Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "Ten tytuł, lub adres strony już istnieje!");

                    return View(model);
                }

                dto.Title = model.Title;
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;
                dto.Sorting = 1000;

                //DTO save
                db.Pages.Add(dto);
                db.SaveChanges();
            }

            TempData["SM"] = "Dodałeś nową stronę";

            return RedirectToAction("AddPage"); 
        }

        // GET: Admin/Pages/EditPage
        [HttpGet]
        public ActionResult EditPage(int id)
        {
            // Page View Model declaration
            PageVM model;

            using (Db db = new Db())
            {
                //get page from db 
                PageDTO dto = db.Pages.Find(id);

                //check if page with exist
                if (dto == null)
                {
                    return Content("Strona nie istnieje");
                }

                model = new PageVM(dto); 
            }

            return View(model); 
        }

        // POST: Admin/Pages/EditPage
        [HttpPost]
        public ActionResult EditPage(PageVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                // pobranie Id strony
                int id = model.Id;

                // inicjalizacja slug
                string slug = "home";

                // pobranie strony do edycji
                PageDTO dto = db.Pages.Find(id);

                if (model.Slug != "home")
                {
                    if (string.IsNullOrWhiteSpace(model.Slug))
                    {
                        slug = model.Title.Replace(" ", "-").ToLower();
                    }
                    else
                    {
                        slug = model.Slug.Replace(" ", "-").ToLower();
                    }
                }

                // sprawdzamy unikalnosc strony, adresu
                if (db.Pages.Where(x => x.Id != id).Any(x => x.Title == model.Title) ||
                    db.Pages.Where(x => x.Id != id).Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "Strona lub adres strony już istnieje");
                }

                // modyfikacje DTO
                dto.Title = model.Title;
                dto.Slug = slug;
                dto.HasSidebar = model.HasSidebar;
                dto.Body = model.Body;

                // zapis sytowanej strony do bazy
                db.SaveChanges();
            }

            // ustawienie komunikatu 
            TempData["SM"] = "Wyedytowałeś stronę";

            //Redirect
            return RedirectToAction("EditPage");
        }

        // GET: Admin/Pages/Details/id
        [HttpGet]
        public ActionResult Details(int id)
        {
            //PageVM declaration
            PageVM model;

            using (Db db = new Db())
            {
                //Get page with id 
                PageDTO dto = db.Pages.Find(id);

                //Check if page with id exist
                if (dto == null)
                {
                    return Content("Strona o podanym id nie istnieje."); 
                }

                //PageVM initialization
                model = new PageVM(dto); 



            }

            return View(model); 
        }

        // GET: Admin/Pages/DeletePage/id
        [HttpGet]
        public ActionResult Delete(int id)
        {
            using (Db db = new Db())
            {
                //Get page for delete
                PageDTO dto = db.Pages.Find(id);

                //Delete page from DB
                db.Pages.Remove(dto);

                //Save
                db.SaveChanges(); 
            }

            return RedirectToAction("Index"); 
        }

        // POST: Admin/Pages/ReorderPages
        [HttpPost]
        public ActionResult ReorderPages(int[] id)
        {
            using (Db db = new Db())
            {
                int count = 1;
                PageDTO dto;

                //Page sorting and save in db

                foreach (var pageId in id)
                {
                    dto = db.Pages.Find(pageId);
                    dto.Sorting = count;

                    db.SaveChanges();

                    count++; 
                }
            }

            return View(); 
        }

        // GET: Admin/Pages/EditSidebar
        [HttpGet]
        public ActionResult EditSidebar()
        {
            // Declaration SidebarVM

            SidebarVM model;

            using (Db db = new Db())
            {
                //Get SidebarDTO

                SidebarDTO dto = db.Sidebar.Find(1);

                //Model initialization 

                model = new SidebarVM(dto);
            }


            return View(model); 
        }

        // POST: Admin/Pages/EditSidebar
        [HttpPost]
        public ActionResult EditSidebar(SidebarVM model)
        {
            using (Db db = new Db())
            {
                //Get SidebarDTO
                SidebarDTO dto = db.Sidebar.Find(1);

                //Sidebar modify
                dto.Body = model.Body;

                //Save 
                db.SaveChanges(); 


            }

            //Comunicate inform about sidebar modify
            TempData["SM"] = "Zmodyfikowałeś pasek boczny";
            

            //Redirect
            return RedirectToAction("EditSidebar");
        }
    }
}