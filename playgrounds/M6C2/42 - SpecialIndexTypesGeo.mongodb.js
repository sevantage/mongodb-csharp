use("sample_restaurants");

db.restaurants.createIndex({
  "address.coord": "2dsphere",
});

// # of restaurants within an area around Empire State Building.
db.restaurants
  .find({
    "address.coord": {
      $geoWithin: {
        $geometry: {
          type: "Polygon",
          coordinates: [
            [
              [-73.986891, 40.751093],
              [-73.980432, 40.748427],
              [-73.985067, 40.742136],
              [-73.991461, 40.744786],
              [-73.986891, 40.751093],
            ],
          ],
        },
      },
    },
  })
  .count();

// Restaurants with a max distance of 100 meters to Chrysler Building
db.restaurants
  .find({
    "address.coord": {
      $near: {
        $geometry: {
          type: "Point",
          coordinates: [-73.9795807, 40.7610219],
        },
        $maxDistance: 100,
      },
    },
  }, { _id: 0, name: 1, cuisine: 1 });
