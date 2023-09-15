use("sample_mflix");

// This aggregation demonstrates the use of $setWindowFields to
// find movies with Al Pacino in their cast and assemble a document
// per decade that contains movies above and below the average
// IMDB-rating in this decade.
db.movies.aggregate([
  {
    $unwind: "$cast",
  },
  {
    $match: {
      cast: "Al Pacino",
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
    $set: {
      decade: {
        $multiply: [
          {
            $toInt: {
              $divide: ["$year", 10],
            },
          },
          10,
        ],
      },
    },
  },
  {
    $setWindowFields: {
      partitionBy: "$decade",
      output: {
        avgRating: {
          $avg: "$imdb.rating",
        },
      },
    },
  },
  {
    $project: {
      title: 1,
      decade: 1,
      year: 1,
      rating: "$imdb.rating",
      avgRating: 1,
    },
  },
  {
    $set: {
      aboveAvg: {
        $gte: ["$rating", "$avgRating"],
      },
    },
  },
  {
    $group: {
      _id: "$decade",
      movies: {
        $addToSet: {
          title: "$title",
          aboveAvg: "$aboveAvg",
        },
      },
    },
  },
  {
    $set: {
      aboveAvg: {
        $map: {
          input: {
            $filter: {
              input: "$movies",
              cond: "$$this.aboveAvg",
            },
          },
          in: "$$this.title",
        },
      },
      belowAvg: {
        $map: {
          input: {
            $filter: {
              input: "$movies",
              cond: { $not: "$$this.aboveAvg" },
            },
          },
          in: "$$this.title",
        },
      },
    },
  },
  {
    $unset: "movies",
  },
  {
    $sort: {
      _id: 1,
    },
  },
]);
