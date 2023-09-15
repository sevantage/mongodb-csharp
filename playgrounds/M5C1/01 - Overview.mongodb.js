use("sample_mflix");

db.movies.aggregate([
  {
    $group: {
      _id: "$year",
      cnt: { $sum: 1 },
    },
  },
  {
    $sort: {
        cnt: -1
    }
  }, 
  {
    $limit: 3
  }
]);
