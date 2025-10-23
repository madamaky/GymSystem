using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymSystemBLL.ViewModels.TrainerViewModels;

namespace GymSystemBLL.Services.Interfaces
{
    public interface ITrainerService
    {
        IEnumerable<TrainerViewModel> GetAllTrainers();
        bool CreateTrainer(CreateTrainerViewModel createdTrainer);
        TrainerViewModel? GetTrainerDetails(int trainerId);
        TrainerToUpdateViewModel? GetTrainerToUpdate(int trainerId);
        bool UpdateTrainerDetails(TrainerToUpdateViewModel updatedTrainer, int trainerId);
        bool RemoveTrainer(int trainerId);
    }
}
