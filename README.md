# OData for ASP.NET 6

## Introduction
OData is an open data protocol to add capabilities to your API to shape, order, and select portions of data (among other things). These queries are executed on the server without having to create new endpoints whenever a new capability needs to be added and without having to return large json objects that retain irrelevant information. So long as the data returned is an IQueryable, OData will be able to execute a query on the stored data.
</br></br>
The general structure should be brokers, services, and controllers. Brokers propvide the data, services manipulate the data and create a level of abstraction, and controllers offer that data. This tutorial removes the broker layer.