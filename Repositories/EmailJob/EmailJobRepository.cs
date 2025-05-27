using MailCrafter.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailCrafter.Repositories.EmailJob
{
    public class EmailJobRepository : MongoCollectionRepostioryBase<EmailJobEntity>, IEmailJobRepository
    {
        public EmailJobRepository(IMongoDBRepository mongoDBRepository) : base("EmailJobs", mongoDBRepository)
        {
        }
    }
  
}
