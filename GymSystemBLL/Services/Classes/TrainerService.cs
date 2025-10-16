using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymSystemBLL.Services.Interfaces;
using GymSystemBLL.ViewModels;
using GymSystemDAL.Entities;
using GymSystemDAL.Repositories.Interfaces;

namespace GymSystemBLL.Services.Classes
{
    internal class TrainerService : ITrainerService
    {
        private readonly IGenericRepository<Trainer> _trainerRepository;
        private readonly IGenericRepository<Session> _sessionRepository;

        public TrainerService
            (IGenericRepository<Trainer> trainerRepository,
            IGenericRepository<Session> sessionRepository)
        {
            _trainerRepository = trainerRepository;
            _sessionRepository = sessionRepository;
        }

        public bool CreateTrainers(CreateTrainerViewModel createTrainer)
        {
            try
            {
                if (IsEmailExist(createTrainer.Email) || IsPhoneExist(createTrainer.Phone)) return false;

                var trainer = new Trainer()
                {
                    Name = createTrainer.Name,
                    Email = createTrainer.Email,
                    Phone = createTrainer.Phone,
                    Gender = createTrainer.Gender,
                    DateOfBirth = createTrainer.DateOfBirth,
                    Address = new Address()
                    {
                        BuildingNumber = createTrainer.BuildingNumber,
                        Street = createTrainer.Street,
                        City = createTrainer.City
                    },
                    Specialties = createTrainer.Specialties
                };

                return _trainerRepository.Add(trainer) > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public IEnumerable<TrainerViewModel> GetAllTrainers()
        {
            var Trainers = _trainerRepository.GetAll();
            
            if (Trainers == null || !Trainers.Any()) return [];

            var TrainerViewModels = Trainers.Select(X => new TrainerViewModel
            {
                Id = X.Id,
                Name = X.Name,
                Email = X.Email,
                Phone = X.Phone,
                Specialization = X.Specialties.ToString()
            });

            return TrainerViewModels;
        }

        public TrainerViewModel? GetTrainerDetails(int TrainerId)
        {
            var Trainer = _trainerRepository.GetById(TrainerId);
            if (Trainer == null) return null;

            var ViewModel = new TrainerViewModel()
            {
                Name = Trainer.Name,
                Specialization = Trainer.Specialties.ToString(),
                Email = Trainer.Email,
                Phone = Trainer.Phone,
                DateOfBirth = Trainer.DateOfBirth.ToShortDateString(),
                Address = $"{Trainer.Address.BuildingNumber} - {Trainer.Address.Street} - {Trainer.Address.City}"
            };

            return ViewModel;
        }

        public TrainerToUpdateViewModel? GetTrainerToUpdate(int TrainerId)
        {
            var Trainer = _trainerRepository.GetById(TrainerId);
            if (Trainer is null) return null;

            return new TrainerToUpdateViewModel()
            {
                Email = Trainer.Email,
                Phone = Trainer.Phone,
                Name = Trainer.Name,
                BuildingNumber = Trainer.Address.BuildingNumber,
                City = Trainer.Address.City,
                Street = Trainer.Address.Street,
                Specialties = Trainer.Specialties
            };
        }

        public bool RemoveTrainer(int TrainerId)
        {
            var Trainer = _trainerRepository.GetById(TrainerId);
            if (Trainer is null) return false;

            var HasTrainerActiveSessions = 
                _sessionRepository.GetAll(X => X.TrainerId == TrainerId && X.StartDate > DateTime.Now).Any();
            if (HasTrainerActiveSessions) return false;

            try
            {
                return _trainerRepository.Delete(Trainer) > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateTrainerDetails(int id, TrainerToUpdateViewModel updatedTrainer)
        {
            try
            {
                if (IsEmailExist(updatedTrainer.Email) || IsPhoneExist(updatedTrainer.Phone)) return false;

                var Trainer = _trainerRepository.GetById(id);
                if (Trainer is null) return false;

                Trainer.Email = updatedTrainer.Email;
                Trainer.Phone = updatedTrainer.Phone;
                Trainer.Address.BuildingNumber = updatedTrainer.BuildingNumber;
                Trainer.Address.Street = updatedTrainer.Street;
                Trainer.Address.City = updatedTrainer.City;
                Trainer.Specialties = updatedTrainer.Specialties;
                Trainer.UpdatedAt = DateTime.Now;

                return _trainerRepository.Update(Trainer) > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #region Helper Methods

        private bool IsEmailExist(string email)
        {
            return _trainerRepository.GetAll(X => X.Email == email).Any();
        }
        private bool IsPhoneExist(string phone)
        {
            return _trainerRepository.GetAll(X => X.Phone == phone).Any();
        }

        #endregion
    }
}
