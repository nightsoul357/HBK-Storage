docker stop hbk-storage-db
docker rm hbk-storage-db
docker rmi hbk-storage-db-image

docker stop hbk-storage-api
docker rm hbk-storage-api
docker rmi hbk-storage-api-image

docker stop hbk-storage-web
docker rm hbk-storage-web
docker rmi hbk-storage-web-image

docker stop hbk-storage-sync
docker rm hbk-storage-sync
docker rmi hbk-storage-sync-image

docker build -t hbk-storage-db-image -f Dockerfile_DB .
docker build -t hbk-storage-api-image -f Dockerfile_Api .
docker build -t hbk-storage-web-image -f Dockerfile_Web .
docker build -t hbk-storage-sync-image -f Dockerfile_Sync .

docker create -p 1433:1433 --name hbk-storage-db hbk-storage-db-image
docker start hbk-storage-db

docker create -p 2080:80 --name hbk-storage-api hbk-storage-api-image
docker start hbk-storage-api

docker create -p 1080:80 --name hbk-storage-web hbk-storage-web-image
docker start hbk-storage-web

docker create --name hbk-storage-sync hbk-storage-sync-image
docker start hbk-storage-sync