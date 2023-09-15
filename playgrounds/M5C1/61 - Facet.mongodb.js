use("sample_mflix");

// This aggregation demonstrates the use of $facet for
// paging in a single request
db.movies.aggregate([
  {
    $match: {
      year: 1972,
    },
  },
  {
    $facet: {
      count: [{ $count: "cnt" }],
      page: [
        {
          $sort: {
            title: 1,
          },
        },
        {
          $skip: 100,
        },
        {
          $limit: 10,
        },
        {
          $project: {
            _id: 0,
            title: 1, 
            released: 1
          }
        }
      ],
    },
  },
]);
