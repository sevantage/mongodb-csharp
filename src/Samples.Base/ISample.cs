using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.Base
{
    public interface ISample
    {
    }

    public interface ISampleWithClient : ISample
    {
        Task RunAsync(IMongoClient client, Configuration config);
    }

    public interface ISampleWithDatabase : ISample
    {
        Task RunAsync(IMongoDatabase db, Configuration config);
    }
}
