use("sample_mflix");

// This sample shows the unwind-and-regroup pattern
// by unwinding the cast, filtering for specific actors 
// and then rebuilding the original documents
db.movies.aggregate([
  {
    $unwind: "$cast",
  },
  {
    $match: {
      $or: [
        { cast: "Al Pacino" },
        { cast: "Robert De Niro" },
        { cast: "Meryl Streep" },
        { cast: "Susan Sarandon" },
      ],
    },
  },
  {
    $replaceRoot: {
      newRoot: {
        orig: {
          $mergeObjects: ["$$ROOT", { cast: null }],
        },
        cast: "$cast",
      },
    },
  },
  {
    $group: {
      _id: "$orig",
      cast: { $push: "$cast" },
    },
  },
  {
    $replaceRoot: {
      newRoot: {
        $mergeObjects: [
          "$_id", 
          { cast: "$cast" }
        ]
      }
    }
  }
]);
