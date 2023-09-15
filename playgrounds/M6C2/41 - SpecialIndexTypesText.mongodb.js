use("sample_mflix");

db.movies.createIndex({
  cast: "text",
  fullplot: "text",
  genres: "text",
  title: "text",
});

db.movies.find(
  {
     $text: { $search: "Godfather" }
  }
).explain()

