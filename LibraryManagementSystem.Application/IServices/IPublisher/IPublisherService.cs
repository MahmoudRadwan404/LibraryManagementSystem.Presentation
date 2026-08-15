using LibraryManagementSystem.Application.DTOs.Publisher;
using LibraryManagementSystem.Application.IServices.IGeneric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Application.IServices.IPublisher
{
    public interface IPublisherService : IGenericService<PublisherDto, CreatePublisherDto, UpdatePublisherDto> { }
}
