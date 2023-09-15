use("sample_mflix");

// Single-field index
db.movies.createIndex({ year: 1 });

// Compund index
db.movies.createIndex({ type: 1, year: 1, rated: 1 });

// Multi-key index on an array
db.movies.createIndex({ genres: 1 });

// Partial index
db.movies.createIndex(
  { year: 1, rated: 1 },
  {
    partialFilterExpression: {
      year: { $type: "int" },
    },
  }
);

db.movies.getIndexes();
