using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GymSystemBLL.Services.Interfaces;
using GymSystemBLL.ViewModels.SessionViewModels;
using GymSystemDAL.Entities;
using GymSystemDAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;

namespace GymSystemBLL.Services.Classes
{
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SessionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public bool CreateSession(CreateSessionViewModel createdSession)
        {
            try
            {
                // Check if trainer exist
                // Check if Category exist
                // Check StartDate Before EndDate

                if (!IsTrainerExist(createdSession.TrainerId)) return false;
                if (!IsCategoryExist(createdSession.CategoryId)) return false;
                if (!IsDateTimeValid(createdSession.StartDate, createdSession.EndDate)) return false;
                if (createdSession.Capacity > 25 || createdSession.Capacity < 0) return false;

                //var SessionEntity = _mapper.Map<CreateSessionViewModel, Session>(createdSession);
                var SessionEntity = _mapper.Map<Session>(createdSession);

                _unitOfWork.GetRepository<Session>().Add(SessionEntity);

                return _unitOfWork.SaveChanges() > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public IEnumerable<SessionViewModel> GetAllSessions()
        {
            //var Sessions = _unitOfWork.GetRepository<Session>().GetAll();
            var Sessions = _unitOfWork.SessionRepository.GetAllSessionsWithTrainerAndCategory();

            if (!Sessions.Any()) return [];

            return Sessions.Select(X => new SessionViewModel
            {
                Id = X.Id,
                Description = X.Description,
                StartDate = X.StartDate,
                EndDate = X.EndDate,
                Capacity = X.Capacity,
                TrainerName = X.SessionTrainer.Name,            // Related Data
                CategoryName = X.SessionCategory.CategoryName,  // Related Data
                AvailableSlot = X.Capacity - _unitOfWork.SessionRepository.GetCountOfBookedSlot(X.Id)
            });
        }

        public SessionViewModel? GetSessionById(int sessionId)
        {
            var Session = _unitOfWork.SessionRepository.GetSessionWithTrainerAndCategory(sessionId);

            if(Session == null) return null;

            ///return new SessionViewModel
            ///{
            ///    Id = Session.Id,
            ///    Description = Session.Description,
            ///    StartDate = Session.StartDate,
            ///    EndDate = Session.EndDate,
            ///    Capacity = Session.Capacity,
            ///    TrainerName = Session.SessionTrainer.Name,            // Related Data
            ///    CategoryName = Session.SessionCategory.CategoryName,  // Related Data
            ///    AvailableSlot = Session.Capacity - _unitOfWork.SessionRepository.GetCountOfBookedSlot(Session.Id)
            ///};

            // Auto Mapper
            var MappedSession = _mapper.Map<Session, SessionViewModel>(Session);

            MappedSession.AvailableSlot = MappedSession.Capacity - _unitOfWork.SessionRepository.GetCountOfBookedSlot(MappedSession.Id);

            return MappedSession;
        }

        public UpdateSessionViewModel? GetSessionToUpdate(int sessionId)
        {
            var Session = _unitOfWork.SessionRepository.GetById(sessionId);

            if (!IsSessionAvailableToUpdate(Session!)) return null;

            return _mapper.Map<UpdateSessionViewModel>(Session);
        }

        public bool UpdateSession(UpdateSessionViewModel updatedSession, int sessionId)
        {
            try
            {
                var Session = _unitOfWork.SessionRepository.GetById(sessionId);

                if (!IsSessionAvailableToUpdate(Session!)) return false;
                if (!IsTrainerExist(updatedSession.TrainerId)) return false;
                if (!IsDateTimeValid(updatedSession.StartDate, updatedSession.EndDate)) return false;

                _mapper.Map(updatedSession, Session);

                Session!.UpdatedAt = DateTime.Now;
                _unitOfWork.SessionRepository.Update(Session);

                return _unitOfWork.SaveChanges() > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool RemoveSession(int sessionId)
        {
            try
            {
                var Session = _unitOfWork.SessionRepository.GetById(sessionId);

                if (!IsSessionAvailableForDelete(Session!)) return false;

                _unitOfWork.SessionRepository.Delete(Session!);

                return _unitOfWork.SaveChanges() > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public IEnumerable<TrainerSelectViewModel> GetTrainerForSessions()
        {
            var Trainers = _unitOfWork.GetRepository<Trainer>().GetAll();
            return _mapper.Map<IEnumerable<TrainerSelectViewModel>>(Trainers);
        }

        public IEnumerable<CategorySelectViewModel> GetCategoryForSessions()
        {
            var Categories = _unitOfWork.GetRepository<Category>().GetAll();
            return _mapper.Map<IEnumerable<CategorySelectViewModel>>(Categories);
        }

        #region Helper Methods

        private bool IsTrainerExist(int TrainerId)
        {
            return _unitOfWork.GetRepository<Trainer>().GetById(TrainerId) is not null;
        }

        private bool IsCategoryExist(int CategoryId)
        {
            return _unitOfWork.GetRepository<Category>().GetById(CategoryId) is not null;
        }

        private bool IsDateTimeValid(DateTime StartDate, DateTime EndDate)
        {
            return StartDate < EndDate;
        }

        private bool IsSessionAvailableToUpdate(Session session)
        {
            if (session is null) return false;

            // If Session Completed => Cannot Update
            if (session.EndDate < DateTime.Now) return false;
            // If Session Started => Cannot Update
            if (session.StartDate <= DateTime.Now) return false;
            // If Session Has Active Booking => Cannot Update
            var ActiveBooking = _unitOfWork.SessionRepository.GetCountOfBookedSlot(session.Id) > 0;
            if (ActiveBooking) return false;

            return true;
        }

        private bool IsSessionAvailableForDelete(Session session)
        {
            if (session is null) return false;

            //// If Session Completed => Cannot Delete
            //if (session.EndDate < DateTime.Now) return false;
            // If Session Upcoming => Cannot Delete
            if (session.StartDate > DateTime.Now) return false;
            // If Session Ongoing => Cannot Delete
            if (session.StartDate <= DateTime.Now && session.EndDate > DateTime.Now) return false;
            // If Session Has Active Booking => Cannot Delete
            var ActiveBooking = _unitOfWork.SessionRepository.GetCountOfBookedSlot(session.Id) > 0;
            if (ActiveBooking) return false;

            return true;
        }

        #endregion
    }
}
