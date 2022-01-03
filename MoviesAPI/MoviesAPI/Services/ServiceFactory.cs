using MoviesAPI.Services.CommonDtos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Services
{
    public abstract class ServiceFactory<T, U> where T : struct, IConvertible where U : BaseService
    {
        private Type serviceEnumType;
        public ServiceFactory()
        {
            if (typeof(T).IsEnum)
            {
                this.serviceEnumType = typeof(T);
            }
            else
            {
                throw new ArgumentException("T must be an enumerated type");
            }
        }

        public ServiceInfo GetServiceInfo(T serviceType, bool includeAvailabilityState)
        {
            var memInfo = serviceEnumType.GetMember(serviceEnumType.GetEnumName(serviceType));
            var descriptionAttribute = memInfo[0]
                .GetCustomAttributes(typeof(DescriptionAttribute), false)
                .FirstOrDefault() as DescriptionAttribute;

            if (descriptionAttribute != null)
            {
                object type = serviceType;

                return new ServiceInfo()
                {
                    Description = descriptionAttribute.Description,
                    Id = (int)type,
                    Available = includeAvailabilityState ? PingService(serviceType) : null
                };
            }

            return null;
        }
        public IEnumerable<ServiceInfo> GetServicesInfo(bool includeAvailabilityState)
        {
            var infoList = new List<ServiceInfo>();
            var tasks = new List<Task>();
            foreach (var val in Enum.GetValues(serviceEnumType))
            {
                var memInfo = serviceEnumType.GetMember(serviceEnumType.GetEnumName(val));
                var descriptionAttribute = memInfo[0]
                    .GetCustomAttributes(typeof(DescriptionAttribute), false)
                    .FirstOrDefault() as DescriptionAttribute;

                if (descriptionAttribute != null)
                {
                    tasks.Add(new Task(() =>
                    {
                        infoList.Add(GetServiceInfo((T)val, includeAvailabilityState));
                    }));
                }
            }

            tasks.ForEach(t => t.Start());

            Task.WaitAll(tasks.ToArray());

            return infoList;
        }

        protected bool PingService(T serviceType)
        {
           return GetService(serviceType).PingAsync().Result;
        }

        public abstract U GetService(T serviceType);
    }
}
