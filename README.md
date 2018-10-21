# Lds.MongoRepository
Mongo Repository for LOB apps

This repository add some opioniated logic to using MongoDB in a Business Application.

Focus is on abstracting repeated and boring database and entity handling away from the business logic.

## Examples
You can decorate your classes with Interfaces to ensure certain behaviour. E.g. IDeleteProctected, IHasAuditing etc.

### Delete procteced
Decorate your entity class with IDeleteProtected and your data objects will not be deleted, when you use the framework.

