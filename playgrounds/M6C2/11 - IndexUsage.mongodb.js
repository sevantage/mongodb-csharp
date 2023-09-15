use("sample_mflix");

// Doesn't use an index (COLLSCAN)
db.movies.find({ title: "The Godfather" }).explain();

// Uses year_1 (IXSCAN + FETCH)
// db.movies.find({ title: "The Godfather", year: 1972 }).explain();

// Uses type_1_year_1_rated_1 for filtering, but no index for sorting
// (IXSCAN, FETCH, SORT)
// db.movies
//   .find({ year: 1972, type: "movie" }, {}, { sort: { title: 1 } })
//   .explain();

// Uses type_1_year_1_rated_1 for sorting (IXSCAN, FETCH)
// db.movies.find({}, {}, { sort: { type: 1, year: 1 } }).explain();

// Doesn't use an index (COLLSCAN, SORT)
// db.movies.find({}, {}, { sort: { title: 1 } }).explain();
