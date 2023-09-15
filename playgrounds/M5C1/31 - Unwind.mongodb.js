use("sample_mflix");

db.movies.aggregate([
  {
    $unwind: "$cast"
  },
  {
    $match: {
      $or: [
        {cast: "Al Pacino"},
        {cast: "Robert De Niro"},
        {cast: "Meryl Streep"},
        {cast: "Susan Sarandon"},
      ]
    },
  },
  {
    $group: {
      _id: "$cast",
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
