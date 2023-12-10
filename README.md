# Dating APP build using .Net7 and Angular16

## Commands to run in Docker

```
ng build
docker build -t vijeth11/datingapp .
docker run --rm -it -p 8080:80 vijeth11/datingapp:latest
```

Need to set up database in docker container and update connection string in appsettings.json to

https://learn.microsoft.com/en-us/sql/linux/quickstart-install-connect-docker?view=sql-server-ver16&pivots=cs1-bash
