use("sample_mflix");

// This sample shows the unwind-and-regroup pattern
// by unwinding the cast, filtering for specific actors
// and then rebuilding the original documents
db.movies.aggregate([
  {
    $set: {
      cast: {
        $filter: {
          input: "$cast",
          cond: {
            $in: [
              "$$this",
              ["Al Pacino", "Meryl Streep", "Robert De Niro", "Susan Sarandon"],
            ],
          },
        },
      },
    },
  },
  {
    $match: {
      $expr: {
        $and: [{ $ne: ["$cast", null] }, { $gt: [{ $size: "$cast" }, 0] }],
      },
    },
  },
]);
