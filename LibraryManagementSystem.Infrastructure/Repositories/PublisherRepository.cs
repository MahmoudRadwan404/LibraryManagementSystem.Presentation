using LibraryManagementSystem.Application;
using LibraryManagementSystem.Application.IRepositories.IPublisher;
using LibraryManagementSystem.Domain.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Infrastructure.Repositories
{
    public class PublisherRepository : GenericRepository<Publisher>, IPublisherRepository
    {
        public PublisherRepository(ApplicationDbContext context) : base(context) { }
    }
}
