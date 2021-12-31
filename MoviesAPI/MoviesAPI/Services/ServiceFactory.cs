using MoviesAPI.Services.CommonDtos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Services
{
    public abstract class ServiceFactory<T, U> where T : struct, IConvertible where U : IService
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
        public IEnumerable<ServiceInfo> GetServicesInfo()
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
                        infoList.Add(new ServiceInfo()
                        {
                            Description = descriptionAttribute.Description,
                            Id = (int)val,
                            Available = PingService((T)val)
                        });
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
