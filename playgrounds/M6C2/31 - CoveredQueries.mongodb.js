use("sample_mflix");

// Drop index
db.movies.dropIndex("type_1_title_1_year_1");

// Query: movies, sorted by title, between 1970 & 1979
// IXSCAN, FETCH, PROJECTION_SIMPLE, SORT
db.movies
  .find(
    { type: "movie", year: { $gte: 1970, $lte: 1979 } },
    { _id: 0, title: 1, year: 1 },
    { sort: { title: 1 } }
  )
  .explain("executionStats");

// Create an index supporting ESR
db.movies.createIndex({ type: 1, title: 1, year: 1 });

// Query: movies, sorted by title, between 1970 & 1979
// IXSCAN, PROJECTION_COVERED
db.movies
  .find(
    { type: "movie", year: { $gte: 1970, $lte: 1979 } },
    { _id: 0, title: 1, year: 1 },
    { sort: { title: 1 } }
  )
  .explain("executionStats");
