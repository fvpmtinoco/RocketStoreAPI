using AutoMapper;
using System;

namespace RocketStoreApi.Managers
{
    /// <summary>
    /// Defines the mapping profile used by the application.
    /// </summary>
    /// <seealso cref="Profile" />
    internal partial class MappingProfile : Profile
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingProfile"/> class.
        /// </summary>
        public MappingProfile()
        {
            this.CreateMap<Models.Customer, Entities.Customer>().ForMember(d => d.Email, o => o.MapFrom(s => s.EmailAddress))
                .AfterMap(
                    (source, target) =>
                    {
                        target.Id = Guid.NewGuid().ToString();
                    });
        }

        #endregion
    }
}
