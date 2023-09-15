use("sample_mflix");

db.movies.aggregate([
  {
    $match: {
      genres: "Crime",
    },
  },
  {
    $set: {
      year: {
        $cond: {
          if: { $isNumber: "$year" },
          then: "$year",
          else: {
            $toInt: {
              $substr: ["$year", 0, 4],
            },
          },
        },
      },
    },
  },
  {
    $sort: {
      year: 1,
    },
  },
  {
    $limit: 1,
  },
  {
    $set: {
      age: {
        $subtract: [{ $year: "$$NOW" }, "$year"],
      },
    },
  },
  {
    $project: {
      title: 1,
      year: 1,
      age: 1,
      _id: -1,
    },
  },
]);
