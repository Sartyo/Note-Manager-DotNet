# Note Manager DotNet

This project is a backend API built using the ASP.NET Core framework.

## What it Does

This backend API is a note manager. It allows users to create notes and tags, and handle them separately. It uses authentication as well, so users can have their separate notes and tags from each other.

## What I Used For This

I use the following to build this API:

* **ASP.NET Core** as the framework
* **MySQL** for handling the database
* **JWT** for authentication
* **Swagger** for documentation of the API endpoints

---

## How To Use (Installation & Running)

If you want to try out the API on your own machine, you can do the following (it is assumed that you have already installed the **.NET SDK** and **MySQL** in your computer):

1.  Clone the repository:
    ```bash
    git clone https://github.com/Sartyo/Note-Manager-DotNet.git
    ```
2.  Restore the packages that the project needs (the ones that appear on the .csproj file on the root folder) by executing the command:
    ```bash
    dotnet restore
    ```
3.  Install the **Entity Framework tool** on your .NET SDK so you can execute the Entity Framework commands for running migrations into the MySQL database by executing:
    ```bash
    dotnet tool install --global dotnet-ef
    ```
    (In case you don't want the tool installed globally, omit `--global`.)
4.  You will have to configure a few settings. It is recommended to create an `appsettings.Development.json` file to set environment variables. In the **`DefaultConnection`** string, specify your MySQL server, database, user, and password, and also change the **JWT key**, if desired.
5.  Run the .NET project using `dotnet run` or `dotnet watch`. You can use the `--launch-profile` argument with `https` to run it using the HTTPS profile (ports defined in `launchSettings.json`).
6.  You should now be able to interact with the API by making requests to it in **localhost** (remember the port that is running!).
7.  On a browser, open `localhost:PORT/swagger` to view the **Swagger UI** and see the API endpoints documentation in order to build your requests to it.

---

## Project Motivation

There really isn't much to it; I made it to learn ASP.NET Core mostly. It would be deployed alongside a frontend for it at some point. I think it is fine and minimal, it is a simple notes app that I want to do and learn C# and .NET as well.