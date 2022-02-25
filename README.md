# Internal Tools Backend C# developer project Test

## Description of implemented changes

1. Creation of new methods to access to the local user data (Get by Id and pagination)
2. Implementation of the ExternalContactsRepository to access to the External Contacts API
   - The access to the API uses the HttpClient from .NET Core
   - The client uses the Polly package for client resilience and retries to the External Contacts API
3. Changes in the front-end, implemented a Vue.js Nuxtjs client app, using Net SPA package
4. Pagination/Filter/Order of the results
5. Implementation of action from the client to GDPR deletion
6. Selection of users for masive GDPR deletion
7. Creation of new test project using the Nunit for testing
   - Created test for selection of users
   - Created test for GDPR deletion process

### Observation for the first run in development

In the first run of the project, the npm manager must download the needed packages for the client app, so it's can have a initial delay for start