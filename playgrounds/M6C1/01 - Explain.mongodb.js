use("sample_mflix");

db.movies
  .aggregate([
    // {
    //   $match: {
    //     year: { $in: [ 1972, 1973 ]}
    //   }
    // },
    {
      $group: {
        _id: "$year",
        cnt: { $sum: 1 },
      },
    },
    {
      $sort: {
        cnt: -1,
      },
    },
    {
      $limit: 3,
    },
  ])
  .explain("allPlansExecution");
