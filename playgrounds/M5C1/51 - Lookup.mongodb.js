use("sample_mflix");

// This aggregation demonstrates the use of $lookup to
// retrieve the last 10 comments for each movie in the year 1972
db.movies.aggregate([
  {
    $match: {
      year: 1972,
    },
  },
  {
    $lookup: {
      from: "comments",
      localField: "_id",
      foreignField: "movie_id",
      pipeline: [
        {
          $sort: {
            date: -1,
          },
        },
        {
          $limit: 10,
        },
        {
          $project: {
            _id: 0,
            date: 1,
            text: 1,
          },
        },
      ],
      as: "last_comments",
    },
  },
  {
    $set: {
      num_comments: {
        $size: "$last_comments",
      },
    },
  },
  {
    $match: {
      num_comments: { $gt: 0 },
    },
  },
  {
    $project: {
      _id: 0,
      title: 1,
      last_comments: 1,
    },
  },
]);
