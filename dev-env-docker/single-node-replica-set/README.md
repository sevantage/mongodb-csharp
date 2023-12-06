1. Download sample data
2. Move sample data to ./data
3. Open console in container, e.g. docker exec -it single-node-replica-set-mongo-rs-1 /bin/bash
4. Import sample data into cluster: mongorestore --archive=/var/data/sampledata.archive