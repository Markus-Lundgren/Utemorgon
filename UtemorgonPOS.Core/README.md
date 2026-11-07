# Core

This is the core utilities classlib. It contains everything we need to speak with the database and manage the object returns.

## About mixing of concerns

Since we are a 3 man team of mixed experience, some code purity has been sacraficed in favor of easy collaboration.  
Concenrs regarding data fetching has been mixed to some degree, and we do not utilize things like finance or order managers,
instead we opt for centralized queries to be used in pure C#.

## TODO:

- Add all necessary models from DB schema
- Add db connection
- Add SQL queries for Data
