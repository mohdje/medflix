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

        public async Task<ServiceInfo> GetServiceInfoAsync(T serviceType, bool includeAvailabilityState)
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
                    Available = includeAvailabilityState ? await PingServiceAsync(serviceType) : null
                };
            }

            return null;
        }
        public async Task<IEnumerable<ServiceInfo>> GetServicesInfoAsync(bool includeAvailabilityState)
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
                    tasks.Add(Task.Run(async() =>
                    {
                        infoList.Add(await GetServiceInfoAsync((T)val, includeAvailabilityState));
                    }));
                }
            }

            await Task.WhenAll(tasks.ToArray());

            return infoList;
        }

        protected async Task<bool> PingServiceAsync(T serviceType)
        {
           return await GetService(serviceType).PingAsync();
        }

        public abstract U GetService(T serviceType);
    }
}
