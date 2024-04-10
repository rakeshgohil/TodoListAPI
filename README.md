# TodoList
Open and Build the solution in Visual Studio

Execute following commands in package manager console window (Tools-->Nuget Package Manager-->Package Manager Console)
1. Add-Migration InitialCreate
2. Update-Database

Run the application and that will open the swagger screen
In swagger, using the Auth controller, execute the post request with following body
{
  "userName": "admin",
  "password": "admin"
}
copy the token Value and click on AUthorize(top right corner) and paste it in Value field and click on Authorize button and close the window,

You can test other Todos API engpoints

 


