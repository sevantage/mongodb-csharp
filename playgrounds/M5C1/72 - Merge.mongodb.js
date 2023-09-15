use("sample_mflix");

// This aggregation demonstrates the use of $out to
// store the top rated movies by decade in a separate collection
db.movies.aggregate([
  {
    $match: {
      "imdb.rating": { $gt: 0 },
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
    $group: {
      _id: "$decade",
      top_movies: {
        $addToSet: {
          title: "$title",
          rating: "$imdb.rating",
        },
      },
    },
  },
  {
    $set: {
      top_movies: {
        $slice: [
          {
            $sortArray: {
              input: "$top_movies",
              sortBy: {
                rating: -1,
                title: 1,
              },
            },
          },
          10,
        ],
      },
    },
  },
  {
    $sort: {
      _id: -1,
    },
  },
  {
    $merge: {
      into: {
        db: "mongodb-csharp",
        coll: "m5c1_topMovies_merge",
      },
      on: "_id", 
      whenMatched: "keepExisting", 
      whenNotMatched: "insert"
    },
  },
]);
