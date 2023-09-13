using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.Base
{
    public class Configuration
    {
        public string ConnectionString { get; set; } = "mongodb://localhost:27017";

        public string SampleMoviesDatabaseName { get; set; } = "sample_mflix";

        public string SampleRestaurantsDatabaseName { get; set; } = "sample_restaurants";

        public string Sample { get; set; } = string.Empty;
    }
}
