use("mongodb-csharp");

// This sample demonstrates an update using a aggregation pipeline that 
// - purges outdated entries from an array
// - adds a new item to the array
// - updates the avg and cnt fields based upon the array contents

let yesterday = ISODate("2023-08-15T00:00:00Z");

// Prepare data for update
db.m5c1_updateAggregationPipeline.deleteMany({});
db.m5c1_updateAggregationPipeline.insertOne({
  _id: 1,
  last24h: [
    {
      timestamp: ISODate("2023-08-14T01:00:00Z"),
      value: 100,
    },
    {
      timestamp: ISODate("2023-08-14T03:00:00Z"),
      value: 200,
    },
    {
      timestamp: ISODate("2023-08-15T01:00:00Z"),
      value: 9,
    },
    {
      timestamp: ISODate("2023-08-15T03:00:00Z"),
      value: 7,
    },
  ],
  avg: 79,
  cnt: 4,
});

// Perform update
db.m5c1_updateAggregationPipeline.updateOne(
  {
    _id: 1,
  },
  [
    {
      $set: {
        last24h: {
          $concatArrays: [
            {
              $filter: {
                input: "$last24h",
                cond: {
                  $gte: ["$$this.timestamp", yesterday],
                },
              },
            },
            [
              {
                timestamp: ISODate("2023-08-15T12:34:56Z"),
                value: 2,
              },
            ],
          ],
        },
      },
    },
    {
      $set: {
        cnt: { $size: "$last24h" },
        avg: { $avg: "$last24h.value" },
      },
    },
  ]
);

db.m5c1_updateAggregationPipeline.findOne({ _id: 1 });
