# banking
Simple banking app utilizing C# and PostgreSQL
## Description
This app lets the user connect to a PostgreSQL database, create new accounts and log into them. Once the user logs into their account, they are able to deposit or withdraw money as well as transfer it to other users registered in the database. The details of every transfer are saved in the database, giving user the ability to view their transfer history.
![App Screenshot](/img/appscreen.png)
## Usage
Before you compile the code, you need to create a Postgres database and give it the right structure using the .sql file from the folder *dbschema*. Once you do that, make sure that the server and superuser credentials in the first lines of the C# code match the ones you use to connect with your database.