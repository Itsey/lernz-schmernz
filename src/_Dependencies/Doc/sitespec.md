## Test and Learn Website Spec

### Overview

This site has a simple UI which consists of a login screen and a single account page.  The account page displays the user name as well as their balance.  There is a single text box entry field which allows the user to make a deposit or a withdrawal.  The user can specify a positive amount to deposit or a negative amount to withdraw.  The account balance is updated accordingly when the user submits the transaction.

There is an api layer which handles the actual details of how to update the account balance.  The api layer exposes a single endpoint which accepts a user id and an amount to deposit or withdraw.  The api layer is responsible for validating the transaction and updating the account balance in the database.  The database is a static class, not a real database and it should be created in a file called database.cs.   This site is for learning so it does not need to be secure or robust.

### Api Specification

There should be three controllers.  Each controller should be in its own file and use dependency injection to get access to the repository layer.

The controllers will be as follows:

* MobileController - handles requests from the mobile app
* WebController - handles requests from the web app
* ClientController - handles requests from other clients


Each controller will have the same endpoints as follows:

There should be an API endpoint which accepts a POST request to login a user with a given user id and password. This will return a logon token which needs to be passed to the other API endpoints.  This should be a randomly generated string which is stored in memory for the duration of the application.  No need to persist it.  It should be stored in a dictionary with the user id as the key.  It should be 8 characters long.

All of the other APIs should require the logon token to be passed in the header and they should validate that it is a valid token for the given user id.

There should be an API endpoint which accepts a GET request to return the user details from a given user id.
There should be an API endpoint which accepts a GET request to return the user balance for a given user id.
There should be an API endpoint which accepts a POST request to create a new user with a given user id, user name and password.  The new user should have a balance of 0 and be enabled by default.
There should be an API endpoint which accepts a POST request to update the user balance for a given user id and amount at a specified date. The amount can be positive or negative.  The API should validate that the user has sufficient funds for a withdrawal.  If the transaction is successful it should return the new balance.

### Repository Specification

The repository wil be in a separate file and will be responsible for persisting the details from the users.  Each user will have a separate text file which will store their dtails.  The text file will be named with the user id and will be stored in a folder called "data".  The repository will have methods to get the user details, get the user balance and update the user balance.  The text file will be in a simple name=value format with one line per field.  The fields will be as follows:
* userId
* userName
* password
* balance
* enabled
* lastLogin





