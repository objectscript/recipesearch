Main goal of this project is to provide enhanced functionality for recipes search. It uses iKnow to analyze recipes text, finds similar recipes and groups them before show to user.

Demo URL - [recipes.somee.com](http://recipes.somee.com) (no guarantees it will be always working). Username/password for admin part - admin/admin.

# Dev Environment Setup Guide
#### Install necessary software
- Visual Studio 2012/2013
- Cache 2015.2 (with iKnow license)

#### Enable WCF http activation
1. Click Start > Control Panel > Turn Windows features on or off.
2. Click Next until the Select Features window appears.
3. Expand .NET Framework 4.5 Features.
4. Expand WCF Services.
5. Select HTTP Activation and click Install.
6. Follow the prompts and finish the installation.

#### Import and compile Cache project
- Allow import to the CACHELIB. Go to System Administration -> Configuration -> System Configuration -> Local Databases, then open CACHELIB, uncheck "Always Mount Read-Only" and save updated configuration.
- Main Cache project (cacheRecipesSearch.xml) is located in the RecipesSearch.CacheProject folder.

#### Run database seed sproc
- The seed procedure must be executed in order to populate initial data. Seed function - RecipeSearch.Data.Utils.Seed.Run(). E. g. you can call it from SQL using the following command:

  ```
  call RecipeSearch_Data_Utils.Seed_Run()
  ```
- To seed the stemming and spellchecking dictionaries another seed procedure must be executed - Data.Utils.Seed.LoadDictionaries(directory). Where 'directory' - *full path* to the directory with dictionaries (\RecipesSearch.CacheProject\Dictionaries).

#### Setup the Cache REST service:
1. Go to System Administration -> Security -> Applications -> Web Applications
2. Create a new Web Application
3. Enter "\recipes" as a name (slash at the beginning is important)
4. Choose correct namespace
5. Enable iKnow for this application
6. Add password to Allowed Authentication Methods
7. Set RecipeSearch.SearchAPI.RESTBroker as Dispatch Class
8. Save application

#### Ensure Cache connection settings are valid
Connections settings are located in the Web.Base.config of the RecipesSearch.WebApplication and RecipesSearch.ImporterService projects. There are 2 settings group for now:

- Connection string. It must have the following structure:
  ```
  Server={{serverAddress}}; Port=1972; Namespace={{nameSpaceWithCacheProject}}; Password={{Password}}; User ID={{Username}};
  ```
  By default connection string has the following values. You must change it if you imported Cache project to another Namespace (or if you are using other user)
  ```
  Server=localhost; Port=1972; Namespace=DEV; Password=SYS; User ID=_SYSTEM;
  ```

- REST configuration. There are 3 keys in the appSettings section which have the following structure:
  ```
  <add key="BaseURL" value="http://{{serverAddress}}:57772" />
  <add key="Username" value="{{Username}}" />
  <add key="Password" value="{{Password}}" />
  ```
  By default the localhost server address and the _SYSTEM user are used:
  ```
  <add key="BaseURL" value="http://localhost:57772" />
  <add key="Username" value="_SYSTEM" />
  <add key="Password" value="SYS" />
  ```

#### Open Visual Studio solution, build and run the web application
- Make sure RecipesSearch.WebApplication has been set as a startup project.

# Data Import Guide
1. Go to the Admin part of the site.
2. Go to the Crawlering control tab and crawl pages from sites.
3. Open the Tasks tab.
4. Update Tf/Idf data.
5. Update nearest neighbors data.
