using GymSystemBLL.Services.Classes;
using GymSystemBLL.Services.Interfaces;
using GymSystemBLL.ViewModels.SessionViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymSystemPL.Controllers
{
    public class SessionController : Controller
    {
        private readonly ISessionService _sessionService;

        public SessionController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        #region Get All Sessions

        public IActionResult Index()
        {
            var Sessions = _sessionService.GetAllSessions();
            return View(Sessions);
        }

        #endregion

        #region Get Session Details

        public ActionResult Details(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Invalid Session Id";
                return RedirectToAction(nameof(Index));
            }

            var Session = _sessionService.GetSessionById(id);
            if (Session == null)
            {
                TempData["ErrorMessage"] = "Session Not Found";
                return RedirectToAction(nameof(Index));
            }

            return View(Session);
        }

        #endregion

        #region Create Sessions

        public ActionResult Create()
        {
            LoadDropDownsForTrainer();
            LoadDropDownsForCategory();

            return View();
        }

        [HttpPost]
        public ActionResult Create(CreateSessionViewModel CreatedSession)
        {
            if (!ModelState.IsValid)
            {
                LoadDropDownsForTrainer();
                LoadDropDownsForCategory();

                return View(CreatedSession);
            }

            var Result = _sessionService.CreateSession(CreatedSession);
            if (!Result)
            {
                LoadDropDownsForTrainer();
                LoadDropDownsForCategory();

                TempData["ErrorMessage"] = "Failed to Create Session";
                return View(CreatedSession);
            }
            else
            {
                TempData["SuccessMessage"] = "Session Created Successfully";
                return RedirectToAction(nameof(Index));
            }
        }

        #endregion

        #region Edit Session

        public ActionResult Edit(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Invalid Session Id";
                return RedirectToAction(nameof(Index));
            }

            var Session = _sessionService.GetSessionToUpdate(id);
            if (Session is null)
            {
                TempData["ErrorMessage"] = "Session Not Found";
                return RedirectToAction(nameof(Index));
            }

            LoadDropDownsForTrainer();

            return View(Session);
        }

        [HttpPost]
        public ActionResult Edit([FromRoute]int id, UpdateSessionViewModel UpdatedSession)
        {
            if (!ModelState.IsValid)
            {
                LoadDropDownsForTrainer();
                return View(UpdatedSession);
            }

            var Result = _sessionService.UpdateSession(UpdatedSession, id);
            if (Result)
            {
                TempData["SuccessMessage"] = "Session Updated Successfully";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to Update Session";
                LoadDropDownsForTrainer();

                return View(UpdatedSession);
            }
        }

        #endregion

        #region Delete Session

        public ActionResult Delete(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Invalid Session Id";
                return RedirectToAction(nameof(Index));
            }

            var Session = _sessionService.GetSessionById(id);
            if (Session is null)
            {
                TempData["ErrorMessage"] = "Session Not Found";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.SessionId = id;
            return View(Session);
        }

        [HttpPost]
        public ActionResult DeleteConfirmed(int id)
        {
            var Result = _sessionService.RemoveSession(id);
            if(Result)
                TempData["SuccessMessage"] = "Session Deleted Successfully";
            else
                TempData["ErrorMessage"] = "Failed to Delete Session";

            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Helper Methods

        private void LoadDropDownsForTrainer()
        {
            var Trainers = _sessionService.GetTrainerForSessions();
            ViewBag.Trainers = new SelectList(Trainers, "Id", "Name");
        }

        private void LoadDropDownsForCategory()
        {
            var Categories = _sessionService.GetCategoryForSessions();
            ViewBag.Categories = new SelectList(Categories, "Id", "Name");
        }

        #endregion
    }
}
