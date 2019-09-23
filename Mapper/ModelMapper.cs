using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WM.Common.Interfaces;

namespace WM.Common.Mapper
{
    public class ModelMapper<T> : IModelMapper<T> where T : class
    {
        IModelMapper<T> modelMapper;
        public ModelMapper()
        {
            modelMapper = (IModelMapper<T>)Activator.CreateInstance(typeof(T), new object[] { });
        }

        public ModelMapper(HttpRequestMessage request)
        {
            modelMapper = (IModelMapper<T>)Activator.CreateInstance(typeof(T), new object[] { request });
        }

        public T MapBusinessModelToServiceModel(object BusinessModel)
        {
            if (BusinessModel != null)
                return modelMapper.MapBusinessModelToServiceModel(BusinessModel);

            return null;
        }

        public List<T> MapBusinessModelsToServiceModels(object BusinessModels)
        {
            if (BusinessModels != null && ((IList)BusinessModels).Count > 0)
                return modelMapper.MapBusinessModelsToServiceModels(BusinessModels);

            return null;
        }

        public object MapServiceModelToBusinessModel(T ServiceModel)
        {
            if (ServiceModel != null)
                return modelMapper.MapServiceModelToBusinessModel(ServiceModel);

            return null;
        }

        public object MapServiceModelsToBusinessModels(List<T> ServiceModels)
        {
            if (ServiceModels != null && ServiceModels.Count() > 0)
                return modelMapper.MapServiceModelsToBusinessModels(ServiceModels);

            return null;
        }
    }
}
