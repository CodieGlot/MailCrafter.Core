using MailCrafter.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailCrafter.Repositories;

public interface IEmailJobRepository : IMongoCollectionRepostioryBase<EmailJobEntity>
{
}
