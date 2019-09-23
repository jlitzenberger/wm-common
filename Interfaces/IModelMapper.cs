using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WM.Common.Interfaces
{
    public interface IModelMapper<T> where T : class
    {
        T MapBusinessModelToServiceModel(object BusinessModel);
        List<T> MapBusinessModelsToServiceModels(object BusinessModels);
        object MapServiceModelToBusinessModel(T ServiceModel);
        object MapServiceModelsToBusinessModels(List<T> ServiceModels);
    }
}
