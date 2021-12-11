# Jobberwocky
Avature's Jobberwocky!

## Code sample
This project is a proposed solution for recruiting evaluators. Items to note are:
* Both compulsory and optional items where completed
* Repositories data is held in memory for as long as the API server is running.
  * Some fake data is pre-loaded for ease of evaluation.
* While only a single External Provider is set as additional sources, the code is ready to add more with no modifications.
* Unit Tests for all repositories and services except optional job alert service.
  * If actual repositories where to be created later, existing unit tests would ensure they are working as expected.
* Job alert service only mocks email sending.

## How to use it
1. Run the Jobberwocky external provider API as explained in Home Project's instructions.
1. Run the API project (Jobberwocky.Api).
1. Import provided API call collection into Postman (Avature.postman_collection.json).
1. Execute API calls.
