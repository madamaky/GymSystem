using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymSystemBLL.ViewModels;

namespace GymSystemBLL.Services.Interfaces
{
    internal interface ITrainerService
    {
        IEnumerable<TrainerViewModel> GetAllTrainers();

        bool CreateTrainers(CreateTrainerViewModel createTrainer);

        TrainerViewModel? GetTrainerDetails(int TrainerId);

        // GetTrainerId To Update View
        TrainerToUpdateViewModel? GetTrainerToUpdate(int TrainerId);
        // Apply Update
        bool UpdateTrainerDetails(int id, TrainerToUpdateViewModel updatedTrainer);

        bool RemoveTrainer(int TrainerId);
    }
}
