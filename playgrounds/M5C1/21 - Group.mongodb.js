use("sample_mflix");

db.movies.aggregate([
  {
    $match: {
      cast: "Al Pacino",
    },
  },
  {
    $group: {
      _id: {
        $multiply: [{ $toInt: { $divide: ["$year", 10] } }, 10],
      },
      cnt: { $sum: 1 },
      minYear: {
        $min: "$year"
      }, 
      maxYear: {
        $max: "$year"
      }
    },
  },
  {
    $sort: { _id: 1 }
  }
]);
